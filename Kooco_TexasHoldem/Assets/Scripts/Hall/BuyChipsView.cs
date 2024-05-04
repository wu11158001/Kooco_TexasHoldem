using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using RequestBuf;

public class BuyChipsView : MonoBehaviour
{
    [SerializeField]
    Request_BuyChipsView baseRequest;

    [SerializeField]
    Text title_Txt, preBuyChips_Txt, minBuyChips_Txt, maxBuyChips_Txt;
    [SerializeField]
    Slider buyChips_Sli;
    [SerializeField]
    Button return_Btn, buy_Btn, plus_Btn, minus_Btn;

    private ThisData thisData;
    /// <summary>
    /// 購買資料
    /// </summary>
    public class ThisData
    {
        public double SmallBlind;                           //小盲值
        public UnityAction<MainPack> BuyChipsCallback;      //購買籌碼回傳
    }

    public void Awake()
    {
        ListenerEvent();
    }

    private void OnEnable()
    {
        thisData = new ThisData();
    }

    /// <summary>
    /// 事件聆聽
    /// </summary>
    private void ListenerEvent()
    {
        //返回
        return_Btn.onClick.AddListener(() =>
        {
            baseRequest.SendRequest_ExitRoom();
            UIManager.Instance.OpenView(ViewName.JoinRoomView);
            gameObject.SetActive(false);
        });

        //購買Slider單位設定
        buyChips_Sli.onValueChanged.AddListener((value) =>
        {
            float stepSize = (float)thisData.SmallBlind * 2;
            float newRaiseValue = Mathf.Round(value / stepSize) * stepSize;
            buyChips_Sli.value = newRaiseValue >= buyChips_Sli.maxValue ? buyChips_Sli.maxValue : newRaiseValue;
            preBuyChips_Txt.text = StringUtils.SetChipsUnit(newRaiseValue);
        });

        //購買按鈕
        buy_Btn.onClick.AddListener(() =>
        {
            string id = Entry.TestInfoData.LocalUserId;
            int buyChipsValue = (int)buyChips_Sli.value;

            baseRequest.SendRequest_BuyChips(id, buyChipsValue);
        });

        //+按鈕
        plus_Btn.onClick.AddListener(() =>
        {
            buyChips_Sli.value += (float)thisData.SmallBlind * 2;
        });

        //-按鈕
        minus_Btn.onClick.AddListener(() =>
        {
            buyChips_Sli.value -= (float)thisData.SmallBlind * 2;
        });
    }

    /// <summary>
    /// 設定購買介面
    /// </summary>
    /// <param name="pack"></param>
    /// <param name="buyChipsCallback">購買籌碼回傳</param>
    public void SetBuyChipsViewInfo(MainPack pack, UnityAction<MainPack> buyChipsCallback)
    {
        thisData.SmallBlind = pack.InsufficientChipsPack.SmallBlind;
        thisData.BuyChipsCallback = buyChipsCallback;

        title_Txt.text = $"{thisData.SmallBlind} / {thisData.SmallBlind * 2} Texas Holdem";
        TexasHoldemUtil.SetBuySlider(thisData.SmallBlind, buyChips_Sli);
        minBuyChips_Txt.text = $"{StringUtils.SetChipsUnit(thisData.SmallBlind * TexasHoldemUtil.MinMagnification)}";
        maxBuyChips_Txt.text = $"{StringUtils.SetChipsUnit(thisData.SmallBlind * TexasHoldemUtil.MinMagnification)}";
    }

    /// <summary>
    /// 購買籌碼
    /// </summary>
    /// <param name="pack"></param>
    public void BuyedChips(MainPack pack)
    {
        thisData.BuyChipsCallback(pack);
        gameObject.SetActive(false);
    }
}
