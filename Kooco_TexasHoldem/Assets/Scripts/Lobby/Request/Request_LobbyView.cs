using RequestBuf;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Request_LobbyView : BaseRequest
{
    [SerializeField]
    LobbyView thisView;

    public override void Awake()
    {
        requestDic = new List<ActionCode>()
        {
           
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
    /// 發送進入積分房
    /// </summary>
    public void SendRequest_InBattleRoom()
    {
        MainPack pack = new MainPack();
        pack.ActionCode = ActionCode.Request_PlayerInOutRoom;

        PlayerInfoPack playerInfoPack = new PlayerInfoPack();
        playerInfoPack.UserID = Entry.TestInfoData.LocalUserId;
        playerInfoPack.NickName = Entry.TestInfoData.NickName;
        playerInfoPack.Chips = 10000;

        PlayerInOutRoomPack playerInOutRoomPack = new PlayerInOutRoomPack();
        playerInOutRoomPack.IsInRoom = true;
        playerInOutRoomPack.PlayerInfoPack = playerInfoPack;

        pack.PlayerInOutRoomPack = playerInOutRoomPack;

        GameRoomManager.Instance.CerateGameRoom(pack, GameRoomTypeEnum.BattleRoomType, 500);
        //LoadSceneManager.Instance.LoadScene(SceneEnum.Game);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }
}
