using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using RequestBuf;

public class LobbyView : MonoBehaviour
{
    [SerializeField]
    RectTransform cashRoomParent;



    readonly List<int> smallblindList = new List<int>
    {
        50,
        200,
        400,
    };

    [SerializeField]
    Dropdown blind_Dr;
    [SerializeField]
    Slider carryChips_Sli;
    [SerializeField]
    Text carryChips_Txt, walletId_Txt;
    [SerializeField]
    Button start_Btn;

    private void Awake()
    {
        blind_Dr.onValueChanged.AddListener((value) =>
        {
            TexasHoldemUtil.SetBuySlider(smallblindList[value], carryChips_Sli);
        });

        carryChips_Sli.onValueChanged.AddListener((value) =>
        {
            float stepSize = smallblindList[blind_Dr.value] * 2;
            float newRaiseValue = Mathf.Round(value / stepSize) * stepSize;
            carryChips_Sli.value = newRaiseValue >= carryChips_Sli.maxValue ? carryChips_Sli.maxValue : newRaiseValue;
            carryChips_Txt.text = StringUtils.SetChipsUnit(newRaiseValue);
        });

        start_Btn.onClick.AddListener(() =>
        {
            Entry.Instance.RoomSmallBlind = smallblindList[blind_Dr.value];
            Entry.Instance.gameServer.SmallBlind = smallblindList[blind_Dr.value];
            Entry.Instance.gameServer.gameObject.SetActive(true);            
            
            MainPack pack = new MainPack();
            pack.ActionCode = ActionCode.Request_PlayerInOutRoom;

            PlayerInfoPack playerInfoPack = new PlayerInfoPack();
            playerInfoPack.UserID = Entry.TestInfoData.LocalUserId;
            playerInfoPack.NickName = Entry.TestInfoData.NickName;
            playerInfoPack.Chips = (int)carryChips_Sli.value;

            PlayerInOutRoomPack playerInOutRoomPack = new PlayerInOutRoomPack();
            playerInOutRoomPack.IsInRoom = true;
            playerInOutRoomPack.PlayerInfoPack = playerInfoPack;

            pack.PlayerInOutRoomPack = playerInOutRoomPack;
            Entry.Instance.gameServer.Request_PlayerInOutRoom(pack);

            UIManager.Instance.OpenView(ViewEnum.GameView);
        });
    }

    private void Start()
    {
        SetUserInfo();
        CreateRoom();


        blind_Dr.value = 1;
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
        }
        cashRoomParent.anchoredPosition = Vector2.zero;
    }
}
