using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameDataManager
{
    [Header("場景")]
    public static SceneEnum CurrScene { get; set; }     //當前場景


    [Header("用戶")]
    public static string UserWalletAddress { get; set; }     //用戶錢包地址
    public static string UserWalletBalance { get; set; }     //用戶錢包餘額


    [Header("大廳")]
    public static RoomEnum CurrRoomType;                                        //當前選擇房間類型 
    public static int MinMagnification = 40;                                    //購買籌碼最小倍率
    public static int MaxMagnification = 200;                                   //購買籌碼最大倍率
    public static List<double> CashRoomSmallBlindList = new List<double>        //現金房間小盲值
    {
        50, 100, 200, 400,
    };


    [Header("遊戲")]
    public static double RoomSmallBlind { get; set; }           //進入房間的小盲值
}
