using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Amazon.Lambda.Model;

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

    [Header("Button滑塊")]
    [SerializeField]
    GameObject MaskObject;
    TextMeshProUGUI MaskText;

    [Header("領取完成顯示遮罩")]
    [SerializeField]
    GameObject ReceivedMask;

    private void Awake()
    {
        MaskText = MaskObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        DailyBtn.onClick.AddListener(() =>
        {
            if (!DailyQuestParent.gameObject.activeSelf)
            {
                DailyQuestParent.gameObject.SetActive(true);
                WeeklyQuestParent.gameObject.SetActive(false);

                MaskMove(DailyQuestParent.parent.gameObject);
            }
        });

        WeeklyBtn.onClick.AddListener(() =>
        {
            if (!WeeklyQuestParent.gameObject.activeSelf)
            {
                DailyQuestParent.gameObject.SetActive(false);
                WeeklyQuestParent.gameObject.SetActive(true);

                MaskMove(WeeklyQuestParent.parent.gameObject);
            }
        });

        CloseQuest.onClick.AddListener(() =>
        {
            Destroy(this.gameObject);
        });

    }
    private void Start()
    {
        WeeklyQuestParent.gameObject.SetActive(false);

        CreateQuestView();
    }

    private void Update()
    {
        //  更新任務測試
        if (Input.GetKeyDown(KeyCode.X))
        {
            UpdateQuestInfo("Cash Bet", 200);
        }
    }

    public void CreateQuestView()
    {
        
        RectTransform Dailyrect = Instantiate(DailyQuestView, DailyQuestParent).GetComponent<RectTransform>();
        Dailyrect.gameObject.SetActive(true);

        RectTransform Weeklyrect = Instantiate(WeeklyQuestView, WeeklyQuestParent).GetComponent<RectTransform>();
        Weeklyrect.gameObject.SetActive(true);
    }

    /// <summary>
    /// 滑塊遮罩移動
    /// </summary>
    /// <param name="QuestMask">滑塊物件</param>
    public void MaskMove(GameObject QuestMask)
    {
        RectTransform Maskrect = MaskObject.GetComponent<RectTransform>();
        RectTransform QuestMaskRect = QuestMask.GetComponent<RectTransform>();

        Maskrect.localPosition = QuestMaskRect.localPosition;
        MaskText.text = QuestMask.name;
    }

    /// <summary>
    /// 更新任務測試
    /// </summary>
    public void UpdateQuestInfo(string updateQuestName, int UpdateAmount)
    {
        foreach (var QuestInfo in DataManager.DailyQuestList)
        {
            if (QuestInfo.QuestName.Contains(updateQuestName))
            {
                QuestInfo.CurrentProgress += UpdateAmount;
            }
        }

        foreach (var QuestInfo in DataManager.WeeklyQuestList)
        {
            if (QuestInfo.QuestName.Contains(updateQuestName))
            {
                QuestInfo.CurrentProgress += UpdateAmount;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void ReceiveMaskActive()
    {
        ReceivedMask.SetActive(!ReceivedMask.activeSelf);
    }

}
