using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleResultView : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI result_Txt, confirmBtn_Txt;
    [SerializeField]
    Button confirm_Btn;

    private ThidData thidData;
    public class ThidData
    {
        public string RoomName;
    }

    private void Awake()
    {
        thidData = new ThidData();
        ListenerEvent();
    }

    /// <summary>
    /// 事件聆聽
    /// </summary>
    private void ListenerEvent()
    {
        //確認
        confirm_Btn.onClick.AddListener(() =>
        {
            GameRoomManager.Instance.RemoveGameRoom(thidData.RoomName);
        });
    }

    /// <summary>
    /// 設定積分結果
    /// </summary>
    /// <param name="isWin"></param>
    /// <param name="roomName"></param>
    public void OnSetBattleResult(bool isWin, string roomName)
    {
        thidData.RoomName = roomName;
        string resultStr = isWin ? "Win" : "Fail";
        result_Txt.text = resultStr;
        confirmBtn_Txt.text = "Confirm";
    }
}
