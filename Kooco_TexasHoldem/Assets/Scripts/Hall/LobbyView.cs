using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

using RequestBuf;

public class LobbyView : MonoBehaviour
{
    [SerializeField]
    Request_LobbyView baseRequest;

    [Header("用戶訊息")]
    [SerializeField]
    Text walletId_Txt;

    [Header("房間")]
    [SerializeField]
    RectTransform cashRoomParent;
    [SerializeField]
    Button battle_Btn;

    [Header("等待計時")]
    [SerializeField]
    RectTransform waitingTime_Tr;
    [SerializeField]
    Text waitingTimg_Txt;
    [SerializeField]
    Button cancelWait_Btn;

    DateTime startPairTime; //開始配對時間

    private void Awake()
    {
        waitingTime_Tr.gameObject.SetActive(false);

        ListenerEvent();
    }

    /// <summary>
    /// 事件聆聽
    /// </summary>
    private void ListenerEvent()
    {
        //積分房
        battle_Btn.onClick.AddListener(() =>
        {
            GameDataManager.CurrRoomType = RoomEnum.BattleRoom;

            //開啟計時器
            waitingTime_Tr.gameObject.SetActive(true);
            waitingTime_Tr.anchoredPosition = new Vector2(0, waitingTime_Tr.rect.height);
            startPairTime = DateTime.Now;
        });

        //取消等待配對
        cancelWait_Btn.onClick.AddListener(() =>
        {
            waitingTime_Tr.gameObject.SetActive(false);
        });
    }

    private void Start()
    {
        SetUserInfo();
        CreateRoom();
    }

    private void Update()
    {
        //等待計時器
        if (waitingTime_Tr.gameObject.activeSelf)
        {
            if (waitingTime_Tr.anchoredPosition.y > 0)
            {
                waitingTime_Tr.Translate(new Vector3(0, -100 * Time.deltaTime, 0), Space.Self);
            }

            TimeSpan waitingTime = DateTime.Now - startPairTime;
            waitingTimg_Txt.text = $"{(int)waitingTime.TotalMinutes} : {waitingTime.Seconds:00}";

            if (waitingTime.Seconds >= 3)
            {
                baseRequest.SendRequest_InBattleRoom();
                waitingTime_Tr.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 設置用戶訊息
    /// </summary>
    private void SetUserInfo()
    {
        walletId_Txt.text = $"WalletID:{GameDataManager.UserWalletId}";
    }

    /// <summary>
    /// 創建房間
    /// </summary>
    private void CreateRoom()
    {
        //現金桌        
        GameObject cashRoomBtnObj = GameAssetsManager.Instance.CashRoomBtnObj;
        float cashRoomSpacing = cashRoomParent.GetComponent<HorizontalLayoutGroup>().spacing;
        Rect cashRoomRect = cashRoomBtnObj.GetComponent<RectTransform>().rect;
        cashRoomParent.sizeDelta = new Vector2((cashRoomRect.width + cashRoomSpacing) * GameDataManager.CashRoomSmallBlindList.Count, cashRoomRect.height);
        foreach (var smallBlind in GameDataManager.CashRoomSmallBlindList)
        {
            RectTransform rt = Instantiate(cashRoomBtnObj).GetComponent<RectTransform>();
            rt.SetParent(cashRoomParent);
            rt.GetComponent<CashRoomBtn>().SetCashRoomBtnInfo(smallBlind);
            rt.localScale = Vector3.one;
        }
        cashRoomParent.anchoredPosition = Vector2.zero;
    }
}
