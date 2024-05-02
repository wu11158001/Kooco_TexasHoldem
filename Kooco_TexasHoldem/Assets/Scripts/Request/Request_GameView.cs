using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RequestBuf;

public class Request_GameView : BaseViewRequest
{
    GameView thisView;

    bool isStartReceiveRequest;     //是否開始接收協議

    public override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        isStartReceiveRequest = false;
    }

    public override void SetThisView()
    {
        thisView = thisBaseView as GameView;
    }

    public override void RegisterBroadcast()
    {
        RequestManager.Instance.RegisterBroadcast(ActionCode.Request_PlayerInOutRoom, HandleRoomBroadcast);
        RequestManager.Instance.RegisterBroadcast(ActionCode.BroadCastRequest_GameStage, HandleRoomBroadcast);
        RequestManager.Instance.RegisterBroadcast(ActionCode.BroadCastRequest_PlayerActingRound, HandleRoomBroadcast);
        RequestManager.Instance.RegisterBroadcast(ActionCode.BroadCastRequest_ActingCD, HandleRoomBroadcast);
        RequestManager.Instance.RegisterBroadcast(ActionCode.BroadCastRequest_ShowActing, HandleRoomBroadcast);
        RequestManager.Instance.RegisterBroadcast(ActionCode.BroadCast_Request_SideReault, HandleRoomBroadcast);
        RequestManager.Instance.RegisterBroadcast(ActionCode.Request_ShowFoldPoker, HandleRoomBroadcast);
    }

    public override void SendRequest(MainPack pack)
    {
        base.SendRequest(pack);
    }

    public override void HandleRequest(MainPack pack)
    {
        if (thisView.gameObject.activeSelf == false) return;

        switch (pack.ActionCode)
        {
            //更新房間訊息
            case ActionCode.Request_UpdateRoomInfo:
                isStartReceiveRequest = true;
                thisView.UpdateGameRoomInfo(pack);
                break;
        }
    }

    public override void HandleRoomBroadcast(MainPack pack)
    {
        if (isStartReceiveRequest == false || thisView.gameObject.activeSelf == false) return;

        switch (pack.ActionCode)
        {
            //玩家進出房間
            case ActionCode.Request_PlayerInOutRoom:
                if (pack.PlayerInOutRoomPack.IsInRoom)
                {
                    //新玩家進入
                    thisView.AddPlayer(pack.PlayerInOutRoomPack.PlayerInfoPack);
                }
                else
                {
                    //玩家退出
                    thisView.PlayerExitRoom(pack.PlayerInOutRoomPack.PlayerInfoPack.UserID);
                }
                break;

            //遊戲階段
            case ActionCode.BroadCastRequest_GameStage:
                thisView.AutoActionState = GameView.AutoActingEnum.None;
                StartCoroutine(thisView.IGameStage(pack));
                break;

            //本地玩家行動回合
            case ActionCode.BroadCastRequest_PlayerActingRound:
                thisView.ILocalPlayerRound(pack.PlayerActingRoundPack);
                break;

            //玩家行動倒數
            case ActionCode.BroadCastRequest_ActingCD:
                //行動框
                GamePlayerInfo player = thisView.GetPlayer(pack.ActingCDPack.ActingPlayerId);
                player.ActionFrame = true;
                player.StartCountDown(pack.ActingCDPack.TotalCDTime,
                                      pack.ActingCDPack.CD);
                break;

            //演示玩家行動
            case ActionCode.BroadCastRequest_ShowActing:
                thisView.TotalPot = pack.GameRoomInfoPack.TotalPot;
                thisView.GetPlayerAction(pack.PlayerActedPack);
                break;

            //邊池結果
            case ActionCode.BroadCast_Request_SideReault:
                StartCoroutine(thisView.SideResult(pack));
                break;

            //顯示手牌
            case ActionCode.Request_ShowFoldPoker:
                thisView.GetShowFoldPoker(pack);
                break;
        }
    }

    /// <summary>
    /// 發送更新房間訊息
    /// </summary>
    public void SendRequest_UpdateRoomInfo()
    {
        MainPack pack = new MainPack();
        pack.ActionCode = ActionCode.Request_UpdateRoomInfo;

        SendRequest(pack);
    }

    /// <summary>
    /// 發送玩家採取行動
    /// </summary>
    /// <param name="id">玩家ID</param>
    /// <param name="acting">採取行動</param>
    /// <param name="betValue">下注值</param>
    public void SendRequest_PlayerActed(string id, ActingEnum acting, int betValue)
    {
        MainPack pack = new MainPack();
        pack.ActionCode = ActionCode.Request_PlayerActed;

        PlayerActedPack playerActedPack = new PlayerActedPack();
        playerActedPack.ActPlayerId = id;
        playerActedPack.ActingEnum = acting;
        playerActedPack.BetValue = betValue;

        pack.PlayerActedPack = playerActedPack;
        SendRequest(pack);
    }

    /// <summary>
    /// 發送顯示棄牌手牌
    /// </summary>
    /// <param name="index"></param>
    public void SendShowFoldPoker(int index)
    {
        MainPack pack = new MainPack();
        pack.ActionCode = ActionCode.Request_ShowFoldPoker;

        ShowFoldPokerPack showFoldPokerPack = new ShowFoldPokerPack();
        showFoldPokerPack.HandPokerIndex = index;
        showFoldPokerPack.UserID = Entry.TestInfoData.LocalUserId;

        pack.ShowFoldPokerPack = showFoldPokerPack;
        SendRequest(pack);
    }

    /// <summary>
    /// 離開房間
    /// </summary>
    /// <param name="id"></param>
    public void SendRequest_ExitRoom(string id)
    {
        MainPack pack = new MainPack();
        pack.ActionCode = ActionCode.Request_PlayerInOutRoom;

        PlayerInfoPack playerInfoPack = new PlayerInfoPack();
        playerInfoPack.UserID = id;

        PlayerInOutRoomPack playerInOutRoomPack = new PlayerInOutRoomPack();
        playerInOutRoomPack.IsInRoom = false;
        playerInOutRoomPack.PlayerInfoPack = playerInfoPack;

        pack.PlayerInOutRoomPack = playerInOutRoomPack;
        SendRequest(pack);
    }
}
