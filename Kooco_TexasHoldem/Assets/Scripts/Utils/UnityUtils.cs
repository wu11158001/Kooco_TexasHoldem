using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.Events;

public class UnityUtils : UnitySingleton<UnityUtils>
{
    public override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// 介面滑進滑出
    /// </summary>
    /// <param name="isSlideIn">滑進/滑出</param>
    /// <param name="rt">移動介面</param>
    /// <param name="startDirection">滑動起始方向</param>
    /// <param name="moveTime">滑動時間</param>
    /// <param name="endAction">滑動完成執行方法</param>
    /// <returns></returns>
    public IEnumerator IViewSlide(bool isSlideIn, RectTransform rt, DirectionEnum startDirection, float moveTime, UnityAction endAction = null)
    {
        rt.gameObject.SetActive(true);

        if (isSlideIn == true)
        {
            switch (startDirection)
            {
                case DirectionEnum.Up:
                    rt.anchoredPosition = new Vector2(0, rt.rect.height);
                    break;
                case DirectionEnum.Left:
                    rt.anchoredPosition = new Vector2(-rt.rect.width, 0);
                    break;
                case DirectionEnum.Right:
                    rt.anchoredPosition = new Vector2(rt.rect.width, 0);
                    break;
                case DirectionEnum.Down:
                    rt.anchoredPosition = new Vector2(0, -rt.rect.height);
                    break;
            }            
        }

        //起始位置
        float start = 0;
        switch (startDirection)
        {
            case DirectionEnum.Up:
                start = rt.anchoredPosition.y;
                break;
            case DirectionEnum.Left:
                start = rt.anchoredPosition.x;
                break;
            case DirectionEnum.Right:
                start = rt.anchoredPosition.x;
                break;
            case DirectionEnum.Down:
                start = rt.anchoredPosition.y;
                break;

        }

        //目標位置
        float target = 0;
        if (!isSlideIn)
        {
            switch (startDirection)
            {
                case DirectionEnum.Up:
                    target = rt.rect.height;
                    break;
                case DirectionEnum.Left:
                    target = -rt.rect.width;
                    break;
                case DirectionEnum.Right:
                    target = rt.rect.width;
                    break;
                case DirectionEnum.Down:
                    target = -rt.rect.height;
                    break;
                default:
                    break;
            }
        }

        //介面滑動
        DateTime startTime = DateTime.Now;
        while ((DateTime.Now - startTime).TotalSeconds < moveTime)
        {
            float progress = (float)(DateTime.Now - startTime).TotalSeconds / moveTime;
            float pos = Mathf.Lerp(start, target, progress);

            switch (startDirection)
            {
                case DirectionEnum.Up:
                    rt.anchoredPosition = new Vector2(0, pos);
                    break;
                case DirectionEnum.Left:
                    rt.anchoredPosition = new Vector2(pos, 0);
                    break;
                case DirectionEnum.Right:
                    rt.anchoredPosition = new Vector2(pos, 0);
                    break;
                case DirectionEnum.Down:
                    rt.anchoredPosition = new Vector2(0, pos);
                    break;
            }
            
            yield return null;
        }

        //移動結束設定置目標位置
        switch (startDirection)
        {
            case DirectionEnum.Up:
                rt.anchoredPosition = new Vector2(0, target);
                break;
            case DirectionEnum.Left:
                rt.anchoredPosition = new Vector2(target, 0);
                break;
            case DirectionEnum.Right:
                rt.anchoredPosition = new Vector2(target, 0);
                break;
            case DirectionEnum.Down:
                rt.anchoredPosition = new Vector2(0, target);
                break;
        }

        endAction?.Invoke();

        rt.gameObject.SetActive(isSlideIn);
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
    public void ColorFade(TextMeshProUGUI text, Image image, float fadeInTime, float fadeOutTime, float during)
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
    private IEnumerator IColorFade(TextMeshProUGUI text, Image image, float fadeInTime, float fadeOutTime, float during)
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
