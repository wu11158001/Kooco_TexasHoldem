using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class TransactionHistorySample : MonoBehaviour
{
    [SerializeField]
    Button Copy_Btn;
    [SerializeField]
    Image Status_Img;
    [SerializeField]
    TextMeshProUGUI Type_Txt, TypeTime_Txt, Title1_Txt, Title2_Txt, PL_Txt, Copied_Txt;

    private void Awake()
    {
        ListenerEvent();
    }

    private void Start()
    {
        Color copiedColor = Copied_Txt.color;
        copiedColor.a = 0;
        Copied_Txt.color = copiedColor;
    }

    /// <summary>
    /// 事件聆聽
    /// </summary>
    private void ListenerEvent()
    {
        //複製錢包地址
        Copy_Btn.onClick.AddListener(() =>
        {
            StringUtils.CopyText(Title2_Txt.text);
            UnityUtils.Instance.ColorFade(Copied_Txt,
                                          null,
                                          0.2f,
                                          0.5f,
                                          1.5f);
        });
    }

    /// <summary>
    /// 設定交易紀錄
    /// </summary>
    /// <param name="data"></param>
    public void SetTransactionHistory(TransactionHistoryData data)
    {
        string typeStr = "";
        switch (data.type)
        {
            case "Pledge":
                typeStr = LanguageManager.Instance.GetText("Pledge");
                break;

            case "Buy-In":
                typeStr = LanguageManager.Instance.GetText("BUY-IN");
                break;
        }

        Type_Txt.text = typeStr;
        DateTime date = Utils.ConvertTimestampToDate(long.Parse(data.time));
        TypeTime_Txt.text = $"{date.Month}/{date.Day} {date.Hour}:{date.Minute}";
        StringUtils.StrExceedSize(data.title1, Title1_Txt);
        StringUtils.StrExceedSize(data.title2, Title2_Txt);

        Sprite status = null;
        switch (data.status)
        {
            //交易失敗
            case -1:
                status = AssetsManager.Instance.GetAlbumAsset(AlbumEnum.TransactionStatusAlbum).album[2];
                break;

            //交易處理中
            case 0:
                status = AssetsManager.Instance.GetAlbumAsset(AlbumEnum.TransactionStatusAlbum).album[0];
                break;

            //交易成功
            case 1:
                status = AssetsManager.Instance.GetAlbumAsset(AlbumEnum.TransactionStatusAlbum).album[1];
                break;
        }
        Status_Img.sprite = status;
        PL_Txt.text = data.status == -1 ?
                      "#" :
                      data.pl;

        Copy_Btn.gameObject.SetActive(data.type == "Pledge");
    }
}
