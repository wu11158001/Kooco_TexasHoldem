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
    public static string LineChannelId = "2005465224";                                                                                  //Line頻道ID
    public static string LineChannelSecret = "2ce90d03cf058e20c60f1c8a421889c6";                                                        //Line密鑰
    public static string LineMail { get; set; }                                                                                         //Line信箱


    [Header("公用")]
    public static string RedirectUri = "https://kooco.github.io/ACEdemo/demo.asiapoker/index.html";                                  //重定向Url
    public static bool IsMobilePlatform { get; set; }                                                                                //是否為移動平台
    public static bool IsDefaultBrowser { get; set; }                                                                                //是否在預設瀏覽器內


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
