using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestInfo
{
    public string QuestName;        //任務名稱
    public int GetPoint;            //獲得點數
    public int FinishProgress;      //完成進度
    public int CurrentProgress;     //當前任務進度
    public double GetCoin;          //完成任務後獲取籌碼
    public bool Received;           //已領取任務
}

public enum QuestEnum
{
    Daily,
    Weekly
}