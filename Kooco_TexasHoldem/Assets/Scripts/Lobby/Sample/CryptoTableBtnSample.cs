using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CryptoTableBtnSample : MonoBehaviour
{
    [SerializeField]
    Button Launch_Btn;
    [SerializeField]
    TextMeshProUGUI Blinds_Txt, MinBuy_Txt;
    [SerializeField]
    GameObject JoinRoomViewObj;

    /// <summary>
    /// 設定加密貨幣桌按鈕訊息
    /// </summary>
    /// <param name="smallBlind">小盲</param>
    /// <param name="lobbyView">大廳</param>
    public void SetCryptoTableBtnInfo(double smallBlind, LobbyView lobbyView)
    {
        Blinds_Txt.text = $"{StringUtils.SetChipsUnit(smallBlind)} / {StringUtils.SetChipsUnit(smallBlind * 2)}";
        MinBuy_Txt.text = $"{StringUtils.SetChipsUnit(smallBlind * DataManager.MinMagnification)}";

        Launch_Btn.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayConfirmClick();
            if (GameRoomManager.Instance.JudgeIsCanBeCreateRoom())
            {
                JoinRoomView joinRoomView = ViewManager.Instance.CreateViewInMainCanvas<JoinRoomView>(JoinRoomViewObj);
                joinRoomView.SetCreatRoomViewInfo(TableTypeEnum.CryptoTable, smallBlind);
            }
            else
            {
                lobbyView.ShowMaxRoomTip();
            }            
        });
    }
}
