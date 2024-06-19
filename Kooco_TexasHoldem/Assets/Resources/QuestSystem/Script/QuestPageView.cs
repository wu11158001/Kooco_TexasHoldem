using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestPageView : MonoBehaviour
{
    [Header("¥ô°ÈPrefab")]
    [SerializeField]
    GameObject QuestView;
    [SerializeField]
    RectTransform QuestParent;


    private void Start()
    {
        CreateQuest();
    }

    public void CreateQuest()
    {
        float DailySpacing = QuestParent.GetComponent<VerticalLayoutGroup>().spacing;
        Rect DailyRect = QuestView.GetComponent<RectTransform>().rect;
        QuestParent.sizeDelta = new Vector2(DailyRect.width, (DailyRect.height + DailySpacing) * 8);

        for (int i=0;i<8;i++)
        {
            RectTransform rect = Instantiate(QuestView).GetComponent<RectTransform>();
            rect.gameObject.SetActive(true);
            rect.SetParent(QuestParent);
            rect.GetComponent<QuestPageBtn>().SetQuestBtnInfo();
            rect.localScale = Vector3.one;
        }
    }
}
