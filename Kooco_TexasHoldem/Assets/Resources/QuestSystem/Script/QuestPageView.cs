using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestPageView : MonoBehaviour
{

    [Header("任務類別")]
    [SerializeField]
    public QuestEnum questEnum;

    [Header("任務Prefab")]
    [SerializeField]
    GameObject QuestPage;
    [SerializeField]
    RectTransform QuestParent;

    [Header("任務pageList")]
    [SerializeField]
    List<QuestPageBtn> QuestPageList;

    private void Awake()
    {
        switch (questEnum)
        {
            case QuestEnum.Daily:
                CreateQuest(DataManager.DailyQuestList);
                break;

            case QuestEnum.Weekly:
                CreateQuest(DataManager.WeekQuestList);
                break;
        }
    }

    private void Update()
    {

        //  更新任務測試
        if (Input.GetKeyDown(KeyCode.X))
        {
            UpdateQuestInfo("Cash Bet", 200);
        }
    }

    /// <summary>
    /// 創建任務介面
    /// </summary>
    /// <param name="QuestList">任務資料集</param>
    public void CreateQuest(List<QuestInfo> QuestList)
    {
        QuestPage.SetActive(false);
        float QuestSpacing = QuestParent.GetComponent<VerticalLayoutGroup>().spacing;
        Rect QuestRect = QuestPage.GetComponent<RectTransform>().rect;
        QuestParent.sizeDelta = new Vector2(QuestRect.width, (QuestRect.height + QuestSpacing) * QuestList.Count);

        foreach (var questInfo in QuestList)
        {
            RectTransform rect = Instantiate(QuestPage).GetComponent<RectTransform>();
            QuestPageList.Add(rect.GetComponent<QuestPageBtn>());
            rect.gameObject.SetActive(true);
            rect.SetParent(QuestParent);
            rect.GetComponent<QuestPageBtn>().SetQuestInfo(questInfo);
            rect.localScale = Vector3.one;
        }
    }

    /// <summary>
    /// 更新任務進程
    /// </summary>
    public void UpdateQuestInfo(string updateQuestName, int UpdateAmount)
    {
        foreach (var page in QuestPageList)
        {
            if (page.GetQuestName().Contains(updateQuestName))
            {
                string[] Progress = page.GetQuestProgress().Split("/");
                int num = Int32.Parse(Progress[0]);
                num += UpdateAmount;
                page.SetQuestProgress(num + "/" + Int32.Parse(Progress[1]));
                if (num >= Int32.Parse(Progress[1]))
                {
                    page.ReceiveActive(true);
                }

            }
        }
    }

}
