using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Poker : MonoBehaviour
{
    [SerializeField]
    Image poker_Img, frame_Img;
    [SerializeField]
    GameObject shining_Obj;

    int pokerNumber;    //撲克數字(-1=背面)

    /// <summary>
    /// 撲克數字(-1=背面)
    /// </summary>
    public int PokerNum
    {
        get
        {
            return pokerNumber;
        }
        set
        {
            pokerNumber = value;
            if (value <= -1)
            {
                poker_Img.sprite = AssetsManager.Instance.GetAlbumAsset(AlbumEnum.pokerBackAlbum).album[0];
            }
            else
            {
                poker_Img.sprite = AssetsManager.Instance.GetAlbumAsset(AlbumEnum.PokerNumAlbum).album[value];
            }            
        }
    }

    /// <summary>
    /// 外框激活
    /// </summary>
    public bool PokerFrameEnable
    {
        set
        {
            frame_Img.enabled = value;
            poker_Img.rectTransform.localScale = Vector3.one;
            if (value == false)
            {
                shining_Obj.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 設定撲克Alpha值
    /// </summary>
    public float SetColor
    {
        set
        {
            poker_Img.color = new Color(value, value, value, 1);
        }
    }

    private void OnEnable()
    {
        PokerFrameEnable = false;
        shining_Obj.SetActive(false);
    }

    /// <summary>
    /// 播放贏家效果
    /// </summary>
    public void StartWinEffect()
    {
        shining_Obj.SetActive(true);
    }

    /// <summary>
    /// 水平翻牌效果
    /// </summary>
    /// <param name="frontNum">正面數字</param>
    /// <returns></returns>
    public IEnumerator IHorizontalFlopEffect(int frontNum)
    {
        poker_Img.rectTransform.rotation = Quaternion.Euler(poker_Img.rectTransform.eulerAngles.x, 180, poker_Img.rectTransform.eulerAngles.z);
        poker_Img.sprite = AssetsManager.Instance.GetAlbumAsset(AlbumEnum.pokerBackAlbum).album[0];

        float turnTime = 0.5f;
        DateTime startTime = DateTime.Now;
        while ((DateTime.Now - startTime).TotalSeconds < turnTime)
        {
            float progess = (float)(DateTime.Now - startTime).TotalSeconds / turnTime;
            float rotY = Mathf.Lerp(180, 0, progess);
            poker_Img.rectTransform.rotation = Quaternion.Euler(poker_Img.rectTransform.eulerAngles.x, rotY, poker_Img.rectTransform.eulerAngles.z);

            if (rotY <= 90)
            {
                PokerNum = frontNum;
            }

            yield return null;
        }

        poker_Img.rectTransform.rotation = Quaternion.Euler(poker_Img.rectTransform.eulerAngles.x, 0, poker_Img.rectTransform.eulerAngles.z);
    }
}
