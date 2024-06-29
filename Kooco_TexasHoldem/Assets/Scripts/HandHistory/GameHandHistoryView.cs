using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHandHistoryView : MonoBehaviour
{
    [SerializeField]
    Button Close_Btn;
    [SerializeField]
    GameObject HistorySampleObj;
    [SerializeField]
    Transform HistroyParent;

    private void Awake()
    {
        //關閉按鈕
        Close_Btn.onClick.AddListener(() =>
        {
            GameRoomManager.Instance.IsCanMoveSwitch = true;
            Destroy(gameObject);
        });
    }

    private void Start()
    {
        CreateHistory();
    }

    /// <summary>
    /// 創建結果紀錄
    /// </summary>
    private void CreateHistory()
    {
        HistorySampleObj.gameObject.SetActive(false);
        List<ResultHistoryData> resultDatas = HandHistoryManager.Instance.GetResultDataList();
        for (int i = resultDatas.Count - 1; i >= 0; i--)
        {
            HistorySample historySample = Instantiate(HistorySampleObj, HistroyParent).GetComponent<HistorySample>();
            historySample.gameObject.SetActive(true);
            historySample.SetData(resultDatas[i], i + 1);
        }
    }
}
