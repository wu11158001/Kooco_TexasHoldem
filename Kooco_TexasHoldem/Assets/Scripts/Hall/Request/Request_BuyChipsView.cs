using RequestBuf;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Request_BuyChipsView : BaseRequest
{
    [SerializeField]
    BuyChipsPartsView thisView;

    public override void Awake()
    {
        requestDic = new List<ActionCode>()
        {
            ActionCode.Request_BuyChips,
        };

        roomBroadcastDic = new List<ActionCode>()
        {

        };

        base.Awake();
    }

    public override void HandleRequest(MainPack pack)
    {
        switch (pack.ActionCode)
        {
            //購買籌碼
            case ActionCode.Request_BuyChips:
                thisView.BuyedChips(pack);
                break;
        }
    }

    /// <summary>
    /// 發送進入現金房
    /// </summary>
    /// <param name="smallBlind">小盲值</param>
    /// <param name="carryChips">攜帶籌碼</param>
    public void SendRequest_InCashRoom(double smallBlind, double carryChips)
    {
        Entry.Instance.RoomSmallBlind = smallBlind;
        Entry.Instance.gameServer.SmallBlind = smallBlind;
        Entry.Instance.gameServer.gameObject.SetActive(true);

        MainPack pack = new MainPack();
        pack.ActionCode = ActionCode.Request_PlayerInOutRoom;

        PlayerInfoPack playerInfoPack = new PlayerInfoPack();
        playerInfoPack.UserID = Entry.TestInfoData.LocalUserId;
        playerInfoPack.NickName = Entry.TestInfoData.NickName;
        playerInfoPack.Chips = carryChips;

        PlayerInOutRoomPack playerInOutRoomPack = new PlayerInOutRoomPack();
        playerInOutRoomPack.IsInRoom = true;
        playerInOutRoomPack.PlayerInfoPack = playerInfoPack;

        pack.PlayerInOutRoomPack = playerInOutRoomPack;
        Entry.Instance.gameServer.Request_PlayerInOutRoom(pack);

        LoadSceneManager.Instance.LoadScene(SceneEnum.Game);
    }

    /// <summary>
    /// 發送離開房間
    /// </summary>
    public void SendRequest_ExitRoom()
    {
        MainPack pack = new MainPack();
        pack.ActionCode = ActionCode.Request_PlayerInOutRoom;

        PlayerInfoPack playerInfoPack = new PlayerInfoPack(); ;
        playerInfoPack.UserID = Entry.TestInfoData.LocalUserId;

        PlayerInOutRoomPack playerInOutRoomPack = new PlayerInOutRoomPack();
        playerInOutRoomPack.IsInRoom = false;
        playerInOutRoomPack.PlayerInfoPack = playerInfoPack;

        pack.PlayerInOutRoomPack = playerInOutRoomPack;
        SendRequest(pack);
    }

    /// <summary>
    /// 發送購買籌碼
    /// </summary>
    /// <param name="id">ID</param>
    /// <param name="buyChipsValue">購買籌碼數量</param>
    public void SendRequest_BuyChips(string id, double buyChipsValue)
    {
        MainPack pack = new MainPack();
        pack.ActionCode = ActionCode.Request_BuyChips;

        BuyChipsPack buyChipsPack = new BuyChipsPack();
        buyChipsPack.UserId = id;
        buyChipsPack.BuyChipsValue = buyChipsValue;

        pack.BuyChipsPack = buyChipsPack;
        SendRequest(pack);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }
}
