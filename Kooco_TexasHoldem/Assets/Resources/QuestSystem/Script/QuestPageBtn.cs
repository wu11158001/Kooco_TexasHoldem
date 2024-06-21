using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestPageBtn : MonoBehaviour
{
    [SerializeField]
    Button Receive;
    [SerializeField]
    Text QuestName,CoinAmount;
    [SerializeField]
    Text QuestProgress;
    [SerializeField]
    Sprite Pointicon;

    public void SetQuestBtnInfo(QuestInfo info)
    {
        QuestName.text = info.QuestName;
        CoinAmount.text = $"{info.GetCoin}";
        QuestProgress.text = $"{info.CurrentProgress}/{info.FinishProgress}";

    }


}
