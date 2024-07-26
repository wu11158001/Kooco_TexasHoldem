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
    public List<QuestPageBtn> QuestPageList;

    private void Awake()
    {
        switch (questEnum)
        {
            case QuestEnum.Daily:
                CreateQuest(DataManager.DailyQuestList);
                break;

            case QuestEnum.Weekly:
                CreateQuest(DataManager.WeeklyQuestList);
                break;
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
}
