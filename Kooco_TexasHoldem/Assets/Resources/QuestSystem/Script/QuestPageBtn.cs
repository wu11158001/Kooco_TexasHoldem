using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class QuestPageBtn : MonoBehaviour
{
    [Header("領取按鈕")]
    [SerializeField]
    Button Receive;

    [Header("已領取")]
    [SerializeField]
    GameObject Received;

    [Header("任務名稱和報酬")]
    [SerializeField]
    Text QuestName;
    [SerializeField]
    Text CoinAmount;

    [Header("任務進程")]
    [SerializeField]
    GameObject schedule;
    [SerializeField]
    Text QuestProgress;


    [Header("紅利Point Icon")]
    [SerializeField]
    Sprite Pointicon;
    [SerializeField]
    int BonusAmount;


    /// <summary>
    /// 設置任務訊息
    /// </summary>
    /// <param name="info">任物資料</param>
    public void SetQuestInfo(QuestInfo info)
    {
        QuestName.text = info.QuestName;
        CoinAmount.text = $"* {info.GetCoin}";
        QuestProgress.text = $"{info.CurrentProgress}/{info.FinishProgress}";

        BonusAmount = info.GetPoint;

        //  任務完成後領取獎勵
        Receive.onClick.AddListener(() =>
        {
            ReceiveActive(!Receive.gameObject.activeSelf);
            Received.SetActive(true);

            Debug.Log($"你領取了: {info.GetCoin} 籌碼");

            DataManager.CurrentBonusAmount += BonusAmount;
            GameObject.FindFirstObjectByType<QuestBonus>().SetBonus();          //  查詢場景QuestBonus並更新

            Debug.Log($"你獲得了: {BonusAmount} 點數");
        });
    }

    /// <summary>
    /// 獲取任務名稱
    /// </summary>
    /// <returns></returns>
    public string GetQuestName() { return QuestName.text; }
    
    /// <summary>
    /// 獲取任務進程
    /// </summary>
    /// <returns></returns>
    public string GetQuestProgress() { return QuestProgress.text; }

    /// <summary>
    /// 設置任務進程
    /// </summary>
    /// <param name="Progress"></param>
    public void SetQuestProgress(string Progress) { QuestProgress.text = Progress; }

    public void ReceiveActive(bool Active)
    {
        if(Receive != null)
            Receive.gameObject.SetActive(Active);
    }
}
