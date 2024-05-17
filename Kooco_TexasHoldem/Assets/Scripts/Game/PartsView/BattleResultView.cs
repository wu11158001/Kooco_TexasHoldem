using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleResultView : MonoBehaviour
{
    [SerializeField]
    Text result_Txt;
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
        result_Txt.text = isWin ? "Win" : "Fail";
    }
}
