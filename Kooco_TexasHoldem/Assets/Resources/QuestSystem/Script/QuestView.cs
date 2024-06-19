using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class QuestView : MonoBehaviour
{
    [Header("關閉Floo4")]
    [SerializeField]
    Button CloseQuest;

    [Header("每日任務")]
    [SerializeField]
    GameObject DailyQuestView;
    [SerializeField]
    RectTransform DailyQuestParent;
    [SerializeField]
    Button DailyBtn;

    [Header("每周任務")]
    [SerializeField]
    GameObject WeeklyQuestView;
    [SerializeField]
    RectTransform WeeklyQuestParent;
    [SerializeField]
    Button WeeklyBtn;

    private void Awake()
    {
        DailyBtn.onClick.AddListener(() =>
        {
            if (!DailyQuestParent.gameObject.activeSelf)
            {
                DailyQuestParent.gameObject.SetActive(true);
                WeeklyQuestParent.gameObject.SetActive(false);
            }
            
        });

        WeeklyBtn.onClick.AddListener(() =>
        {
            if (!WeeklyQuestParent.gameObject.activeSelf)
            {
                DailyQuestParent.gameObject.SetActive(false);
                WeeklyQuestParent.gameObject.SetActive(true);
            }
        });

        CloseQuest.onClick.AddListener(() =>
        {
            if (transform.parent.gameObject.activeSelf)
                transform.parent.gameObject.SetActive(!transform.parent.gameObject.activeSelf);
        });


    }
    private void Start()
    {
        WeeklyQuestParent.gameObject.SetActive(false);

        CreateQuestView();
    }
    public void CreateQuestView()
    {
        
        RectTransform Dailyrect = Instantiate(DailyQuestView, DailyQuestParent).GetComponent<RectTransform>();
        Dailyrect.gameObject.SetActive(true);

        RectTransform Weeklyrect = Instantiate(WeeklyQuestView, WeeklyQuestParent).GetComponent<RectTransform>();
        Weeklyrect.gameObject.SetActive(true);
    }

    
}
