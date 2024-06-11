using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

using RequestBuf;

public class GamePlayerInfo : MonoBehaviour
{
    [Header("用戶訊息")]
    [SerializeField]
    Text NickName_Txt, Chips_Txt, Tip_Txt, PokerShape_Txt, PotWinner_Txt, SideWinner_Txt, BlindCharacter_Txt;
    [SerializeField]
    GameObject ActionFrame_Obj, PotWinner_Obj, SideWinner_Obj;
    [SerializeField]
    Image CDMask_Img, Avatar_Img, ButtonCharacter_Img;

    [Header("手牌")]
    [SerializeField]
    RectTransform HandPoker_Tr;
    [SerializeField]
    Poker[] HandPokers;

    [Header("下注籌碼")]
    [SerializeField]
    GameObject CurrBetChips_Obj;
    [SerializeField]
    RectTransform BetChips_Tr;
    [SerializeField]
    Text CurrBrtChips_Txt, BetChips_Txt;

    [Header("行動")]
    [SerializeField]
    GameObject Action_Obj;
    [SerializeField]
    Text Action_Txt;

    [Header("動畫")]
    [SerializeField]
    Animator PokerShape_Ani;

    readonly int isWinHash = Animator.StringToHash("IsWin");

    Coroutine cdCoroutine;      //倒數協程

    double currRoomChips;       //當前擁有籌碼
    RectTransform potPoint;     //底池位置
    int pokerShapeIndex;        //牌型編號

    /// <summary>
    /// 座位編號
    /// </summary>
    public int SeatIndex { get; set; }

    /// <summary>
    /// 用戶ID
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// 當前下注值
    /// </summary>
    public double CurrBetValue { get; set; }

    /// <summary>
    /// 更新文本翻譯
    /// </summary>
    private void UpdateLanguage()
    {
        PotWinner_Txt.text = $"{LanguageManager.Instance.GetText("PotWinner")}";
        SideWinner_Txt.text = $"{LanguageManager.Instance.GetText("SideWinner")}";
        PokerShape_Txt.text = LanguageManager.Instance.GetText(PokerShape.shapeStr[pokerShapeIndex]);
    }

    private void Awake()
    {
        LanguageManager.Instance.AddUpdateLanguageFunc(UpdateLanguage);
        Chips_Txt.text = "0";
    }

    private void OnEnable()
    {
        StopCountDown();
        OpenInfoMask = true;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        CurrBetValue = 0;
        CurrBrtChips_Txt.text = StringUtils.SetChipsUnit(0);
        ButtonCharacter_Img.gameObject.SetActive(false);
        BlindCharacter_Txt.text = "";
        BetChips_Txt.text = CurrBetValue.ToString();
        Action_Obj.SetActive(false);
        BetChips_Tr.gameObject.SetActive(false);
        ActionFrame = false;
        HandPokers[0].gameObject.SetActive(false);
        HandPokers[1].gameObject.SetActive(false);
        PotWinner_Obj.SetActive(false);
        SideWinner_Obj.SetActive(false);
        SetTip = "";
        PokerShape_Txt.text = "";
        PokerShape_Ani.SetBool(isWinHash, false);
    }

    /// <summary>
    /// 玩家房間籌碼值
    /// </summary>
    public double PlayerRoomChips
    {
        get
        {
            return currRoomChips;
        }
        set
        {
            currRoomChips = value;
            StringUtils.ChipsChangeEffect(Chips_Txt, currRoomChips);
        }
    }

    /// <summary>
    /// 設置提示
    /// </summary>
    public string SetTip
    {
        set
        {
            Tip_Txt.text = value;
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
    /// 設定底池贏家顯示
    /// </summary>
    public bool SetPotWinnerActive
    {
        set
        {
            PotWinner_Obj.SetActive(value);
        }
    }

    /// <summary>
    /// 設定邊池贏家顯示
    /// </summary>
    public bool SetSideWinnerActive
    {
        set
        {
            SideWinner_Obj.SetActive(value);
        }
    }

    /// <summary>
    /// 設定贏家效果
    /// </summary>
    public bool SetWinnerEffect
    {
        set
        {
            PokerShape_Ani.SetBool(isWinHash, value);
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
    public bool OpenInfoMask
    {
        set
        {
            Avatar_Img.color = value == true ? new Color(1, 1, 1, 0.5f) : new Color(1, 1, 1, 1);
            CDMask_Img.fillAmount = value == true ? 1 : 0;
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
    /// <param name="potPoint">底池位置</param>
    public void SetInitPlayerInfo(int seatIndex, string userId, string nickName, double initChips, Sprite avatar, RectTransform potPoint)
    {
        Init();

        SeatIndex = seatIndex;
        currRoomChips = initChips;
        UserId = userId;
        Avatar_Img.sprite = avatar;
        NickName_Txt.text = nickName;
        Chips_Txt.text = $"{StringUtils.SetChipsUnit(initChips)}";
        this.potPoint = potPoint;
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

        foreach (var poker in HandPokers)
        {
            poker.gameObject.SetActive(true);
            poker.SetColor = 1;
        }

        HandPokers[0].PokerNum = hand0;
        HandPokers[1].PokerNum = hand1;
        OpenInfoMask = false;
    }

    /// <summary>
    /// 設置座位的腳色
    /// </summary>
    /// <param name="seatCharacter"></param>
    public void SetSeatCharacter(SeatCharacterEnum seatCharacter)
    {
        switch (seatCharacter)
        {
            case SeatCharacterEnum.Button:
                ButtonCharacter_Img.gameObject.SetActive(true);
                break;
            case SeatCharacterEnum.SB:
                BlindCharacter_Txt.text = "SB";
                break;
            case SeatCharacterEnum.BB:
                BlindCharacter_Txt.text = "BB";
                break;
            default:
                ButtonCharacter_Img.gameObject.SetActive(false);
                BlindCharacter_Txt.text = "";
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
    /// 玩家下注
    /// </summary>
    /// <param name="betValue">下注值</param>
    /// <param name="chips">玩家籌碼</param>
    public void PlayerBet(double betValue, double chips)
    {
        /*
        //下注籌碼移動效果
        if (!betChips_Tr.gameObject.activeSelf)
        {
            betChips_Txt.text = "0";
            betChips_Tr.gameObject.SetActive(true);
            betChips_Tr.anchoredPosition = Vector2.zero;
            ObjMoveUtils.ObjMoveTowardsTarget(betChips_Tr, potPoint.transform.position, 0.5f, 240);
        }

        //下注籌碼文字效果
        StringUtils.ChipsChangeEffect(betChips_Txt, betValue);
        */

        StringUtils.ChipsChangeEffect(CurrBrtChips_Txt, betValue);
        PlayerRoomChips = chips;
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
                                            BetChips_Tr.gameObject.SetActive(false);
                                        }                                        
                                    });
    }

    /// <summary>
    /// 顯示行動文字
    /// </summary>
    /// <param name="actionStr">行動文字</param>
    /// <returns></returns>
    private IEnumerator ShowActionStr(string actionStr)
    {
        Action_Obj.SetActive(true);
        Action_Txt.text = LanguageManager.Instance.GetText(actionStr);

        yield return new WaitForSeconds(1.0f);

        Action_Obj.SetActive(false);
    }

    /// <summary>
    /// 玩家行動
    /// </summary>
    /// <param name="actionEnum">行動</param>
    /// <param name="betValue">下注值</param>
    /// <param name="chips">玩家籌碼</param>
    /// <param name="isLocalPlayer">是否本地玩家</param>
    public void PlayerAction(ActingEnum actionEnum, double betValue, double chips, bool isLocalPlayer = false)
    {
        if (!gameObject.activeSelf) return;

        StopCountDown();

        CurrBetValue += CurrBetValue + betValue;
        ActionFrame = false;

        StartCoroutine(ShowActionStr(actionEnum.ToString()));

        switch (actionEnum)
        {
            //大小盲
            case ActingEnum.Blind:
                PlayerBet(betValue, chips);
                break;

            //棄牌
            case ActingEnum.Fold:
                if (isLocalPlayer)
                {
                    //本地玩家
                    foreach (var poker in HandPokers)
                    {
                        poker.SetColor = 0.5f;
                    }
                    PokerShape_Txt.text = "";
                }
                else
                {
                    //其他玩家
                    HandPokers[0].gameObject.SetActive(false);
                    HandPokers[1].gameObject.SetActive(false);
                }

                OpenInfoMask = true;
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
        PokerShape_Txt.text = LanguageManager.Instance.GetText(PokerShape.shapeStr[pokerShapeIndex]);
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
