using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Poker : MonoBehaviour
{
    RectTransform thisRt;

    [SerializeField]
    Image Poker_Img, frame_Img;
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
                Poker_Img.sprite = GameAssetsManager.Instance.GetPokerBack(0);
            }
            else
            {
                Poker_Img.sprite = GameAssetsManager.Instance.GetPokerNum(value);
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
            thisRt.localScale = Vector3.one;
            if (value == false)
            {
                shining_Obj.SetActive(false);
            }
        }
    }

    private void Awake()
    {
        thisRt = GetComponent<RectTransform>();
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
        thisRt.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        shining_Obj.SetActive(true);
    }


    /// <summary>
    /// 翻牌效果
    /// </summary>
    /// <param name="frontNum">正面數字</param>
    /// <returns></returns>
    public IEnumerator IFlopEffect(int frontNum)
    {
        thisRt.rotation = Quaternion.Euler(0, 180, 0);
        Poker_Img.sprite = GameAssetsManager.Instance.GetPokerBack(0);

        float turnTime = 0.5f;
        DateTime startTime = DateTime.Now;
        while ((DateTime.Now - startTime).TotalSeconds < turnTime)
        {
            float progess = (float)(DateTime.Now - startTime).TotalSeconds / turnTime;
            float rotY = Mathf.Lerp(180, 0, progess);
            thisRt.rotation = Quaternion.Euler(0, rotY, 0);

            if (rotY <= 90)
            {
                Poker_Img.sprite = GameAssetsManager.Instance.GetPokerNum(frontNum);
            }

            yield return null;
        }

        thisRt.rotation = Quaternion.Euler(0, 0, 0);
    }
}
