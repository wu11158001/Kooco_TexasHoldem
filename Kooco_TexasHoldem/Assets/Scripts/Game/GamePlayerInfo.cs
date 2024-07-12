using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using TMPro;

using RequestBuf;

public class GamePlayerInfo : MonoBehaviour
{
    [Header("用戶訊息")]
    [SerializeField]
    TextMeshProUGUI Nickname_Txt, Chips_Txt, BackChips_Txt, PokerShape_Txt, BlindCharacter_Txt;
    [SerializeField]
    GameObject ActionFrame_Obj, Winner_Obj, InfoMask_Obj;
    [SerializeField]
    Image CDMask_Img, Avatar_Img, ButtonCharacter_Img;

    [Header("手牌")]
    [SerializeField]
    RectTransform HandPoker_Tr;
    [SerializeField]
    Poker[] HandPokers;

    [Header("下注籌碼")]
    [SerializeField]
    RectTransform BetChips_Tr;

    [Header("行動")]
    [SerializeField]
    Image Action_Img;
    [SerializeField]
    TextMeshProUGUI Action_Txt;
    [SerializeField]
    Color foldColor, callColor, checkColor, raiseColor, allInColor, blindColor;

    [Header("條天訊息")]
    [SerializeField]
    GameObject Chat_Obj;
    [SerializeField]
    TextMeshProUGUI Chat_Txt;

    Coroutine cdCoroutine;              //倒數協程
    Coroutine chatCoroutine;            //聊天協程

    int pokerShapeIndex;                //牌型編號

    Vector2 betChipsr_TrInitPos;         //下注籌碼物件初始位置

    /// <summary>
    /// 是否有在進行遊戲
    /// </summary>
    public bool IsPlaying { get; set; }

    /// <summary>
    /// 是否已棄牌
    /// </summary>
    public bool IsFold { get; set; }

    /// <summary>
    /// 是否已All In
    /// </summary>
    public bool IsAllIn { get; set; }

    /// <summary>
    /// 當前下注行為
    /// </summary>
    public BetActionEnum CurrBetAction { get; set; }

    /// <summary>
    /// 座位編號
    /// </summary>
    public int SeatIndex { get; set; }

    /// <summary>
    /// 用戶ID
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// 暱稱
    /// </summary>
    public string Nickname { get; set; }

    /// <summary>
    /// 頭像
    /// </summary>
    public int Avatar { get; set; }

    /// <summary>
    /// 座位上的腳色
    /// </summary>
    public SeatCharacterEnum SeatCharacter { get; set; }

    /// <summary>
    /// 當前擁有籌碼
    /// </summary>
    public double CurrRoomChips { get; set; }

    /// <summary>
    /// 當前下注值
    /// </summary>
    public double CurrBetValue { get; set; }

    /// <summary>
    /// 更新文本翻譯
    /// </summary>
    private void UpdateLanguage()
    {
        SetPokerShapeTxtStr = LanguageManager.Instance.GetText(PokerShape.shapeStr[pokerShapeIndex]);
    }

    private void Awake()
    {
        LanguageManager.Instance.AddUpdateLanguageFunc(UpdateLanguage);
        StopCountDown();
        Chat_Obj.SetActive(false);
        IsOpenInfoMask = true;

        Init();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        IsFold = false;
        IsAllIn = false;
        CurrBetAction = BetActionEnum.None;
        CurrBetValue = 0;
        ButtonCharacter_Img.gameObject.SetActive(false);
        BlindCharacter_Txt.text = "";
        Action_Img.gameObject.SetActive(false);
        betChipsr_TrInitPos = BetChips_Tr.anchoredPosition;
        BetChips_Tr.gameObject.SetActive(false);
        ActionFrame = false;
        HandPokers[0].PokerNum = -1;
        HandPokers[0].gameObject.SetActive(false);
        HandPokers[1].PokerNum = -1;
        HandPokers[1].gameObject.SetActive(false);        
        Winner_Obj.SetActive(false);
        SetTip = "";
        SetPokerShapeTxtStr = "";
    }

    /// <summary>
    /// 設置牌型文字元件文字
    /// </summary>
    public string SetPokerShapeTxtStr
    {
        set
        {
            PokerShape_Txt.text = value;
        }
    }

    /// <summary>
    /// 玩家房間籌碼值
    /// </summary>
    public double PlayerRoomChips
    {
        get
        {
            return CurrRoomChips;
        }
        set
        {
            CurrRoomChips = value;
            StringUtils.ChipsChangeEffect(Chips_Txt, CurrRoomChips);
        }
    }

    /// <summary>
    /// 設置提示
    /// </summary>
    public string SetTip
    {
        set
        {
            BackChips_Txt.text = value;
        }
    }

    /// <summary>
    /// 行動框
    /// </summary>
    public bool ActionFrame
    {
        set
        {
            ActionFrame_Obj.SetActive(value);
        }
    }

    /// <summary>
    /// 設定贏家顯示
    /// </summary>
    public bool IsWinnerActive
    {
        set
        {
            Winner_Obj.SetActive(value);
        }
    }

    /// <summary>
    /// 設定本地玩家手牌位置
    /// </summary>
    public RectTransform SetLocalHandPokerPosition
    {
        set
        {
            HandPoker_Tr.SetParent(value);
            HandPoker_Tr.anchoredPosition = Vector2.zero;
            HandPoker_Tr.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        }
    }

    /// <summary>
    /// 獲取手牌
    /// </summary>
    public Poker[] GetHandPoker
    {
        get
        {
            return HandPokers;
        }
    }

    /// <summary>
    /// 獲取下注物件激活狀態
    /// </summary>
    public bool GetBetChipsActive
    {
        get
        {
            return BetChips_Tr.gameObject.activeSelf;
        }
    }

    /// <summary>
    /// 開訊息遮罩
    /// </summary>
    public bool IsOpenInfoMask
    {
        set
        {
            InfoMask_Obj.SetActive(value);
        }
    }

    /// <summary>
    /// 設置擁有籌碼文字
    /// </summary>
    public double SetChipsTxt
    {
        set
        {
            StringUtils.ChipsChangeEffect(Chips_Txt,
                                          value);
        }
    }

    /// <summary>
    /// 設定初始玩家訊息
    /// </summary>
    /// <param name="seatIndex">座位編號</param>
    /// <param name="userId">userId</param>
    /// <param name="nickName">暱稱</param>
    /// <param name="initChips">初始籌碼</param>
    /// <param name="avatar">頭像</param>
    public void SetInitPlayerInfo(int seatIndex, string userId, string nickName, double initChips, int avatar)
    {
        Init();

        UserId = userId;
        Nickname = nickName;
        Avatar = avatar;
        SeatIndex = seatIndex;

        CurrRoomChips = initChips;
        Avatar_Img.sprite = AssetsManager.Instance.GetAlbumAsset(AlbumEnum.AvatarAlbum).album[avatar];
        Nickname_Txt.text = $"@{nickName}";
        Chips_Txt.text = $"{StringUtils.SetChipsUnit(initChips)}";
    }

    /// <summary>
    /// 顯示聊天訊息
    /// </summary>
    /// <param name="chatContent"></param>
    public void ShowChatInfo(string chatContent)
    {
        Chat_Obj.SetActive(true);
        StringUtils.StrExceedSize(chatContent, Chat_Txt);

        if (chatCoroutine != null)
        {
            StopCoroutine(chatCoroutine);
        }
        chatCoroutine = StartCoroutine(IYieldCloseChatInfo());
    }

    /// <summary>
    /// 關閉聊訊息
    /// </summary>
    /// <returns></returns>
    private IEnumerator IYieldCloseChatInfo()
    {
        yield return new WaitForSeconds(3);
        Chat_Obj.SetActive(false);
    }

    /// <summary>
    /// 設定手牌
    /// </summary>
    /// <param name="handPoker0"></param>
    /// <param name="handPoker1"></param>
    public void SetHandPoker(int hand0, int hand1)
    {
        Init();
        StopCountDown();

        IsPlaying = true;

        foreach (var poker in HandPokers)
        {
            poker.gameObject.SetActive(true);
            poker.SetColor = 1;
        }

        HandPokers[0].PokerNum = hand0;
        HandPokers[1].PokerNum = hand1;

        IsOpenInfoMask = false;
    }

    /// <summary>
    /// 設置座位的腳色
    /// </summary>
    /// <param name="seatCharacter"></param>
    public void SetSeatCharacter(SeatCharacterEnum seatCharacter)
    {
        SeatCharacter = seatCharacter;

        switch (seatCharacter)
        {
            case SeatCharacterEnum.None:
                ButtonCharacter_Img.gameObject.SetActive(false);
                BlindCharacter_Txt.text = "";
                break;

            case SeatCharacterEnum.Button:
                ButtonCharacter_Img.gameObject.SetActive(true);
                break;

            case SeatCharacterEnum.SB:
                BlindCharacter_Txt.text = "SB";
                break;

            case SeatCharacterEnum.BB:
                BlindCharacter_Txt.text = "BB";
                break;
        }
    }

    /// <summary>
    /// 停止倒數
    /// </summary>
    public void StopCountDown()
    {
        if(cdCoroutine != null) StopCoroutine(cdCoroutine);
        CDMask_Img.fillAmount = 0;
    }

    /// <summary>
    /// 開始倒數
    /// </summary>
    /// <param name="cdTime">倒數總時間</param>
    /// <param name="cd">倒數</param>
    public void StartCountDown(int cdTime, int cd)
    {
        cdCoroutine = StartCoroutine(ICountDown(cdTime, cd));
    }

    /// <summary>
    /// 倒數效果
    /// </summary>
    /// <param name="cdTime">倒數總時間</param>
    /// <param name="cd">倒數</param>
    private IEnumerator ICountDown(int cdTime, int cd)
    {
        float currFillAmount = CDMask_Img.fillAmount;
        float target = ((float)cdTime - (cd - 1)) / (float)cdTime;

        DateTime startTime = DateTime.Now;
        while ((DateTime.Now - startTime).TotalSeconds < 1)
        {
            float process = Mathf.Clamp01((float)(DateTime.Now - startTime).TotalSeconds / 1);
            float value = Mathf.Lerp(currFillAmount, target, process);

            CDMask_Img.fillAmount = value;
            yield return null;
        }
    }

    /// <summary>
    /// 每輪回合開始初始
    /// </summary>
    public void RountInit()
    {
        CurrBetValue = 0;
    }

    /// <summary>
    /// 玩家下注
    /// </summary>
    /// <param name="betValue">下注值</param>
    /// <param name="chips">玩家籌碼</param>
    public void PlayerBet(double betValue, double chips)
    {
        BetChips_Tr.gameObject.SetActive(true);
        PlayerRoomChips = chips;
        CurrBetValue = betValue;
    }

    /// <summary>
    /// 下注籌碼集中效果
    /// </summary>
    /// <param name="potPointPos">底池位置</param>
    public void ConcentrateBetChips(Vector2 potPointPos)
    {
        float during = 0.5f;//效果時間

        ObjMoveUtils.ObjMoveToTarget(BetChips_Tr, potPointPos, during,
                                    () =>
                                    {
                                        if(BetChips_Tr != null)
                                        {
                                            BetChips_Tr.anchoredPosition = betChipsr_TrInitPos;
                                            BetChips_Tr.gameObject.SetActive(false);
                                        }                                        
                                    });
    }

    /// <summary>
    /// 是否顯示行動文字
    /// </summary>
    /// <param name="isShow"></param>
    /// <param name="betValue">下注值</param>
    /// <param name="actionStr">行動文字</param>
    public void SetShowActionStr(bool isShow, double betValue = 0, BetActionEnum betActionEnum = BetActionEnum.None)
    {
        Action_Img.gameObject.SetActive(isShow);

        switch (betActionEnum)
        {
            case BetActionEnum.Fold:
                Action_Img.color = foldColor;
                break;

            case BetActionEnum.Check:
                Action_Img.color = checkColor;
                break;

            case BetActionEnum.Raise:
                Action_Img.color = raiseColor;
                break;

            case BetActionEnum.Call:
                Action_Img.color = callColor;
                break;

            case BetActionEnum.AllIn:
                Action_Img.color = allInColor;
                break;

            case BetActionEnum.Blind:
                Action_Img.color = blindColor;
                break;
        }

        if (betActionEnum == BetActionEnum.Blind ||
            betActionEnum == BetActionEnum.Call ||
            betActionEnum == BetActionEnum.Raise ||
            betActionEnum == BetActionEnum.AllIn)
        {
            StringUtils.ChipsChangeEffect(Action_Txt,
                                          betValue,
                                          $"{betActionEnum}\n");
        }
        else
        {
            Action_Txt.text = betActionEnum != BetActionEnum.None ?
                              betActionEnum.ToString() :
                              "";
        }

        float txtWidth = Action_Txt.preferredHeight;
        Action_Img.rectTransform.sizeDelta = new Vector2(Action_Img.rectTransform.rect.width,
                                                         txtWidth + 5);
    }

    /// <summary>
    /// 玩家行動
    /// </summary>
    /// <param name="actionEnum">行動</param>
    /// <param name="betValue">下注值</param>
    /// <param name="chips">玩家籌碼</param>
    public void PlayerAction(ActingEnum actionEnum, double betValue, double chips)
    {
        if (!gameObject.activeSelf) return;

        StopCountDown();
        ActionFrame = false;

        CurrBetAction = (BetActionEnum)Convert.ToInt32(actionEnum);

        SetShowActionStr(true,
                         betValue,
                         (BetActionEnum)(int)actionEnum);

        switch (actionEnum)
        {
            //大小盲
            case ActingEnum.Blind:
                PlayerBet(betValue, chips);
                break;

            //棄牌
            case ActingEnum.Fold:
                IsFold = true;
                foreach (var poker in HandPokers)
                {
                    poker.SetColor = 0.5f;
                }

                IsOpenInfoMask = true;
                break;

            //過牌
            case ActingEnum.Check:
                break;

            //加注
            case ActingEnum.Raise:
                PlayerBet(betValue, chips);
                break;

            //跟注
            case ActingEnum.Call:
                PlayerBet(betValue, chips);
                break;

            //All In
            case ActingEnum.AllIn:
                IsAllIn = true;
                PlayerBet(betValue, chips);
                break;
        }
    }

    /// <summary>
    /// 設置牌型文字
    /// </summary>
    /// <param name="shapeIndex"></param>
    public void SetPokerShapeStr(int shapeIndex)
    {
        pokerShapeIndex = shapeIndex;
        SetPokerShapeTxtStr = LanguageManager.Instance.GetText(PokerShape.shapeStr[pokerShapeIndex]);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnDestroy()
    {
        LanguageManager.Instance.RemoveLanguageFun(UpdateLanguage);
        StopAllCoroutines();
    }
}
