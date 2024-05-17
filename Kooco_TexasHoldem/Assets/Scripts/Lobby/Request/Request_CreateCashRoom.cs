using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RequestBuf;

public class Request_CreateCashRoom : BaseRequest
{
    [SerializeField]
    CreateCashRoomView thisView;

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

    }

    /// <summary>
    /// 發送創建現金房
    /// </summary>
    /// <param name="smallBlind">小盲值</param>
    /// <param name="carryChips">攜帶籌碼</param>
    /// <param name="createCount">創建數量</param>
    public void SendRequest_CreateCashRoom(double smallBlind, double carryChips, int createCount)
    {
        StartCoroutine(ICreateRoom(smallBlind, carryChips, createCount));
    }

    /// <summary>
    /// 創建現金房
    /// </summary>
    /// <param name="smallBlind">小盲值</param>
    /// <param name="carryChips">攜帶籌碼</param>
    /// <param name="createCount">創建數量</param>
    /// <returns></returns>
    private IEnumerator ICreateRoom(double smallBlind, double carryChips, int createCount)
    {
        for (int i = 0; i < createCount; i++)
        {
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

            GameRoomManager.Instance.CerateGameRoom(pack, GameRoomEnum.CashRoomView, smallBlind);

            yield return new WaitForSeconds(0.1f);
        }

        gameObject.SetActive(false);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }
}
