using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Poker : MonoBehaviour
{
    [SerializeField]
    RectTransform thisRt;
    [SerializeField]
    Image poker_Img, frame_Img, Pattern_Img, SmallPattern_Img;
    [SerializeField]
    GameObject shining_Obj, PokerBack_Obj;
    [SerializeField]
    TextMeshProUGUI Num_Txt;

    int pokerNumber;    //撲克數字(-1=背面)

    private void OnEnable()
    {
        PokerInit();
    }

    /// <summary>
    /// 撲克初始化
    /// </summary>
    public void PokerInit()
    {
        PokerEffectEnable = false;
        shining_Obj.SetActive(false);
    }

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
            PokerBack_Obj.SetActive(value <= -1);
            if (value >= 0)
            {
                //數字
                string suitsStr = "";
                int num = value % 13 + 1;

                switch (num)
                {
                    case 1:
                        suitsStr = "A";
                        break;

                    case 11:
                        suitsStr = "J";
                        break;

                    case 12:
                        suitsStr = "Q";
                        break;

                    case 13:
                        suitsStr = "K";
                        break;

                    default:
                        suitsStr = num.ToString(); ;
                        break;
                }
                Num_Txt.text = suitsStr;

                //花色
                int suitsImg = value / 13;
                Pattern_Img.sprite = AssetsManager.Instance.GetAlbumAsset(AlbumEnum.PokerSuitsAlbum).album[suitsImg];
                SmallPattern_Img.sprite = AssetsManager.Instance.GetAlbumAsset(AlbumEnum.PokerSuitsAlbum).album[suitsImg];
                if (suitsImg == 0 ||
                    suitsImg == 3)
                {
                    Num_Txt.color = Color.black;
                }
                else
                {
                    Num_Txt.color = Color.red;
                }

            }          
        }
    }

    /// <summary>
    /// 撲克效果激活
    /// </summary>
    public bool PokerEffectEnable
    {
        set
        {
            frame_Img.enabled = false;
            shining_Obj.SetActive(value);
            thisRt.localScale = Vector3.one;

            /*frame_Img.enabled = value;
            poker_Img.rectTransform.localScale = Vector3.one;
            if (value == false)
            {
                shining_Obj.SetActive(false);
            }*/
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

    /// <summary>
    /// 播放贏家效果
    /// </summary>
    public void StartWinEffect()
    {
        shining_Obj.SetActive(true);
        thisRt.localScale = new Vector3(1.1f, 1.1f, 1);
    }

    /// <summary>
    /// 水平翻牌效果
    /// </summary>
    /// <param name="frontNum">正面數字</param>
    /// <returns></returns>
    public IEnumerator IHorizontalFlopEffect(int frontNum)
    {
        poker_Img.rectTransform.rotation = Quaternion.Euler(poker_Img.rectTransform.eulerAngles.x, 180, poker_Img.rectTransform.eulerAngles.z);
        PokerBack_Obj.SetActive(true);

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
