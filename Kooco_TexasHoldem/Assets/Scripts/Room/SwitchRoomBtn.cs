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
    /// <param name="btnIndex">按鈕編號</param>
    /// <param name="controlTr"></param>
    /// <param name="roomName"></param>
    public void SetSwitchBtnInfo(int btnIndex, string roomName)
    {
        roomName_Txt.text = roomName;
        thisBtn.onClick.AddListener(() =>
        {
            GameRoomManager.Instance.CurrRoomIndex = btnIndex;
            GameRoomManager.Instance.ChangeRoom(btnIndex);
            SetSelectFrameActive = true;
        });
    }
}
