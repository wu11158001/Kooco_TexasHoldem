using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using Thirdweb;

using RequestBuf;

public class GameView : MonoBehaviour
{
    [SerializeField]
    Request_GameView baseRequest;

    [Header("座位上玩家訊息")]
    [SerializeField]
    List<GamePlayerInfo> SeatGamePlayerInfoList;
    [SerializeField]
    List<Button> SeatButtonList;

    [Header("操作按鈕")]
    [SerializeField]
    Button Raise_Btn, Call_Btn, Fold_Btn;
    [SerializeField]
    Text RaiseBtn_Txt, CallBtn, FoldBtn_Txt;
    [SerializeField]
    RectTransform AutoActionFrame_Tr;
    [SerializeField]
    Button[] ShowPokerBtnList;

    [Header("加注操作")]
    [SerializeField]
    List<Button> PotPercentRaiseBtnList;
    [SerializeField]
    List<Text> PotPercentRaiseTxtList;
    [SerializeField]
    Slider Raise_Sli;
    [SerializeField]
    RectTransform Raise_Tr;
    [SerializeField]
    SliderClickDetection SliderClickDetection;
    [SerializeField]
    Text RaiseSliHandle_Txt, CurrRaise_Txt, MinRaiseBtn_Txt;
    [SerializeField]
    Button AllIn_Btn, MinRaise_Btn;

    [Header("訊息")]
    [SerializeField]
    Text TotalPot_Txt, Tip_Txt, WinType_Txt;
    [SerializeField]
    Image Pot_Img;

    [Header("公共牌")]
    [SerializeField]
    List<Poker> CommunityPokerList;

    [Header("選單")]
    [SerializeField]
    RectTransform MenuBg_Tr;
    [SerializeField]
    Button Menu_Btn, MenuMask_Btn, BackGame_Btn, LogOut_Btn;
    [SerializeField]
    Text MenuNickname_Txt, MenuWalletAddr_Txt;

    [Header("遊戲結果")]
    [SerializeField]
    RectTransform BuyChipsView, BattleResultView;

    [Header("遊戲暫停")]
    [SerializeField]
    GameObject GamePause_Obj;
    [SerializeField]
    Button GameContinue_Btn;

    //底池倍率
    float[] potPercentRate = new float[]
    {
        33, 50, 80, 100,
    };

    //加註大盲倍率
    float[] potBbRate = new float[]
    {
        2.1f, 2.5f, 3.0f, 4.0f,
    };

    List<GamePlayerInfo> gamePlayerInfoList;                    //玩家資料

    Vector2 InitPotPointPos;    //初始底池位置

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
        public bool isLocalPlayerTurn;                     //本地玩家回合
        public bool isFold;                                //是否已棄牌
        public List<int> CurrCommunityPoker;               //當前公共牌
        public List<string> potWinnerList;                 //主池贏家
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
        ListenerEvent();

        //初始底池位置
        InitPotPointPos = Pot_Img.rectTransform.anchoredPosition;
    }

    /// <summary>
    /// 聆聽事件
    /// </summary>
    private void ListenerEvent()
    {
        //遊戲繼續按鈕
        GameContinue_Btn.onClick.AddListener(() =>
        {
            GameRoomManager.Instance.OnGamePause(false);
        });

        #region 選單

        //選單
        Menu_Btn.onClick.AddListener(() =>
        {
            StartCoroutine(IIsShowMenu(true));
        });

        //選單遮罩按鈕
        MenuMask_Btn.onClick.AddListener(() =>
        {
            StartCoroutine(IIsShowMenu(false));
        });

        //離開房間
        LogOut_Btn.onClick.AddListener(() =>
        {
            GameRoomManager.Instance.RemoveGameRoom(transform.name);
        });

        //返回遊戲
        BackGame_Btn.onClick.AddListener(() =>
        {
            StartCoroutine(IIsShowMenu(false));
        });

        #endregion

        #region 操作按鈕

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
    }

    private void OnEnable()
    {
        thisData = new ThisData();
        thisData.IsPlaying = false;

        strData = new StrData();

        if (gamePlayerInfoList != null)
        {
            foreach (var player in gamePlayerInfoList)
            {
                Destroy(player.gameObject);
            }
        }

        gamePlayerInfoList = new List<GamePlayerInfo>();
        BuyChipsView.gameObject.SetActive(false);
        BattleResultView.gameObject.SetActive(false);


        Init();
        GameInit();
    }

    private void Start()
    {
        LanguageManager.Instance.AddUpdateLanguageFunc(UpdateLanguage);
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
    public double TotalPot
    {
        set
        {
            StringUtils.ChipsChangeEffect(TotalPot_Txt, value);
        }
    }

    /// <summary>
    /// 顯示選單
    /// </summary>
    /// <param name="isShow"></param>
    /// <returns></returns>
    private IEnumerator IIsShowMenu(bool isShow)
    {
        MenuMask_Btn.gameObject.SetActive(isShow);
        MenuBg_Tr.gameObject.SetActive(true);

        if (isShow == true)
        {
            MenuBg_Tr.anchoredPosition = new Vector2(-MenuBg_Tr.rect.width, 0);
            StringUtils.StrExceedSize(DataManager.UserWalletAddress, MenuWalletAddr_Txt);
            MenuNickname_Txt.text = $"@{DataManager.UserNickname}";
        }

        GameRoomManager.Instance.IsCanMoveSwitch = !isShow;

        //選單移動
        float moveTime = 0.25f;
        float currX = MenuBg_Tr.anchoredPosition.x;
        float targetX = isShow ?
                        0 :
                        -MenuBg_Tr.rect.width;

        DateTime startTime = DateTime.Now;
        while ((DateTime.Now - startTime).TotalSeconds < moveTime)
        {
            float progress = (float)(DateTime.Now - startTime).TotalSeconds / moveTime;
            float x = Mathf.Lerp(currX, targetX, progress);
            MenuBg_Tr.anchoredPosition = new Vector2(x, 0);
            yield return null;
        }

        MenuBg_Tr.gameObject.SetActive(isShow);
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
        TotalPot_Txt.text = "$0";

        MenuMask_Btn.gameObject.SetActive(false);
        Raise_Tr.gameObject.SetActive(false);
        StartCoroutine(IIsShowMenu(false));
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
            player.SetPotWinnerActive = false;
            player.SetSideWinnerActive = false;
            player.SetTip = "";
            player.GetHandPoker[0].gameObject.SetActive(false);
            player.GetHandPoker[1].gameObject.SetActive(false);
            player.OpenInfoMask = true;
        }
        foreach (var show in ShowPokerBtnList)
        {
            show.gameObject.SetActive(false);
        }

        TotalPot_Txt.text = "$0";
        WinType_Txt.text = "";
        Pot_Img.enabled = false;
        SetActionButton = false;
        AutoActionState = AutoActingEnum.None;
        Fold_Btn.gameObject.SetActive(true);
        Call_Btn.gameObject.SetActive(true);
        Raise_Btn.gameObject.SetActive(true);

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
                         (int)((float)(thisData.SmallBlindValue * 2) * potBbRate[btnIndex]) :
                         (int)((float)thisData.TotalPot * (potPercentRate[btnIndex] / 100));
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
                    PotPercentRaiseTxtList[i].text = $"{potBbRate[i]}BB";
                }
                else
                {
                    PotPercentRaiseTxtList[i].text = $"{potPercentRate[i]}%";
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
                                         AssetsManager.Instance.GetAlbumAsset(AlbumEnum.AvatarAlbum).album[playerInfoPack.Avatar],
                                         Pot_Img.rectTransform);

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

                    //關閉所有撲克外框
                    List<Poker> pokers = CommunityPokerList.Concat(thisData.LocalGamePlayerInfo.GetHandPoker.ToList()).ToList();
                    foreach (var poker in pokers)
                    {
                        poker.PokerFrameEnable = false;
                    }
                    break;

                //All In
                case ActingEnum.AllIn:
                    SetActingButtonEnable = false;
                    break;
            }

        }

        GamePlayerInfo playerInfo = GetPlayer(id);
        if (playerInfo != null && playerInfo.gameObject.activeSelf)
        {
            playerInfo.PlayerAction(actionEnum,
                                    betValue,
                                    chips,
                                    isLocalPlayer);
        }

        if (actionEnum == ActingEnum.AllIn)
        {
            playerInfo.SetTip = "All In";
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
            if(pack.UpdateRoomInfoPack.playingIdList.Contains(gamePlayerInfo.UserId))
            {
                gamePlayerInfo.SetHandPoker(-1, -1);
            }
            if (player.CurrBetValue > 0)
            {
                gamePlayerInfo.PlayerBet(player.CurrBetValue, player.Chips);
            }            
        }

        //底池
        TotalPot = pack.UpdateRoomInfoPack.TotalPot;
        if ((int)pack.UpdateRoomInfoPack.flowEnum >= 2)
        {
            SetPotActive = true;
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

        /*buttonSeat_Tr.gameObject.SetActive(true);
        buttonSeat_Tr.position = buttonPlayer.gameObject.transform.position;

        ObjMoveUtils.ObjMoveTowardsTarget(buttonSeat_Tr, pot_Img.transform.position, 0.5f, 160);*/
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
    /// 翻開公共牌
    /// </summary>
    /// <param name="currCommunityPoker"></param>
    /// <returns></returns>
    private IEnumerator IFlopCommunityPoker(List<int> currCommunityPoker)
    {
        thisData.CurrCommunityPoker = currCommunityPoker;

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

        //關閉所有撲克外框
        Poker[] localHandPoker = GetPlayer(Entry.TestInfoData.LocalUserId).GetHandPoker;
        List<Poker> allPokerList = CommunityPokerList.Concat(localHandPoker.ToList()).ToList();
        foreach (var poker in allPokerList)
        {
            poker.PokerFrameEnable = false;
        }

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

        //贏得類型顯示
        WinType_Txt.text = LanguageManager.Instance.GetText("Pot");
        TotalPot_Txt.text = StringUtils.SetChipsUnit(pack.WinnerPack.WinChips);

        //贏家效果
        thisData.potWinnerList = pack.WinnerPack.WinnerDic.Select(x => x.Key).ToList();
        int count = 0;
        foreach (var winner in pack.WinnerPack.WinnerDic)
        {
            count++;

            GamePlayerInfo player = GetPlayer(winner.Key);
            JudgePokerShape(player, true, true);

            player.SetPotWinnerActive = true;
            player.SetWinnerEffect = true;
            Vector2 winnerSeatPos = player.gameObject.transform.position;

            //產生贏得籌碼物件
            RectTransform rt = Instantiate(AssetsManager.Instance.GetObjtypeAsset(ObjTypeEnum.WinChips)).GetComponent<RectTransform>();
            rt.SetParent(transform);
            rt.anchoredPosition = Pot_Img.rectTransform.anchoredPosition;
            WinChips winChips = rt.GetComponent<WinChips>();
            winChips.SetWinChips = pack.WinnerPack.WinChips;
            SetPotActive = false;

            yield return new WaitForSeconds(0.5f);

            ObjMoveUtils.ObjMoveToTarget(rt, winnerSeatPos, 0.5f,
                                        () =>
                                        {
                                            Destroy(rt.gameObject);
                                            player.PlayerRoomChips = winner.Value;
                                        });

            yield return new WaitForSeconds(2);

            //最後一位不關
            if (count < pack.WinnerPack.WinnerDic.Count())
            {
                //關閉撲克外框
                Poker[] handPoker = player.GetHandPoker;
                List<Poker> pokerList = CommunityPokerList.Concat(handPoker.ToList()).ToList();
                foreach (var poker in pokerList)
                {
                    poker.PokerFrameEnable = false;
                }
                player.SetWinnerEffect = false;
            }
        }
    }

    /// <summary>
    /// 邊池結果
    /// </summary>
    /// <param name="pack"></param>
    public IEnumerator SideResult(MainPack pack)
    {
        //贏得類型顯示
        bool isShow = true;
        foreach (var item in pack.SidePack.SideWinnerDic)
        {
            if (thisData.potWinnerList.Contains(item.Key))
            {
                isShow = false;
            }
        }

        if (isShow)
        {
            WinType_Txt.text = LanguageManager.Instance.GetText("Side");
            TotalPot_Txt.text = StringUtils.SetChipsUnit(pack.SidePack.TotalSideChips);
        }

        //關閉所有撲克外框
        foreach (var player in gamePlayerInfoList)
        {
            //關閉撲克外框
            Poker[] handPoker = player.GetHandPoker;
            List<Poker> pokerList = CommunityPokerList.Concat(handPoker.ToList()).ToList();
            foreach (var poker in pokerList)
            {
                poker.PokerFrameEnable = false;
            }
            player.SetWinnerEffect = false;
        }

        //邊池贏家效果
        foreach (var sideWinner in pack.SidePack.SideWinnerDic)
        {
            GamePlayerInfo player = GetPlayer(sideWinner.Key);
            player.SetWinnerEffect = true;
            player.SetSideWinnerActive = isShow;

            Vector2 winnerSeatPos = player.gameObject.transform.position;
            JudgePokerShape(player, true, true);

            //有贏得籌碼
            if (sideWinner.Value > 0)
            {
                RectTransform rt = Instantiate(AssetsManager.Instance.GetObjtypeAsset(ObjTypeEnum.WinChips)).GetComponent<RectTransform>();
                rt.SetParent(transform);
                rt.anchoredPosition = Pot_Img.rectTransform.anchoredPosition;
                WinChips winChips = rt.GetComponent<WinChips>();
                winChips.SetWinChips = sideWinner.Value;

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
            player.SetWinnerEffect = false;
        }

        yield return new WaitForSeconds(0.5f);

        //顯示退回籌碼
        foreach (var backChips in pack.SidePack.BackChips)
        {
            if (backChips.Value > 0)
            {
                GetPlayer(backChips.Key).SetTip = $"+{StringUtils.SetChipsUnit(backChips.Value)}";
            }
        }

        //玩家籌碼
        foreach (var player in pack.SidePack.AllPlayerChips)
        {
            GetPlayer(player.Key).PlayerRoomChips = player.Value;
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

        //判斷當前遊戲進程
        switch (pack.GameStagePack.flowEnum)
        {
            //發牌
            case FlowEnum.Licensing:
                GameInit();
                Tip_Txt.text = "";

                //設置公共牌
                List<string> userIdList = new List<string>();
                foreach (var player in gamePlayerInfoList)
                {
                    userIdList.Add(player.UserId);
                }
                //設置手牌
                HandPokerLicensing(pack.LicensingStagePack.HandPokerDic);

                //Button座位
                SetButtonSeat(pack.LicensingStagePack.ButtonSeatId);

                break;

            //大小盲
            case FlowEnum.SetBlind:
                SetActingButtonEnable = true;
                TotalPot = pack.GameRoomInfoPack.TotalPot;
                SetBlind(pack.BlindStagePack);
                break;

            //翻牌
            case FlowEnum.Flop:
                yield return IFlopCommunityPoker(pack.CommunityPokerPack.CurrCommunityPoker);
                break;

            //轉牌
            case FlowEnum.Turn:
                yield return IFlopCommunityPoker(pack.CommunityPokerPack.CurrCommunityPoker);
                break;

            //河牌
            case FlowEnum.River:
                yield return IFlopCommunityPoker(pack.CommunityPokerPack.CurrCommunityPoker);
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

        if (RoomType == TableTypeEnum.CryptoTable ||
            RoomType == TableTypeEnum.VCTable)
        {
            BuyChipsView.gameObject.SetActive(true);
            BuyChipsView buyChipsV = BuyChipsView.GetComponent<BuyChipsView>();
            buyChipsV.SetBuyChipsViewInfo(pack.InsufficientChipsPack.SmallBlind, 
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
        BuyChipsView.gameObject.SetActive(false);
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
