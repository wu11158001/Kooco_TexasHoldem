using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using RequestBuf;

public class BuyChipsView : MonoBehaviour
{
    [SerializeField]
    Text title_Txt, preBuyChips_Txt, minBuyChips_Txt, maxBuyChips_Txt, returnBtn_Txt, buyBtn_Txt;
    [SerializeField]
    Slider buyChips_Sli;
    [SerializeField]
    Button return_Btn, buy_Btn, plus_Btn, minus_Btn;

    private ThisData thisData;
    public class ThisData
    {
        public string RoomName;                               //掛載的房間名
        public double SmallBlind;                             //小盲值
        public UnityAction<double> SendBuyChipsCallback;      //發送購買籌碼回傳
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
            GameRoomManager.Instance.RemoveGameRoom(thisData.RoomName);
        });

        //購買Slider單位設定
        buyChips_Sli.onValueChanged.AddListener((value) =>
        {
            float newRaiseValue = TexasHoldemUtil.SliderValueChange(buyChips_Sli,
                                                                    value,
                                                                    (float)thisData.SmallBlind * 2,
                                                                    buyChips_Sli.minValue,
                                                                    buyChips_Sli.maxValue);
            preBuyChips_Txt.text = StringUtils.SetChipsUnit(newRaiseValue);
        });

        //購買按鈕
        buy_Btn.onClick.AddListener(() =>
        {
            double buyChipsValue = buyChips_Sli.value;
            thisData.SendBuyChipsCallback(buyChipsValue);
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
    /// <param name="smallBlind"></param>
    /// <param name="roomName"></param>
    /// <param name="sendBuyCallback"></param>
    public void SetBuyChipsViewInfo(double smallBlind, string roomName, UnityAction<double> sendBuyCallback)
    {
        thisData.RoomName = roomName;
        thisData.SmallBlind = smallBlind;
        thisData.SendBuyChipsCallback = sendBuyCallback;

        returnBtn_Txt.text = LanguageManager.Instance.GetText("ReturnLobby");
        buyBtn_Txt.text = LanguageManager.Instance.GetText("Buy");
        title_Txt.text = $"{thisData.SmallBlind} / {thisData.SmallBlind * 2} {LanguageManager.Instance.GetText("TexasHoldem")}";
        TexasHoldemUtil.SetBuySlider(thisData.SmallBlind, buyChips_Sli);
        minBuyChips_Txt.text = $"{StringUtils.SetChipsUnit(thisData.SmallBlind * DataManager.MinMagnification)}";
        maxBuyChips_Txt.text = $"{StringUtils.SetChipsUnit(Entry.TestInfoData.LocalUserChips)}";
    }
}
