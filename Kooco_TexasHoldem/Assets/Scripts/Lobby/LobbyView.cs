using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Thirdweb;
using System.Threading.Tasks;
using TMPro;

public class LobbyView : MonoBehaviour
{
    [SerializeField]
    Request_LobbyView baseRequest;

    [Header("遮罩物件")]
    [SerializeField]
    GameObject Mask_Obj;

    [Header("用戶訊息")]
    [SerializeField]
    Button Avatar_Btn;
    [SerializeField]
    TextMeshProUGUI Nickname_Txt, Stamina_Txt, CryptoChips_Txt;

    [Header("用戶資源列表")]
    [SerializeField]
    Button ShowAssets_Btn;
    [SerializeField]
    GameObject AssetList_Obj;
    [SerializeField]
    Image ShowAssetsBtn_Img;
    [SerializeField]
    TextMeshProUGUI Assets_CryptoChips_Txt, Assets_VCChips_Txt, Assets_Gold_Txt,
                    Assets_Stamina_Txt, Assets_OTProps_Txt;

    [Header("積分房")]
    [SerializeField]
    Button Integral_Btn;
    [SerializeField]
    TextMeshProUGUI IntegralBtn_Txt;

    [Header("提示")]
    [SerializeField]
    TextMeshProUGUI Tip_Txt;

    [Header("項目按鈕")]
    [SerializeField]
    RectTransform Floor3;
    [SerializeField]
    Button Mine_Btn, Shop_Btn, Main_Btn, Activity_Btn, Ranking_Btn;
    [SerializeField]
    GameObject LobbyMainPageView, LobbyMinePageView, LobbyRankingView, LobbyShopView;
    [SerializeField]
    TextMeshProUGUI MineBtn_Txt, ShopBtn_Txt, ActivityBtn_Txt, RankingBtn_Txt;

    [Header("任務介面")]
    [SerializeField]
    RectTransform Floor4;
    [SerializeField]
    GameObject QuestView;

    [Header("設置暱稱")]
    [SerializeField]
    GameObject SetNicknameViewObj;


    [Header("存提款介面")]
    [SerializeField]
    GameObject Transfers_AnteView;


    /// <summary>
    /// 項目按鈕類型
    /// </summary>
    enum ItemType
    {
        Mine,
        Shop,
        Main,
        Activity,
        Ranking,
    }

    Coroutine tipCorutine;
    bool isShowAssetList;               //是否顯示用戶資源列表

    /// <summary>
    /// 更新文本翻譯
    /// </summary>
    private void UpdateLanguage()
    {
        IntegralBtn_Txt.text = LanguageManager.Instance.GetText("INTEGRAL");

        #region 用戶資源列表

        Assets_CryptoChips_Txt.text = LanguageManager.Instance.GetText("Crypto Table");
        Assets_VCChips_Txt.text = LanguageManager.Instance.GetText("VC Table");
        Assets_Gold_Txt.text = LanguageManager.Instance.GetText("Gold");
        Assets_Stamina_Txt.text = LanguageManager.Instance.GetText("Stamina");
        Assets_OTProps_Txt.text = LanguageManager.Instance.GetText("OT Props");

        #endregion

        #region 項目按鈕

        MineBtn_Txt.text = LanguageManager.Instance.GetText("Mine");
        ShopBtn_Txt.text = LanguageManager.Instance.GetText("Shop");
        ActivityBtn_Txt.text = LanguageManager.Instance.GetText("Activity");
        RankingBtn_Txt.text = LanguageManager.Instance.GetText("Ranking");

        #endregion
    }

    private void OnDestroy()
    {
        LanguageManager.Instance.RemoveLanguageFun(UpdateLanguage);
        WalletManager.Instance.CancelCheckConnect();
    }

    private void Awake()
    {
        battleData = new BattleData();

        LanguageManager.Instance.AddUpdateLanguageFunc(UpdateLanguage);
        ListenerEvent();
    }

    /// <summary>
    /// 事件聆聽
    /// </summary>
    private void ListenerEvent()
    {
        //頭像按鈕
        Avatar_Btn.onClick.AddListener(() =>
        {
            OpenItemPage(ItemType.Mine);
        });

        //顯示用戶資源列表
        ShowAssets_Btn.onClick.AddListener(() =>
        {
            isShowAssetList = !isShowAssetList;
            SetIsShowAssetList = isShowAssetList;
        });

        //積分房
        Integral_Btn.onClick.AddListener(() =>
        {
            if (battleData.isPairing)
            {
                //正在配對取消配對
                EndPair();
            }
            else
            {
                if (GameRoomManager.Instance.JudgeIsCanBeCreateRoom())
                {
                    //開始配對
                    battleData.isPairing = true;
                    battleData.startPairTime = DateTime.Now;
                }
                else
                {
                    //房間數已達上限
                    ShowMaxRoomTip();
                }
            }
        });

        #region 項目按鈕

        //主頁
        Main_Btn.onClick.AddListener(() =>
        {
            OpenItemPage(ItemType.Main);
        });

        //用戶訊息
        Mine_Btn.onClick.AddListener(() =>
        {
            OpenItemPage(ItemType.Mine);
        });

        //排名
        Ranking_Btn.onClick.AddListener(() =>
        {
            OpenItemPage(ItemType.Ranking);
        });

        //商店
        Shop_Btn.onClick.AddListener(() =>
        {
            OpenItemPage(ItemType.Shop);
        });

        #endregion
    }

    private void OnEnable()
    {
        Color tipColor = Tip_Txt.color;
        tipColor.a = 0;
        Tip_Txt.color = tipColor;

        isShowAssetList = false;
        SetIsShowAssetList = isShowAssetList;

        HandHistoryManager.Instance.LoadHandHistoryData();

        UpdateUserInfo();
        OpenItemPage(ItemType.Main);
    }

    private void Start()
    {
        StartCoroutine(IOpenSetNickname());
    }

    private void Update()
    {
        //積分配對計時器
        if (battleData.isPairing)
        {
            TimeSpan waitingTime = DateTime.Now - battleData.startPairTime;
            IntegralBtn_Txt.text = $"{LanguageManager.Instance.GetText("Pairing")}:{(int)waitingTime.TotalMinutes} : {waitingTime.Seconds:00}";

            if (waitingTime.Seconds >= 3)
            {
                baseRequest.SendRequest_InBattleRoom();
                EndPair();
            }
        }

        //  任務生成測試
        if (Input.GetKeyDown(KeyCode.Q))
        {
            DisplayFloor4UI(QuestView);
        }

        //  存提款介面測試
        if (Input.GetKeyDown(KeyCode.S))
        {
            DisplayFloor4UI(Transfers_AnteView);
        }
        

#if UNITY_EDITOR

        if (Input.GetKeyDown(KeyCode.E))
        {
            WalletManager.Instance.OnWalletDisconnect();
            LoadSceneManager.Instance.LoadScene(SceneEnum.Login);
        }

#endif
    }

    /// <summary>
    /// 開啟設置暱稱
    /// </summary>
    /// <returns></returns>
    private IEnumerator IOpenSetNickname()
    {
        Mask_Obj.SetActive(true);
        yield return new WaitForSeconds(0.5f);

        Instantiate(SetNicknameViewObj, transform);
        Mask_Obj.SetActive(false);
    }

    /// <summary>
    /// 是否顯示用戶資源列表
    /// </summary>
    private bool SetIsShowAssetList
    {
        set
        {
            AssetList_Obj.SetActive(value);
            ShowAssetsBtn_Img.sprite = value == false ?
                           AssetsManager.Instance.GetAlbumAsset(AlbumEnum.ArrowAlbum).album[1] :
                           AssetsManager.Instance.GetAlbumAsset(AlbumEnum.ArrowAlbum).album[0];
        }
    }

    /// <summary>
    /// 開啟項目頁面
    /// </summary>
    /// <param name="itemType"></param>
    private void OpenItemPage(ItemType itemType)
    {
        for (int i = 0; i < Floor3.childCount; i++)
        {
            Destroy(Floor3.GetChild(i).gameObject);
        }

        GameObject itemObj = null;
        switch (itemType)
        {
            //主頁
            case ItemType.Main:
                itemObj = LobbyMainPageView;
                break;

            //用戶訊息
            case ItemType.Mine:
                itemObj = LobbyMinePageView;
                break;

            //排名
            case ItemType.Ranking:
                itemObj = LobbyRankingView;
                break;

            //商店
            case ItemType.Shop:
                itemObj = LobbyShopView;
                break;
        }

        if (itemObj != null)
        {
            RectTransform itemPageView = Instantiate(itemObj, Floor3).GetComponent<RectTransform>();
            ViewManager.Instance.InitViewTr(itemPageView, itemType.ToString());
        }
    }

    /// <summary>
    /// 更新用戶訊息
    /// </summary>
    public void UpdateUserInfo()
    {
        Nickname_Txt.text = $"@{DataManager.UserNickname}";
        Avatar_Btn.image.sprite = AssetsManager.Instance.GetAlbumAsset(AlbumEnum.AvatarAlbum).album[DataManager.UserAvatar];
        Stamina_Txt.text = $"{DataManager.UserStamina}/{DataManager.MaxStaminaValue}";
        CryptoChips_Txt.text = string.IsNullOrEmpty(DataManager.UserWalletBalance) ? "0" : DataManager.UserWalletBalance;
    }

    /// <summary>
    /// 積分房資料
    /// </summary>
    private BattleData battleData;
    public class BattleData
    {
        public bool isPairing;              //是否正在配對中
        public DateTime startPairTime;      //開始配對時間
    }

    /// <summary>
    /// 結束配對
    /// </summary>
    private void EndPair()
    {
        IntegralBtn_Txt.text = LanguageManager.Instance.GetText("INTEGRAL");
        battleData.isPairing = false;
    }

    /// <summary>
    /// 顯示已達房間數量提示
    /// </summary>
    public void ShowMaxRoomTip()
    {
        if (tipCorutine != null) StopCoroutine(tipCorutine);
        tipCorutine = StartCoroutine(IShowTip(LanguageManager.Instance.GetText("MaxRoomTip")));
    }

    /// <summary>
    /// 顯示提示
    /// </summary>
    /// <param name="tipContent">提示內容</param>
    /// <returns></returns>
    public IEnumerator IShowTip(string tipContent)
    {
        float showTime = 0.5f;

        Tip_Txt.text = tipContent;
        Color tipColor = Tip_Txt.color;
        tipColor.a = 0;

        DateTime startTime = DateTime.Now;

        while ((DateTime.Now - startTime).TotalSeconds < showTime)
        {
            float progress = (float)(DateTime.Now - startTime).TotalSeconds / showTime;
            float alpha = Mathf.Lerp(0, 1, progress);
            tipColor.a = alpha;
            Tip_Txt.color = tipColor;

            yield return null;
        }

        tipColor.a = 1;
        Tip_Txt.color = tipColor;

        yield return new WaitForSeconds(2.5f);

        startTime = DateTime.Now;

        while ((DateTime.Now - startTime).TotalSeconds < showTime)
        {
            float progress = (float)(DateTime.Now - startTime).TotalSeconds / showTime;
            float alpha = Mathf.Lerp(1, 0, progress);
            tipColor.a = alpha;
            Tip_Txt.color = tipColor;

            yield return null;
        }

        tipColor.a = 0;
        Tip_Txt.color = tipColor;
    }

    /// <summary>
    /// 開啟Floor4介面
    /// </summary>
    public void DisplayFloor4UI(GameObject UIobj)
    {
        if (Floor4.childCount < 1)
        {
            Instantiate(UIobj, Floor4);
        }
        else
        {
            
            Destroy(Floor4.GetChild(0).gameObject);
        }
    }
}
