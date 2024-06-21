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
    GameObject QuestView;
    [SerializeField]
    RectTransform QuestParent;

    private void Start()
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

    /// <summary>
    /// 創建任務介面
    /// </summary>
    /// <param name="QuestList">任務資料集</param>
    public void CreateQuest(List<QuestInfo> QuestList)
    {
        //  每日任務
        QuestView.SetActive(false);
        float QuestSpacing = QuestParent.GetComponent<VerticalLayoutGroup>().spacing;
        Rect QuestRect = QuestView.GetComponent<RectTransform>().rect;
        QuestParent.sizeDelta = new Vector2(QuestRect.width, (QuestRect.height + QuestSpacing) * QuestList.Count);

        foreach (var questPage in QuestList)
        {
            RectTransform rect = Instantiate(QuestView).GetComponent<RectTransform>();
            rect.gameObject.SetActive(true);
            rect.SetParent(QuestParent);
            rect.GetComponent<QuestPageBtn>().SetQuestBtnInfo(questPage);
            rect.localScale = Vector3.one;
        }
    }
}
