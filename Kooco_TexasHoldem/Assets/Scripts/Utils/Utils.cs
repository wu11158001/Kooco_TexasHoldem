using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
