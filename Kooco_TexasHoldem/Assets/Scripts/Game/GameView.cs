using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using TMPro;

using RequestBuf;

public class GameView : MonoBehaviour
{
    [SerializeField]
    Request_GameView baseRequest;

    [Header("遮罩按鈕")]
    [SerializeField]
    Button Mask_Btn;

    [Header("座位上玩家訊息")]
    [SerializeField]
    List<GamePlayerInfo> SeatGamePlayerInfoList;
    [SerializeField]
    List<Button> SeatButtonList;

    [Header("操作按鈕")]
    [SerializeField]
    Button Raise_Btn, Call_Btn, Fold_Btn;
    [SerializeField]
    TextMeshProUGUI RaiseBtn_Txt, CallBtn, FoldBtn_Txt;
    [SerializeField]
    RectTransform AutoActionFrame_Tr;
    [SerializeField]
    Button[] ShowPokerBtnList;
    [SerializeField]
    Button BackToSit_Btn;

    [Header("加注操作")]
    [SerializeField]
    List<Button> PotPercentRaiseBtnList;
    [SerializeField]
    List<TextMeshProUGUI> PotPercentRaiseTxtList;
    [SerializeField]
    Slider Raise_Sli;
    [SerializeField]
    RectTransform Raise_Tr;
    [SerializeField]
    SliderClickDetection SliderClickDetection;
    [SerializeField]
    TextMeshProUGUI RaiseSliHandle_Txt, CurrRaise_Txt, MinRaiseBtn_Txt;
    [SerializeField]
    Button AllIn_Btn, MinRaise_Btn;

    [Header("底池")]
    [SerializeField]
    TextMeshProUGUI TotalPot_Txt, Tip_Txt, WinType_Txt;
    [SerializeField]
    Image Pot_Img;

    [Header("公共牌")]
    [SerializeField]
    List<Poker> CommunityPokerList;

    [Header("選單")]
    [SerializeField]
    RectTransform MenuPage_Tr;
    [SerializeField]
    Button Menu_Btn, MenuClose_Btn, SitOut_Btn, BuyChips_Btn, LogOut_Btn, HandHistory_Btn;
    [SerializeField]
    TextMeshProUGUI SitOutBtn_Txt, MenuNickname_Txt, MenuWalletAddr_Txt;
    [SerializeField]
    Image MenuAvatar_Img;

    [Header("聊天")]
    [SerializeField]
    RectTransform ChatPage_Tr, ChatContent_Tr, NotReadChat_Tr;
    [SerializeField]
    Button Chat_Btn, ChatClose_Btn, ChatSend_Btn, NewMessage_Btn;
    [SerializeField]
    TMP_InputField Chat_If;
    [SerializeField]
    GameObject OtherChatSample, LocalChatSample;
    [SerializeField]
    ScrollRect ChatArea_Sr;
    [SerializeField]
    TextMeshProUGUI NotReadChat_Txt;

    [Header("手牌紀錄")]
    [SerializeField]
    RectTransform HandHistoryPage_Tr;
    [SerializeField]
    Button HandHistoryClose_Btn;

    [Header("購買籌碼")]
    [SerializeField]
    BuyChipsView buyChipsView;

    [Header("遊戲結果")]
    [SerializeField]
    RectTransform BattleResultView;
    [SerializeField]
    GameObject WinChipsObj;

    [Header("遊戲暫停")]
    [SerializeField]
    GameObject GamePause_Obj;
    [SerializeField]
    Button GameContinue_Btn;

    const float PageMoveTime = 0.25f;                           //滑動頁面移動時間

    //底池倍率
    readonly float[] PotPercentRate = new float[]
    {
        33, 50, 80, 100,
    };

    //加註大盲倍率
    readonly float[] PotBbRate = new float[]
    {
        2.1f, 2.5f, 3.0f, 4.0f,
    };

    const int MaxChatCount = 5;                                //保留聊天最大訊息數

    ObjPool objPool;
    List<GamePlayerInfo> gamePlayerInfoList;                    //玩家資料

    Vector2 InitPotPointPos;                                    //初始底池位置
    int notReadMsgCount;                                        //未讀取數

    #region 遊戲過程紀錄

    List<int> exitPlayerSeatList;                               //玩家離開座位
    GameInitHistoryData gameInitHistoryData;                    //遊戲初始資料紀錄
    ProcessHistoryData processHistoryData;                      //遊戲過程資料紀錄
    ResultHistoryData saveResultData;                           //遊戲結果資料紀錄

    #endregion

    private TableTypeEnum roomType;
    /// <summary>
    /// 房間類型
    /// </summary>
    public TableTypeEnum RoomType
    {
        get
        {
            return roomType;
        }
        set
        {
            roomType = value;

            //積分房
            if (roomType == TableTypeEnum.IntegralTable)
            {
                BuyChips_Btn.interactable = false;
                SitOut_Btn.interactable = false;

                for (int i = 0; i < SeatButtonList.Count; i++)
                {
                    if (i != 0 && i != 3)
                    {
                        SeatButtonList[i].gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    private ThisData thisData;
    public class ThisData
    {
        public GamePlayerInfo LocalGamePlayerInfo;         //本地玩家
        public int LocalPlayerSeat;                        //本地玩家座位
        public double LocalPlayerChips;                    //本地玩家籌碼
        public double LocalPlayerCurrBetValue;             //本地玩家當前下注值
        public double TotalPot;                            //當前底池
        public double CallDifference;                      //當前跟注差額
        public double CurrCallValue;                       //當前跟注值
        public double CurrRaiseValue;                      //當前加注值
        public double MinRaiseValue;                       //最小加注值
        public double SmallBlindValue;                     //小盲值
        public bool IsFirstRaisePlayer;                    //首位加注玩家
        public bool IsUnableRaise;                         //無法加注
        public bool IsPlaying;                             //有參與遊戲
        public bool IsSitOut;                              //是否離開座位
        public bool isLocalPlayerTurn;                     //本地玩家回合
        public bool isFold;                                //是否已棄牌
        public List<int> CurrCommunityPoker;               //當前公共牌
        public List<string> PotWinnerList;                 //主池贏家
        public double PowWinChips;                         //主池贏得籌碼
        public List<string> SideWinnerList;                //邊池贏家
        public double SideWinChips;                        //邊池贏得籌碼
        public Dictionary<int, double> BackChipsDic;       //退回籌碼(座位,退回籌碼值)
    }

    private StrData strData;
    public class StrData
    {
        public string FoldStr { get; set; }
        public string CallStr { get; set; }
        public string CallValueStr { get; set; }
        public string RaiseStr { get; set; }
        public string RaiseValueStr { get; set; }
    }

    /// <summary>
    /// 更新文本翻譯
    /// </summary>
    private void UpdateLanguage()
    {
        #region 操作按鈕

        FoldBtn_Txt.text = LanguageManager.Instance.GetText(strData.FoldStr);
        CallBtn.text = LanguageManager.Instance.GetText(strData.CallStr) + strData.CallValueStr;
        RaiseBtn_Txt.text = LanguageManager.Instance.GetText(strData.RaiseStr) + strData.RaiseValueStr;

        #endregion
    }

    public void Awake()
    {
        objPool = new ObjPool(transform, MaxChatCount);

        ListenerEvent();

        //初始底池位置
        InitPotPointPos = Pot_Img.rectTransform.anchoredPosition;
    }

    /// <summary>
    /// 聆聽事件
    /// </summary>
    private void ListenerEvent()
    {
        //遮罩按鈕
        Mask_Btn.onClick.AddListener(() =>
        {
            if (MenuPage_Tr.gameObject.activeSelf)
            {
                Mask_Btn.gameObject.SetActive(false);
                StartCoroutine(UnityUtils.Instance.IViewSlide(false,
                                                              MenuPage_Tr,
                                                              DirectionEnum.Left,
                                                              PageMoveTime,
                                                              () =>
                                                              {
                                                                  GameRoomManager.Instance.IsCanMoveSwitch = true;
                                                              }));
            }
            else if (ChatPage_Tr.gameObject.activeSelf)
            {
                CloseChatPage();
            }
        });

        //遊戲繼續按鈕
        GameContinue_Btn.onClick.AddListener(() =>
        {
            GameRoomManager.Instance.OnGamePause(false);
        });

        #region 選單

        //開啟選單
        Menu_Btn.onClick.AddListener(() =>
        {
            GameRoomManager.Instance.IsCanMoveSwitch = false;
            Mask_Btn.gameObject.SetActive(true);
            StartCoroutine(UnityUtils.Instance.IViewSlide(true,
                                                          MenuPage_Tr,
                                                          DirectionEnum.Left,
                                                          PageMoveTime));
        });

        //離開房間
        LogOut_Btn.onClick.AddListener(() =>
        {
            ConfirmView confirmView = ViewManager.Instance.OpenConfirmView();
            confirmView.SetContent("return to the lobby?",
                                    "If you leave now, you will not be able to get back your staked chips.");
            confirmView.SetBnt(() =>
            {
                GameRoomManager.Instance.RemoveGameRoom(transform.name);
            });            
        });

        //關閉選單
        MenuClose_Btn.onClick.AddListener(() =>
        {
            CloseMenu();
        });

        //購買籌碼
        BuyChips_Btn.onClick.AddListener(() =>
        {
            buyChipsView.gameObject.SetActive(true);
            buyChipsView.SetBuyChipsViewInfo(true,
                                             thisData.SmallBlindValue,
                                             transform.name,
                                             RoomType,
                                             SendRequest_BuyChips);
        });

        //離開/回到座位
        SitOut_Btn.onClick.AddListener(() =>
        {
            thisData.IsSitOut = !thisData.IsSitOut;
            SetSitOutDisplay(thisData.IsSitOut);
            baseRequest.SendRequest_SitOut(thisData.IsSitOut);

            CloseMenu();
        });

        #endregion

        #region 操作按鈕

        //回到座位
        BackToSit_Btn.onClick.AddListener(() =>
        {
            thisData.IsSitOut = false;
            SetSitOutDisplay(thisData.IsSitOut);
            baseRequest.SendRequest_SitOut(thisData.IsSitOut);
        });

        //棄牌顯示手牌按鈕
        for (int i = 0; i < ShowPokerBtnList.Count(); i++)
        {
            int index = i;
            ShowPokerBtnList[i].onClick.AddListener(delegate { ShowFoldPoker(index); });
        }

        //加注滑條
        Raise_Sli.onValueChanged.AddListener((value) =>
        {
            float newRaiseValue = TexasHoldemUtil.SliderValueChange(Raise_Sli,
                                                                    value,
                                                                    (float)thisData.SmallBlindValue * 2,
                                                                    (float)thisData.MinRaiseValue,
                                                                    (float)thisData.LocalPlayerChips,
                                                                    SliderClickDetection);

            strData.RaiseStr = newRaiseValue >= thisData.LocalPlayerChips ?
                               "AllIn" :
                               "RaiseTo";
            strData.RaiseValueStr = newRaiseValue >= thisData.LocalPlayerChips ?
                                    $"\n{StringUtils.SetChipsUnit(thisData.LocalPlayerChips)}" :
                                    $"\n{StringUtils.SetChipsUnit(newRaiseValue)}";
            //RaiseBtn_Txt.text = LanguageManager.Instance.GetText(strData.RaiseStr) + strData.RaiseValueStr;

            SetRaiseToText = newRaiseValue;
            thisData.CurrRaiseValue = (int)newRaiseValue;
        });

        //最小加注
        MinRaise_Btn.onClick.AddListener(() =>
        {
            Raise_Sli.value = (float)thisData.MinRaiseValue;
        });

        //All In
        AllIn_Btn.onClick.AddListener(() =>
        {
            Raise_Sli.value = (float)thisData.LocalPlayerChips;
        });

        //底池百分比加註
        for (int i = 0; i < PotPercentRaiseBtnList.Count; i++)
        {
            int index = i;
            PotPercentRaiseBtnList[i].onClick.AddListener(delegate { PotRaisePercent(index); });
        }

        //棄牌
        Fold_Btn.onClick.AddListener(() =>
        {
            if (thisData.isLocalPlayerTurn)
            {
                OnFold();
            }
            else
            {
                AutoActionState = AutoActionState == AutoActingEnum.CheckAndFold ?
                                  AutoActingEnum.None :
                                  AutoActingEnum.CheckAndFold;
            }
        });

        //跟注/過牌
        Call_Btn.onClick.AddListener(() =>
        {
            if (thisData.isLocalPlayerTurn)
            {
                OnCallAndCheck();
            }
            else
            {
                AutoActionState = AutoActionState == AutoActingEnum.Check ?
                             AutoActingEnum.None :
                             AutoActingEnum.Check;
            }
        });

        //加注/All In
        Raise_Btn.onClick.AddListener(() =>
        {
            if (thisData.isLocalPlayerTurn)
            {
                bool isAllIn = thisData.LocalPlayerChips < thisData.MinRaiseValue ||
                               thisData.CurrRaiseValue == thisData.LocalPlayerChips;

                ActingEnum acting = isAllIn == true ?
                                    ActingEnum.AllIn :
                                    ActingEnum.Raise;

                if (Raise_Tr.gameObject.activeSelf || isAllIn == true)
                {
                    double betValue = isAllIn == true ?
                                  thisData.LocalPlayerChips :
                                  thisData.CurrRaiseValue;

                    baseRequest.SendRequest_PlayerActed(Entry.TestInfoData.LocalUserId,
                                                        acting,
                                                        betValue);
                }
                else
                {
                    Raise_Tr.gameObject.SetActive(true);
                    strData.RaiseStr = "RaiseTo";
                    strData.RaiseValueStr = $"\n{StringUtils.SetChipsUnit(thisData.CurrRaiseValue)}";
                    //RaiseBtn_Txt.text = LanguageManager.Instance.GetText(strData.RaiseStr) + strData.RaiseValueStr;
                }
            }
            else
            {
                AutoActionState = AutoActionState == AutoActingEnum.CallAny ?
                                  AutoActingEnum.None :
                                  AutoActingEnum.CallAny;
            }
        });

        #endregion

        #region 聊天

        //開啟聊天
        Chat_Btn.onClick.AddListener(() =>
        {
            SetNotReadChatCount = 0;
            GameRoomManager.Instance.IsCanMoveSwitch = false;
            Mask_Btn.gameObject.SetActive(true);
            StartCoroutine(UnityUtils.Instance.IViewSlide(true,
                                                          ChatPage_Tr,
                                                          DirectionEnum.Left,
                                                          PageMoveTime));
            StartCoroutine(IYieldSetNewMessageActive());
        });

        //發送聊天訊息
        ChatSend_Btn.onClick.AddListener(() =>
        {
            SendChat();
        });

        //關閉聊天
        ChatClose_Btn.onClick.AddListener(() =>
        {
            CloseChatPage();
        });

        //聊天移動至最新訊息位置
        NewMessage_Btn.onClick.AddListener(() =>
        {
            StartCoroutine(IGoNewChatMessage());
        });

        //聊天區域
        ChatArea_Sr.onValueChanged.AddListener((value) =>
        {
            if (NewMessage_Btn.gameObject.activeSelf &&
                IsChatOnBottom())
            {
                NewMessage_Btn.gameObject.SetActive(false);
            }
        });

        #endregion

        #region 手牌紀錄

        //開啟手牌紀錄
        HandHistory_Btn.onClick.AddListener(() =>
        {
            Mask_Btn.gameObject.SetActive(false);
            StartCoroutine(UnityUtils.Instance.IViewSlide(false,
                                                          MenuPage_Tr,
                                                          DirectionEnum.Left,
                                                          PageMoveTime));
            StartCoroutine(UnityUtils.Instance.IViewSlide(true,
                                                          HandHistoryPage_Tr,
                                                          DirectionEnum.Up,
                                                          PageMoveTime));
        });

        //關閉手牌紀錄
        HandHistoryClose_Btn.onClick.AddListener(() =>
        {            
            StartCoroutine(UnityUtils.Instance.IViewSlide(false,
                                                          HandHistoryPage_Tr,
                                                          DirectionEnum.Up,
                                                          PageMoveTime,
                                                          () =>
                                                          {
                                                              GameRoomManager.Instance.IsCanMoveSwitch = true;
                                                          }));

        });

        #endregion
    }

    private void OnEnable()
    {
        thisData = new ThisData();
        thisData.IsPlaying = false;
        SetSitOutDisplay(false);

        strData = new StrData();

        if (gamePlayerInfoList != null)
        {
            foreach (var player in gamePlayerInfoList)
            {
                Destroy(player.gameObject);
            }
        }

        gamePlayerInfoList = new List<GamePlayerInfo>();
        buyChipsView.gameObject.SetActive(false);
        BattleResultView.gameObject.SetActive(false);
        BackToSit_Btn.gameObject.SetActive(false);
        TotalPot_Txt.text = StringUtils.SetChipsUnit(0);

        //選單玩家訊息
        StringUtils.StrExceedSize(DataManager.UserWalletAddress, MenuWalletAddr_Txt);
        MenuNickname_Txt.text = $"@{DataManager.UserNickname}";
        MenuAvatar_Img.sprite = AssetsManager.Instance.GetAlbumAsset(AlbumEnum.AvatarAlbum).album[DataManager.UserAvatar];

        SetNotReadChatCount = 0;

        Init();
        GameInit();
    }

    private void Start()
    {
        OtherChatSample.SetActive(false);
        LocalChatSample.SetActive(false);
        NewMessage_Btn.gameObject.SetActive(false);
        Mask_Btn.gameObject.SetActive(false);
        MenuPage_Tr.gameObject.SetActive(false);
        ChatPage_Tr.gameObject.SetActive(false);
        HandHistoryPage_Tr.gameObject.SetActive(false);

        LanguageManager.Instance.AddUpdateLanguageFunc(UpdateLanguage);
    }

    private void Update()
    {
        #region 測試

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            baseRequest.gameServer.TextChat();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            MainPack exitPack = new MainPack();
            exitPack.ActionCode = ActionCode.Request_PlayerInOutRoom;

            PlayerInfoPack playerInfoPack = new PlayerInfoPack();
            playerInfoPack.UserID = gamePlayerInfoList[0].UserId;

            PlayerInOutRoomPack playerInOutRoomPack = new PlayerInOutRoomPack();
            playerInOutRoomPack.IsInRoom = false;
            playerInOutRoomPack.PlayerInfoPack = playerInfoPack;

            exitPack.PlayerInOutRoomPack = playerInOutRoomPack;
            baseRequest.gameServer.Request_PlayerInOutRoom(exitPack);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            MainPack exitPack = new MainPack();
            exitPack.ActionCode = ActionCode.Request_PlayerInOutRoom;

            PlayerInfoPack playerInfoPack = new PlayerInfoPack();
            playerInfoPack.UserID = gamePlayerInfoList[1].UserId;

            PlayerInOutRoomPack playerInOutRoomPack = new PlayerInOutRoomPack();
            playerInOutRoomPack.IsInRoom = false;
            playerInOutRoomPack.PlayerInfoPack = playerInfoPack;

            exitPack.PlayerInOutRoomPack = playerInOutRoomPack;
            baseRequest.gameServer.Request_PlayerInOutRoom(exitPack);
        }

        #endregion

        //發送聊天訊息
        if ((Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.KeypadEnter)) &&
            ChatPage_Tr.gameObject.activeSelf &&
            !string.IsNullOrEmpty(Chat_If.text))
        {
            SendChat();
            Chat_If.ActivateInputField();
            Chat_If.Select();
        }
    }

    /// <summary>
    /// 遊戲暫停
    /// </summary>
    public bool GamePause
    {
        set
        {
            GamePause_Obj.SetActive(value);
        }
    }

    /// <summary>
    /// 關閉選單
    /// </summary>
    private void CloseMenu()
    {
        Mask_Btn.gameObject.SetActive(false);
        StartCoroutine(UnityUtils.Instance.IViewSlide(false,
                                                      MenuPage_Tr,
                                                      DirectionEnum.Left,
                                                      PageMoveTime,
                                                      () =>
                                                      {
                                                          GameRoomManager.Instance.IsCanMoveSwitch = true;
                                                      }));
    }

    /// <summary>
    /// 設置離/回座顯示
    /// </summary>
    /// <param name="isSitOut"></param>
    private void SetSitOutDisplay(bool isSitOut)
    {
        SitOutBtn_Txt.text = thisData.IsSitOut ?
                             "Back To Sit" :
                             "Next Rount Sit Out";

        if (!thisData.IsPlaying)
        {
            BackToSit_Btn.gameObject.SetActive(thisData.IsSitOut);
            Tip_Txt.text = thisData.IsSitOut ?
                           "" :
                           "Waiting...";
        }
    }

    /// <summary>
    /// 設定加註至文字
    /// </summary>
    public double SetRaiseToText
    {
        set
        {
            if (value >= thisData.LocalPlayerChips)
            {
                //All In
                CurrRaise_Txt.text = $"All In";
                RaiseSliHandle_Txt.text = $"All In";
            }
            else
            {
                CurrRaise_Txt.text = StringUtils.SetChipsUnit(value);
                RaiseSliHandle_Txt.text = StringUtils.SetChipsUnit(value);
            }
        }
    }

    /// <summary>
    /// 設置行動按鈕文字(是否為玩家回合)
    /// </summary>
    public bool SetActionButton
    {
        set
        {
            thisData.isLocalPlayerTurn = value;
            if (SetActingButtonEnable == true)
            {
                if (value == false)
                {
                    Raise_Tr.gameObject.SetActive(false);
                    strData.FoldStr = "CheckOrFold";
                    FoldBtn_Txt.text = LanguageManager.Instance.GetText(strData.FoldStr);
                    strData.CallStr = "Check";
                    strData.CallValueStr = "";
                    CallBtn.text = LanguageManager.Instance.GetText(strData.CallStr) + strData.CallValueStr;
                    strData.RaiseStr = "CallAny";
                    strData.RaiseValueStr = "";
                    RaiseBtn_Txt.text = LanguageManager.Instance.GetText(strData.RaiseStr) + strData.RaiseValueStr;
                }
                else
                {
                    strData.FoldStr = "Fold";
                    FoldBtn_Txt.text = LanguageManager.Instance.GetText(strData.FoldStr);
                }
            }
        }
    }

    /// <summary>
    /// 行動按鈕激活
    /// </summary>
    public bool SetActingButtonEnable
    {
        get
        {
            return Raise_Btn.interactable;
        }
        set
        {
            Raise_Btn.interactable = value;
            Call_Btn.interactable = value;
            Fold_Btn.interactable = value;
        }
    }

    private AutoActingEnum autoActingEnum;
    /// <summary>
    /// 自動操作
    /// </summary>
    public enum AutoActingEnum
    {
        None,
        CallAny,
        Check,
        CheckAndFold,
    }

    /// <summary>
    /// 自動操作狀態
    /// </summary>
    public AutoActingEnum AutoActionState
    {
        get
        {
            return autoActingEnum;
        }
        set
        {
            autoActingEnum = value;
            switch (value)
            {
                case AutoActingEnum.None:
                    SetAutoAction(false);
                    break;
                case AutoActingEnum.CallAny:
                    SetAutoAction(true, Raise_Btn.transform);
                    break;
                case AutoActingEnum.Check:
                    SetAutoAction(true, Call_Btn.transform);
                    break;
                case AutoActingEnum.CheckAndFold:
                    SetAutoAction(true, Fold_Btn.transform);
                    break;
            }
        }
    }

    /// <summary>
    /// 自動操作設定
    /// </summary>
    /// <param name="isActive"></param>
    /// <param name="parent"></param>
    private void SetAutoAction(bool isActive, Transform parent = null)
    {
        AutoActionFrame_Tr.gameObject.SetActive(isActive);
        if (parent != null)
        {
            AutoActionFrame_Tr.SetParent(parent);
            AutoActionFrame_Tr.anchoredPosition = Vector2.zero;
            AutoActionFrame_Tr.offsetMax = Vector2.zero;
            AutoActionFrame_Tr.offsetMin = Vector2.zero;
        }
    }

    /// <summary>
    /// 設置底池顯示
    /// </summary>
    public bool SetPotActive
    {
        set
        {
            Pot_Img.enabled = value;
            Pot_Img.rectTransform.anchoredPosition = InitPotPointPos;
        }
    }

    /// <summary>
    /// 設置底池籌碼
    /// </summary>
    public double SetTotalPot
    {
        set
        {
            StringUtils.ChipsChangeEffect(TotalPot_Txt, value);
            thisData.TotalPot = value;
        }
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        Tip_Txt.text = $"{LanguageManager.Instance.GetText("Waiting")}...";
        strData.FoldStr = "CheckOrFold";
        FoldBtn_Txt.text = LanguageManager.Instance.GetText(strData.FoldStr);
        strData.CallStr = "Check";
        strData.CallValueStr = "";
        CallBtn.text = LanguageManager.Instance.GetText(strData.CallStr) + strData.CallValueStr;
        strData.RaiseStr = "CallAny";
        strData.RaiseValueStr = "";
        RaiseBtn_Txt.text = LanguageManager.Instance.GetText(strData.RaiseStr) + strData.RaiseValueStr;
        SetTotalPot = 0;

        Raise_Tr.gameObject.SetActive(false);
    }

    /// <summary>
    /// 遊戲初始化
    /// </summary>
    public void GameInit()
    {
        foreach (var poker in CommunityPokerList)
        {
            poker.gameObject.SetActive(false);
        }
        foreach (var player in gamePlayerInfoList)
        {
            player.IsWinnerActive = false;
            player.SetBackChips = 0;
            player.GetHandPoker[0].gameObject.SetActive(false);
            player.GetHandPoker[1].gameObject.SetActive(false);
            player.IsOpenInfoMask = true;
            player.IsPlaying = false;
        }
        foreach (var show in ShowPokerBtnList)
        {
            show.gameObject.SetActive(false);
        }

        SetTotalPot = 0;
        WinType_Txt.text = "";
        SetPotActive = false;
        SetActionButton = false;
        AutoActionState = AutoActingEnum.None;
        Fold_Btn.gameObject.SetActive(true);
        Call_Btn.gameObject.SetActive(true);
        Raise_Btn.gameObject.SetActive(true);

        thisData.IsPlaying = false;
        thisData.isFold = false;
        thisData.CurrCommunityPoker = new List<int>();
    }

    /// <summary>
    /// 發送更新房間
    /// </summary>
    public void SendRequest_UpdateRoomInfo()
    {
        baseRequest.SendRequest_UpdateRoomInfo();
    }

    /// <summary>
    /// 棄牌
    /// </summary>
    private void OnFold()
    {
        baseRequest.SendRequest_PlayerActed(Entry.TestInfoData.LocalUserId,
                                            ActingEnum.Fold,
                                            0);
    }

    /// <summary>
    /// 跟注/過牌
    /// </summary>
    private void OnCallAndCheck()
    {
        double betValue = 0;
        ActingEnum acting = ActingEnum.Call;
        if (thisData.IsFirstRaisePlayer)
        {
            if (thisData.CurrCallValue == thisData.SmallBlindValue)
            {
                acting = ActingEnum.Check;
            }
            else
            {
                betValue = thisData.CurrCallValue;
            }
        }
        else
        {
            if (thisData.LocalPlayerCurrBetValue == thisData.CurrCallValue)
            {
                acting = ActingEnum.Check;
            }
            else if (thisData.LocalPlayerChips <= thisData.CurrCallValue)
            {
                acting = ActingEnum.AllIn;
                betValue = thisData.LocalPlayerChips;
            }
            else
            {
                betValue = thisData.CurrCallValue;
            }
        }

        baseRequest.SendRequest_PlayerActed(Entry.TestInfoData.LocalUserId,
                                            acting,
                                            betValue);
    }

    /// <summary>
    /// 顯示棄牌手牌
    /// </summary>
    /// <param name="index"></param>
    private void ShowFoldPoker(int index)
    {
        ShowPokerBtnList[index].gameObject.SetActive(false);
        baseRequest.SendShowFoldPoker(index);
    }

    /// <summary>
    /// 接收顯示棄牌手牌
    /// </summary>
    /// <param name="pack"></param>
    public void GetShowFoldPoker(MainPack pack)
    {
        string id = pack.ShowFoldPokerPack.UserID;
        int pokerIndex = pack.ShowFoldPokerPack.HandPokerIndex;
        int pokerNum = pack.ShowFoldPokerPack.PokerNum;

        GamePlayerInfo gamePlayerInfo = GetPlayer(id);
        gamePlayerInfo.GetHandPoker[pokerIndex].gameObject.SetActive(true);
        gamePlayerInfo.GetHandPoker[pokerIndex].SetColor = 1;

        if (id != Entry.TestInfoData.LocalUserId)
        {
            gamePlayerInfo.GetHandPoker[pokerIndex].PokerNum = pokerNum;
        }
    }

    /// <summary>
    /// 底池百分比加注
    /// </summary>
    /// <param name="btnIndex"></param>
    private void PotRaisePercent(int btnIndex)
    {
        int raiseValue = thisData.IsFirstRaisePlayer ?
                         (int)((float)(thisData.SmallBlindValue * 2) * PotBbRate[btnIndex]) :
                         (int)((float)thisData.TotalPot * (PotPercentRate[btnIndex] / 100));
        Raise_Sli.value = raiseValue;
    }

    /// <summary>
    /// 本地玩家回合
    /// </summary>
    /// <param name="playerActingRoundPack"></param>
    public void ILocalPlayerRound(PlayerActingRoundPack playerActingRoundPack)
    {
        SetActionButton = true;

        //當前底池
        thisData.TotalPot = playerActingRoundPack.TotalPot;
        //玩家籌碼
        thisData.LocalPlayerChips = playerActingRoundPack.PlayerChips;
        //首位加注玩家
        thisData.IsFirstRaisePlayer = playerActingRoundPack.IsFirstRaisePlayer;
        //當前跟注值
        thisData.CurrCallValue = playerActingRoundPack.CurrCallValue;
        //跟注差額
        thisData.CallDifference = playerActingRoundPack.CallDifference;
        //玩家當前下注值
        thisData.LocalPlayerCurrBetValue = playerActingRoundPack.PlayerCurrBryValue;
        //無法加注
        thisData.IsUnableRaise = playerActingRoundPack.IsUnableRaise;
        //最小加注
        thisData.MinRaiseValue = thisData.CurrCallValue * 2;
        thisData.CurrRaiseValue = thisData.MinRaiseValue;


        if (AutoActionState != AutoActingEnum.None)
        {
            StartCoroutine(JudgeAutoAction());
            return;
        }

        ShowBetArea();
    }

    /// <summary>
    /// 自動操作判斷
    /// </summary>
    private IEnumerator JudgeAutoAction()
    {
        yield return new WaitForSeconds(0.2f);

        switch (AutoActionState)
        {
            //任何跟注
            case AutoActingEnum.CallAny:
                OnCallAndCheck();
                break;

            //過牌
            case AutoActingEnum.Check:
                if (thisData.IsFirstRaisePlayer == true)
                {
                    if (thisData.CurrCallValue == thisData.SmallBlindValue)
                    {
                        OnCallAndCheck();
                    }
                    else
                    {
                        ShowBetArea();
                    }
                }
                else
                {
                    if (thisData.LocalPlayerCurrBetValue == thisData.CurrCallValue)
                    {
                        OnCallAndCheck();
                    }
                    else
                    {
                        ShowBetArea();
                    }
                }
                break;

            //過牌或棄牌
            case AutoActingEnum.CheckAndFold:
                if (thisData.IsFirstRaisePlayer == true)
                {
                    if (thisData.CurrCallValue == thisData.SmallBlindValue)
                    {
                        OnCallAndCheck();
                    }
                    else
                    {
                        OnFold();
                    }
                }
                else
                {
                    if (thisData.LocalPlayerCurrBetValue == thisData.CurrCallValue)
                    {
                        OnCallAndCheck();
                    }
                    else
                    {
                        OnFold();
                    }
                }
                break;
        }


        yield return null;

        AutoActionState = AutoActingEnum.None;
    }

    /// <summary>
    /// 顯示下注區塊
    /// </summary>
    private void ShowBetArea()
    {
        //是否無法在加注
        bool IsUnableRaise = thisData.IsUnableRaise;
        bool isJustAllIn = thisData.LocalPlayerChips <= thisData.CurrCallValue;

        //棄牌
        Fold_Btn.gameObject.SetActive(true);

        //加注&All In
        strData.RaiseStr = isJustAllIn == true ?
                           "AllIn" :
                           "Raise";
        strData.RaiseValueStr = isJustAllIn == true ?
                                $"\n${StringUtils.SetChipsUnit(thisData.LocalPlayerChips)}" :
                                "";
        RaiseBtn_Txt.text = LanguageManager.Instance.GetText(strData.RaiseStr);

        if (IsUnableRaise == true && isJustAllIn == false)
        {
            Raise_Btn.gameObject.SetActive(false);
        }
        else
        {
            Raise_Btn.gameObject.SetActive(true);
        }

        //跟注&過牌
        strData.CallStr = "Call";
        strData.CallValueStr = $"\n{StringUtils.SetChipsUnit(thisData.CurrCallValue - thisData.CallDifference)}";
        if (thisData.IsFirstRaisePlayer == true)
        {
            if (thisData.CurrCallValue == thisData.SmallBlindValue)
            {
                strData.CallStr = "Check";
                strData.CallValueStr = "";
            }
            else
            {
                strData.CallStr = "Call";
                strData.CallValueStr = $"\n{StringUtils.SetChipsUnit(thisData.CallDifference)}";
            }
        }
        else
        {
            if (thisData.LocalPlayerCurrBetValue == thisData.CurrCallValue)
            {
                strData.CallStr = "Check";
                strData.CallValueStr = "";
            }
            else
            {
                strData.CallStr = "Call";
                strData.CallValueStr = $"\n{StringUtils.SetChipsUnit(thisData.CallDifference)}";
            }
        }
        CallBtn.text = LanguageManager.Instance.GetText(strData.CallStr) + strData.CallValueStr;
        if (IsUnableRaise == true && isJustAllIn == false)
        {
            Call_Btn.gameObject.SetActive(true);
        }
        else
        {
            Call_Btn.gameObject.SetActive(isJustAllIn == false);
        }

        //加注區域物件
        Raise_Tr.gameObject.SetActive(false);
        if (isJustAllIn == false)
        {
            //倍數
            float multiple = (int)Mathf.Ceil((float)thisData.LocalPlayerChips / (float)(thisData.SmallBlindValue * 2));
            Raise_Sli.maxValue = (float)(thisData.SmallBlindValue * 2) * multiple;
            Raise_Sli.minValue = 0;
            Raise_Sli.value = (float)thisData.MinRaiseValue;

            //加注值
            SetRaiseToText = thisData.MinRaiseValue;

            //最小加注值
            MinRaiseBtn_Txt.text = thisData.MinRaiseValue.ToString(); ;

            //底池倍率
            for (int i = 0; i < PotPercentRaiseTxtList.Count; i++)
            {
                if (thisData.TotalPot <= thisData.SmallBlindValue * 3)
                {
                    PotPercentRaiseTxtList[i].text = $"{PotBbRate[i]}BB";
                }
                else
                {
                    PotPercentRaiseTxtList[i].text = $"{PotPercentRate[i]}%";
                }
            }
        }
    }

    /// <summary>
    /// 籌碼不足
    /// </summary>
    /// <param name="pack"></param>
    public void OnNotEnoughChips(MainPack pack)
    {
        string id = pack.PlayerInOutRoomPack.PlayerInfoPack.UserID;
        if (id == Entry.TestInfoData.LocalUserId)
        {

        }
        else
        {
            PlayerExitRoom(id);
        }
    }

    /// <summary>
    /// 添加玩家
    /// </summary>
    /// <param name="pack"></param>
    public GamePlayerInfo AddPlayer(PlayerInfoPack playerInfoPack)
    {
        GamePlayerInfo gamePlayerInfo = null;
        int seatIndex = 0;//座位(本地玩家 = 0)
        if (playerInfoPack.UserID != Entry.TestInfoData.LocalUserId)
        {
            if (RoomType == TableTypeEnum.IntegralTable)
            {
                seatIndex = 3;
            }
            else
            {
                seatIndex = playerInfoPack.Seat > thisData.LocalPlayerSeat ?
                            playerInfoPack.Seat - thisData.LocalPlayerSeat :
                            SeatButtonList.Count - (thisData.LocalPlayerSeat - playerInfoPack.Seat);

            }
            gamePlayerInfo = SeatGamePlayerInfoList[seatIndex];
        }
        else
        {
            //本地玩家
            gamePlayerInfo = SeatGamePlayerInfoList[0];
            thisData.LocalGamePlayerInfo = gamePlayerInfo;

            playerInfoPack.NickName = DataManager.UserNickname;
        }

        SeatButtonList[seatIndex].image.enabled = false;
        gamePlayerInfo.gameObject.SetActive(true);

        gamePlayerInfo.SetInitPlayerInfo(seatIndex,
                                         playerInfoPack.UserID,
                                         playerInfoPack.NickName,
                                         playerInfoPack.Chips,
                                         playerInfoPack.Avatar);

        gamePlayerInfoList.Add(gamePlayerInfo);
        return gamePlayerInfo;
    }

    /// <summary>
    /// 有玩家退出房間
    /// </summary>
    /// <param name="id">退出玩家ID</param>
    /// <returns></returns>
    public GamePlayerInfo PlayerExitRoom(string id)
    {
        GamePlayerInfo exitPlayer = GetPlayer(id);

        SeatButtonList[exitPlayer.SeatIndex].image.enabled = true;
        gamePlayerInfoList.Remove(exitPlayer);

        exitPlayerSeatList.Add(exitPlayer.SeatIndex);

        Destroy(exitPlayer.gameObject);

        if (RoomType == TableTypeEnum.IntegralTable)
        {
            SetBattleResult(true);
        }

        return exitPlayer;
    }

    /// <summary>
    /// 獲取玩家
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public GamePlayerInfo GetPlayer(string id)
    {
        return gamePlayerInfoList.Where(x => x.UserId == id).FirstOrDefault();
    }

    /// <summary>
    /// 接收玩家行動
    /// </summary>
    /// <param name="playerActedPack">行動Enum</param>
    public void GetPlayerAction(PlayerActedPack playerActedPack)
    {
        string id = playerActedPack.ActPlayerId;
        ActingEnum actionEnum = playerActedPack.ActingEnum;
        double betValue = playerActedPack.BetValue;
        double chips = playerActedPack.PlayerChips;
        bool isLocalPlayer = id == Entry.TestInfoData.LocalUserId;

        //本地玩家
        if (isLocalPlayer)
        {
            SetActionButton = false;

            switch (actionEnum)
            {
                //棄牌
                case ActingEnum.Fold:
                    SetAutoAction(false);
                    SetActingButtonEnable = false;
                    Raise_Tr.gameObject.SetActive(false);
                    thisData.isFold = true;
                    thisData.IsPlaying = false;
                    break;

                //All In
                case ActingEnum.AllIn:
                    SetActingButtonEnable = false;
                    break;
            }

        }

        GamePlayerInfo playerInfo = GetPlayer(id);
        if (playerInfo != null &&
            playerInfo.gameObject.activeSelf)
        {
            playerInfo.PlayerAction(actionEnum,
                                    betValue,
                                    chips);
        }

        //本地玩家有參與
        if (thisData.LocalGamePlayerInfo.IsPlaying &&
            playerInfo != null)
        {
            //紀錄存檔
            ProcessStepHistoryData processStepHistoryData = AddNewStepHistory();
            processStepHistoryData.ActionPlayerIndex = playerInfo.SeatIndex;
            processStepHistoryData.ActionIndex = (int)actionEnum;

            processHistoryData.processStepHistoryDataList.Add(processStepHistoryData);
        }
    }

    /// <summary>
    /// 下注籌碼集中
    /// </summary>
    /// <returns></returns>
    private IEnumerator IConcentrateBetChips()
    {
        for (int i = 0; i < gamePlayerInfoList.Count; i++)
        {
            if (gamePlayerInfoList[i].GetBetChipsActive == true)
            {
                yield return new WaitForSeconds(1);

                foreach (var player in gamePlayerInfoList)
                {
                    player.ConcentrateBetChips(Pot_Img.transform.position);
                }

                yield return new WaitForSeconds(0.5f);

                break;
            }
        }

        //顯示底池籌碼
        SetPotActive = true;
    }

    /// <summary>
    /// 更新遊戲房間訊息
    /// </summary>
    /// <param name="pack"></param>
    public void UpdateGameRoomInfo(MainPack pack)
    {
        //清除座位上玩家
        for (int i = 1; i < SeatGamePlayerInfoList.Count; i++)
        {
            SeatGamePlayerInfoList[i].gameObject.SetActive(false);
        }

        //本地玩家座位
        thisData.LocalPlayerSeat = pack.PlayerInfoPackList.Where(x => x.UserID == Entry.TestInfoData.LocalUserId)
                                                          .FirstOrDefault()
                                                          .Seat;

        //更新玩家訊息
        foreach (var player in pack.PlayerInfoPackList)
        {
            GamePlayerInfo gamePlayerInfo = AddPlayer(player);
            if (pack.UpdateRoomInfoPack.playingIdList.Contains(gamePlayerInfo.UserId))
            {
                gamePlayerInfo.SetHandPoker(-1, -1);
            }
            if (player.CurrBetValue > 0)
            {
                gamePlayerInfo.PlayerBet(player.CurrBetValue, player.Chips);
            }
        }

        //底池
        SetTotalPot = pack.UpdateRoomInfoPack.TotalPot;
        if ((int)pack.UpdateRoomInfoPack.flowEnum >= 2)
        {
            //SetPotActive = true;
        }

        //公共牌
        List<int> currCommunityPoker = pack.CommunityPokerPack.CurrCommunityPoker;
        for (int i = 0; i < currCommunityPoker.Count; i++)
        {
            CommunityPokerList[i].gameObject.SetActive(true);
            CommunityPokerList[i].PokerNum = currCommunityPoker[i];
        }
    }

    /// <summary>
    /// 設置Button座位物件
    /// </summary>
    /// <param name="id"></param>
    public void SetButtonSeat(string id)
    {
        GamePlayerInfo buttonPlayer = gamePlayerInfoList.Where(x => x.UserId == id).FirstOrDefault();
        buttonPlayer.SetSeatCharacter(SeatCharacterEnum.Button);

        if (gamePlayerInfoList.Count >= 3)
        {
            int buttonSeat = gamePlayerInfoList.Select((v, i) => (v, i))
                                               .Where(x => x.v.UserId == id)
                                               .FirstOrDefault().i;

            int sb = buttonSeat + 1 < gamePlayerInfoList.Count ?
                     buttonSeat + 1 :
                     0;

            int bb = buttonSeat + 2 < gamePlayerInfoList.Count ?
                     buttonSeat + 2 :
                     0;

            gamePlayerInfoList[sb].SetSeatCharacter(SeatCharacterEnum.SB);
            gamePlayerInfoList[bb].SetSeatCharacter(SeatCharacterEnum.BB);
        }
    }

    /// <summary>
    /// 設定大小盲
    /// </summary>
    /// <param name="blindStagePack"></param>
    private void SetBlind(BlindStagePack blindStagePack)
    {
        //小盲
        GamePlayerInfo sbPlayer = GetPlayer(blindStagePack.SBPlayerId);
        sbPlayer.PlayerAction(ActingEnum.Blind,
                              thisData.SmallBlindValue,
                              blindStagePack.SBPlayerChips);

        //大盲
        GamePlayerInfo bbPlayer = GetPlayer(blindStagePack.BBPlayerId);
        bbPlayer.PlayerAction(ActingEnum.Blind,
                              thisData.SmallBlindValue * 2,
                              blindStagePack.BBPlayerChips);
    }

    /// <summary>
    /// 手牌發牌
    /// </summary>
    /// <param name="handPokerDic"></param>
    private void HandPokerLicensing(Dictionary<string, (int, int)> handPokerDic)
    {
        foreach (var dic in handPokerDic)
        {
            GamePlayerInfo player = gamePlayerInfoList.First(x => x.UserId == dic.Key);
            if (player.UserId == Entry.TestInfoData.LocalUserId)
            {
                thisData.IsPlaying = true;
                player.SetHandPoker(dic.Value.Item1,
                                    dic.Value.Item2);

                JudgePokerShape(player, true);
            }
            else
            {
                player.SetHandPoker(-1, -1);
            }
        }
    }

    /// <summary>
    /// 每輪回合開始初始
    /// </summary>
    private void RountInit()
    {
        foreach (var player in gamePlayerInfoList)
        {
            player.RountInit();
        }
    }

    /// <summary>
    /// 翻開公共牌
    /// </summary>
    /// <param name="currCommunityPoker"></param>
    /// <returns></returns>
    private IEnumerator IFlopCommunityPoker(List<int> currCommunityPoker)
    {
        foreach (var player in gamePlayerInfoList)
        {
            if (!player.IsFold && !player.IsAllIn)
            {
                player.CurrBetAction = BetActionEnum.None;
            }
        }

        thisData.CurrCommunityPoker = currCommunityPoker;

        //本地玩家有參與
        if (thisData.LocalGamePlayerInfo.IsPlaying &&
            currCommunityPoker.Count > 0)
        {
            //紀錄存檔
            ProcessStepHistoryData processStepHistoryData = AddNewStepHistory();
            processStepHistoryData.ActionPlayerIndex = -1;
            processStepHistoryData.ActionIndex = -1;

            processHistoryData.processStepHistoryDataList.Add(processStepHistoryData);
        }

        yield return IConcentrateBetChips();

        for (int i = 0; i < currCommunityPoker.Count; i++)
        {
            if (CommunityPokerList[i].gameObject.activeSelf == false)
            {
                CommunityPokerList[i].gameObject.SetActive(true);
                CommunityPokerList[i].PokerNum = currCommunityPoker[i];
                StartCoroutine(CommunityPokerList[i].IHorizontalFlopEffect(currCommunityPoker[i]));
                yield return new WaitForSeconds(0.1f);
            }
        }

        yield return new WaitForSeconds(0.6f);

        if (thisData.IsPlaying == true)
        {
            GamePlayerInfo localPlayer = gamePlayerInfoList.Where(x => x.UserId == Entry.TestInfoData.LocalUserId).FirstOrDefault();
            JudgePokerShape(localPlayer, true);
        }
    }

    /// <summary>
    /// 判斷牌型
    /// </summary>
    /// <param name="player"></param>
    /// <param name="isOpenMatchPokerFrame">是否開啟符合的撲克框</param>
    /// <param name="isWinEffect">贏家效果</param>
    private void JudgePokerShape(GamePlayerInfo player, bool isOpenMatchPokerFrame, bool isWinEffect = false)
    {
        //手牌
        Poker[] handPoker = player.GetHandPoker;
        List<int> judgePoker = new List<int>();
        foreach (var poker in handPoker)
        {
            judgePoker.Add(poker.PokerNum);
        }

        //公共牌
        judgePoker = judgePoker.Concat(thisData.CurrCommunityPoker).ToList();

        List<Poker> pokers = CommunityPokerList.Concat(handPoker.ToList()).ToList();

        //關閉所有外框
        foreach (var poker in pokers)
        {
            poker.PokerFrameEnable = false;
        }

        //判斷牌型
        PokerShape.JudgePokerShape(judgePoker, (resultIndex, matchPokerList) =>
        {
            player.SetPokerShapeStr(resultIndex);

            if (isOpenMatchPokerFrame && resultIndex < 10)
            {
                PokerShape.OpenMatchPokerFrame(pokers,
                                               matchPokerList,
                                               isWinEffect);
            }
        });
    }

    /// <summary>
    /// 主池結果
    /// </summary>
    /// <param name="pack"></param>
    /// <returns></returns>
    private IEnumerator IPotResult(MainPack pack)
    {
        SetActingButtonEnable = false;
        thisData.PowWinChips = pack.WinnerPack.WinChips;

        yield return IConcentrateBetChips();
        yield return IFlopCommunityPoker(pack.CommunityPokerPack.CurrCommunityPoker);

        //顯示手牌牌型
        foreach (var handPoker in pack.LicensingStagePack.HandPokerDic)
        {
            GamePlayerInfo player = GetPlayer(handPoker.Key);
            player.SetHandPoker(handPoker.Value.Item1,
                                handPoker.Value.Item2);

            JudgePokerShape(player, false);
        }

        yield return new WaitForSeconds(1);

        //開啟遮罩
        foreach (var player in gamePlayerInfoList)
        {
            player.IsOpenInfoMask = true;
        }

        //贏得類型顯示
        WinType_Txt.text = LanguageManager.Instance.GetText("Pot");
        TotalPot_Txt.text = StringUtils.SetChipsUnit(pack.WinnerPack.WinChips);

        //贏家效果
        thisData.PotWinnerList = pack.WinnerPack.WinnerDic.Select(x => x.Key).ToList();

        foreach (var winner in pack.WinnerPack.WinnerDic)
        {
            //關閉所有撲克外框
            List<Poker> playersPoker = new List<Poker>();
            foreach (var p in gamePlayerInfoList)
            {
                foreach (var poker in p.GetHandPoker)
                {
                    playersPoker.Add(poker);
                }
            }
            List<Poker> allPokerList = CommunityPokerList.Concat(playersPoker.ToList()).ToList();
            foreach (var poker in allPokerList)
            {
                poker.PokerFrameEnable = false;
            }

            GamePlayerInfo player = GetPlayer(winner.Key);
            player.IsOpenInfoMask = false;

            JudgePokerShape(player, true, true);

            player.IsWinnerActive = true;

            Vector2 winnerSeatPos = player.gameObject.transform.position;
            
            //產生贏得籌碼物件            
            RectTransform rt = Instantiate(WinChipsObj, Pot_Img.transform).GetComponent<RectTransform>();
            rt.anchoredPosition = Vector2.zero;
            SetPotActive = false;

            yield return new WaitForSeconds(0.5f);

            ObjMoveUtils.ObjMoveToTarget(rt, winnerSeatPos, 0.5f,
                                        () =>
                                        {
                                            Destroy(rt.gameObject);
                                            player.PlayerRoomChips = winner.Value;
                                        });
            
            yield return new WaitForSeconds(2);
        }

        //記錄存檔
        int winIndex = 0;
        foreach (var winner in pack.WinnerPack.WinnerDic)
        {
            winIndex++;
            GamePlayerInfo player = GetPlayer(winner.Key);

            //本地玩家有參與
            if (thisData.LocalGamePlayerInfo.IsPlaying)
            {
                //獲勝牌局紀錄存檔
                if (winIndex == 1)
                {
                    string roomName = "";
                    switch (roomType)
                    {
                        case TableTypeEnum.IntegralTable:
                            roomName = "Integral Table";
                            break;
                        case TableTypeEnum.CryptoTable:
                            roomName = "Crypto Table";
                            break;
                        case TableTypeEnum.VCTable:
                            roomName = "VC Table";
                            break;
                    }

                    saveResultData = new ResultHistoryData();
                    saveResultData.RoomType = $"{roomName}";
                    saveResultData.SmallBlind = thisData.SmallBlindValue;
                    saveResultData.NickName = player.Nickname;
                    saveResultData.Avatar = player.Avatar;
                    saveResultData.HandPokers = new int[] { player.GetHandPoker[0].PokerNum,
                                                            player.GetHandPoker[1].PokerNum };
                    saveResultData.CommunityPoker = thisData.CurrCommunityPoker;
                    saveResultData.WinChips = pack.WinnerPack.WinChips;
                }
            }
        }

        //主池紀錄存檔
        if (thisData.LocalGamePlayerInfo.IsPlaying)
        {
            ProcessStepHistoryData processStepHistoryData = AddNewStepHistory();
            processStepHistoryData.ActionPlayerIndex = -1;
            processStepHistoryData.ActionIndex = -1;

            processStepHistoryData.PotWinnerSeatList = new List<int>();
            foreach (var id in thisData.PotWinnerList)
            {
                int potWinSeat = GetPlayer(id).SeatIndex;
                processStepHistoryData.PotWinnerSeatList.Add(potWinSeat);
            }
            processStepHistoryData.PotWinChips = thisData.PowWinChips;

            processHistoryData.processStepHistoryDataList.Add(processStepHistoryData);
        }

    }

    /// <summary>
    /// 邊池結果
    /// </summary>
    /// <param name="pack"></param>
    public IEnumerator SideResult(MainPack pack)
    {
        thisData.SideWinnerList = new List<string>();
        thisData.SideWinChips = 0;

        foreach (var common in CommunityPokerList)
        {
            common.PokerFrameEnable = false;
        }

        //開啟遮罩
        foreach (var player in gamePlayerInfoList)
        {
            player.IsOpenInfoMask = true;
            player.IsWinnerActive = false;
        }

        //邊池贏家效果
        thisData.SideWinnerList = new List<string>();
        foreach (var sideWinner in pack.SidePack.SideWinnerDic)
        {
            //邊池贏家非主池贏家
            bool isShow = !thisData.PotWinnerList.Contains(sideWinner.Key);
            if (!isShow)
            {
                continue;
            }

            WinType_Txt.text = LanguageManager.Instance.GetText("Side");
            TotalPot_Txt.text = StringUtils.SetChipsUnit(sideWinner.Value);

            thisData.SideWinnerList.Add(sideWinner.Key);

            GamePlayerInfo player = GetPlayer(sideWinner.Key);

            //關閉所有撲克外框
            List<Poker> playersPoker = new List<Poker>();
            foreach (var p in gamePlayerInfoList)
            {
                foreach (var poker in p.GetHandPoker)
                {
                    playersPoker.Add(poker);
                }
            }
            List<Poker> allPokerList = CommunityPokerList.Concat(playersPoker.ToList()).ToList();
            foreach (var poker in allPokerList)
            {
                poker.PokerFrameEnable = false;
            }

            thisData.SideWinChips = sideWinner.Value;

            player.IsOpenInfoMask = false;
            player.IsWinnerActive = true;

            Vector2 winnerSeatPos = player.gameObject.transform.position;
            JudgePokerShape(player, true, true);

            player.PlayerRoomChips += sideWinner.Value;

            if (sideWinner.Value > 0)
            {
                RectTransform rt = Instantiate(WinChipsObj, Pot_Img.transform).GetComponent<RectTransform>();
                rt.anchoredPosition = Vector2.zero;

                yield return new WaitForSeconds(0.5f);

                ObjMoveUtils.ObjMoveToTarget(rt, winnerSeatPos, 0.5f,
                                            () =>
                                            {
                                                player.PlayerRoomChips += sideWinner.Value;
                                                Destroy(rt.gameObject);
                                            });
            }

            yield return new WaitForSeconds(2);

            //關閉撲克外框
            Poker[] handPoker = player.GetHandPoker;
            List<Poker> pokerList = CommunityPokerList.Concat(handPoker.ToList()).ToList();
            foreach (var poker in pokerList)
            {
                poker.PokerFrameEnable = false;
            }
        }

        yield return new WaitForSeconds(0.5f);

        //顯示退回籌碼
        thisData.BackChipsDic = new Dictionary<int, double>();
        foreach (var backChips in pack.SidePack.BackChips)
        {
            if (backChips.Value > 0)
            {
                GamePlayerInfo player = GetPlayer(backChips.Key);
                player.SetBackChips = backChips.Value;
                thisData.BackChipsDic.Add(player.SeatIndex, backChips.Value);
            }
        }

        //玩家籌碼
        foreach (var player in pack.SidePack.AllPlayerChips)
        {
            GetPlayer(player.Key).PlayerRoomChips = player.Value;
        }

        //邊池紀錄存檔
        if (thisData.LocalGamePlayerInfo.IsPlaying &&
            thisData.SideWinnerList.Count > 0)
        {
            ProcessStepHistoryData processStepHistoryData = AddNewStepHistory();
            processStepHistoryData.ActionPlayerIndex = -1;
            processStepHistoryData.ActionIndex = -1;

            processStepHistoryData.SildWinnerSeatList = new List<int>();
            foreach (var id in thisData.SideWinnerList)
            {
                int sideWinSeat = GetPlayer(id).SeatIndex;
                processStepHistoryData.SildWinnerSeatList.Add(sideWinSeat);
            }
            processStepHistoryData.SildWinChips = thisData.SideWinChips;
            processStepHistoryData.BackChipsDic = thisData.BackChipsDic;

            processHistoryData.processStepHistoryDataList.Add(processStepHistoryData);
        }

    }

    /// <summary>
    /// 遊戲階段
    /// </summary>
    /// <returns></returns>
    public IEnumerator IGameStage(MainPack pack)
    {
        thisData.SmallBlindValue = pack.GameStagePack.SmallBlind;
        thisData.CurrRaiseValue = thisData.SmallBlindValue * 2;

        //重製玩家行動文字顯示
        if (pack.GameStagePack.flowEnum == FlowEnum.PotResult)
        {
            //階段=遊戲結果
            foreach (var player in gamePlayerInfoList)
            {
                player.DisplayBetAction(false);
            }
        }
        else
        {
            //翻牌階段
            foreach (var player in gamePlayerInfoList)
            {
                if (!player.IsFold && !player.IsAllIn)
                {
                    player.DisplayBetAction(false);
                }
            }
        }

        //判斷當前遊戲進程
        switch (pack.GameStagePack.flowEnum)
        {
            //發牌
            case FlowEnum.Licensing:
                SavePreGame();
                GameInit();
                Tip_Txt.text = "";

                HandPokerLicensing(pack.LicensingStagePack.HandPokerDic);
                SetButtonSeat(pack.LicensingStagePack.ButtonSeatId);
                SetSitOutDisplay(thisData.IsSitOut);
                break;

            //大小盲
            case FlowEnum.SetBlind:
                SetActingButtonEnable = thisData.IsPlaying;
                SetTotalPot = pack.GameRoomInfoPack.TotalPot;
                SetBlind(pack.BlindStagePack);

                gameInitHistoryData = HandHistoryManager.Instance.SetGameInitData(gamePlayerInfoList,
                                                                                  thisData.TotalPot);
                break;

            //翻牌
            case FlowEnum.Flop:
                yield return IFlopCommunityPoker(pack.CommunityPokerPack.CurrCommunityPoker);
                RountInit();
                break;

            //轉牌
            case FlowEnum.Turn:
                yield return IFlopCommunityPoker(pack.CommunityPokerPack.CurrCommunityPoker);
                RountInit();
                break;

            //河牌
            case FlowEnum.River:
                yield return IFlopCommunityPoker(pack.CommunityPokerPack.CurrCommunityPoker);
                RountInit();
                break;

            //主池結果
            case FlowEnum.PotResult:
                //棄牌顯示手牌按鈕
                if (thisData.isFold == true)
                {
                    foreach (var show in ShowPokerBtnList)
                    {
                        show.gameObject.SetActive(true);
                    }
                }
                yield return IPotResult(pack);
                break;
        }

        yield return null;
    }

    /// <summary>
    /// 籌碼不足
    /// </summary>
    /// <param name="pack"></param>
    public void OnInsufficientChips(MainPack pack)
    {
        thisData.IsPlaying = false;

        //正在觀看紀錄影片
        LobbyHandHistoryView handHistoryMainView = GameObject.FindAnyObjectByType<LobbyHandHistoryView>();
        if (handHistoryMainView != null)
        {
            Destroy(handHistoryMainView.gameObject);
        }
        HistoryVideoView historyVideoView = GameObject.FindAnyObjectByType<HistoryVideoView>();
        if (historyVideoView != null)
        {
            Destroy(historyVideoView.gameObject);
        }

        //顯示購買金幣/積分結果
        if (RoomType == TableTypeEnum.CryptoTable ||
            RoomType == TableTypeEnum.VCTable)
        {
            buyChipsView.gameObject.SetActive(true);
            buyChipsView.SetBuyChipsViewInfo(false,
                                             pack.InsufficientChipsPack.SmallBlind,
                                             transform.name,
                                             RoomType,
                                             SendRequest_BuyChips);
            thisData.LocalGamePlayerInfo.Init();
        }
        else if (RoomType == TableTypeEnum.IntegralTable)
        {
            SetBattleResult(false);
        }
    }

    /// <summary>
    /// 發送購買籌碼
    /// </summary>
    /// <param name="buyValue"></param>
    public void SendRequest_BuyChips(double buyValue)
    {
        string id = Entry.TestInfoData.LocalUserId;
        baseRequest.SendRequest_BuyChips(id, buyValue);
    }

    /// <summary>
    /// 購買籌碼回到遊戲
    /// </summary>
    /// <param name="pack"></param>
    public void BuyChipsGoBack(MainPack pack)
    {
        CloseMenu();

        buyChipsView.gameObject.SetActive(false);
        double newChips = pack.BuyChipsPack.BuyChipsValue;
        thisData.LocalGamePlayerInfo.PlayerRoomChips = newChips;
    }

    /// <summary>
    /// 設置積分結果
    /// </summary>
    /// <param name="isWin"></param>
    public void SetBattleResult(bool isWin)
    {
        BattleResultView.gameObject.SetActive(true);
        BattleResultView battleResult = BattleResultView.GetComponent<BattleResultView>();
        battleResult.OnSetBattleResult(isWin, transform.name);
    }

    #region 聊天

    /// <summary>
    /// 關閉聊天
    /// </summary>
    private void CloseChatPage()
    {
        Mask_Btn.gameObject.SetActive(false);
        StartCoroutine(UnityUtils.Instance.IViewSlide(false,
                                                      ChatPage_Tr,
                                                      DirectionEnum.Left,
                                                      PageMoveTime,
                                                      () =>
                                                      {
                                                          //判斷保留訊息數量
                                                          if (ChatContent_Tr.childCount > MaxChatCount)
                                                          {
                                                              int closeCount = ChatContent_Tr.childCount - MaxChatCount;
                                                              for (int i = 0; i < closeCount; i++)
                                                              {
                                                                  if (ChatContent_Tr.GetChild(2 + i).gameObject.activeSelf)
                                                                  {
                                                                      ChatContent_Tr.GetChild(2 + i).gameObject.SetActive(false);
                                                                      float chatHeight = ChatContent_Tr.GetChild(2 + i).GetComponent<RectTransform>().rect.height;
                                                                      float reduce = Mathf.Max(0, ChatContent_Tr.anchoredPosition.y - chatHeight);
                                                                      ChatContent_Tr.anchoredPosition = new Vector2(ChatContent_Tr.anchoredPosition.x,
                                                                                                                    reduce);
                                                                  }
                                                              }
                                                          }

                                                          StartCoroutine(IGoNewChatMessage());
                                                          GameRoomManager.Instance.IsCanMoveSwitch = true;
                                                      }));
    }

    /// <summary>
    /// 設置未讀聊天訊息數
    /// </summary>
    private int SetNotReadChatCount
    {
        get
        {
            return notReadMsgCount;
        }
        set
        {
            notReadMsgCount = value;
            string countStr = value > 99 ?
                              "99+" :
                              $"{value}";
            NotReadChat_Txt.text = countStr;
            NotReadChat_Tr.gameObject.SetActive(value > 0);

            NotReadChat_Tr.sizeDelta = value > 99 ?
                                       new Vector2(20, 15) :
                                       new Vector2(15, 15);
        }
    }

    /// <summary>
    /// 延遲設置新訊息按鈕是否顯示
    /// </summary>
    /// <returns></returns>
    private IEnumerator IYieldSetNewMessageActive()
    {
        yield return null;
        NewMessage_Btn.gameObject.SetActive(!IsChatOnBottom());
    }

    /// <summary>
    /// 聊天移動至最新訊息位置
    /// </summary>
    /// <returns></returns>
    private IEnumerator IGoNewChatMessage()
    {
        yield return null;

        NewMessage_Btn.gameObject.SetActive(false);

        //顯示在最新訊息
        float chatAreaHeight = ChatArea_Sr.GetComponent<RectTransform>().rect.height;
        float currChatContentHeight = ChatContent_Tr.rect.height;
        float goPosY = Mathf.Max(0, currChatContentHeight - chatAreaHeight);
        ChatContent_Tr.anchoredPosition = new Vector2(ChatContent_Tr.anchoredPosition.x,
                                                      goPosY);
    }

    /// <summary>
    /// 判斷聊天位置是否在最底部
    /// </summary>
    /// <returns></returns>
    private bool IsChatOnBottom()
    {
        float chatAreaHeight = ChatArea_Sr.GetComponent<RectTransform>().rect.height;
        float currChatContentHeight = ChatContent_Tr.rect.height;
        float bottomPosY = Mathf.Max(0, currChatContentHeight - chatAreaHeight);
        return ChatContent_Tr.anchoredPosition.y >= Mathf.Max(0, bottomPosY - 20);
    }

    /// <summary>
    /// 發送聊天訊息
    /// </summary>
    private void SendChat()
    {
        if (string.IsNullOrEmpty(Chat_If.text))
        {
            return;
        }

        baseRequest.SendRequestRequest_Chat(Chat_If.text);
        CreateChatContent(DataManager.UserAvatar,
                          DataManager.UserNickname,
                          Chat_If.text,
                          true);

        Chat_If.text = "";

        if (!DataManager.IsMobilePlatform)
        {
            Chat_If.Select();
        }

        StartCoroutine(IGoNewChatMessage());
    }

    /// <summary>
    /// 接收聊天訊息
    /// </summary>
    /// <param name="pack"></param>
    public void ReciveChat(MainPack pack)
    {
        string id = pack.ChatPack.Id;
        string nickname = pack.ChatPack.Nickname;
        string content = pack.ChatPack.Content;
        int avatar = pack.ChatPack.Avatar;

        //判斷是否在最新訊息位置
        bool isBottom = IsChatOnBottom();
        if (ChatPage_Tr.gameObject.activeSelf)
        {
            NewMessage_Btn.gameObject.SetActive(!isBottom);
        }
        else
        {
            NewMessage_Btn.gameObject.SetActive(true);
        }

        CreateChatContent(avatar,
                          nickname,
                          content,
                          false);

        if (isBottom)
        {
            StartCoroutine(IGoNewChatMessage());
        }

        //未開啟聊天頁面顯示新訊息提示
        if (!ChatPage_Tr.gameObject.activeSelf &&
            id != DataManager.UserId)
        {
            GamePlayerInfo player = GetPlayer(id);
            player.ShowChatInfo(content);

            SetNotReadChatCount = ++SetNotReadChatCount;
        }
    }

    /// <summary>
    /// 產生聊天內容
    /// </summary>
    /// <param name="avatar"></param>
    /// <param name="nickname"></param>
    /// <param name="content">聊天內容</param>
    /// <param name="isLocal">是否為本地玩家</param>
    private void CreateChatContent(int avatar, string nickname, string content, bool isLocal)
    {
        GameObject sample = isLocal ?
                            LocalChatSample :
                            OtherChatSample;

        ChatInfoSample chatInfo = objPool.CreateObj<ChatInfoSample>(sample, ChatContent_Tr);
        chatInfo.gameObject.SetActive(true);
        chatInfo.GetComponent<RectTransform>().SetSiblingIndex(ChatContent_Tr.childCount + 1);
        chatInfo.SetChatInfo(avatar,
                             nickname,
                             content);
    }

    #endregion

    #region 記錄存檔

    /// <summary>
    /// 上一局遊戲紀錄存檔
    /// </summary>
    private void SavePreGame()
    {
        //本地玩家有參與
        if (thisData.LocalGamePlayerInfo.IsPlaying)
        {
            HandHistoryManager.Instance.SaveResult(saveResultData);
            HandHistoryManager.Instance.SaveGameInit(gameInitHistoryData);
            HandHistoryManager.Instance.SaveProcess(processHistoryData);
        }

        exitPlayerSeatList = new List<int>();
        processHistoryData = new ProcessHistoryData();
        processHistoryData.processStepHistoryDataList = new List<ProcessStepHistoryData>();


        //更新存檔資料
        HandHistoryView handHistoryView = GameObject.FindAnyObjectByType<HandHistoryView>();
        handHistoryView?.UpdateHitoryDate();
    }

    /// <summary>
    /// 添加行動存檔新紀錄
    /// </summary>
    /// <returns></returns>
    private ProcessStepHistoryData AddNewStepHistory()
    {
        //紀錄存檔
        ProcessStepHistoryData processStepHistoryData = new ProcessStepHistoryData();

        processStepHistoryData.SeatList = new List<int>();
        processStepHistoryData.ChipsList = new List<double>();
        processStepHistoryData.BetChipsList = new List<double>();
        processStepHistoryData.HandPoker1 = new List<int>();
        processStepHistoryData.HandPoker2 = new List<int>();
        processStepHistoryData.BetActionEnumIndex = new List<int>();
        foreach (var player in gamePlayerInfoList)
        {
            processStepHistoryData.SeatList.Add(player.SeatIndex);
            processStepHistoryData.ChipsList.Add(player.CurrRoomChips);
            processStepHistoryData.BetChipsList.Add(player.CurrBetValue);
            processStepHistoryData.HandPoker1.Add(player.GetHandPoker[0].PokerNum);
            processStepHistoryData.HandPoker2.Add(player.GetHandPoker[1].PokerNum);
            processStepHistoryData.BetActionEnumIndex.Add(Convert.ToInt32(player.CurrBetAction));
        }
        processStepHistoryData.CommunityPoker = thisData.CurrCommunityPoker;
        processStepHistoryData.TotalPot = thisData.TotalPot;
        processStepHistoryData.ExitPlayerSeatList = exitPlayerSeatList;

        return processStepHistoryData;
    }

    #endregion

    /// <summary>
    /// 移除資料
    /// </summary>
    private void OnRemoveData()
    {
        thisData = null;
        StopAllCoroutines();
    }

    private void OnDisable()
    {
        OnRemoveData();
    }

    private void OnDestroy()
    {
        LanguageManager.Instance.RemoveLanguageFun(UpdateLanguage);
        OnRemoveData();
    }
}
