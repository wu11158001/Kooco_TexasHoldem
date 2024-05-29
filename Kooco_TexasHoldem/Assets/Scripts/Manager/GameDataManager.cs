using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using MetaMask.Unity;
using System.Numerics;
using System;

public static class GameDataManager
{
    [Header("Line")]
    public static string LineChannelId = "2005465224";
    public static string LineChannelSecret = "2ce90d03cf058e20c60f1c8a421889c6";
    public static string LineRedirectUri = "https://wu11158001.github.io/asiapoker_self/demo.asiapoker/index.html";
    public static string LineMail { get; set; }


    [Header("公用")]
    public static bool IsMobilePlatform { get; set; }       //是否為移動平台

    [Header("場景")]
    public static SceneEnum CurrScene { get; set; }         //當前場景

    [Header("用戶")]
    public static string UserWalletAddress { get; set; }     //用戶錢包地址
    public static string UserWalletBalance { get; set; }     //用戶錢包餘額


    [Header("大廳")]
    public static int MinMagnification = 40;                                    //購買籌碼最小倍率
    public static int MaxMagnification = 200;                                   //購買籌碼最大倍率
    public static List<double> CashRoomSmallBlindList = new List<double>        //現金房小盲值
    {
        50, 100, 200, 400,
    };
    public static int MaxRoomCount = 8;                                         //最大房間數量
}
