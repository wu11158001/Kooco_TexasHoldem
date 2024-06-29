using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class HandHistoryManager : UnitySingleton<HandHistoryManager>
{
    string PlayerPrefsKey;
    const int MaxSaveCount = 2;

    List<ResultHistoryData> dataList;



    public override void Awake()
    {
        PlayerPrefsKey = $"AsiaPoker_ResultHistoryDataList_{DataManager.UserId}";
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

        string json = PlayerPrefs.GetString(PlayerPrefsKey, "[]");
        dataList = JsonConvert.DeserializeObject<List<ResultHistoryData>>(json) ?? new List<ResultHistoryData>();

        Debug.Log("Load Result History Data");
    }

    /// <summary>
    /// 遊戲結果存檔
    /// </summary>
    /// <param name="newData"></param>
    public void SaveResult(ResultHistoryData newData)
    {
        if (dataList.Count >= MaxSaveCount)
        {
            //移除第一筆數據
            dataList.RemoveAt(0);
        }

        dataList.Add(newData);
        string json = JsonConvert.SerializeObject(dataList);
        PlayerPrefs.SetString(PlayerPrefsKey, json);
        PlayerPrefs.Save();
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
