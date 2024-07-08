using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using System.Linq;
using TMPro;

public class LobbyMinePageView : MonoBehaviour
{
    [Header("Mask")]
    [SerializeField]
    Mask Viewport;

    [Header("用戶訊息")]
    [SerializeField]
    GameObject UserPorfile_Obj;
    [SerializeField]
    Button EditorAvatar_Btn, CopyWalletAddress_Btn;
    [SerializeField]
    TextMeshProUGUI Nickname_Txt, WalletAddress_Txt, Copied_Txt;

    [Header("更換頭像")]
    [SerializeField]
    RectTransform ChangeAvatar_Tr, AvatarListParent_Tr, SelectAvatarIcon_Tr;
    [SerializeField]
    GameObject AvatarSapmle;
    [SerializeField]
    Button CloseChangeAvatar_Btn, ChangeAvatarSubmit_Btn;

    [Header("帳戶餘額")]
    [SerializeField]
    RectTransform AccountBalance_Obj;
    [SerializeField]
    Button AccountBalanceExpand_Btn, AccountBalanceReflash_Btn;
    [SerializeField]
    Image AccountBalanceExpand_Img;
    [SerializeField]
    TextMeshProUGUI CryptoTableValue_Txt, VCTableValue_Txt, GoldValue_Txt, StaminaValue_Txt, OTPropsValue_Txt;

    [Header("分數紀錄")]
    [SerializeField]
    RectTransform ScoreRecord_Obj;
    [SerializeField]
    Button ScoreRecordExpand_Btn, ScoreRecordReflash_Btn;
    [SerializeField]
    Image ScoreRecordExpand_Img;
    [SerializeField]
    Slider VPIP_Sli, PFR_Sli, ATS_Sli, ThreeBET_Sli;
    [SerializeField]
    TextMeshProUGUI VPIPpercent_Txt, PFRpercent_Txt, ATSpercent_Txt, ThreeBETpercent_Txt;

    [Header("第三方連接")]
    [SerializeField]
    Button IGLink_Btn, LineLink_Btn;
    [SerializeField]
    TextMeshProUGUI IGLinked_Txt, LineLinked_Txt;
    [SerializeField]
    GameObject IGNotYetLinked_Obj, LineNotYetLinked_Obj;
    [SerializeField]
    Image IGLinked_Img, LineLinked_Img;
    const string expandContentName = "Content";                                 //展開內容物件名稱
    const string expandTopBgName = "TopBg";                                     //收起上方物件名稱
    const float expandTIme = 0.1f;                                              //內容展開時間

    [Header("交易紀錄")]
    [SerializeField]
    Button TransactionHistory_Btn;
    [SerializeField]
    GameObject TransactionHistoryViewObj;

    [Header("My NFT")]
    [SerializeField]
    Button MyNFT_Btn;
    [SerializeField]
    GameObject MyNFTViewObj;

    [Header("手牌紀錄")]
    [SerializeField]
    Button HandHistory_Btn;
    [SerializeField]
    GameObject LobbyHandHistoryViewObj;

    List<Button> avatarBtnList;                                                 //頭像按鈕
    int tempAvatarIndex;                                                        //零時頭像index
    bool isAccountBalanceExpand;                                                //是否展開帳戶餘額
    bool isScoreRecordExpand;                                                   //是否展開分數紀錄

    private void Awake()
    {
        //錢包地址已複製文字
        Color color = Copied_Txt.color;
        color.a = 0;
        Copied_Txt.color = color;

        ChangeAvatar_Tr.gameObject.SetActive(false);

        //紀錄展開物件初始化
        List<RectTransform> expandObjList = new()
        {
            AccountBalance_Obj,     //帳戶餘額
            ScoreRecord_Obj,        //分數紀錄
        };
        foreach (var expandObj in expandObjList)
        {
            RectTransform contentObj = expandObj.Find(expandContentName).GetComponent<RectTransform>();
            RectTransform TopBgObj = expandObj.Find(expandTopBgName).GetComponent<RectTransform>();
            contentObj.gameObject.SetActive(false);
            expandObj.sizeDelta = new Vector2(expandObj.rect.width, TopBgObj.rect.height);
        }
        AccountBalanceExpand_Img.sprite = AssetsManager.Instance.GetAlbumAsset(AlbumEnum.ArrowAlbum).album[1];
        ScoreRecordExpand_Img.sprite = AssetsManager.Instance.GetAlbumAsset(AlbumEnum.ArrowAlbum).album[1];

        ListenerEvent();
    }

    /// <summary>
    /// 事件聆聽
    /// </summary>
    private void ListenerEvent()
    {
        #region 用戶訊息

        //複製錢包地址
        CopyWalletAddress_Btn.onClick.AddListener(() =>
        {
            if (!string.IsNullOrEmpty(WalletAddress_Txt.text))
            {
                StringUtils.CopyText(DataManager.UserWalletAddress);
                UnityUtils.Instance.ColorFade(WalletAddress_Txt,
                                              null,
                                              0.2f,
                                              0.5f,
                                              1.5f);
            }
        });

        //開啟更換頭像
        EditorAvatar_Btn.onClick.AddListener(() =>
        {
            UserPorfile_Obj.SetActive(false);
            ChangeAvatar_Tr.gameObject.SetActive(true);
        });

        //關閉選擇頭像
        CloseChangeAvatar_Btn.onClick.AddListener(() =>
        {
            UserPorfile_Obj.SetActive(true);
            ChangeAvatar_Tr.gameObject.SetActive(false);
        });

        //提交更換頭像
        ChangeAvatarSubmit_Btn.onClick.AddListener(() =>
        {
            DataManager.UserAvatar = tempAvatarIndex;
            EditorAvatar_Btn.image.sprite = AssetsManager.Instance.GetAlbumAsset(AlbumEnum.AvatarAlbum).album[DataManager.UserAvatar];
            GameObject.FindAnyObjectByType<LobbyView>().UpdateUserInfo();

            UserPorfile_Obj.SetActive(true);
            ChangeAvatar_Tr.gameObject.SetActive(false);
        });

        #endregion

        #region 帳戶餘額

        //帳戶餘額展開
        AccountBalanceExpand_Btn.onClick.AddListener(() =>
        {
            isAccountBalanceExpand = !isAccountBalanceExpand;
            StartCoroutine(ISwitchContent(isAccountBalanceExpand,
                                          AccountBalance_Obj,
                                          AccountBalanceExpand_Img));
        });

        //帳戶餘額刷新
        AccountBalanceReflash_Btn.onClick.AddListener(() =>
        {
            UpdatetAccountBalance("2,000 ETH", 40000, 3000, 5, 30);
        });

        #endregion

        #region 分數紀錄

        //分數紀錄展開
        ScoreRecordExpand_Btn.onClick.AddListener(() =>
        {
            isScoreRecordExpand = !isScoreRecordExpand;
            StartCoroutine(ISwitchContent(isScoreRecordExpand,
                                          ScoreRecord_Obj,
                                          ScoreRecordExpand_Img));
        });

        //紀錄分數刷新
        ScoreRecordReflash_Btn.onClick.AddListener(() =>
        {
            UpdateScoreRecord(70, 33, 25, 60);
        });

        #endregion

        #region 第三方連接

        //IG連接
        IGLink_Btn.onClick.AddListener(() =>
        {
            StartInstagram();
        });

        //Line連接
        LineLink_Btn.onClick.AddListener(() =>
        {
            StartLineLogin();
        });

        #endregion

        #region 交易紀錄

        //交易紀錄按鈕
        TransactionHistory_Btn.onClick.AddListener(() =>
        {
            Transform lobbyView = GameObject.Find("LobbyView").transform;
            RectTransform transactionHistoryView = Instantiate(TransactionHistoryViewObj, lobbyView).GetComponent<RectTransform>();
            ViewManager.Instance.InitViewTr(transactionHistoryView, "TransactionHistoryView");
        });

        #endregion

        #region My NFT

        MyNFT_Btn.onClick.AddListener(() =>
        {
            Transform lobbyView = GameObject.Find("LobbyView").transform;
            RectTransform MyNFTView = Instantiate(MyNFTViewObj, lobbyView).GetComponent<RectTransform>();
            ViewManager.Instance.InitViewTr(MyNFTView, "MyNFTView");
        });

        #endregion

        #region 手牌紀錄

        HandHistory_Btn.onClick.AddListener(() =>
        {
            Transform lobbyView = GameObject.Find("LobbyView").transform;
            RectTransform lobbyHandHistoryView = Instantiate(LobbyHandHistoryViewObj, lobbyView).GetComponent<RectTransform>();
            ViewManager.Instance.InitViewTr(lobbyHandHistoryView, "LobbyHandHistoryView");
        });

        #endregion
    }

    private void OnEnable()
    {
        SetUserInfo();
    }

    private void Start()
    {
        //產生選擇的頭像
        avatarBtnList = new List<Button>();
        AvatarSapmle.gameObject.SetActive(false);
        Sprite[] avatars = AssetsManager.Instance.GetAlbumAsset(AlbumEnum.AvatarAlbum).album;
        for (int i = 0; i < avatars.Length; i++)
        {
            Button avatarBtn = Instantiate(AvatarSapmle, AvatarListParent_Tr).GetComponent<Button>();
            avatarBtn.gameObject.SetActive(true);
            avatarBtn.image.sprite = avatars[i];
            int index = i;

            avatarBtn.onClick.AddListener(() =>
            {
                SelectAvatarIcon_Tr.SetParent(avatarBtn.transform);
                SelectAvatarIcon_Tr.anchoredPosition = Vector2.zero;
                tempAvatarIndex = index;
            });

            avatarBtnList.Add(avatarBtn);
        }
        SelectAvatarIcon_Tr.SetParent(avatarBtnList[DataManager.UserAvatar].transform);
        SelectAvatarIcon_Tr.anchoredPosition = Vector2.zero;


        Viewport.enabled = true;

        UpdatetAccountBalance(string.IsNullOrEmpty(DataManager.UserWalletBalance) ? "0" : DataManager.UserWalletBalance,
                              DataManager.UserVCChips,
                              DataManager.UserGoldChips,
                              DataManager.UserStamina,
                              DataManager.UserOTProps);

        UpdateScoreRecord(50, 60, 70, 80);
    }

    /// <summary>
    /// 設置用戶訊息
    /// </summary>
    private void SetUserInfo()
    {
        //頭像
        EditorAvatar_Btn.image.sprite = AssetsManager.Instance.GetAlbumAsset(AlbumEnum.AvatarAlbum).album[DataManager.UserAvatar];

        //暱稱
        Nickname_Txt.text = $"@{DataManager.UserNickname}";

        //錢包地址
        StringUtils.StrExceedSize(DataManager.UserWalletAddress, WalletAddress_Txt);

        //IG連接
        IGNotYetLinked_Obj.SetActive(string.IsNullOrEmpty(DataManager.IGIUserIdAndName));
        IGLink_Btn.interactable = string.IsNullOrEmpty(DataManager.IGIUserIdAndName);                                  
        IGLinked_Txt.text = string.IsNullOrEmpty(DataManager.IGIUserIdAndName) ?
                            "LINK NOW" :
                            "LINKED";
        IGLinked_Img.sprite = string.IsNullOrEmpty(DataManager.LineMail) ?
                        AssetsManager.Instance.GetAlbumAsset(AlbumEnum.LinkAlbum).album[0] :
                        AssetsManager.Instance.GetAlbumAsset(AlbumEnum.LinkAlbum).album[1];

        //Line連接
        LineNotYetLinked_Obj.SetActive(string.IsNullOrEmpty(DataManager.LineMail));
        LineLink_Btn.interactable = string.IsNullOrEmpty(DataManager.IGIUserIdAndName);
        LineLinked_Txt.text = string.IsNullOrEmpty(DataManager.LineMail) ?
                            "LINK NOW" :
                            "LINKED";
        LineLinked_Img.sprite = string.IsNullOrEmpty(DataManager.LineMail) ?
                                AssetsManager.Instance.GetAlbumAsset(AlbumEnum.LinkAlbum).album[0] :
                                AssetsManager.Instance.GetAlbumAsset(AlbumEnum.LinkAlbum).album[1];
    }

    /// <summary>
    /// 更新帳號餘額
    /// </summary>
    /// <param name="crypto"></param>
    /// <param name="vc"></param>
    /// <param name="gold"></param>
    /// <param name="Stamina"></param>
    /// <param name="ot"></param>
    private void UpdatetAccountBalance(string crypto, double vc, double gold, int Stamina, int ot)
    {
        DataManager.UserWalletBalance = crypto.ToString();
        DataManager.UserStamina = Stamina;

        CryptoTableValue_Txt.text = crypto.ToString();
        VCTableValue_Txt.text = StringUtils.SetChipsUnit(vc);
        GoldValue_Txt.text = StringUtils.SetChipsUnit(gold);
        StaminaValue_Txt.text = $"{DataManager.UserStamina}/{DataManager.MaxStaminaValue}";
        OTPropsValue_Txt.text = $"{ot}";

        GameObject.FindAnyObjectByType<LobbyView>().UpdateUserInfo();
    }

    /// <summary>
    /// 更新分數紀錄
    /// </summary>
    /// <param name="vpip"></param>
    /// <param name="pfr"></param>
    /// <param name="ats"></param>
    /// <param name="threeBet"></param>
    private void UpdateScoreRecord(float vpip, float pfr, float ats, float threeBet)
    {
        VPIP_Sli.value = vpip / 100;
        VPIPpercent_Txt.text = $"{vpip}%";

        PFR_Sli.value = pfr / 100;
        PFRpercent_Txt.text = $"{pfr}%";

        ATS_Sli.value = ats / 100;
        ATSpercent_Txt.text = $"{ats}%";

        ThreeBET_Sli.value = threeBet / 100;
        ThreeBETpercent_Txt.text = $"{threeBet}%";
    }

    /// <summary>
    /// 介面內容展開縮放
    /// </summary>
    /// <param name="isExpand">是否展開</param>
    /// <param name="rt">展開內容物件</param>
    /// <param name="img">展開按鈕圖</param>
    /// <param name="completeCallback">完成回傳</param>
    /// <returns></returns>
    private IEnumerator ISwitchContent(bool isExpand, RectTransform rt, Image img, UnityAction completeCallback = null)
    {
        //展開內容物件
        RectTransform contentObj = rt.Find(expandContentName).GetComponent<RectTransform>();
        //收回高度
        float pullbackHeight = rt.Find(expandTopBgName).GetComponent<RectTransform>().rect.height;
        //目標高度
        float targetHeight = isExpand == true ?
                             contentObj.rect.height :
                             pullbackHeight;
        //初始高度
        float initHeight = isExpand == true ?
                           pullbackHeight :
                           rt.rect.height;

        contentObj.gameObject.SetActive(false);
        rt.sizeDelta = new Vector2(rt.rect.width, initHeight);

        DateTime startTime = DateTime.Now;
        while ((DateTime.Now - startTime).TotalSeconds < expandTIme)
        {
            float progress = (float)(DateTime.Now - startTime).TotalSeconds / expandTIme;
            float height = Mathf.Lerp(initHeight, targetHeight, progress);
            rt.sizeDelta = new Vector2(rt.rect.width, height);     

            yield return null;
        }

        contentObj.gameObject.SetActive(isExpand);
        rt.sizeDelta = new Vector2(rt.rect.width, targetHeight);
        img.sprite = isExpand == true ?
                     AssetsManager.Instance.GetAlbumAsset(AlbumEnum.ArrowAlbum).album[0] :
                     AssetsManager.Instance.GetAlbumAsset(AlbumEnum.ArrowAlbum).album[1];

        completeCallback?.Invoke();
    }

    #region 第三方連接

    /// <summary>
    /// 開始Instagram登入
    /// </summary>
    public void StartInstagram()
    {
        string authUrl = $"https://api.instagram.com/oauth/authorize?client_id=" +
                         $"{DataManager.InstagramChannelID}&redirect_uri={DataManager.InstagramRedirectUri}" +
                         $"&scope=user_profile,user_media&response_type=code";
        Application.OpenURL(authUrl);
    }

    /// <summary>
    /// 開始Line登入
    /// </summary>
    public void StartLineLogin()
    {
        string state = GenerateRandomString();
        string nonce = GenerateRandomString();
        string authUrl = $"https://access.line.me/oauth2/v2.1/authorize?response_type=code&" +
                         $"client_id={DataManager.LineChannelId}&" +
                         $"redirect_uri={DataManager.RedirectUri}&" +
                         $"state={state}&" +
                         $"scope=profile%20openid%20email&nonce={nonce}";


        JSBridgeManager.Instance.LocationHref(authUrl);
    }
    private string GenerateRandomString(int length = 16)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new System.Random();
        return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
    }

    #endregion
}
