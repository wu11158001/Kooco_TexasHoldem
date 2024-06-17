using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;

public static class Utils
{
    /// <summary>
    /// 產生QRCode
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static Texture2D GenerateQRCodeTexture(string text)
    {
        BarcodeWriter barcodeWriter = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = 256,
                Width = 256
            }
        };

        Color32[] pixels = barcodeWriter.Write(text);
        Texture2D qrCodeTexture = new Texture2D(256, 256);
        qrCodeTexture.SetPixels32(pixels);
        qrCodeTexture.Apply();

        return qrCodeTexture;
    }

    /// <summary>
    /// 設置Dropdown項目
    /// </summary>
    /// <param name="dropdown"></param>
    /// <param name="options"></param>
    public static void SetOptionsToDropdown(Dropdown dropdown, List<string> options)
    {
        //清空當前選項
        dropdown.ClearOptions();

        //添加新的選項
        dropdown.AddOptions(options);
    }

    /// <summary>
    /// 日期轉時間戳
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static long ConvertDateTimeToTimestamp(DateTime dateTime)
    {
        // 確保DateTime對象是基於UTC
        DateTime utcDateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);

        // 使用DateTimeOffset將其轉換為Unix時間戳
        long timestamp = new DateTimeOffset(utcDateTime).ToUnixTimeSeconds();

        return timestamp;
    }

    /// <summary>
    /// 時間戳轉日期
    /// </summary>
    /// <param name="timestamp"></param>
    /// <returns></returns>
    public static DateTime ConvertTimestampToDate(long timestamp)
    {
        // 使用DateTimeOffset將Unix時間戳轉換為DateTime
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(timestamp);
        DateTime dateTime = dateTimeOffset.DateTime;

        // 如果需要UTC時間，可以使用:
        // DateTime dateTime = dateTimeOffset.UtcDateTime;

        return dateTime;
    }
}
