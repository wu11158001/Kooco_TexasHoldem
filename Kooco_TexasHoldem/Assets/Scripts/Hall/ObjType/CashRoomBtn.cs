using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using RequestBuf;

public class CashRoomBtn : MonoBehaviour
{
    [SerializeField]
    Button ThisRoom_Btn;
    [SerializeField]
    Text smallBlind_Txt, buyRange_Txt;

    /// <summary>
    /// 設定現金房間按鈕訊息
    /// </summary>
    /// <param name="smallBlind"></param>
    public void SetCashRoomBtnInfo(double smallBlind)
    {
        smallBlind_Txt.text = $"({StringUtils.SetChipsUnit(smallBlind)} / {StringUtils.SetChipsUnit(smallBlind * 2)})";
        buyRange_Txt.text = $"{StringUtils.SetChipsUnit(smallBlind * GameDataManager.MinMagnification)} ~ {StringUtils.SetChipsUnit(smallBlind * GameDataManager.MaxMagnification)}";

        ThisRoom_Btn.onClick.AddListener(() =>
        {
            BuyChipsPartsView buyChipsView = UIManager.Instance.OpenPartsView(ViewEnum.BuyChipsPartsView).GetComponent<BuyChipsPartsView>();
            buyChipsView.SetBuyChipsViewInfo(smallBlind, null);
        });
    }
}
