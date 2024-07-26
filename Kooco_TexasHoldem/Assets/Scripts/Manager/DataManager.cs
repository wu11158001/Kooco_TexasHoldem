using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using MetaMask.Unity;
using System.Numerics;
using System;
using Thirdweb;

public static class DataManager
{
    #region IG

    public static string IGClientId = "1019674972416526";                                                                               //IG應用程式編號
    public static string IGSecret = "2880b1cc76d96b0aa3e9da7251170e7f";                                                                 //IG密鑰                                      
    public static string IGIUserIdAndName { get; set; }                                                                                 //IG訊息

    #endregion

    #region Line

    public static string LineChannelId = "2005465224";                                                                                  //Line頻道ID
    public static string LineChannelSecret = "2ce90d03cf058e20c60f1c8a421889c6";                                                        //Line密鑰
    public static string LineMail { get; set; }                                                                                         //Line信箱

    #endregion

    #region Instagram

    public static string InstagramChannelID = "1380953359235919";
    public static string InstagramChannelSecret = "b38352322a94cad7812394a311ad26a7";
    public static string InstagramRedirectUri = "https://kooco.github.io/ACEdemo/demo.asiapoker/index.html";

    #endregion

    #region 公用

    public static string RedirectUri = "https://kooco.github.io/ACEdemo/demo.asiapoker/index.html";                                  //重定向Url
    public static bool IsNotFirstInLogin { get; set; }                                                                               //非首次進入登入
    public static bool IsMobilePlatform { get; set; }                                                                                //是否為移動平台
    public static bool IsDefaultBrowser { get; set; }                                                                                //是否在預設瀏覽器內
    public static bool IsInCoinbase { get; set; }                                                                                    //是否在Coinbase瀏覽器
    public static bool IsOpenDownloadWallet { get; set; }                                                                            //是否跳轉到下載錢包頁面
    /// <summary>
    /// 國碼
    /// </summary>
    public static List<string> CountryCode = new()                                                                                  
    {
        "+886",
        "+885",
        "+884",
    };

    #endregion

    #region 場景

    public static SceneEnum CurrScene { get; set; }                 //當前場景

    #endregion

    #region 用戶訊息

    public static string UserWalletAddress { get; set; }            //用戶錢包地址
    public static string UserWalletBalance { get; set; }            //用戶錢包餘額
    public static string UserId { get; set; }                       //用戶ID
    public static string UserNickname { get; set; }                 //用戶暱稱
    public static int UserAvatar { get; set; }                      //用戶頭像
    public static int UserStamina { get; set; }                     //用戶耐力
    public static int UserOTProps { get; set; }                     //用戶加時道具數量
    public static double UserCryptoChips { get; set; }              //用戶加密貨幣籌碼
    public static double UserVCChips { get; set; }                  //用戶虛擬貨幣籌碼
    public static double UserGoldChips { get; set; }                 //用戶Gold籌碼

    #endregion

    #region 大廳

    public static int CurrBillboardIndex { get; set; }                                            //當前廣告刊版Index
    public static int MaxStaminaValue { get { return 50; } }                                      //最大耐力值
    public static int MinMagnification { get { return 40; } }                                     //購買籌碼最小倍率
    public static int MaxMagnification { get { return 200; } }                                    //購買籌碼最大倍率
    public static List<double> CryptoSmallBlindList = new List<double>                            //加密貨幣桌小盲值
    {
        50, 100, 200, 400,
    };
    public static List<double> VCSmallBlindList = new List<double>                                //虛擬貨幣桌小盲值
    {
        200, 400, 800, 1000,
    };

    #endregion

    #region NFT

    public static string NFTApiKey = "3ba7e50212234586a43a5d9ac50b7cd0";

    #endregion

    #region 每日任務資料

    //  每日任務資料
    public static List<QuestInfo> DailyQuestList = new List<QuestInfo>
    {
        new QuestInfo(){ QuestName = "Daily Check-in",GetPoint = 30,FinishProgress = 5,CurrentProgress = 5,GetCoin = 3000,Received = false},
        new QuestInfo(){ QuestName = "Cash Bet 1000 Amount",GetPoint = 10,FinishProgress = 1000,CurrentProgress = 0,GetCoin = 1000,Received = false},
        new QuestInfo(){ QuestName = "Cash Bet 2000 Amount",GetPoint = 20,FinishProgress = 2000,CurrentProgress = 0,GetCoin = 2000,Received = false},
        new QuestInfo(){ QuestName = "Cash Bet 3000 Amount",GetPoint = 30,FinishProgress = 3000,CurrentProgress = 0,GetCoin = 3000,Received = false},
        new QuestInfo(){ QuestName = "Cash Bet 4000 Amount",GetPoint = 40,FinishProgress = 4000,CurrentProgress = 0,GetCoin = 4000,Received = false},
        new QuestInfo(){ QuestName = "Cash Bet 5000 Amount",GetPoint = 50,FinishProgress = 5000,CurrentProgress = 0,GetCoin = 5000,Received = false},
        new QuestInfo(){ QuestName = "Cash Bet 6000 Amount",GetPoint = 60,FinishProgress = 6000,CurrentProgress = 0,GetCoin = 6000, Received = false},
        new QuestInfo(){ QuestName = "Cash Bet 7000 Amount",GetPoint = 70,FinishProgress = 7000,CurrentProgress = 0,GetCoin = 7000, Received = false}
    };
    //  每周任務資料
    public static List<QuestInfo> WeeklyQuestList = new List<QuestInfo>
    {
        new QuestInfo(){ QuestName = "Weekly Check-in",GetPoint = 100,FinishProgress = 7,CurrentProgress = 0,GetCoin = 50000, Received = false},
        new QuestInfo(){ QuestName = "Cash Bet 1000 Amount",GetPoint = 100,FinishProgress = 1000,CurrentProgress = 0,GetCoin = 5000, Received = false},
        new QuestInfo(){ QuestName = "Cash Bet 2000 Amount",GetPoint = 200,FinishProgress = 2000,CurrentProgress = 0,GetCoin = 6000, Received = false},
        new QuestInfo(){ QuestName = "Cash Bet 3000 Amount",GetPoint = 300,FinishProgress = 3000,CurrentProgress = 0,GetCoin = 7000, Received = false},
        new QuestInfo(){ QuestName = "Cash Bet 4000 Amount",GetPoint = 400,FinishProgress = 4000,CurrentProgress = 0,GetCoin = 8000, Received = false},
        new QuestInfo(){ QuestName = "Cash Bet 5000 Amount",GetPoint = 500,FinishProgress = 5000,CurrentProgress = 0,GetCoin = 9000, Received = false},
    };
    public static int CurrentBonusAmount { get; set; }                     //  當前任務紅利進度
    public static int BonusMax { get; set; }                               //  任務紅利最大值

    #endregion

    #region 排名

    public static List<RankData> CurrSeasonIntegralRankList;                //當季積分排名
    public static List<RankData> CurrSeasonCashRankList;                    //當季Cash排名
    public static List<RankData> CurrSeasonGoldenRankList;                  //當季Golden排名
    public static List<RankData> PreSeasonIntegralRankList;                 //前季積分排名
    public static List<RankData> PreSeasonCashRankList;                     //前季Cash排名
    public static List<RankData> PreSeasonGoldenRankList;                   //前季Golden排名
    public static DateTime RandEndDate;                                     //賽季結束日期
    public static int CurrRankSeason;                                       //當前賽季
    public static List<RankData> LocalUserRankData;                         //本地玩家排名資料


    //  體力商品資料
    public static List<ShopData> Stamina_Shop = new List<ShopData>()
    {
        new ShopData(){BuffName = "Percentage",BuffAmount = 10,CostCoin = 30 },
        new ShopData(){BuffName = "Percentage",BuffAmount = 50,CostCoin = 150 },
        new ShopData(){BuffName = "Percentage",BuffAmount = 100,CostCoin = 300},
        //new ShopData(){BuffName = "rrr",BuffAmount = 29328392,CostCoin = 8787},
    };

    //  金幣商品資料
    public static List<ShopData> Gold_Shop = new List<ShopData>()
    {
        new ShopData(){BuffName = "Points", BuffAmount = 100,CostCoin = 30},
        new ShopData(){BuffName = "Points", BuffAmount = 300,CostCoin = 90},
        new ShopData(){BuffName = "Points", BuffAmount = 400,CostCoin = 120},
        new ShopData(){BuffName = "Points", BuffAmount = 500,CostCoin = 150},
        new ShopData(){BuffName = "Points", BuffAmount = 850,CostCoin = 250},
        new ShopData(){BuffName = "Points", BuffAmount = 1000,CostCoin = 300},
        //new ShopData(){BuffName = "Testtttt", BuffAmount = 23333333,CostCoin = 888888},
    };

    //  加時商品資料
    public static List<ShopData> ExtraTime_Shop = new List<ShopData>()
    {
        new ShopData(){BuffName = "Second", BuffAmount = 10,CostCoin = 30},
        new ShopData(){BuffName = "Second", BuffAmount = 30,CostCoin = 90},
        new ShopData(){BuffName = "Second", BuffAmount = 40,CostCoin = 120},
        new ShopData(){BuffName = "Second", BuffAmount = 50,CostCoin = 150},
        new ShopData(){BuffName = "Second", BuffAmount = 85,CostCoin = 250},
        new ShopData(){BuffName = "Second", BuffAmount = 100,CostCoin = 300},
    };


    /// <summary>
    /// 接收排名資料
    /// </summary>
    public static void ReciveRankData()
    {
        CurrRankSeason = 2;

        //賽季剩餘時間(分)
        int timeLest = 144000;
        RandEndDate = DateTime.Now.AddMinutes(timeLest);

        //本地玩家排名
        LocalUserRankData = new List<RankData>()
        {
            new RankData() { rank = 1001, point = 5, award = 5},        //當季積分
            new RankData() { rank = 5, point = 800, award = 60},        //當季Cash
            new RankData() { rank = 1, point = 2000, award = 100},      //當季Golend

            new RankData() { rank = 6, point = 750, award = 60},        //前季積分
            new RankData() { rank = 3, point = 900, award = 80},        //前季Cash
            new RankData() { rank = 88, point = 5, award = 5},          //前季Golend
        };

        //當季積分排名
        CurrSeasonIntegralRankList = new List<RankData>()
        {
            new RankData() { avatar = 0, nickname = "AA", point = 1000, award = 100 },
            new RankData() { avatar = 1, nickname = "BB", point = 950, award = 90 },
            new RankData() { avatar = 2, nickname = "CC", point = 900, award = 80 },
            new RankData() { avatar = 3, nickname = "DD", point = 850, award = 70 },
            new RankData() { avatar = 4, nickname = "EE", point = 800, award = 60 },
            new RankData() { avatar = 5, nickname = "FF", point = 750, award = 60 },
            new RankData() { avatar = 6, nickname = "GG", point = 700, award = 60 },
            new RankData() { avatar = 7, nickname = "HH", point = 650, award = 60 },
            new RankData() { avatar = 6, nickname = "II", point = 600, award = 60 },
            new RankData() { avatar = 5, nickname = "JJ", point = 550, award = 60 },
            new RankData() { avatar = 4, nickname = "KK", point = 450, award = 40 },
        };
        //前季積分排名
        PreSeasonIntegralRankList = new List<RankData>()
        {
            new RankData() { avatar = 0, nickname = "OO", point = 1230, award = 100 },
            new RankData() { avatar = 1, nickname = "BB", point = 950, award = 90 },
            new RankData() { avatar = 2, nickname = "PP", point = 900, award = 80 },
            new RankData() { avatar = 3, nickname = "DD", point = 850, award = 70 },
            new RankData() { avatar = 4, nickname = "YY", point = 800, award = 60 },
            new RankData() { avatar = UserAvatar, nickname = UserNickname, point = 750, award = 60 },
            new RankData() { avatar = 6, nickname = "LL", point = 700, award = 60 },
            new RankData() { avatar = 7, nickname = "HH", point = 650, award = 60 },
            new RankData() { avatar = 6, nickname = "II", point = 600, award = 60 },
            new RankData() { avatar = 5, nickname = "JJ", point = 550, award = 60 },
            new RankData() { avatar = 4, nickname = "KK", point = 450, award = 40 },
        };

        //當季Cash排名
        CurrSeasonCashRankList = new List<RankData>()
        {
            new RankData() { avatar = 0, nickname = "ZZ", point = 1100, award = 100 },
            new RankData() { avatar = 1, nickname = "BB", point = 1000, award = 90 },
            new RankData() { avatar = 2, nickname = "AA", point = 900, award = 80 },
            new RankData() { avatar = 3, nickname = "DD", point = 880, award = 70 },
            new RankData() { avatar = UserAvatar, nickname = UserNickname, point = 800, award = 60 },
            new RankData() { avatar = 5, nickname = "FF", point = 780, award = 60 },
            new RankData() { avatar = 6, nickname = "UU", point = 710, award = 60 },
            new RankData() { avatar = 7, nickname = "HH", point = 650, award = 60 },
            new RankData() { avatar = 6, nickname = "II", point = 630, award = 60 },
            new RankData() { avatar = 5, nickname = "JJ", point = 550, award = 60 },
            new RankData() { avatar = 4, nickname = "PP", point = 455, award = 40 },
        };
        //前季Cash排名
        PreSeasonCashRankList = new List<RankData>()
        {
            new RankData() { avatar = 0, nickname = "AA", point = 1005, award = 100 },
            new RankData() { avatar = 1, nickname = "BB", point = 1000, award = 90 },
            new RankData() { avatar = UserAvatar, nickname = UserNickname, point = 900, award = 80 },
            new RankData() { avatar = 3, nickname = "TT", point = 880, award = 70 },
            new RankData() { avatar = 4, nickname = "EE", point = 800, award = 60 },
            new RankData() { avatar = 5, nickname = "FF", point = 780, award = 60 },
            new RankData() { avatar = 6, nickname = "UU", point = 710, award = 60 },
            new RankData() { avatar = 7, nickname = "HH", point = 650, award = 60 },
            new RankData() { avatar = 6, nickname = "II", point = 630, award = 60 },
            new RankData() { avatar = 5, nickname = "JJ", point = 550, award = 60 },
            new RankData() { avatar = 4, nickname = "PP", point = 455, award = 40 },
        };

        //當季Golden排名
        CurrSeasonGoldenRankList = new List<RankData>()
        {
            new RankData() { avatar = UserAvatar, nickname = UserNickname, point = 2000, award = 100 },
            new RankData() { avatar = 1, nickname = "AA", point = 1500, award = 90 },
            new RankData() { avatar = 2, nickname = "BB", point = 900, award = 80 },
            new RankData() { avatar = 3, nickname = "DD", point = 870, award = 70 },
            new RankData() { avatar = 4, nickname = "EE", point = 830, award = 60 },
            new RankData() { avatar = 5, nickname = "FF", point = 770, award = 60 },
            new RankData() { avatar = 6, nickname = "YY", point = 730, award = 60 },
            new RankData() { avatar = 7, nickname = "HH", point = 670, award = 60 },
            new RankData() { avatar = 6, nickname = "II", point = 630, award = 60 },
            new RankData() { avatar = 5, nickname = "JJ", point = 530, award = 60 },
            new RankData() { avatar = 4, nickname = "RR", point = 450, award = 40 },
        };
        //前季Golden排名
        PreSeasonGoldenRankList = new List<RankData>()
        {
            new RankData() { avatar = 0, nickname = "LL", point = 2500, award = 100 },
            new RankData() { avatar = 1, nickname = "AA", point = 1500, award = 90 },
            new RankData() { avatar = 2, nickname = "SS", point = 1000, award = 80 },
            new RankData() { avatar = 3, nickname = "DD", point = 870, award = 70 },
            new RankData() { avatar = 4, nickname = "EE", point = 830, award = 60 },
            new RankData() { avatar = 5, nickname = "YY", point = 770, award = 60 },
            new RankData() { avatar = 6, nickname = "YY", point = 730, award = 60 },
            new RankData() { avatar = 7, nickname = "HH", point = 670, award = 60 },
            new RankData() { avatar = 6, nickname = "II", point = 630, award = 60 },
            new RankData() { avatar = 5, nickname = "JJ", point = 530, award = 60 },
            new RankData() { avatar = 4, nickname = "RR", point = 450, award = 40 },
        };

        //刷新排名
        LobbyRankingView lobbyRankingView = GameObject.FindFirstObjectByType<LobbyRankingView>();
        if (lobbyRankingView != null &&
            lobbyRankingView.gameObject.activeSelf)
        {
            lobbyRankingView.SetRank();
        }
    }

    #endregion

    #region 遊戲

    public static int MaxPlayerCount = 6;                                           //最大遊戲人數
    public static int MaxVideoSaveCount = 20;                                       //最大紀錄影片數量

    #endregion
}

public class RankData
{
    public int rank;
    public int avatar;
    public string nickname;
    public int point;
    public int award;
}

//  商店商品資料
public class ShopData
{
    public string BuffName;
    public int BuffAmount;
    public int CostCoin;
}
