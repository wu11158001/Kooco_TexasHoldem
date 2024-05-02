using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

using RequestBuf;

public class GamePlayerInfo : MonoBehaviour
{
    [Header("頭像")]
    [SerializeField]
    Image avatar_Img;

    [Header("用戶訊息")]
    [SerializeField]
    Text nickName_Txt, chips_Txt, tip_Txt, pokerShape_Txt;
    [SerializeField]
    GameObject actionFrame_Obj, potWinner_Obj, sideWinner_Obj;
    [SerializeField]
    Image cdMask_Img;

    [Header("手牌")]
    [SerializeField]
    Poker[] handPoker;

    [Header("下注籌碼")]
    [SerializeField]
    RectTransform betChips_Tr;
    [SerializeField]
    Text betChips_Txt;

    [Header("行動")]
    [SerializeField]
    GameObject action_Obj;
    [SerializeField]
    Text action_Txt;

    [Header("動畫")]
    [SerializeField]
    Animator pokerShape_Ani;

    int isWinHash = Animator.StringToHash("IsWin");

    Coroutine cdCoroutine;      //倒數協程

    int currChips;              //當前擁有籌碼
    Vector2 potPointPos;        //底池位置

    /// <summary>
    /// 用戶ID
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// 當前下注值
    /// </summary>
    public int CurrBetValue { get; set; }

    /// <summary>
    /// 籌碼值
    /// </summary>
    public int Chips 
    {
        get
        {
            return currChips;
        }
        set
        {
            currChips = value;
            StringUtils.ChipsChangeEffect(chips_Txt, currChips.ToString(), "$");
        }
    }

    /// <summary>
    /// 設置提示
    /// </summary>
    public string SetTip
    {
        set
        {
            tip_Txt.text = value;
        }
    }

    /// <summary>
    /// 行動框
    /// </summary>
    public bool ActionFrame
    {
        set
        {
            actionFrame_Obj.SetActive(value);
        }
    }

    /// <summary>
    /// 設定底池贏家顯示
    /// </summary>
    public bool SetPotWinnerActive
    {
        set
        {
            potWinner_Obj.SetActive(value);           
        }
    }

    /// <summary>
    /// 設定邊池贏家顯示
    /// </summary>
    public bool SetSideWinnerActive
    {
        set
        {
            sideWinner_Obj.SetActive(value);
        }
    }

    /// <summary>
    /// 設定贏家效果
    /// </summary>
    public bool SetWinnerEffect
    {
        set
        {
            pokerShape_Ani.SetBool(isWinHash, value);
        }
    }

    /// <summary>
    /// 獲取手牌
    /// </summary>
    public Poker[] GetHandPoker
    {
        get
        {
            return handPoker;
        }
    }

    /// <summary>
    /// 獲取下注物件激活狀態
    /// </summary>
    public bool GetBetChipsActive
    {
        get
        {
            return betChips_Tr.gameObject.activeSelf;
        }
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        CurrBetValue = 0;
        betChips_Txt.text = CurrBetValue.ToString();
        action_Obj.SetActive(false);
        betChips_Tr.gameObject.SetActive(false);
        ActionFrame = false;
        handPoker[0].gameObject.SetActive(false);
        handPoker[1].gameObject.SetActive(false);
        potWinner_Obj.SetActive(false);
        sideWinner_Obj.SetActive(false);
        SetTip = "";
        pokerShape_Txt.text = "";
        pokerShape_Ani.SetBool(isWinHash, false);
        StopCountDown();
    }

    /// <summary>
    /// 設定初始玩家訊息
    /// </summary>
    /// <param name="userId">userId</param>
    /// <param name="nickName">暱稱</param>
    /// <param name="initChips">初始籌碼</param>
    /// <param name="avatar">頭像</param>
    /// <param name="potPointPos">底池位置</param>
    public void SetInitPlayerInfo(string userId, string nickName, int initChips, Sprite avatar, Vector2 potPointPos)
    {
        currChips = initChips;

        UserId = userId;
        avatar_Img.sprite = avatar;
        nickName_Txt.text = nickName;
        chips_Txt.text = $"${StringUtils.SetChipsUnit(initChips)}";
        this.potPointPos = potPointPos;

        Init();
    }

    /// <summary>
    /// 設定手牌
    /// </summary>
    /// <param name="handPoker0"></param>
    /// <param name="handPoker1"></param>
    public void SetHandPoker(int hand0, int hand1)
    {
        Init();

        handPoker[0].gameObject.SetActive(true);
        handPoker[1].gameObject.SetActive(true);
        handPoker[0].PokerNum = hand0;
        handPoker[1].PokerNum = hand1;
    }

    /// <summary>
    /// 停止倒數
    /// </summary>
    public void StopCountDown()
    {
        if(cdCoroutine != null) StopCoroutine(cdCoroutine);
        cdMask_Img.fillAmount = 0;
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
        float currFillAmount = cdMask_Img.fillAmount;
        float target = ((float)cdTime - (cd - 1)) / (float)cdTime;

        DateTime startTime = DateTime.Now;
        while ((DateTime.Now - startTime).TotalSeconds < 1)
        {
            float process = Mathf.Clamp01((float)(DateTime.Now - startTime).TotalSeconds / 1);
            float value = Mathf.Lerp(currFillAmount, target, process);

            cdMask_Img.fillAmount = value;
            yield return null;
        }
    }

    /// <summary>
    /// 玩家下注
    /// </summary>
    /// <param name="betValue">下注值</param>
    /// <param name="chips">玩家籌碼</param>
    public void PlayerBet(int betValue, int chips)
    {
        //下注籌碼移動效果
        if (!betChips_Tr.gameObject.activeSelf)
        {
            betChips_Txt.text = "0";
            betChips_Tr.gameObject.SetActive(true);
            betChips_Tr.anchoredPosition = Vector2.zero;
            ObjMoveUtils.ObjMoveTowardsTarget(betChips_Tr, potPointPos, 0.5f, 240);
        }

        //下注籌碼文字效果
        StringUtils.ChipsChangeEffect(betChips_Txt, betValue.ToString());

        Chips = chips;
    }

    /// <summary>
    /// 下注籌碼集中效果
    /// </summary>
    /// <param name="potPointPos">底池位置</param>
    public void ConcentrateBetChips(Vector2 potPointPos)
    {
        float during = 0.5f;//效果時間

        ObjMoveUtils.ObjMoveToTarget(betChips_Tr, potPointPos, during,
                                    () =>
                                    {
                                        if(betChips_Tr != null)
                                        {
                                            betChips_Tr.gameObject.SetActive(false);
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
        action_Obj.SetActive(true);
        action_Txt.text = actionStr;

        yield return new WaitForSeconds(1.0f);

        action_Obj.SetActive(false);
    }

    /// <summary>
    /// 玩家行動
    /// </summary>
    /// <param name="actionEnum">行動</param>
    /// <param name="betValue">下注值</param>
    /// <param name="chips">玩家籌碼</param>
    public void PlayerAction(ActingEnum actionEnum, int betValue, int chips)
    {
        if (!gameObject.activeSelf) return;

        StopCountDown();

        CurrBetValue += int.Parse(StringUtils.StringAddition(CurrBetValue.ToString(), betValue.ToString()));

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
                handPoker[0].gameObject.SetActive(false);
                handPoker[1].gameObject.SetActive(false);
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
    /// <param name="matchPokerList"></param>
    /// <param name="isOpenMatchPokerFrame"></param>
    public void SetPokerShapeStr(int shapeIndex)
    {
        pokerShape_Txt.text = PokerShape.shapeStr[shapeIndex];
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
