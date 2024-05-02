using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinChips : MonoBehaviour
{
    [SerializeField]
    Text winChip_Txt;

    /// <summary>
    /// 設置贏的籌碼數量
    /// </summary>
    public double SetWinChips
    {
        set
        {
            winChip_Txt.text = $"${StringUtils.SetChipsUnit(value)}";
        }
    }
}
