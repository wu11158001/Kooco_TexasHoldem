using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using MetaMask.Unity;
using System.Numerics;
using System;

public static class DataManager
{
    [Header("IG")]
    public static string IGClientId = "1019674972416526";                                                                               //IG應用程式編號
    public static string IGSecret = "2880b1cc76d96b0aa3e9da7251170e7f";                                                                 //IG密鑰                                      
    public static string IGIUserIdAndName { get; set; }                                                                                 //IG訊息

    [Header("Line")]
    public static string LineChannelId = "2005465224";                                                                                  //Line頻道ID
    public static string LineChannelSecret = "2ce90d03cf058e20c60f1c8a421889c6";                                                        //Line密鑰
    public static string LineMail { get; set; }                                                                                         //Line信箱

    [Header("Instagram")]
    public static string InstagramChannelID = "1380953359235919";
    public static string InstagramChannelSecret = "b38352322a94cad7812394a311ad26a7";
    public static string InstagramRedirectUri = "https://kooco.github.io/ACEdemo/demo.asiapoker/index.html";

    [Header("公用")]
    public static string RedirectUri = "https://kooco.github.io/ACEdemo/demo.asiapoker/index.html";                                  //重定向Url
    public static bool IsNotFirstInLogin { get; set; }                                                                               //非首次進入登入
    public static bool IsMobilePlatform { get; set; }                                                                                //是否為移動平台
    public static bool IsDefaultBrowser { get; set; }                                                                                //是否在預設瀏覽器內
    public static bool IsInCoinbase { get; set; }                                                                                    //是否在Coinbase瀏覽器

    [Header("場景")]
    public static SceneEnum CurrScene { get; set; }                 //當前場景

    [Header("用戶訊息")]
    public static string UserWalletAddress { get; set; }            //用戶錢包地址
    public static string UserWalletBalance { get; set; }            //用戶錢包餘額
    public static string UserNickname { get; set; }                 //用戶暱稱
    public static int UserAvatar { get; set; }                      //用戶頭像
    public static int UserStamina { get; set; }                     //用戶耐力
    public static int UserOTProps { get; set; }                     //用戶加時道具數量
    public static double UserCryptoChips { get; set; }              //用戶加密貨幣籌碼
    public static double UserVCChips { get; set; }                  //用戶虛擬貨幣籌碼
    public static double UserGoldChips{ get; set; }                 //用戶Gold籌碼

    [Header("大廳")]
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

    [Header("NFT")]
    public static string NFTApiKey = "3ba7e50212234586a43a5d9ac50b7cd0";

    //  每日任務資料
    public static List<QuestInfo> DailyQuestList = new List<QuestInfo>
    {
        new QuestInfo(){ QuestName = "Daily Check-in",GetPoint = 30,FinishProgress = 5,CurrentProgress = 0,GetCoin = 3000},
        new QuestInfo(){ QuestName = "Cash Bet 1000 Amount",GetPoint = 10,FinishProgress = 1000,CurrentProgress = 0,GetCoin = 1000},
        new QuestInfo(){ QuestName = "Cash Bet 2000 Amount",GetPoint = 20,FinishProgress = 2000,CurrentProgress = 0,GetCoin = 2000},
        new QuestInfo(){ QuestName = "Cash Bet 3000 Amount",GetPoint = 30,FinishProgress = 3000,CurrentProgress = 0,GetCoin = 3000},
        new QuestInfo(){ QuestName = "Cash Bet 4000 Amount",GetPoint = 40,FinishProgress = 4000,CurrentProgress = 536,GetCoin = 4000},
        new QuestInfo(){ QuestName = "Cash Bet 5000 Amount",GetPoint = 50,FinishProgress = 5000,CurrentProgress = 333,GetCoin = 5000},
        new QuestInfo(){ QuestName = "Cash Bet 6000 Amount",GetPoint = 60,FinishProgress = 6000,CurrentProgress = 666,GetCoin = 6000},
        new QuestInfo(){ QuestName = "Cash Bet 7000 Amount",GetPoint = 70,FinishProgress = 7000,CurrentProgress = 999,GetCoin = 7000}
    };
    //  每周任務資料
    public static List<QuestInfo> WeekQuestList = new List<QuestInfo>
    {
        new QuestInfo(){ QuestName = "Weekly Check-in",GetPoint = 100,FinishProgress = 7,CurrentProgress = 0,GetCoin = 50000},
        new QuestInfo(){ QuestName = "Cash Bet 1000 Amount",GetPoint = 100,FinishProgress = 1000,CurrentProgress = 0,GetCoin = 5000},
        new QuestInfo(){ QuestName = "Cash Bet 2000 Amount",GetPoint = 200,FinishProgress = 2000,CurrentProgress = 0,GetCoin = 6000},
        new QuestInfo(){ QuestName = "Cash Bet 3000 Amount",GetPoint = 300,FinishProgress = 3000,CurrentProgress = 0,GetCoin = 7000},
        new QuestInfo(){ QuestName = "Cash Bet 4000 Amount",GetPoint = 400,FinishProgress = 4000,CurrentProgress = 0,GetCoin = 8000},
        new QuestInfo(){ QuestName = "Cash Bet 5000 Amount",GetPoint = 500,FinishProgress = 5000,CurrentProgress = 0,GetCoin = 9000},
        new QuestInfo(){ QuestName = "Cash Bet 6000 Amount",GetPoint = 600,FinishProgress = 6000,CurrentProgress = 0,GetCoin = 10000},
    };

    [Header("排名")]
    public static List<RankData> CurrSeasonIntegralRankList;                //當季積分排名
    public static List<RankData> CurrSeasonCashRankList;                    //當季Cash排名
    public static List<RankData> CurrSeasonGoldenRankList;                  //當季Golden排名
    public static List<RankData> PreSeasonIntegralRankList;                 //前季積分排名
    public static List<RankData> PreSeasonCashRankList;                     //前季Cash排名
    public static List<RankData> PreSeasonGoldenRankList;                   //前季Golden排名
    public static DateTime RandEndDate;                                     //賽季結束日期
    public static int CurrRankSeason;                                       //當前賽季
    public static RankData LocalUserRankData;                               //本地玩家排名資料

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
        LocalUserRankData = new RankData() { rank = 1001, point = 5, award = 5};

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
        PreSeasonIntegralRankList = new List<RankData>()
        {
            new RankData() { avatar = 0, nickname = "OO", point = 1230, award = 100 },
            new RankData() { avatar = 1, nickname = "BB", point = 950, award = 90 },
            new RankData() { avatar = 2, nickname = "PP", point = 900, award = 80 },
            new RankData() { avatar = 3, nickname = "DD", point = 850, award = 70 },
            new RankData() { avatar = 4, nickname = "YY", point = 800, award = 60 },
            new RankData() { avatar = 5, nickname = "FF", point = 750, award = 60 },
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
            new RankData() { avatar = 4, nickname = "EE", point = 800, award = 60 },
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
            new RankData() { avatar = 2, nickname = "AA", point = 900, award = 80 },
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
            new RankData() { avatar = 0, nickname = "XX", point = 2000, award = 100 },
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
    }
}

public class RankData
{
    public int rank;
    public int avatar;
    public string nickname;
    public int point;
    public int award;
}
