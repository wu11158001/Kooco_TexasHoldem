using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

using RequestBuf;

public class GameView : BaseView
{
    Request_GameView thisRequestView;
    public override void SetRequestView()
    {
        thisRequestView = requestView as Request_GameView;
    }

    [Header("操作按鈕")]
    [SerializeField]
    Button raiseAndAllIn_Btn, callAndCheck_Btn, fold_Btn;
    [SerializeField]
    Text raiseAndAllInBtn_Txt, callAndCheckBt_Txt, foldBtn_Txt;
    [SerializeField]
    RectTransform autoActionFrame_Tr;
    [SerializeField]
    Button[] showPokerBtnList;

    [Header("加注操作")]
    [SerializeField]
    List<Button> potPercentRaiseBtnList;
    [SerializeField]
    List<Text> potPercentRaiseTxtList;
    [SerializeField]
    Slider raise_Sli;
    [SerializeField]
    RectTransform raise_Tr;
    [SerializeField]
    SliderClickDetection sliderClickDetection;
    [SerializeField]
    Text raiseSliHandle_Txt, raiseAllIn_Txt, raiseTo_Txt;
    [SerializeField]
    GameObject raiseTo_Obj;

    [Header("訊息")]
    [SerializeField]
    Text totalPot_Txt, tip_Txt, winType_Txt;
    [SerializeField]
    Image pot_Img;
    [SerializeField]
    RectTransform buttonSeat_Tr, localHandPokerPoint_Tr;

    [Header("公共牌")]
    [SerializeField]
    List<Poker> communityPokerList;

    [Header("選單")]
    [SerializeField]
    Button menu_Btn, menuMask_Btn, exitRoom_Btn;

    [Header("遊戲暫停")]
    [SerializeField]
    GameObject gamePause_Obj;
    [SerializeField]
    Button gameContinue_Btn;

    //底池倍率
    float[] potPercentRate = new float[]
    {
        33, 50, 75, 100,
    };

    //加註大盲倍率
    float[] potBbRate = new float[]
    {
        2.1f, 2.5f, 3.0f, 4.0f,
    };


    List<Button> seatList = new List<Button>();                 //玩家座位位置
    List<GamePlayerInfo> gamePlayerInfoList;                    //玩家資料

    Vector2 InitPotPointPos;    //初始底池位置

    private GameData gameData;
    public class GameData
    {
        public GamePlayerInfo LocalGamePlayerInfo;      //本地玩家
        public int LocalPlayerSeat;                     //本地玩家座位
        public int LocalPlayerChips;                    //本地玩家籌碼
        public int LocalPlayerCurrBetValue;             //本地玩家當前下注值
        public int TotalPot;                            //當前底池
        public int CallDifference;                      //當前跟注差額
        public int CurrCallValue;                       //當前跟注值
        public int CurrRaiseValue;                      //當前加注值
        public int MinRaiseValue;                       //最小加注值
        public int SmallBlindValue;                     //小盲值
        public bool IsFirstRaisePlayer;                 //首位加注玩家
        public bool IsUnableRaise;                      //無法加注
        public bool IsPlaying;                          //有參與遊戲
        public bool isLocalPlayerTurn;                  //本地玩家回合
        public bool isFold;                             //是否已棄牌
        public List<int> CurrCommunityPoker;            //當前公共牌
    }

    /// <summary>
    /// 遊戲暫停
    /// </summary>
    public bool GamePause
    {
        set
        {
            Entry.isPause = value;
            Time.timeScale = value == true ? 0 : 1;
            gamePause_Obj.SetActive(value);
        }
    }

    /// <summary>
    /// 設定加註至文字
    /// </summary>
    public double SetRaiseToText
    {
        set
        {
            if (value >= gameData.LocalPlayerChips)
            {
                //All In
                raiseTo_Txt.text = $"<size=9><color=#000000>Raise To</color></size>\n<b><size=13><color=#2E1675>All In</color></size></b>";
                raiseSliHandle_Txt.text = $"All In";
            }
            else
            {
                raiseTo_Txt.text = $"<size=9><color=#000000>Raise To</color></size>\n<b><size=13><color=#2E1675>${StringUtils.SetChipsUnit(value)}</color></size></b>";
                raiseSliHandle_Txt.text = $"${StringUtils.SetChipsUnit(value)}";
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
            gameData.isLocalPlayerTurn = value;
            if (SetActingButtonEnable == true)
            {
                if (value == false)
                {
                    raise_Tr.gameObject.SetActive(false);
                    foldBtn_Txt.text = "Check / Fold";
                    callAndCheckBt_Txt.text = "Check";
                    raiseAndAllInBtn_Txt.text = "Call Any";
                }
                else
                {
                    foldBtn_Txt.text = "Fold";
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
            return raiseAndAllIn_Btn.interactable;
        }
        set
        {
            raiseAndAllIn_Btn.interactable = value;
            callAndCheck_Btn.interactable = value;
            fold_Btn.interactable = value;
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
                    SetAutoAction(true, raiseAndAllIn_Btn.transform);
                    break;
                case AutoActingEnum.Check:
                    SetAutoAction(true, callAndCheck_Btn.transform);
                    break;
                case AutoActingEnum.CheckAndFold:
                    SetAutoAction(true, fold_Btn.transform);
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
        autoActionFrame_Tr.gameObject.SetActive(isActive);
        if (parent != null)
        {
            autoActionFrame_Tr.SetParent(parent);
            autoActionFrame_Tr.anchoredPosition = Vector2.zero;
            autoActionFrame_Tr.offsetMax = Vector2.zero;
            autoActionFrame_Tr.offsetMin = Vector2.zero;
        }
    }

    /// <summary>
    /// 設置底池顯示
    /// </summary>
    public bool SetPotActive
    {
        set
        {
            pot_Img.enabled = value;
            pot_Img.rectTransform.anchoredPosition = InitPotPointPos;
        }
    }

    /// <summary>
    /// 設置底池籌碼
    /// </summary>
    public int TotalPot
    {
        set
        {
            StringUtils.ChipsChangeEffect(totalPot_Txt, value.ToString(), "$");
        }
    }

    /// <summary>
    /// 初始化
    /// </summary>
    private void Init()
    {
        foreach (var poker in communityPokerList)
        {
            poker.gameObject.SetActive(false);
        }
        foreach (var player in gamePlayerInfoList)
        {
            player.SetPotWinnerActive = false;
            player.SetSideWinnerActive = false;
            player.SetTip = "";
        }
        foreach (var show in showPokerBtnList)
        {
            show.gameObject.SetActive(false);
        }

        totalPot_Txt.text = "$0";
        winType_Txt.text = "";
        pot_Img.enabled = false;
        SetActionButton = false;
        buttonSeat_Tr.gameObject.SetActive(false);
        AutoActionState = AutoActingEnum.None;
        fold_Btn.gameObject.SetActive(true);
        callAndCheck_Btn.gameObject.SetActive(true);
        raiseAndAllIn_Btn.gameObject.SetActive(true);

        gameData.isFold = false;
        gameData.CurrCommunityPoker = new List<int>();
    }

    public override void Awake()
    {
        base.Awake();

        Entry.gameView = this;

        ListenerEvent();

        //座位點
        Transform seatPoint = transform.Find("SeatPoints");
        for (int i = 0; i < seatPoint.childCount; i++)
        {
            seatList.Add(seatPoint.GetChild(i).GetComponent<Button>());
        }

        //初始底池位置
        InitPotPointPos = pot_Img.rectTransform.anchoredPosition;
    }

    private void OnEnable()
    {
        gameData = new GameData();
        gameData.IsPlaying = false;

        if (gamePlayerInfoList != null)
        {
            foreach (var player in gamePlayerInfoList)
            {
                Destroy(player.gameObject);
            }
        }

        gamePlayerInfoList = new List<GamePlayerInfo>();

        tip_Txt.text = "Waiting...";
        foldBtn_Txt.text = "Check / Fold";
        callAndCheckBt_Txt.text = "Check";
        raiseAndAllInBtn_Txt.text = "Call Any";
        totalPot_Txt.text = "$0";

        menuMask_Btn.gameObject.SetActive(false);
        raise_Tr.gameObject.SetActive(false);
        SetActingButtonEnable = false;

        Init();
        thisRequestView.SendRequest_UpdateRoomInfo();
    }

    private void Update()
    {
        //加注至物件顯示
        raiseTo_Obj.SetActive(sliderClickDetection.GetSkiderClicked);
    }

    /// <summary>
    /// 聆聽事件
    /// </summary>
    private void ListenerEvent()
    {
        //遊戲繼續按鈕
        gameContinue_Btn.onClick.AddListener(() =>
        {
            GamePause = false;
        });

        //選單
        menu_Btn.onClick.AddListener(() =>
        {
            menuMask_Btn.gameObject.SetActive(!menuMask_Btn.gameObject.activeSelf);
        });

        //選單遮罩按鈕
        menuMask_Btn.onClick.AddListener(() =>
        {
            menuMask_Btn.gameObject.SetActive(false);
        });

        //離開房間
        exitRoom_Btn.onClick.AddListener(() =>
        {
            thisRequestView.SendRequest_ExitRoom(Entry.TestInfoData.LocalUserId);
            Entry.Instance.gameServer.gameObject.SetActive(false);
            UIManager.Instance.OpenView(ViewName.JoinRoomView);
        });

        //棄牌顯示手牌按鈕
        for (int i = 0; i < showPokerBtnList.Count(); i++)
        {
            int index = i;
            showPokerBtnList[i].onClick.AddListener(delegate { ShowFoldPoker(index); });
        }

        //加注滑條
        raise_Sli.onValueChanged.AddListener((value) =>
        {
            float stepSize = gameData.SmallBlindValue * 2;
            float newRaiseValue = sliderClickDetection.GetSkiderClicked ? Mathf.Round(value / stepSize) * stepSize : value;
            raise_Sli.value = newRaiseValue >= raise_Sli.maxValue ? raise_Sli.maxValue : newRaiseValue;

            SetRaiseToText = newRaiseValue;

            if (newRaiseValue <= gameData.CurrCallValue)
            {
                newRaiseValue = gameData.CurrCallValue;
                raise_Sli.value = newRaiseValue;
            }

            if (newRaiseValue >= gameData.LocalPlayerChips)
            {
                newRaiseValue = gameData.LocalPlayerChips;
                raiseAndAllInBtn_Txt.text = $"All In\n${StringUtils.SetChipsUnit(gameData.LocalPlayerChips)}";
            }
            else
            {
                raiseAndAllInBtn_Txt.text = $"Raise to\n${StringUtils.SetChipsUnit(newRaiseValue)}";
            }            

            gameData.CurrRaiseValue = (int)newRaiseValue;
        });

        //底池百分比加註
        for (int i = 0; i < potPercentRaiseBtnList.Count; i++)
        {
            int index = i;
            potPercentRaiseBtnList[i].onClick.AddListener(delegate { PotRaisePercent(index); });
        }

        //棄牌
        fold_Btn.onClick.AddListener(() =>
        {
            if (gameData.isLocalPlayerTurn)
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
        callAndCheck_Btn.onClick.AddListener(() =>
        {
            if (gameData.isLocalPlayerTurn)
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
        raiseAndAllIn_Btn.onClick.AddListener(() =>
        {
            if (gameData.isLocalPlayerTurn)
            {
                bool isAllIn = gameData.LocalPlayerChips < gameData.MinRaiseValue ||
                           gameData.CurrRaiseValue == gameData.LocalPlayerChips;

                ActingEnum acting = isAllIn == true ?
                                    ActingEnum.AllIn :
                                    ActingEnum.Raise;

                int betValue = isAllIn == true ?
                               gameData.LocalPlayerChips :
                               gameData.CurrRaiseValue;

                thisRequestView.SendRequest_PlayerActed(Entry.TestInfoData.LocalUserId,
                                                        acting,
                                                        betValue);
            }
            else
            {
                AutoActionState = AutoActionState == AutoActingEnum.CallAny ?
                                  AutoActingEnum.None :
                                  AutoActingEnum.CallAny;
            }
        });
    }

    /// <summary>
    /// 棄牌
    /// </summary>
    private void OnFold()
    {
        thisRequestView.SendRequest_PlayerActed(Entry.TestInfoData.LocalUserId,
                                                ActingEnum.Fold,
                                                0);       
    }

    /// <summary>
    /// 跟注/過牌
    /// </summary>
    private void OnCallAndCheck()
    {
        int betValue = 0;
        ActingEnum acting = ActingEnum.Call;
        if (gameData.IsFirstRaisePlayer)
        {
            if (gameData.CurrCallValue == gameData.SmallBlindValue)
            {
                acting = ActingEnum.Check;
            }
            else
            {
                betValue = gameData.CurrCallValue;
            }
        }
        else
        {
            if (gameData.LocalPlayerCurrBetValue == gameData.CurrCallValue)
            {
                acting = ActingEnum.Check;
            }
            else if (gameData.LocalPlayerChips <= gameData.CurrCallValue)
            {
                acting = ActingEnum.AllIn;
                betValue = gameData.LocalPlayerChips;
            }
            else
            {
                betValue = gameData.CurrCallValue;
            }
        }

        thisRequestView.SendRequest_PlayerActed(Entry.TestInfoData.LocalUserId,
                                                acting,
                                                betValue);
    }

    /// <summary>
    /// 顯示棄牌手牌
    /// </summary>
    /// <param name="index"></param>
    private void ShowFoldPoker(int index)
    {
        showPokerBtnList[index].gameObject.SetActive(false);  
        thisRequestView.SendShowFoldPoker(index);
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
        int raiseValue = gameData.IsFirstRaisePlayer ?
                         (int)((float)(gameData.SmallBlindValue * 2) * potBbRate[btnIndex]) :
                         (int)((float)gameData.TotalPot * (potPercentRate[btnIndex] / 100));
        raise_Sli.value = raiseValue;
    }

    /// <summary>
    /// 本地玩家回合
    /// </summary>
    /// <param name="playerActingRoundPack"></param>
    public void ILocalPlayerRound(PlayerActingRoundPack playerActingRoundPack)
    {
        SetActionButton = true;

        //當前底池
        gameData.TotalPot = playerActingRoundPack.TotalPot;
        //玩家籌碼
        gameData.LocalPlayerChips = playerActingRoundPack.PlayerChips;
        //首位加注玩家
        gameData.IsFirstRaisePlayer = playerActingRoundPack.IsFirstRaisePlayer;
        //當前跟注值
        gameData.CurrCallValue = playerActingRoundPack.CurrCallValue;
        //跟注差額
        gameData.CallDifference = playerActingRoundPack.CallDifference;
        //玩家當前下注值
        gameData.LocalPlayerCurrBetValue = playerActingRoundPack.PlayerCurrBryValue;
        //無法加注
        gameData.IsUnableRaise = playerActingRoundPack.IsUnableRaise;
        //最小加注
        gameData.MinRaiseValue = gameData.CurrCallValue * 2;
        gameData.CurrRaiseValue = gameData.MinRaiseValue;


        if (AutoActionState != AutoActingEnum.None)
        {
            StartCoroutine(JudgeAutoAction());
            return;
        }

        ShowBetArea();
    }

    /// <summary>
    /// 顯示下注區塊
    /// </summary>
    private void ShowBetArea()
    {
        //是否無法在加注
        bool IsUnableRaise = gameData.IsUnableRaise;
        bool isJustAllIn = gameData.LocalPlayerChips <= gameData.CurrCallValue;

        //棄牌
        fold_Btn.gameObject.SetActive(true);

        //加注&All In
        raiseAndAllInBtn_Txt.text = isJustAllIn == true ?
                                    $"All In\n${gameData.LocalPlayerChips}" :
                                    $"Raise to\n${gameData.CurrRaiseValue}";
        if (IsUnableRaise == true && isJustAllIn == false)
        {
            raiseAndAllIn_Btn.gameObject.SetActive(false);
        }
        else
        {
            raiseAndAllIn_Btn.gameObject.SetActive(true);
        }

        //跟注&過牌
        string callAndChcckStr = $"Call\n${gameData.CurrCallValue - gameData.CallDifference}";
        if (gameData.IsFirstRaisePlayer == true)
        {
            if (gameData.CurrCallValue == gameData.SmallBlindValue)
            {
                callAndChcckStr = "Check";
            }
            else
            {
                callAndChcckStr = $"Call\n${gameData.CallDifference}";
            }
        }
        else
        {
            if (gameData.LocalPlayerCurrBetValue == gameData.CurrCallValue)
            {
                callAndChcckStr = "Check";
            }
            else
            {
                callAndChcckStr = $"Call\n${gameData.CallDifference}";
            }
        }
        callAndCheckBt_Txt.text = callAndChcckStr;
        if (IsUnableRaise == true && isJustAllIn == false)
        {
            callAndCheck_Btn.gameObject.SetActive(true);
        }
        else
        {
            callAndCheck_Btn.gameObject.SetActive(isJustAllIn == false);
        }

        //加注區域物件
        raise_Tr.gameObject.SetActive(isJustAllIn == false &&
                                      gameData.LocalPlayerChips > gameData.MinRaiseValue);
        if (isJustAllIn == false)
        {
            //倍數
            int multiple = (int)Mathf.Ceil((float)gameData.LocalPlayerChips / (float)(gameData.SmallBlindValue * 2));
            raise_Sli.maxValue = (gameData.SmallBlindValue * 2) * multiple;
            raise_Sli.minValue = gameData.MinRaiseValue;
            raise_Sli.value = gameData.MinRaiseValue;

            //加注值
            SetRaiseToText = gameData.MinRaiseValue;

            //All值
            raiseAllIn_Txt.text = $"<color=#1FE822>All In</color>\n<color=#D3D3D3>${StringUtils.SetChipsUnit(gameData.LocalPlayerChips)}</color>";

            //底池倍率
            for (int i = 0; i < potPercentRaiseTxtList.Count; i++)
            {
                if (gameData.TotalPot <= gameData.SmallBlindValue * 3)
                {
                    potPercentRaiseTxtList[i].text = $"{potBbRate[i]}BB";
                }
                else
                {
                    potPercentRaiseTxtList[i].text = $"{potPercentRate[i]}%";
                }
            }
        }
    }

    /// <summary>
    /// 自動操作判斷
    /// </summary>
    private IEnumerator JudgeAutoAction()
    {
        yield return null;

        switch (AutoActionState)
        {
            //任何跟注
            case AutoActingEnum.CallAny:                
                OnCallAndCheck();
                break;

            //過牌
            case AutoActingEnum.Check:
                if (gameData.IsFirstRaisePlayer == true)
                {
                    if (gameData.CurrCallValue == gameData.SmallBlindValue)
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
                    if (gameData.LocalPlayerCurrBetValue == gameData.CurrCallValue)
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
                if (gameData.IsFirstRaisePlayer == true)
                {
                    if (gameData.CurrCallValue == gameData.SmallBlindValue)
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
                    if (gameData.LocalPlayerCurrBetValue == gameData.CurrCallValue)
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
        RectTransform rt = Instantiate(GameAssetsManager.Instance.GamePlayerInfoObj).GetComponent<RectTransform>();
        GamePlayerInfo gamePlayerInfo = rt.GetComponent<GamePlayerInfo>();

        int seatIndex = 0;//座位(本地玩家 = 0)
        if (playerInfoPack.UserID != Entry.TestInfoData.LocalUserId)
        {
            seatIndex = playerInfoPack.Seat > gameData.LocalPlayerSeat ?
            playerInfoPack.Seat - gameData.LocalPlayerSeat :
            seatList.Count - (gameData.LocalPlayerSeat - playerInfoPack.Seat);            
        }
        else
        {
            //本地玩家
            gameData.LocalGamePlayerInfo = gamePlayerInfo;
            gamePlayerInfo.SetLocalHandPokerPosition = localHandPokerPoint_Tr;
        }
        seatList[seatIndex].image.enabled = false;
        rt.transform.SetParent(seatList[seatIndex].transform);
        rt.anchoredPosition = Vector2.zero;
        rt.localScale = Vector3.one;

        gamePlayerInfo.SetInitPlayerInfo(seatIndex,
                                         playerInfoPack.UserID,
                                         playerInfoPack.NickName,
                                         playerInfoPack.Chips,
                                         null,
                                         pot_Img.transform.position);

        gamePlayerInfoList.Add(gamePlayerInfo);
        return gamePlayerInfo;
    }

    /// <summary>
    /// 玩家退出房間
    /// </summary>
    /// <param name="id">退出玩家ID</param>
    /// <returns></returns>
    public GamePlayerInfo PlayerExitRoom(string id)
    {
        GamePlayerInfo exitPlayer = GetPlayer(id);
        seatList[exitPlayer.SeatIndex].image.enabled = true;
        gamePlayerInfoList.Remove(exitPlayer);
        Destroy(exitPlayer.gameObject);

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
        int betValue = playerActedPack.BetValue;
        int chips = playerActedPack.PlayerChips;
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
                    raise_Tr.gameObject.SetActive(false);
                    gameData.isFold = true;
                    gameData.IsPlaying = false;

                    //關閉所有撲克外框
                    List<Poker> pokers = communityPokerList.Concat(gameData.LocalGamePlayerInfo.GetHandPoker.ToList()).ToList();
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
                    player.ConcentrateBetChips(pot_Img.transform.position);
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
        for (int i = 1; i < seatList.Count; i++)
        {
            if (seatList[i].transform.childCount != 0)
            {
                Destroy(seatList[i].transform.GetChild(0).gameObject);
                seatList[i].image.enabled = true;
            }
        }

        //本地玩家座位
        gameData.LocalPlayerSeat = pack.PlayerInfoPackList.Where(x => x.UserID == Entry.TestInfoData.LocalUserId)
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
            communityPokerList[i].gameObject.SetActive(true);
            communityPokerList[i].PokerNum = currCommunityPoker[i];
        }
    }

    /// <summary>
    /// 設置Button座位物件
    /// </summary>
    /// <param name="id"></param>
    public void SetButtonSeat(string id)
    {
        GamePlayerInfo buttonPlayer = gamePlayerInfoList.Where(x => x.UserId == id).FirstOrDefault();

        buttonSeat_Tr.gameObject.SetActive(true);
        buttonSeat_Tr.position = buttonPlayer.gameObject.transform.position;

        ObjMoveUtils.ObjMoveTowardsTarget(buttonSeat_Tr, pot_Img.transform.position, 0.5f, 160);
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
                              gameData.SmallBlindValue,
                              blindStagePack.SBPlayerChips);

        //大盲
        GamePlayerInfo bbPlayer = GetPlayer(blindStagePack.BBPlayerId);
        bbPlayer.PlayerAction(ActingEnum.Blind,
                              gameData.SmallBlindValue * 2,
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
        gameData.CurrCommunityPoker = currCommunityPoker;

        yield return IConcentrateBetChips();

        for (int i = 0; i < currCommunityPoker.Count; i++)
        {
            if (communityPokerList[i].gameObject.activeSelf == false)
            {
                communityPokerList[i].gameObject.SetActive(true);
                communityPokerList[i].PokerNum = currCommunityPoker[i];
                StartCoroutine(communityPokerList[i].IHorizontalFlopEffect(currCommunityPoker[i]));
                yield return new WaitForSeconds(0.1f);
            }
        }

        yield return new WaitForSeconds(0.6f);

        if (gameData.IsPlaying == true)
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
        judgePoker = judgePoker.Concat(gameData.CurrCommunityPoker).ToList();

        List<Poker> pokers = communityPokerList.Concat(handPoker.ToList()).ToList();

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
        List<Poker> allPokerList = communityPokerList.Concat(localHandPoker.ToList()).ToList();
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
        winType_Txt.text = "Pot";

        //贏家效果
        int count = 0; ;
        foreach (var winner in pack.WinnerPack.WinnerDic)
        {
            count++;

            GamePlayerInfo player = GetPlayer(winner.Key);
            JudgePokerShape(player, true, true);

            player.SetPotWinnerActive = true;
            player.SetWinnerEffect = true;
            Vector2 winnerSeatPos = player.gameObject.transform.position;

            //產生贏得籌碼物件
            RectTransform rt = UIManager.Instance.CreatePrefab(GameAssetsManager.Instance.WinChipsObj);
            rt.anchoredPosition = pot_Img.rectTransform.anchoredPosition;
            WinChips winChips = rt.GetComponent<WinChips>();
            winChips.SetWinChips = pack.WinnerPack.WinChips;
            SetPotActive = false;

            yield return new WaitForSeconds(0.5f);

            ObjMoveUtils.ObjMoveToTarget(rt, winnerSeatPos, 0.5f,
                                        () =>
                                        {
                                            Destroy(rt.gameObject);
                                            player.Chips = winner.Value;
                                        });

            yield return new WaitForSeconds(2);

            //最後一位不關
            if (count < pack.WinnerPack.WinnerDic.Count())
            {
                //關閉撲克外框
                Poker[] handPoker = player.GetHandPoker;
                List<Poker> pokerList = communityPokerList.Concat(handPoker.ToList()).ToList();
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
        winType_Txt.text = "Side";
        totalPot_Txt.text = $"${pack.SidePack.TotalSideChips}";

        //邊池贏家效果
        foreach (var sideWinner in pack.SidePack.SideWinnerDic)
        {
            GamePlayerInfo player = GetPlayer(sideWinner.Key);
            player.SetSideWinnerActive = true;
            player.SetWinnerEffect = true;
            Vector2 winnerSeatPos = player.gameObject.transform.position;
            JudgePokerShape(player, true, true);

            //有贏得籌碼
            if (sideWinner.Value > 0)
            {
                RectTransform rt = UIManager.Instance.CreatePrefab(GameAssetsManager.Instance.WinChipsObj);
                rt.anchoredPosition = pot_Img.rectTransform.anchoredPosition;
                WinChips winChips = rt.GetComponent<WinChips>();
                winChips.SetWinChips = sideWinner.Value;

                yield return new WaitForSeconds(0.5f);

                ObjMoveUtils.ObjMoveToTarget(rt, winnerSeatPos, 0.5f,
                                            () =>
                                            {
                                                Destroy(rt.gameObject);
                                            });
            }

            yield return new WaitForSeconds(2);

            //關閉撲克外框
            Poker[] handPoker = player.GetHandPoker;
            List<Poker> pokerList = communityPokerList.Concat(handPoker.ToList()).ToList();
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
            GetPlayer(player.Key).Chips = player.Value;
        }
    }

    /// <summary>
    /// 遊戲階段
    /// </summary>
    /// <returns></returns>
    public IEnumerator IGameStage(MainPack pack)
    {
        gameData.SmallBlindValue = pack.GameStagePack.SmallBlind;
        gameData.CurrRaiseValue = gameData.SmallBlindValue * 2;

        //判斷當前遊戲進程
        switch (pack.GameStagePack.flowEnum)
        {
            //發牌
            case FlowEnum.Licensing:
                Init();
                tip_Txt.text = "";
                gameData.IsPlaying = true;

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
                if (gameData.isFold == true)
                {
                    foreach (var show in showPokerBtnList)
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
    /// 移除資料
    /// </summary>
    private void OnRemoveData()
    {
        gameData = null;
        StopAllCoroutines();
    }

    private void OnDisable()
    {
        OnRemoveData();
    }

    private void OnDestroy()
    {
        OnRemoveData();
    }
}
