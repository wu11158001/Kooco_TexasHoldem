using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class TransactionHistorySample : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI Type_Txt, TypeTime_Txt, Title1_Txt, Title2_Txt, PL_Txt;
    [SerializeField]
    Button Copy_Btn;
    [SerializeField]
    Image Status_Img;

    private void Awake()
    {
        ListenerEvent();
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
        });
    }

    /// <summary>
    /// 設定交易紀錄
    /// </summary>
    /// <param name="data"></param>
    public void SetTransactionHistory(TransactionHistoryData data)
    {
        Type_Txt.text = data.type;
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
