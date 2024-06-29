using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class HandHistoryManager : UnitySingleton<HandHistoryManager>
{
    private string filePath;
    private List<ResultHistoryData> dataList;

    public override void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, "AsiaPoker_ResultHistoryData.json");
        LoadResultData();

        base.Awake();
    }

    #region 遊戲結果存檔

    /// <summary>
    /// 獲取遊戲結果存檔
    /// </summary>
    /// <returns></returns>
    public List<ResultHistoryData> GetResultDataList()
    {
        return dataList;
    }

    /// <summary>
    /// 讀取結果存檔
    /// </summary>
    private void LoadResultData()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            dataList = JsonConvert.DeserializeObject<List<ResultHistoryData>>(json) ?? new List<ResultHistoryData>();
        }
        else
        {
            dataList = new List<ResultHistoryData>();
        }

        Debug.Log("Load Result History Data");
    }

    /// <summary>
    /// 遊戲結果存檔
    /// </summary>
    /// <param name="newData"></param>
    public void SaveResult(ResultHistoryData newData)
    {
        if (dataList.Count >= 20)
        {
            dataList.RemoveAt(0); // 移除第一筆數據
        }

        dataList.Add(newData);
        string json = JsonConvert.SerializeObject(dataList, Formatting.Indented);
        File.WriteAllText(filePath, json);

        Debug.Log("Save Reult History Data");
    }

    #endregion
}

/// <summary>
/// 紀錄結果資料
/// </summary>
public class ResultHistoryData
{
    public string TypeAndBlindStr;                  //房間類型與盲注
    public string NickName;                         //獲勝玩家暱稱
    public int Avatar;                              //獲勝玩家頭像
    public int[] HandPokers;                        //獲勝玩家手牌
    public List<int> CommunityPoker;                //公共牌
    public double WinChips;                         //贏得籌碼
}
