using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TipMsgView : MonoBehaviour
{
    [SerializeField]
    Image Bg_Img;
    [SerializeField]
    TextMeshProUGUI Msg_Txt;

    const float GradientTime = 0.5f;            //漸變時間
    const float ShowTime = 1f;                  //顯示時間
    const float moveSpeed = 20;                 //移動速度

    float currPosY;                             //當前位置Y

    private void FixedUpdate()
    {
        currPosY += Time.deltaTime * moveSpeed;
        Bg_Img.rectTransform.anchoredPosition = new Vector2(0, currPosY);
    }

    /// <summary>
    /// 設置提示訊息
    /// </summary>
    /// <param name="msg">訊息內容</param>
    public void SetTipMsg(string msg)
    {
        currPosY = 0;
        Msg_Txt.text = msg;

        StartCoroutine(IEffect());
    }

    private IEnumerator IEffect()
    {
        Color bgColor = Bg_Img.color;
        Color msgColor = Msg_Txt.color;

        //漸變顯示
        DateTime startTime = DateTime.Now;
        while ((DateTime.Now - startTime).TotalSeconds < GradientTime)
        {
            float progress = (float)(DateTime.Now - startTime).TotalSeconds / GradientTime;
            float alpha = Mathf.Lerp(0, 1, progress);

            bgColor.a = alpha;
            msgColor.a = alpha;
            Bg_Img.color = bgColor;
            Msg_Txt.color = msgColor;

            yield return null;
        }

        //持續時間
        bgColor.a = 1;
        msgColor.a = 1;
        Bg_Img.color = bgColor;
        Msg_Txt.color = msgColor;
        yield return new WaitForSeconds(ShowTime);

        //漸變關閉
        startTime = DateTime.Now;
        while ((DateTime.Now - startTime).TotalSeconds < GradientTime)
        {
            float progress = (float)(DateTime.Now - startTime).TotalSeconds / GradientTime;
            float alpha = Mathf.Lerp(1, 0, progress);

            bgColor.a = alpha;
            msgColor.a = alpha;
            Bg_Img.color = bgColor;
            Msg_Txt.color = msgColor;

            yield return null;
        }

        bgColor.a = 0;
        msgColor.a = 0;
        Bg_Img.color = bgColor;
        Msg_Txt.color = msgColor;

        Destroy(gameObject);
    }
}
