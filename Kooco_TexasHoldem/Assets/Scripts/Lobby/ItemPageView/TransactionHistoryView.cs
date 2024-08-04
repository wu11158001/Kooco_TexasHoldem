using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using UnityEngine.Events;
using TMPro;

/// <summary>
/// 交易紀錄資料類
/// </summary>
public class TransactionHistoryData
{
    public string type;
    public string time;
    public string title1;
    public string title2;
    public int status;
    public string pl;
}

public class TransactionHistoryView : MonoBehaviour
{
    [Header("Mask")]
    [SerializeField]
    Mask Viewport;

    [Header("交易紀錄顯示")]
    [SerializeField]
    Button Back_Btn, LeftTurnPage_Btn, RightTurnPage_Btn;
    [SerializeField]
    GameObject TransactionHistorySampleObj;
    [SerializeField]
    Transform HistoryArea;
    [SerializeField]
    Toggle UCoin_Tog, ACoin_Tog, Gold_Tog;
    [SerializeField]
    Image Tip_Img;
    [SerializeField]
    TextMeshProUGUI Title_Txt, Tip_Txt,
                    UCoinTog_Txt, ACoinTog_Txt, GoldTog_Txt,
                    ItemType_Txt, ItemTitle_Txt, ItemStatus_Txt, ItemPL_Txt,
                    CurrPage_Txt;

    [Header("交易紀錄篩選")]
    [SerializeField]
    Button Filter_Btn, FilterMask_Btn, AllTypeExpand_Btn, AllStatusExpand_Btn, FilterSubmit_Btn;
    [SerializeField]
    RectTransform Filter_Rt, FilterBg_Tr, FilterLayout_Tr, AllType_Obj, AllStatus_Obj;
    [SerializeField]
    Image AllTypeExpand_Img, AllStatusExpand_Img;
    [SerializeField]
    TextMeshProUGUI SetDateRangeTitle_Txt, DateRange_Txt, FilterSubmitBtn_Txt;

    [Header("日期範圍設置")]
    [SerializeField]
    RectTransform Filter_Sr;
    [SerializeField]
    Button CloseSetDate_Btn, DateRange_Btn, SetDataRangeSubmit_Btn;
    [SerializeField]
    GameObject SetDateRange_Obj;
    [SerializeField]
    TMP_Dropdown StartFilterYear_Dd, StartFilterMonth_Dd, StartFilterDay_Dd,
                 EndFilterYear_Dd, EndFilterMonth_Dd, EndFilterDay_Dd;
    [SerializeField]
    TextMeshProUGUI StartFilterDateTitle_Txt, EndFilterDateTitle_Txt, SetDataRangeSubmitBtn_Txt;

    [Header("所有類型篩選")]
    [SerializeField]
    Toggle TypeBuyIn_Tog, TypeAnte_Tog, TypeAll_Tog;
    [SerializeField]
    TextMeshProUGUI AllTypeTitle_Txt, TypeBuyInTog_Txt, TypeAnteTog_Txt, TypeAllTog_Txt;

    [Header("所有狀態")]
    [SerializeField]
    Toggle ProcessingStatus_Tog, SuccessStatus_Tog, FailStatus_Tog, AllStatus_Tog;
    [SerializeField]
    TextMeshProUGUI AllStatusTitle_Txt, ProcessingStatusTog_Txt, SuccessStatusTog_Txt,
                    FailStatusTog_Txt, AllStatusTog_Txt;

    const string expandContentName = "Content";                                 //展開內容物件名稱
    const string expandTopBgName = "TopBg";                                     //收起上方物件名稱
    const float expandTIme = 0.1f;                                              //內容展開時間

    List<TransactionHistoryData> uCoinDataList;
    List<TransactionHistoryData> aCoinDataList;
    List<TransactionHistoryData> goldDataList;
    List<TransactionHistoryData> displayDataList;
    int currPage;

    bool isAllTypeExpand;                                                       //是否展開所有類型
    bool isAllSatusExpand;                                                      //是否展開所有狀態

    DateTime currFilterStartDate;                                               //當前篩選起始日期
    DateTime currFilterEndDate;                                                 //當前篩選結束日期
    TypeEnum currFilterType;                                                    //當前篩選交易類型
    StatusEnum currFilterStatus;                                                //當前篩選交易狀態

    /// <summary>
    /// 篩選交易類型
    /// </summary>
    enum TypeEnum
    {
        BuyIn,
        Ante,
        All,
    }

    /// <summary>
    /// 篩選交易狀態
    /// </summary>
    enum StatusEnum
    {
        Processing,
        Success,
        Fail,
        All,
    }

    /// <summary>
    /// 更新文本翻譯
    /// </summary>
    private void UpdateLanguage()
    {
        #region 交易紀錄顯示

        Tip_Txt.text = LanguageManager.Instance.GetText("Only Show Transactions From The Last 3 Months.");
        StringUtils.TextInFrontOfImageFollow(Tip_Txt,
                                     Tip_Img);
        Title_Txt.text = LanguageManager.Instance.GetText("TRANSACTION HISTORY");        
        UCoinTog_Txt.text = LanguageManager.Instance.GetText("U COIN");
        ACoinTog_Txt.text = LanguageManager.Instance.GetText("A COIN");
        GoldTog_Txt.text = LanguageManager.Instance.GetText("GOLD");
        ItemType_Txt.text = LanguageManager.Instance.GetText("Type");
        ItemTitle_Txt.text = LanguageManager.Instance.GetText("Title");
        ItemStatus_Txt.text = LanguageManager.Instance.GetText("Status");
        ItemPL_Txt.text = LanguageManager.Instance.GetText("P&L");

        #endregion

        #region 交易紀錄篩選

        SetDateRangeTitle_Txt.text = LanguageManager.Instance.GetText("Date Range");
        StartFilterDateTitle_Txt.text = LanguageManager.Instance.GetText("Start Date");
        EndFilterDateTitle_Txt.text = LanguageManager.Instance.GetText("End Date");
        SetDataRangeSubmitBtn_Txt.text = LanguageManager.Instance.GetText("SUBMIT");
        FilterSubmitBtn_Txt.text = LanguageManager.Instance.GetText("SUBMIT");

        #endregion

        #region

        AllTypeTitle_Txt.text = LanguageManager.Instance.GetText("All Type");
        TypeBuyInTog_Txt.text = LanguageManager.Instance.GetText("BUY-IN");
        TypeAnteTog_Txt.text = LanguageManager.Instance.GetText("ANTE");
        TypeAllTog_Txt.text = LanguageManager.Instance.GetText("ALL");

        #endregion

        #region 所有狀態

        AllStatusTitle_Txt.text = LanguageManager.Instance.GetText("All Status");
        ProcessingStatusTog_Txt.text = LanguageManager.Instance.GetText("PROCESSING");
        SuccessStatusTog_Txt.text = LanguageManager.Instance.GetText("SUCCESS");
        FailStatusTog_Txt.text = LanguageManager.Instance.GetText("FAIL");
        AllStatusTog_Txt.text = LanguageManager.Instance.GetText("ALL");

        #endregion
    }

    private void OnDestroy()
    {
        LanguageManager.Instance.RemoveLanguageFun(UpdateLanguage);
    }

    private void Awake()
    {
        LanguageManager.Instance.AddUpdateLanguageFunc(UpdateLanguage, gameObject);
        ListenerEvent();
    }

    /// <summary>
    /// 事件聆聽
    /// </summary>
    private void ListenerEvent()
    {
        //返回
        Back_Btn.onClick.AddListener(() =>
        {
            Destroy(gameObject);
        });

        #region 交易紀錄顯示

        //翻頁(左)
        LeftTurnPage_Btn.onClick.AddListener(() =>
        {
            currPage--;
            DisplayHistory(displayDataList, currPage);
        });

        //翻頁(右)
        RightTurnPage_Btn.onClick.AddListener(() =>
        {
            currPage++;
            DisplayHistory(displayDataList, currPage);
        });

        //U幣紀錄
        UCoin_Tog.onValueChanged.AddListener((isOn) =>
        {
            if (isOn)
            {
                displayDataList = uCoinDataList;
                currPage = 1;
                DisplayHistory(displayDataList, currPage);
            }
        });

        //U幣紀錄
        ACoin_Tog.onValueChanged.AddListener((isOn) =>
        {
            if (isOn)
            {
                displayDataList = aCoinDataList;
                currPage = 1;
                DisplayHistory(displayDataList, currPage);
            }
        });

        //Gold紀錄
        Gold_Tog.onValueChanged.AddListener((isOn) =>
        {
            if (isOn)
            {
                displayDataList = goldDataList;
                currPage = 1;
                DisplayHistory(displayDataList, currPage);
            }
        });

        #endregion

        #region 交易紀錄篩選

        //開啟篩選頁面
        Filter_Btn.onClick.AddListener(() =>
        {
            Filter_Btn.interactable = false;
            Filter_Rt.gameObject.SetActive(true);

            //紀錄展開物件初始化
            List<RectTransform> expandObjList = new()
            {
                AllType_Obj,            //所有類型
                AllStatus_Obj,          //所有狀態
            };
            foreach (var expandObj in expandObjList)
            {
                RectTransform contentObj = expandObj.Find(expandContentName).GetComponent<RectTransform>();
                RectTransform TopBgObj = expandObj.Find(expandTopBgName).GetComponent<RectTransform>();
                contentObj.gameObject.SetActive(false);
                expandObj.sizeDelta = new Vector2(expandObj.rect.width, TopBgObj.rect.height);
            }
            isAllTypeExpand = false;
            isAllSatusExpand = false;
            AllTypeExpand_Img.sprite = AssetsManager.Instance.GetAlbumAsset(AlbumEnum.ArrowAlbum).album[3];
            AllStatusExpand_Img.sprite = AssetsManager.Instance.GetAlbumAsset(AlbumEnum.ArrowAlbum).album[3];
        });

        //篩選遮罩按鈕
        FilterMask_Btn.onClick.AddListener(() =>
        {
            Filter_Btn.interactable = true;
            Filter_Rt.gameObject.SetActive(false);
        });

        //所有類型展開
        AllTypeExpand_Btn.onClick.AddListener(() =>
        {
            isAllTypeExpand = !isAllTypeExpand;
            StartCoroutine(ISwitchContent(isAllTypeExpand,
                                         AllType_Obj,
                                         AllTypeExpand_Img));
        });

        //所有狀態展開
        AllStatusExpand_Btn.onClick.AddListener(() =>
        {
            isAllSatusExpand = !isAllSatusExpand;
            StartCoroutine(ISwitchContent(isAllSatusExpand,
                                         AllStatus_Obj,
                                         AllStatusExpand_Img));
        });

        //篩選提交
        FilterSubmit_Btn.onClick.AddListener(() =>
        {
            Filter_Btn.interactable = true;
            Filter_Rt.gameObject.SetActive(false);

            //顯示篩選後資料
            if (UCoin_Tog.isOn)
            {
                Gold_Tog.isOn = true;
                UCoin_Tog.isOn = true;
            }
            if (Gold_Tog.isOn)
            {
                UCoin_Tog.isOn = true;
                Gold_Tog.isOn = true;
            }
        });

        #endregion

        #region 設置日期範圍

        //關閉設置日期範圍
        CloseSetDate_Btn.onClick.AddListener(() =>
        {
            DateRange_Btn.gameObject.SetActive(true);
            SetDateRange_Obj.SetActive(false);
        });

        //開啟設置日期範圍
        DateRange_Btn.onClick.AddListener(() =>
        {
            DateRange_Btn.gameObject.SetActive(false);
            SetDateRange_Obj.SetActive(true);
        });

        //起始年分更改
        StartFilterYear_Dd.onValueChanged.AddListener((value) =>
        {
            UpdateDayDropdown(int.Parse(StartFilterYear_Dd.options[value].text),
                              int.Parse(StartFilterMonth_Dd.options[0].text),
                              StartFilterDay_Dd);
        });

        //結束年分更改
        EndFilterYear_Dd.onValueChanged.AddListener((value) =>
        {
            UpdateDayDropdown(int.Parse(EndFilterYear_Dd.options[value].text),
                              int.Parse(EndFilterMonth_Dd.options[0].text),
                              EndFilterDay_Dd);
        });

        //起始月分更改
        StartFilterMonth_Dd.onValueChanged.AddListener((value) =>
        {
            UpdateDayDropdown(int.Parse(StartFilterYear_Dd.options[StartFilterYear_Dd.value].text),
                              int.Parse(StartFilterMonth_Dd.options[value].text),
                              StartFilterDay_Dd);
        });

        //結束月分更改
        EndFilterMonth_Dd.onValueChanged.AddListener((value) =>
        {
            UpdateDayDropdown(int.Parse(EndFilterYear_Dd.options[EndFilterYear_Dd.value].text),
                              int.Parse(EndFilterMonth_Dd.options[value].text),
                              EndFilterDay_Dd);
        });

        //設置日期提交
        SetDataRangeSubmit_Btn.onClick.AddListener(() =>
        {
            DateTime startDate = new DateTime(int.Parse(StartFilterYear_Dd.options[StartFilterYear_Dd.value].text),
                                              int.Parse(StartFilterMonth_Dd.options[StartFilterMonth_Dd.value].text),
                                              int.Parse(StartFilterDay_Dd.options[StartFilterDay_Dd.value].text));
            DateTime endDate = new DateTime(int.Parse(EndFilterYear_Dd.options[EndFilterYear_Dd.value].text),
                                            int.Parse(EndFilterMonth_Dd.options[EndFilterMonth_Dd.value].text),
                                            int.Parse(EndFilterDay_Dd.options[EndFilterDay_Dd.value].text));

            //設置日期無法判斷
            if ((startDate - endDate).TotalDays > 0)
            {
                StartFilterYear_Dd.value = 0;
                EndFilterYear_Dd.value = EndFilterYear_Dd.options.Count - 1;

                StartFilterMonth_Dd.value = StartFilterMonth_Dd.options.Count - 1;
                EndFilterMonth_Dd.value = 0;

                StartFilterDay_Dd.value = 0;
                EndFilterDay_Dd.value = DateTime.Now.Day - 1;

                Debug.LogError("Set Data Error!!!");
            }

            DateRange_Btn.gameObject.SetActive(true);
            SetDateRange_Obj.SetActive(false);

            SetFilterDate();
        });

        #endregion

        #region 篩選交易類型

        //Buy-In_篩選類型
        TypeBuyIn_Tog.onValueChanged.AddListener((isOn) =>
        {
            if (isOn)
            {
                currFilterType = TypeEnum.BuyIn;
            }
        });

        //ANTE_篩選類型
        TypeAnte_Tog.onValueChanged.AddListener((isOn) =>
        {
            if (isOn)
            {
                currFilterType = TypeEnum.Ante;
            }
        });

        //ALL_篩選類型
        TypeAll_Tog.onValueChanged.AddListener((isOn) =>
        {
            if (isOn)
            {
                currFilterType = TypeEnum.All;
            }
        });

        #endregion

        #region 篩選交易狀態

        //進行中_交易狀態
        ProcessingStatus_Tog.onValueChanged.AddListener((isOn) =>
        {
            if (isOn)
            {
                currFilterStatus = StatusEnum.Processing;
            }
        });

        //成功_交易狀態
        SuccessStatus_Tog.onValueChanged.AddListener((isOn) =>
        {
            if (isOn)
            {
                currFilterStatus = StatusEnum.Success;
            }
        });

        //失敗_交易狀態
        FailStatus_Tog.onValueChanged.AddListener((isOn) =>
        {
            if (isOn)
            {
                currFilterStatus = StatusEnum.Fail;
            }
        });

        //All_交易狀態
        AllStatus_Tog.onValueChanged.AddListener((isOn) =>
        {
            if (isOn)
            {
                currFilterStatus = StatusEnum.All;
            }
        });

        #endregion
    }

    private void OnEnable()
    {
        List<TransactionHistoryData> uCoinList = new List<TransactionHistoryData>()
        {
            new TransactionHistoryData(){ type = "Pledge", time = Utils.ConvertDateTimeToTimestamp(new DateTime(2024,06,15,10,30,0)).ToString(), title1 = "XT123", title2 = "0x1123as456scas12315423156x123", status = 0, pl = "+6400"},
            new TransactionHistoryData(){ type = "Buy-In", time = Utils.ConvertDateTimeToTimestamp(new DateTime(2024,06,15,10,30,0)).ToString(), title1 = "Cash $20,200/20,200", title2 = "1234654684", status = 1, pl = "-400"},
            new TransactionHistoryData(){ type = "Buy-In", time = Utils.ConvertDateTimeToTimestamp(new DateTime(2024,06,14,8,55,0)).ToString(), title1 = "Cash $20,200/20,200", title2 = "1234654684", status = -1, pl = "-400"},
            new TransactionHistoryData(){ type = "Buy-In", time = Utils.ConvertDateTimeToTimestamp(new DateTime(2024,06,13,10,30,0)).ToString(), title1 = "Cash $20,200/20,200", title2 = "1234654684", status = 1, pl = "-1400"},
            new TransactionHistoryData(){ type = "Pledge", time = Utils.ConvertDateTimeToTimestamp(new DateTime(2024,05,15,10,30,0)).ToString(), title1 = "XT123", title2 = "0x1123as456scas12315423156x123", status = 0, pl = "+400"},
            new TransactionHistoryData(){ type = "Pledge", time = Utils.ConvertDateTimeToTimestamp(new DateTime(2024,04,15,10,30,0)).ToString(), title1 = "XT123", title2 = "0x1123as456scas12315423156x123", status = -1, pl = "+2400"},
        };

        List<TransactionHistoryData> aCoinList = new List<TransactionHistoryData>()
        {
        };

        List<TransactionHistoryData> goldList = new List<TransactionHistoryData>()
        {
            new TransactionHistoryData(){ type = "Buy-In", time = Utils.ConvertDateTimeToTimestamp(new DateTime(2024,06,17,10,30,0)).ToString(), title1 = "Cash $20,200/20,200", title2 = "1234654684", status = 0, pl = "-1400"},
            new TransactionHistoryData(){ type = "Pledge", time = Utils.ConvertDateTimeToTimestamp(new DateTime(2024,06,15,10,30,0)).ToString(), title1 = "XT123", title2 = "0x1123as456scas12315423156x123", status = 0, pl = "+6400"},
            new TransactionHistoryData(){ type = "Buy-In", time = Utils.ConvertDateTimeToTimestamp(new DateTime(2024,06,15,10,30,0)).ToString(), title1 = "Cash $20,200/20,200", title2 = "1234654684", status = 1, pl = "-400"},
            new TransactionHistoryData(){ type = "Buy-In", time = Utils.ConvertDateTimeToTimestamp(new DateTime(2024,06,14,8,55,0)).ToString(), title1 = "Cash $20,200/20,200", title2 = "1234654684", status = -1, pl = "-400"},
            new TransactionHistoryData(){ type = "Buy-In", time = Utils.ConvertDateTimeToTimestamp(new DateTime(2024,06,13,10,30,0)).ToString(), title1 = "Cash $20,200/20,200", title2 = "1234654684", status = 1, pl = "-1400"},
            new TransactionHistoryData(){ type = "Pledge", time = Utils.ConvertDateTimeToTimestamp(new DateTime(2024,05,15,10,30,0)).ToString(), title1 = "XT123", title2 = "0x1123as456scas12315423156x123", status = 0, pl = "+400"},
            new TransactionHistoryData(){ type = "Pledge", time = Utils.ConvertDateTimeToTimestamp(new DateTime(2024,04,15,10,30,0)).ToString(), title1 = "XT123", title2 = "0x1123as456scas12315423156x123", status = 1, pl = "+2400"},
            new TransactionHistoryData(){ type = "Pledge", time = Utils.ConvertDateTimeToTimestamp(new DateTime(2024,04,15,10,30,0)).ToString(), title1 = "XT123", title2 = "0x1123as456scas12315423156x123", status = -1, pl = "+2400"},
            new TransactionHistoryData(){ type = "Pledge", time = Utils.ConvertDateTimeToTimestamp(new DateTime(2024,04,15,10,30,0)).ToString(), title1 = "XT123", title2 = "0x1123as456scas12315423156x123", status = 1, pl = "+2400"},
            new TransactionHistoryData(){ type = "Pledge", time = Utils.ConvertDateTimeToTimestamp(new DateTime(2024,04,15,10,30,0)).ToString(), title1 = "XT123", title2 = "0x1123as456scas12315423156x123", status = -1, pl = "+2400"},
            new TransactionHistoryData(){ type = "Pledge", time = Utils.ConvertDateTimeToTimestamp(new DateTime(2024,04,15,10,30,0)).ToString(), title1 = "XT123", title2 = "0x1123as456scas12315423156x123", status = 1, pl = "+2400"},
            new TransactionHistoryData(){ type = "Pledge", time = Utils.ConvertDateTimeToTimestamp(new DateTime(2024,04,15,10,30,0)).ToString(), title1 = "XT123", title2 = "0x1123as456scas12315423156x123", status = 1, pl = "+2400"},
            new TransactionHistoryData(){ type = "Pledge", time = Utils.ConvertDateTimeToTimestamp(new DateTime(2024,04,15,10,30,0)).ToString(), title1 = "XT123", title2 = "0x1123as456scas12315423156x123", status = 1, pl = "+2400"},
            new TransactionHistoryData(){ type = "Pledge", time = Utils.ConvertDateTimeToTimestamp(new DateTime(2024,04,15,10,30,0)).ToString(), title1 = "XT123", title2 = "0x1123as456scas12315423156x123", status = -1, pl = "+2400"},
            new TransactionHistoryData(){ type = "Pledge", time = Utils.ConvertDateTimeToTimestamp(new DateTime(2024,04,15,10,30,0)).ToString(), title1 = "XT123", title2 = "0x1123as456scas12315423156x123", status = 1, pl = "+2400"},
            new TransactionHistoryData(){ type = "Pledge", time = Utils.ConvertDateTimeToTimestamp(new DateTime(2024,04,15,10,30,0)).ToString(), title1 = "XT123", title2 = "0x1123as456scas12315423156x123", status = 1, pl = "+2400"},
            new TransactionHistoryData(){ type = "Pledge", time = Utils.ConvertDateTimeToTimestamp(new DateTime(2024,04,15,10,30,0)).ToString(), title1 = "XT123", title2 = "0x1123as456scas12315423156x123", status = -1, pl = "+2400"},
            new TransactionHistoryData(){ type = "Pledge", time = Utils.ConvertDateTimeToTimestamp(new DateTime(2024,04,15,10,30,0)).ToString(), title1 = "XT123", title2 = "0x1123as456scas12315423156x123", status = 1, pl = "+2400"},
            new TransactionHistoryData(){ type = "Pledge", time = Utils.ConvertDateTimeToTimestamp(new DateTime(2024,04,15,10,30,0)).ToString(), title1 = "XT123", title2 = "0x1123as456scas12315423156x123", status = 0, pl = "+2400"},
            new TransactionHistoryData(){ type = "Pledge", time = Utils.ConvertDateTimeToTimestamp(new DateTime(2024,04,15,10,30,0)).ToString(), title1 = "XT123", title2 = "0x1123as456scas12315423156x123", status = 1, pl = "+2400"},
            new TransactionHistoryData(){ type = "Pledge", time = Utils.ConvertDateTimeToTimestamp(new DateTime(2024,04,15,10,30,0)).ToString(), title1 = "XT123", title2 = "0x1123as456scas12315423156x123", status = 1, pl = "+2400"},
            new TransactionHistoryData(){ type = "Pledge", time = Utils.ConvertDateTimeToTimestamp(new DateTime(2024,04,15,10,30,0)).ToString(), title1 = "XT123", title2 = "0x1123as456scas12315423156x123", status = 1, pl = "+2400"},
            new TransactionHistoryData(){ type = "Pledge", time = Utils.ConvertDateTimeToTimestamp(new DateTime(2024,04,15,10,30,0)).ToString(), title1 = "XT123", title2 = "0x1123as456scas12315423156x123", status = 0, pl = "+2400"},
            new TransactionHistoryData(){ type = "Pledge", time = Utils.ConvertDateTimeToTimestamp(new DateTime(2024,04,15,10,30,0)).ToString(), title1 = "XT123", title2 = "0x1123as456scas12315423156x123", status = 1, pl = "+2400"},
        };

        Filter_Rt.gameObject.SetActive(false);
        SetDateRange_Obj.SetActive(false);

        uCoinDataList = uCoinList;
        aCoinDataList = aCoinList;
        goldDataList = goldList;
        displayDataList = uCoinDataList;
        currPage = 1;

        UCoin_Tog.isOn = true;
        TypeAll_Tog.isOn = true;
        AllStatus_Tog.isOn = true;

        currFilterType = TypeEnum.All;
        currFilterStatus = StatusEnum.All;

        InitFilterDate();
        DisplayHistory(displayDataList, currPage);
    }

    private void Start()
    {
        Viewport.enabled = true;
        TransactionHistorySampleObj.SetActive(false);
    }

    private void Update()
    {
        //背景高度
        float filterHeight = FilterLayout_Tr.rect.height + 20 <= Filter_Rt.rect.height ?
                             FilterLayout_Tr.rect.height + 20 :
                             Filter_Rt.rect.height;
        //篩選背景大小
        FilterBg_Tr.sizeDelta = new Vector2(FilterBg_Tr.rect.width,
                                            filterHeight);
        //Sroll Rect大小
        Filter_Sr.sizeDelta = new Vector2(FilterBg_Tr.rect.width,
                                           filterHeight - 20);

    }

    /// <summary>
    /// 初始篩選日期
    /// </summary>
    private void InitFilterDate()
    {
        //設置年分dropdown
        DateTime today = DateTime.Now;
        if (today.Month < 4)
        {
            List<string> yearOptions = new List<string>();
            yearOptions.Add((today.Year - 1).ToString());
            yearOptions.Add(today.Year.ToString());
            Utils.SetOptionsToDropdown(StartFilterYear_Dd, yearOptions);
            Utils.SetOptionsToDropdown(EndFilterYear_Dd, yearOptions);

            StartFilterYear_Dd.value = 0;
            EndFilterYear_Dd.value = EndFilterYear_Dd.options.Count - 1;
        }

        //設置月分dropdown
        List<string> monthOptions = new List<string>();
        int theMonth = today.Month + 1;
        for (int i = 0; i < 4; i++)
        {
            theMonth = theMonth - 1 <= 0 ?
                         12 :
                         theMonth - 1;

            monthOptions.Add(theMonth.ToString("00"));
        }
        Utils.SetOptionsToDropdown(StartFilterMonth_Dd, monthOptions);
        Utils.SetOptionsToDropdown(EndFilterMonth_Dd, monthOptions);
        StartFilterMonth_Dd.value = StartFilterMonth_Dd.options.Count - 1;
        EndFilterMonth_Dd.value = 0;

        //更新日天數
        UpdateDayDropdown(int.Parse(StartFilterYear_Dd.options[StartFilterYear_Dd.value].text),
                          int.Parse(StartFilterMonth_Dd.options[StartFilterMonth_Dd.value].text),
                          StartFilterDay_Dd);
        UpdateDayDropdown(int.Parse(EndFilterYear_Dd.options[EndFilterYear_Dd.value].text),
                          int.Parse(EndFilterMonth_Dd.options[EndFilterMonth_Dd.value].text),
                          EndFilterDay_Dd);

        StartFilterDay_Dd.value = 0;
        EndFilterDay_Dd.value = today.Day - 1;

        SetFilterDate();
    }

    /// <summary>
    /// 設置篩選日期
    /// </summary>
    private void SetFilterDate()
    {
        currFilterStartDate = new DateTime(int.Parse(StartFilterYear_Dd.options[StartFilterYear_Dd.value].text),
                                           int.Parse(StartFilterMonth_Dd.options[StartFilterMonth_Dd.value].text),
                                           int.Parse(StartFilterDay_Dd.options[StartFilterDay_Dd.value].text));

        currFilterEndDate = new DateTime(int.Parse(EndFilterYear_Dd.options[EndFilterYear_Dd.value].text),
                                         int.Parse(EndFilterMonth_Dd.options[EndFilterMonth_Dd.value].text),
                                         int.Parse(EndFilterDay_Dd.options[EndFilterDay_Dd.value].text));

        DateRange_Txt.text = $"{currFilterStartDate.Year}/{currFilterStartDate.Month}/{currFilterStartDate.Day} - " +
                             $"{currFilterEndDate.Year}/{currFilterEndDate.Month}/{currFilterEndDate.Day}";
    }

    #region 顯示交易紀錄

    /// <summary>
    /// 顯示交易紀錄
    /// </summary>
    /// <param name="dataList"></param>
    /// <param name="page">顯示頁面</param>
    private void DisplayHistory(List<TransactionHistoryData> dataList, int page)
    {
        //移除舊交易紀錄
        for (int i = 0; i < HistoryArea.childCount; i++)
        {
            if (HistoryArea.GetChild(i).name != "TransactionHistorySample")
            {
                Destroy(HistoryArea.GetChild(i).gameObject);
            }
        }

        //篩選資料
        List<TransactionHistoryData> filterData = new List<TransactionHistoryData>();
        foreach (var data in dataList)
        {
            //篩選日期
            DateTime dataDate = Utils.ConvertTimestampToDate(long.Parse(data.time));
            if ((dataDate - currFilterStartDate).TotalDays < 0 ||
                (currFilterEndDate - dataDate).TotalDays < -1)
            {
                continue;
            }

            //篩選交易類型
            if (currFilterType == TypeEnum.BuyIn && data.type != "Buy-In" ||
                currFilterType == TypeEnum.Ante && data.type != "Pledge")
            {
                continue;
            }

            //篩選交易狀態
            if (currFilterStatus == StatusEnum.Processing && data.status != 0 ||
                currFilterStatus == StatusEnum.Success && data.status != 1 ||
                currFilterStatus == StatusEnum.Fail && data.status != -1)
            {
                continue;
            }

            filterData.Add(data);
        }

        //頁面文字
        int maxPages = filterData.Count % 10 == 0 ?
                       filterData.Count / 10 :
                       (filterData.Count / 10) + 1;
        if (page > maxPages)
        {
            page = maxPages;
        }
        if (page <= 1)
        {
            page = 1;
        }
        currPage = page;
        CurrPage_Txt.text = $"<color=#E5F2FF>{page}</color><color=#858BAD> {LanguageManager.Instance.GetText("OF")} {maxPages}</color>";

        List<TransactionHistoryData> getData = filterData.Skip((page - 1) * 10).Take(10).ToList();
        if (getData.Count == 0)
        {
            return;
        }

        //產生交易紀錄
        SetFilterDate();
        foreach (var data in getData)
        {
            TransactionHistorySample transactionHistorySample = Instantiate(TransactionHistorySampleObj, HistoryArea).GetComponent<TransactionHistorySample>();
            transactionHistorySample.gameObject.SetActive(true);
            transactionHistorySample.name = "TransactionHistory";
            transactionHistorySample.SetTransactionHistory(data);
        }
    }

    /// <summary>
    /// 介面內容展開縮放
    /// </summary>
    /// <param name="isExpand">是否展開</param>
    /// <param name="rt">展開內容物件</param>
    /// <param name="img">展開按鈕圖</param>
    /// <param name="completeCallback">完成回傳</param>
    /// <returns></returns>
    private IEnumerator ISwitchContent(bool isExpand, RectTransform rt, Image img, UnityAction completeCallback = null)
    {
        //展開內容物件
        RectTransform contentObj = rt.Find(expandContentName).GetComponent<RectTransform>();
        //收回高度
        float pullbackHeight = rt.Find(expandTopBgName).GetComponent<RectTransform>().rect.height;
        //目標高度
        float targetHeight = isExpand == true ?
                             contentObj.rect.height :
                             pullbackHeight;
        //初始高度
        float initHeight = isExpand == true ?
                           pullbackHeight :
                           rt.rect.height;

        contentObj.gameObject.SetActive(false);
        rt.sizeDelta = new Vector2(rt.rect.width, initHeight);

        DateTime startTime = DateTime.Now;
        while ((DateTime.Now - startTime).TotalSeconds < expandTIme)
        {
            float progress = (float)(DateTime.Now - startTime).TotalSeconds / expandTIme;
            float height = Mathf.Lerp(initHeight, targetHeight, progress);
            rt.sizeDelta = new Vector2(rt.rect.width, height);

            yield return null;
        }

        contentObj.gameObject.SetActive(isExpand);
        rt.sizeDelta = new Vector2(rt.rect.width, targetHeight);
        img.sprite = isExpand == true ?
                     AssetsManager.Instance.GetAlbumAsset(AlbumEnum.ArrowAlbum).album[1] :
                     AssetsManager.Instance.GetAlbumAsset(AlbumEnum.ArrowAlbum).album[3];

        completeCallback?.Invoke();
    }

    #endregion

    #region 篩選交易紀錄

    /// <summary>
    /// 更新dropdown日期天數
    /// </summary>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <param name="dropdown"></param>
    private void UpdateDayDropdown(int year, int month, TMP_Dropdown dropdown)
    {
        int daysInMonth = DateTime.DaysInMonth(year, month);

        List<string> dayOptions = new List<string>();
        for (int day = 1; day <= daysInMonth; day++)
        {
            dayOptions.Add(day.ToString("00"));
        }

        Utils.SetOptionsToDropdown(dropdown, dayOptions);
    }

    #endregion
}
