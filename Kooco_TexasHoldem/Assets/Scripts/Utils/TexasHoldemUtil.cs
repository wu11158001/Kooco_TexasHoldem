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
    public static void SetBuySlider(double smallBlind, Slider sli)
    {
        sli.minValue = (float)smallBlind * GameDataManager.MinMagnification;
        sli.maxValue = (float)smallBlind * GameDataManager.MaxMagnification;
        sli.value = sli.minValue;
    }
}
