using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MonoBehaviourUtils : UnitySingleton<MonoBehaviourUtils>
{
    public override void Awake()
    {
        base.Awake();
    }

    Coroutine fadeCorotine;
    /// <summary>
    /// 物件顏色淡入淡出
    /// </summary>
    /// <param name="text">文字元件</param>
    /// <param name="image">圖片元件</param>
    /// <param name="fadeInTime">淡入時間</param>
    /// <param name="fadeOutTime">淡出時間</param>
    /// <param name="during">完整顯示時間</param>
    public void ColorFade(Text text, Image image, float fadeInTime, float fadeOutTime, float during)
    {
        if (fadeCorotine != null)
        {
            StopCoroutine(fadeCorotine);
        }

        fadeCorotine = StartCoroutine(IColorFade(text,
                                                 image,
                                                 fadeInTime,
                                                 fadeOutTime,
                                                 during));
    }
    private IEnumerator IColorFade(Text text, Image image, float fadeInTime, float fadeOutTime, float during)
    {
        Color textColor = new Color();
        Color imageColor = new Color();
        if (text != null)
        {
            textColor = text.color;
        }
        if (image)
        {
            imageColor = image.color;
        }

        DateTime startTime = DateTime.Now;
        //淡入
        while ((DateTime.Now - startTime).TotalSeconds < fadeInTime)
        {
            float progress = (float)(DateTime.Now - startTime).TotalSeconds / fadeInTime;
            float alpha = Mathf.Lerp(0, 1, progress);

            if (text != null)
            {
                textColor.a = alpha;
                text.color = textColor;
            }
            if (image)
            {
                imageColor.a = alpha;
                image.color = imageColor;
            }

            yield return null;
        }

        yield return new WaitForSeconds(during);

        startTime = DateTime.Now;
        //淡出
        while ((DateTime.Now - startTime).TotalSeconds < fadeOutTime)
        {
            float progress = (float)(DateTime.Now - startTime).TotalSeconds / fadeOutTime;
            float alpha = Mathf.Lerp(1, 0, progress);

            if (text != null)
            {
                textColor.a = alpha;
                text.color = textColor;
            }
            if (image)
            {
                imageColor.a = alpha;
                image.color = imageColor;
            }

            yield return null;
        }

        if (text != null)
        {
            textColor.a = 0;
            text.color = textColor;
        }
        if (image)
        {
            imageColor.a = 0;
            image.color = imageColor;
        }
    }
}
