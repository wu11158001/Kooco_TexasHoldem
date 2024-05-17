using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchRoomBtn : MonoBehaviour
{
    [SerializeField]
    Button thisBtn;
    [SerializeField]
    Text roomName_Txt;
    [SerializeField]
    RectTransform selectFrame_Tr;

    /// <summary>
    /// 設置選擇按鈕激活狀態
    /// </summary>
    public bool SetSelectFrameActive
    {
        set
        {
            selectFrame_Tr.gameObject.SetActive(value);
        }
    }

    /// <summary>
    /// 設置切換房間按鈕訊息
    /// </summary>
    /// <param name="controlTr"></param>
    /// <param name="roomName"></param>
    public void SetSwitchBtnInfo(RectTransform controlTr, string roomName)
    {
        roomName_Txt.text = roomName;
        thisBtn.onClick.AddListener(() =>
        {
            controlTr.SetSiblingIndex(GameRoomManager.Instance.GetRoomCount + 1);
            GameRoomManager.Instance.CloseAllBtnFrame();
            GameRoomManager.Instance.IsShowGameRoom = true;
            SetSelectFrameActive = true;

            Entry.CurrGameServer = controlTr.GetComponent<GameServer>();
        });
    }
}
