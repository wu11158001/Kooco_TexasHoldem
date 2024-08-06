using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class LobbyMainPageView : MonoBehaviour
{
    [Header("廣告刊版")]
    [SerializeField]
    GameObject BillboardSample, PointSample;
    [SerializeField]
    RectTransform BillboardContent, BillboardPoints;

    [Header("積分房")]
    [SerializeField]
    Button Integral_Btn;
    [SerializeField]
    TextMeshProUGUI IntegralBtn_Txt;

    [Header("加密貨幣桌")]
    [SerializeField]
    GameObject CryptoTableBtnSample;
    [SerializeField]
    RectTransform CryptoTableParent;
    [SerializeField]
    TextMeshProUGUI CryptoTableTital_Txt;

    [Header("虛擬貨幣桌")]
    [SerializeField]
    GameObject VCTableBtnSample;
    [SerializeField]
    RectTransform VCTableParent;
    [SerializeField]
    TextMeshProUGUI VCTableTital_Txt;

    [Header("Line客服")]
    [SerializeField]
    Button LineService;

    const string BillbpardBtnName = "BillbpardBtn";         //點擊判斷名稱
    const float BillboardChangeTime = 5;                    //廣告刊版更換時間

    LobbyView lobbyView;

    List<RectTransform> billboardList;                      //廣告刊版
    List<Image> billboardImgList;                           //廣告刊版圖片
    List<Image> billboardPointList;                         //廣告刊版點
    List<int> billboardDisplayIndexList;                    //廣告刊版顯示

    float billboardSizeWidth;                               //廣告刊版寬度
    bool isStartMoveBillboard;                              //是否開始移動廣告刊版
    bool isBillboardClick;                                  //判斷點擊廣告刊版
    Vector2 startMousePos;                                  //移動廣告刊版起始移動點
    DateTime billboardStartTime;                            //廣告刊版輪播起始時間

    /// <summary>
    /// 積分房資料
    /// </summary>
    private IntegralData integralData;
    public class IntegralData
    {
        public bool isPairing;              //是否正在配對中
        public DateTime startPairTime;      //開始配對時間
    }

    /// <summary>
    /// 更新文本翻譯
    /// </summary>
    private void UpdateLanguage()
    {
        IntegralBtn_Txt.text = LanguageManager.Instance.GetText("INTEGRAL");
        CryptoTableTital_Txt.text = LanguageManager.Instance.GetText("CRYPTO TABLE");
        VCTableTital_Txt.text = LanguageManager.Instance.GetText("VIRTUAL CURRENCY TABLE");
    }

    private void OnDestroy()
    {
        LanguageManager.Instance.RemoveLanguageFun(UpdateLanguage);
    }

    private void Awake()
    {
        integralData = new IntegralData();

        ListenerEvent();
        LanguageManager.Instance.AddUpdateLanguageFunc(UpdateLanguage, gameObject);

        billboardSizeWidth = BillboardSample.GetComponent<RectTransform>().rect.width;
    }

    /// <summary>
    /// 事件聆聽
    /// </summary>
    private void ListenerEvent()
    {
        //積分房
        Integral_Btn.onClick.AddListener(() =>
        {
            if (integralData.isPairing)
            {
                //正在配對取消配對
                IntegralEndPair();
            }
            else
            {
                if (GameRoomManager.Instance.JudgeIsCanBeCreateRoom())
                {
                    //開始配對
                    integralData.isPairing = true;
                    integralData.startPairTime = DateTime.Now;
                }
                else
                {
                    //房間數已達上限
                    lobbyView.ShowMaxRoomTip();
                }
            }
        });

        //  Line客服
        LineService.onClick.AddListener(() =>
        {
            StartLineLogin();
        });

    }

    private void Start()
    {
        lobbyView = GameObject.FindAnyObjectByType<LobbyView>();

        InitBillBoard();
        CreateRoomBtn();
    }

    private void Update()
    {
        #region 廣告刊版切換

        if (!GameRoomManager.Instance.IsShow &&
            Input.GetMouseButtonDown(0) &&
            Utils.GetTouchUIObj() != null &&
            Utils.GetTouchUIObj().name == BillbpardBtnName)
        {
            isStartMoveBillboard = true;
            isBillboardClick = true;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(BillboardContent, Input.mousePosition, null, out startMousePos);
        }

        if (Input.GetMouseButton(0) &&
            isStartMoveBillboard)
        {
            billboardStartTime = DateTime.Now;

            for (int i = 0; i < billboardList.Count; i++)
            {
                float startX = -billboardSizeWidth + (billboardSizeWidth * i);
                Vector2 localPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(BillboardContent, Input.mousePosition, null, out localPoint);
                float x = startX + (localPoint.x - startMousePos.x);
                billboardList[i].anchoredPosition = new Vector2(x, billboardList[i].anchoredPosition.y);
            }

            //廣告刊版移動至切換範圍不執行點擊事件
            if (billboardList[1].anchoredPosition.x > billboardSizeWidth * 0.1f ||
                billboardList[1].anchoredPosition.x < -billboardSizeWidth * 0.1f)
            {
                isBillboardClick = false;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isStartMoveBillboard = false;

            int dir = 0;
            if (billboardList[1].anchoredPosition.x > billboardSizeWidth * 0.1f)
            {
                dir = 1;
            }
            else if(billboardList[1].anchoredPosition.x < -billboardSizeWidth * 0.1f)
            {
                dir = -1;
            }
            StartCoroutine(IChangeBillboard(dir));
        }

        //輪播廣告
        if ((DateTime.Now - billboardStartTime).TotalSeconds >= BillboardChangeTime)
        {
            billboardStartTime = DateTime.Now;
            StartCoroutine(IChangeBillboard(-1));
        }

        #endregion

        #region 積分房

        //積分配對計時器
        if (integralData.isPairing)
        {
            TimeSpan waitingTime = DateTime.Now - integralData.startPairTime;
            IntegralBtn_Txt.text = $"{LanguageManager.Instance.GetText("Pairing")}:{(int)waitingTime.TotalMinutes} : {waitingTime.Seconds:00}";

            if (waitingTime.Seconds >= 3)
            {
                lobbyView.baseRequest.SendRequest_InBattleRoom();
                IntegralEndPair();
            }
        }

        #endregion
    }

    #region 廣告刊版

    /// <summary>
    /// 初始儣告勘版
    /// </summary>
    private void InitBillBoard()
    {
        //可顯示判斷
        billboardDisplayIndexList = new List<int>();
        int allBillboardCount = AssetsManager.Instance.GetAlbumAsset(AlbumEnum.BillboardAlbum).album.Length;
        for (int i = 0; i < allBillboardCount; i++)
        {
            if (i == 2)
            {
                continue;
            }

            billboardDisplayIndexList.Add(i);
        }

        BillboardSample.SetActive(false);
        billboardList = new List<RectTransform>();
        billboardImgList = new List<Image>();
        for (int i = -1; i < 2; i++)
        {
            int index = i;

            //產生廣告按鈕
            RectTransform billbpard = Instantiate(BillboardSample, BillboardContent).GetComponent<RectTransform>();
            billbpard.gameObject.SetActive(true);
            billbpard.anchoredPosition = new Vector2(billboardSizeWidth * i, 0);
            billbpard.name = BillbpardBtnName;
            Button billbpardBtn = billbpard.GetComponent<Button>();
            billbpardBtn.onClick.AddListener(() =>
            {
                if (isBillboardClick)
                {
                    Debug.Log($"Billbroad Click: {DataManager.CurrBillboardIndex}");
                }                
            });
            billboardList.Add(billbpard);

            //初始廣告圖片設置
            Image billboardBg_Img = billbpard.transform.Find("BillboardBg_Img").GetComponent<Image>();
            int billboardIndex = JudgeBillboardIndex(DataManager.CurrBillboardIndex + i, i);
            Sprite sp = AssetsManager.Instance.GetAlbumAsset(AlbumEnum.BillboardAlbum).album[billboardIndex];
            billboardBg_Img.sprite = sp;
            billboardImgList.Add(billboardBg_Img);
        }

        //產生點
        PointSample.SetActive(false);
        billboardPointList = new List<Image>();
        for (int i = 0; i < allBillboardCount; i++)
        {
            if (!billboardDisplayIndexList.Contains(i)) continue;

            GameObject pointObj = Instantiate(PointSample, BillboardPoints);
            pointObj.SetActive(true);
            Image poimtImg = pointObj.GetComponent<Image>();
            billboardPointList.Add(poimtImg);
        }

        SetBillboardPoint();

        billboardStartTime = DateTime.Now;
    }

    /// <summary>
    /// 更換廣告刊版
    /// </summary>
    /// <param name="dir">更換方向</param>
    private IEnumerator IChangeBillboard(int dir)
    {
        if (dir >= 1) dir = 1;
        else if (dir <= -1) dir = -1;

        float moveTime = 0.1f;

        //廣告刊版移動
        DateTime startTime = DateTime.Now;
        Vector2[] startPos = billboardList.Select(x => x.anchoredPosition)
                                          .ToArray();
        float[] targetPosX = new float[startPos.Length];
        for (int i = 0; i < targetPosX.Length; i++)
        {
            targetPosX[i] = (billboardSizeWidth * (i - 1)) + (billboardSizeWidth * dir);
            if (dir == 0)
            {
                targetPosX[i] = billboardSizeWidth * (i - 1);
            }
        }

        while ((DateTime.Now - startTime).TotalSeconds < moveTime)
        {
            float progress = (float)(DateTime.Now - startTime).TotalSeconds / moveTime;

            for (int i = 0; i < billboardList.Count; i++)
            {
                float x = Mathf.Lerp(startPos[i].x, targetPosX[i], progress);
                billboardList[i].anchoredPosition = new Vector2(x, billboardList[i].anchoredPosition.y);
            }

            yield return null;
        }

        //判斷當前顯示廣告Index
        DataManager.CurrBillboardIndex = JudgeBillboardIndex(DataManager.CurrBillboardIndex + -dir, -dir); ;

        //重新設置廣告刊版
        for (int i = 0; i < billboardList.Count; i++)
        {
            float x = -billboardSizeWidth + (billboardSizeWidth * i);
            billboardList[i].anchoredPosition = new Vector2(x, billboardList[i].anchoredPosition.y);
            int billboardIndex = JudgeBillboardIndex(DataManager.CurrBillboardIndex + (i - 1), (i - 1));
            Sprite sp = AssetsManager.Instance.GetAlbumAsset(AlbumEnum.BillboardAlbum).album[billboardIndex];
            billboardImgList[i].sprite = sp;
        }

        SetBillboardPoint();
    }

    /// <summary>
    /// 判斷廣告刊版Index
    /// </summary>
    /// <param name="index"></param>
    /// <param name="dir"></param>
    /// <returns></returns>
    private int JudgeBillboardIndex(int index, int dir)
    {
        if (dir >= 1) dir = 1;
        else if (dir <= -1) dir = -1;

        if (index < 0)
        {
            index = AssetsManager.Instance.GetAlbumAsset(AlbumEnum.BillboardAlbum).album.Length - 1;
        }
        else if (index >= AssetsManager.Instance.GetAlbumAsset(AlbumEnum.BillboardAlbum).album.Length)
        {
            index = 0;
        }

        if (!billboardDisplayIndexList.Contains(index))
        {
            index = JudgeBillboardIndex(index + dir, dir);
        }

        return index;
    }

    /// <summary>
    /// 設置廣告刊版點
    /// </summary>
    private void SetBillboardPoint()
    {
        foreach (var point in billboardPointList)
        {
            point.color = new Color(155 / 255, 155 / 255, 155 / 255, 255);
        }

        int billboardIndex = billboardDisplayIndexList.Select((v, i) => (v, i))
                                                      .Where(x => x.v == DataManager.CurrBillboardIndex)
                                                      .FirstOrDefault()
                                                      .i;
        billboardPointList[billboardIndex].color = new Color(1, 1, 1, 1);
    }

    #endregion

    #region 積分房

    /// <summary>
    /// 積分房結束配對
    /// </summary>
    private void IntegralEndPair()
    {
        IntegralBtn_Txt.text = LanguageManager.Instance.GetText("INTEGRAL");
        integralData.isPairing = false;
    }

    #endregion

    /// <summary>
    /// 創建房間按鈕
    /// </summary>
    private void CreateRoomBtn()
    {
        //加密貨幣桌        
        CryptoTableBtnSample.SetActive(false);
        float cryptoSpacing = CryptoTableParent.GetComponent<HorizontalLayoutGroup>().spacing;
        Rect cryptoRect = CryptoTableBtnSample.GetComponent<RectTransform>().rect;
        CryptoTableParent.sizeDelta = new Vector2((cryptoRect.width + cryptoSpacing) * DataManager.CryptoSmallBlindList.Count, cryptoRect.height);
        foreach (var smallBlind in DataManager.CryptoSmallBlindList)
        {
            RectTransform rt = Instantiate(CryptoTableBtnSample).GetComponent<RectTransform>();
            rt.gameObject.SetActive(true);
            rt.SetParent(CryptoTableParent);
            rt.GetComponent<CryptoTableBtnSample>().SetCryptoTableBtnInfo(smallBlind, lobbyView);
            rt.localScale = Vector3.one;
        }
        CryptoTableParent.anchoredPosition = Vector2.zero;

        //虛擬貨幣桌
        VCTableBtnSample.SetActive(false);
        float vcSpacing = VCTableParent.GetComponent<HorizontalLayoutGroup>().spacing;
        Rect vcRect = VCTableBtnSample.GetComponent<RectTransform>().rect;
        VCTableParent.sizeDelta = new Vector2((vcRect.width + vcSpacing) * DataManager.VCSmallBlindList.Count, vcRect.height);
        foreach (var smallBlind in DataManager.VCSmallBlindList)
        {
            RectTransform rt = Instantiate(VCTableBtnSample).GetComponent<RectTransform>();
            rt.gameObject.SetActive(true);
            rt.SetParent(VCTableParent);
            rt.GetComponent<VCTableBtnSample>().SetVCTableBtnInfo(smallBlind, lobbyView);
            rt.localScale = Vector3.one;
        }
        VCTableParent.anchoredPosition = Vector2.zero;
    }

    #region Line客服加好友
    public void StartLineLogin()
    {
        string state = GenerateRandomString();
        string nonce = GenerateRandomString();
        string authUrl = $"https://line.me/R/ti/p/%40linedevelopers";
        /*
                    +
                     $"client_id={DataManager.LineChannelId}&" +
                     $"redirect_uri={DataManager.RedirectUri}&" +
                     $"state={state}&" +
                     $"scope=profile%20openid%20email&nonce={nonce}";
        */

        //JSBridgeManager.Instance.LocationHref(authUrl);

       JSBridgeManager.Instance.onLineService(authUrl);
    }
    private string GenerateRandomString(int length = 16)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new System.Random();
        return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
    }
    #endregion

}
