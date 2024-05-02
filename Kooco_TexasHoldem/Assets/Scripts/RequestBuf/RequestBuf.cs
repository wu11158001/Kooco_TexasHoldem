using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RequestBuf
{
    //發送模式
    public enum SendModeCode
    {
        ModeNone,       
        RoomBroadcast,  //房間廣播
    }

    public enum FlowEnum
    {
        Licensing,          //發牌
        SetBlind,           //大小盲
        Flop,               //翻牌
        Turn,               //轉牌
        River,              //河牌
        PotResult,             //遊戲結果
    }

    public enum PlayerStateEnum
    {
        Waiting,        //等待下局
        Playing,        //遊戲中
        AllIn,          //All In狀態
        Fold,           //棄牌
    }

    public enum ActingEnum
    {
        Blind,      //大小盲
        Fold,       //棄牌
        Check,      //過牌
        Raise,      //加注
        Call,       //跟注
        AllIn,      //All In
    }

    public enum ActionCode
    {
        Request_UpdateRoomInfo,                 //更新房間訊息
        Request_PlayerInOutRoom,                //玩家進出房間
        BroadCastRequest_GameStage,             //遊戲階段
        BroadCastRequest_PlayerActingRound,     //玩家行動回合
        BroadCastRequest_ActingCD,              //玩家行動倒數
        Request_NotEnoughChips,                 //籌碼不足
        Request_PlayerActed,                    //玩家採取行動
        BroadCastRequest_ShowActing,            //演示玩家行動
        BroadCast_Request_SideReault,           //邊池結果
    }

    public class MainPack
    {
        public ActionCode ActionCode;
        public SendModeCode SendModeCode;

        public List<PlayerInfoPack> PlayerInfoPackList;
        public UpdateRoomInfoPack UpdateRoomInfoPack;
        public GameRoomInfoPack GameRoomInfoPack;
        public GameStagePack GameStagePack;
        public PlayerActingRoundPack PlayerActingRoundPack;
        public ActingCDPack ActingCDPack;
        public PlayerActedPack PlayerActedPack;
        public CommunityPokerPack CommunityPokerPack;
        public WinnerPack WinnerPack;
        public PlayerInOutRoomPack PlayerInOutRoomPack;
        public LicensingStagePack LicensingStagePack;
        public BlindStagePack BlindStagePack;
        public SidePack SidePack;
    }

    /// <summary>
    /// 玩家訊息包
    /// </summary>
    public class PlayerInfoPack
    {
        public int Seat;                //座位
        public string UserID;           //ID
        public string NickName;         //暱稱
        public int Chips;               //籌碼
        public int CurrBetValue;        //當前下注籌碼
    }

    /// <summary>
    /// 遊戲房間訊息包
    /// </summary>
    public class GameRoomInfoPack
    {
        public int TotalPot;                                //底池籌碼
        public Dictionary<string, int> AllPlayerChips;      //所有遊戲玩家籌碼
    }

    /// <summary>
    /// 玩家進出房間包
    /// </summary>
    public class PlayerInOutRoomPack
    {
        public bool IsInRoom;                       //true=進入房間,false=退出房間
        public PlayerInfoPack PlayerInfoPack;
    }

    /// <summary>
    /// 更新房間訊息包
    /// </summary>
    public class UpdateRoomInfoPack
    {
        public FlowEnum flowEnum;                           //遊戲階段
        public int TotalPot;                                //底池籌碼
        public List<string> playingIdList;                  //遊戲中玩家ID
    }

    /// <summary>
    /// 公共牌包
    /// </summary>
    public class CommunityPokerPack
    {
        public List<int> CurrCommunityPoker;                //當前公共牌
    }

    /// <summary>
    /// 遊戲階段包
    /// </summary>
    public class GameStagePack
    {
        public FlowEnum flowEnum;                               //遊戲階段
        public int SmallBlind;                                  //小盲值
    }

    /// <summary>
    /// 發牌階段包
    /// </summary>
    public class LicensingStagePack
    {
        public string ButtonSeatId;                             //Button座位玩家ID
        public Dictionary<string, (int, int)> HandPokerDic;     //遊戲玩家手牌(id, (手牌0, 手牌1))
    }

    /// <summary>
    /// 盲注階段包
    /// </summary>
    public class BlindStagePack
    {
        public string SBPlayerId;       //小盲玩家ID
        public string BBPlayerId;       //大盲玩家ID
        public int SBPlayerChips;       //小盲玩家籌碼
        public int BBPlayerChips;       //大盲玩家籌碼
    }

    /// <summary>
    /// 玩家行動回合包
    /// </summary>
    public class PlayerActingRoundPack
    {
        public bool IsUnableRaise;          //是否無法在加注
        public bool IsFirstRaisePlayer;     //首位加注玩家
        public int CurrCallValue;           //當前跟注值
        public int CallDifference;          //跟注差額
        public int TotalPot;                //當前底池
        public int PlayerChips;             //行動玩家籌碼
        public int PlayerCurrBryValue;      //行動玩家當前下注值
    }

    /// <summary>
    /// 行動倒數包
    /// </summary>
    public class ActingCDPack
    {
        public int TotalCDTime;             //總時長
        public int CD;                      //倒數時間
        public string ActingPlayerId;       //行動玩家ID
    }

    /// <summary>
    /// 玩家採取動作包
    /// </summary>
    public class PlayerActedPack
    {
        public string ActPlayerId;          //行動玩家ID
        public ActingEnum ActingEnum;       //採取動作
        public int BetValue;                //下注值
        public int PlayerChips;             //玩家籌碼
    }

    /// <summary>
    /// 獲勝玩家包
    /// </summary>
    public class WinnerPack
    {
        public Dictionary<string, int> WinnerDic;       //獲勝玩家(ID, 當前籌碼)
        public int WinChips;                            //贏得籌碼
    }

    //邊池包
    public class SidePack
    {
        public Dictionary<string, int> AllPlayerChips;      //所有遊戲玩家籌碼
        public Dictionary<string, int> BackChips;           //退回籌碼
        public Dictionary<string, int> SideWinnerDic;       //邊池獲勝玩家(ID, 獲勝籌碼)
        public int TotalSideChips;                          //邊池總籌碼
    }
}