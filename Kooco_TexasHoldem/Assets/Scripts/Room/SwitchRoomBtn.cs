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

    public int BtnIndex { get; set; }

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
    /// <param name="btnStr"></param>
    /// <param name="roomName"></param>
    public void SetSwitchBtnInfo(string btnStr, string roomName, int btnIndex)
    {
        BtnIndex = btnIndex;
        roomName_Txt.text = btnStr;
        thisBtn.onClick.AddListener(() =>
        {
            GameRoomManager.Instance.SwitchBtnClick(btnIndex);
        });
    }
}
