using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using MetaMask.Unity;
using System.Numerics;
using System;

public static class GameDataManager
{
    [Header("場景")]
    public static SceneEnum CurrScene { get; set; }     //當前場景


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
