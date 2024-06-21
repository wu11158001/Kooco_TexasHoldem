using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class NFTSample : MonoBehaviour
{
    [SerializeField]
    Image NFT_Image;
    [SerializeField]
    Text NFTName_Txt, Date_Txt, Rarity_Txt, Describe_Txt;

    /// <summary>
    /// 設置NFT
    /// </summary>
    /// <param name="data"></param>
    /// <param name="index"></param>
    public void SetNFT(NFTData data, int index)
    {
        //時間
        DateTime dateTime = DateTime.ParseExact(data.updated_at, "yyyy-MM-ddTHH:mm:ss.ffffff", null);
        Date_Txt.text = dateTime.ToString("yyyy-MM-dd HH:mm");
        //名稱
        NFTName_Txt.text = data.name;
        //稀有度
        Rarity_Txt.text = $"Rarity {data.rarity}%";
        //說明
        Describe_Txt.text = string.IsNullOrEmpty(data.description) ?
                            "No description." :
                            data.description;
        //圖像
        if (NFTManager.Instance.NFTImageList[index] != null)
        {
            NFT_Image.sprite = NFTManager.Instance.NFTImageList[index];
        }        
    }
}
