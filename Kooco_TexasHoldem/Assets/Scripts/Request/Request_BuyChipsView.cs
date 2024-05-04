using RequestBuf;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Request_BuyChipsView : BaseRequest
{
    [SerializeField]
    BuyChipsView thisView;

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
    public void SendRequest_BuyChips(string id, int buyChipsValue)
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
