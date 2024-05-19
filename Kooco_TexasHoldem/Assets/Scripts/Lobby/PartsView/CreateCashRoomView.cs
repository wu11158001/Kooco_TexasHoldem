using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using RequestBuf;

public class CreateCashRoomView : MonoBehaviour
{
    [SerializeField]
    Request_CreateCashRoom baseRequest;

    [SerializeField]
    Text title_Txt, preBuyChips_Txt, minBuyChips_Txt, maxBuyChips_Txt, roomCount_Txt;
    [SerializeField]
    Slider buyChips_Sli;
    [SerializeField]
    Button cancel_Btn, create_Btn, buyPlus_Btn, buyMinus_Btn, roomCountPlus_Btn, roomCountMinus_Btn;

    private ThisData thisData;
    public class ThisData
    {
        public double SmallBlind;   //小盲值
        public int CreateCount;     //創建房間數量
    }

    /// <summary>
    /// 設置創建房間數量
    /// </summary>
    private int CreateRoomCount
    {
        get
        {
            return thisData.CreateCount;
        }
        set
        {
            if (value > 0 && value <= GameRoomManager.Instance.maxRoomCount)
            {
                roomCount_Txt.text = value.ToString();
                thisData.CreateCount = value;
            }            
        }
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
        //取消
        cancel_Btn.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
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

        //創建按鈕
        create_Btn.onClick.AddListener(() =>
        {
            baseRequest.SendRequest_CreateCashRoom(thisData.SmallBlind, buyChips_Sli.value, thisData.CreateCount);
        });

        //購買+按鈕
        buyPlus_Btn.onClick.AddListener(() =>
        {
            buyChips_Sli.value += (float)thisData.SmallBlind * 2;
        });

        //購買-按鈕
        buyMinus_Btn.onClick.AddListener(() =>
        {
            buyChips_Sli.value -= (float)thisData.SmallBlind * 2;
        });

        //房間數量+按鈕
        roomCountPlus_Btn.onClick.AddListener(() =>
        {
            CreateRoomCount++;
        });

        //房間數量-按鈕
        roomCountMinus_Btn.onClick.AddListener(() =>
        {
            CreateRoomCount--;
        });
    }

    /// <summary>
    /// 設定創建房間介面
    /// </summary>
    /// <param name="smallBlind"></param>
    /// <param name="buyChipsCallback">購買籌碼回傳</param>
    public void SetCreatRoomViewInfo(double smallBlind, UnityAction<MainPack> buyChipsCallback)
    {
        thisData.SmallBlind = smallBlind;
        CreateRoomCount = 1;

        title_Txt.text = $"{thisData.SmallBlind} / {thisData.SmallBlind * 2} Texas Holdem";
        TexasHoldemUtil.SetBuySlider(thisData.SmallBlind, buyChips_Sli);
        minBuyChips_Txt.text = $"{StringUtils.SetChipsUnit(thisData.SmallBlind * GameDataManager.MinMagnification)}";
        maxBuyChips_Txt.text = $"{StringUtils.SetChipsUnit(Entry.TestInfoData.LocalUserChips)}";
    }
}