using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class TexasHoldemUtil
{
    /// <summary>
    /// 特殊排序(0最大)
    /// </summary>
    public class SpecialComparer : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            if (x == 0 && y != 0)
                return -1;
            else if (x != 0 && y == 0)
                return 1;
            else if (x == 0 && y == 0)
                return 0;
            else
                return y.CompareTo(x);
        }
    }

    /// <summary>
    /// 設置購買籌碼Slider
    /// </summary>
    /// <param name="smallBlind">小盲值</param>
    /// <param name="sli">Slider</param>
    public static void SetBuySlider(double smallBlind, Slider sli, TableTypeEnum tableTypeEnum)
    {
        float maxValue = 0;
        switch (tableTypeEnum)
        {
            //加密貨幣桌
            case TableTypeEnum.CryptoTable:
                maxValue = (float)DataManager.UserCryptoChips;
                break;

            //虛擬貨幣桌
            case TableTypeEnum.VCTable:
                maxValue = (float)DataManager.UserVCChips;
                break;
        }

        sli.minValue = (float)smallBlind * DataManager.MinMagnification;
        sli.maxValue = maxValue;
        sli.value = sli.minValue;
    }

    /// <summary>
    /// Slider變化
    /// </summary>
    /// <param name="sli">Slider物件</param>
    /// <param name="currValue">當前滑條值</param>
    /// <param name="stepSize">每單位編更值</param>
    /// <param name="minValue">最小值</param>
    /// <param name="maxValue">最大值</param>
    /// <param name="sliderClickDetection">Slider點擊判斷</param>
    /// <returns></returns>
    public static float SliderValueChange(Slider sli, float currValue, float stepSize, float minValue, float maxValue, SliderClickDetection sliderClickDetection = null)
    {
        float newRaiseValue = sliderClickDetection != null && sliderClickDetection.GetSkiderClicked ? 
                              Mathf.Round(currValue / stepSize) * stepSize : 
                              currValue;

        if (sli.value <= minValue)
        {
            sli.value = minValue;
            return minValue;
        }

        if (newRaiseValue >= sli.maxValue && sli.value < sli.maxValue)
        {
            newRaiseValue -= stepSize;
        }

        if (newRaiseValue >= maxValue)
        {
            newRaiseValue = maxValue;
        }

        return newRaiseValue;
    }
}
