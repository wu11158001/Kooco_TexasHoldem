using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CashRoomBtn : MonoBehaviour
{
    [SerializeField]
    Button ThisRoom_Btn;
    [SerializeField]
    Text smallBlind_Txt, buyMinChips_Txt;

    /// <summary>
    /// 設定現金房間按鈕訊息
    /// </summary>
    /// <param name="smallBlind">小盲</param>
    /// <param name="lobbyView">大廳</param>
    public void SetCashRoomBtnInfo(double smallBlind, LobbyView lobbyView)
    {
        smallBlind_Txt.text = $"{StringUtils.SetChipsUnit(smallBlind)} / {StringUtils.SetChipsUnit(smallBlind * 2)}";
        buyMinChips_Txt.text = $"{StringUtils.SetChipsUnit(smallBlind * DataManager.MinMagnification)}";

        ThisRoom_Btn.onClick.AddListener(() =>
        {
            if (GameRoomManager.Instance.JudgeIsCanBeCreateRoom())
            {
                CreateCashRoomView createCashRoomView = ViewManager.Instance.OpenPartsView(PartsViewEnum.CreateCashRoomView).GetComponent<CreateCashRoomView>();
                createCashRoomView.SetCreatRoomViewInfo(smallBlind, null);
            }
            else
            {
                lobbyView.ShowMaxRoomTip();
            }            
        });
    }
}
