using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LobbyRankingView : MonoBehaviour
{
    [SerializeField]
    Toggle Integral_Tog, Cash_Tog, Golden_Tog;
    [SerializeField]
    Button ChangeSeason_Btn, RefreshRank_Btn;
    [SerializeField]
    Text Season_Txt, TimeLest_Txt, ChangeSeasonBtn_Txt;
    [SerializeField]
    GameObject RankSampleObj;
    [SerializeField]
    Transform RankContent;
    [SerializeField]
    RankSample[] TopThreeData;

    [Header("本地玩家")]
    [SerializeField]
    Text LocalUserRank_Txt, LocalUserPoint_Txt, LocalUserAward_Txt;

    [Header("排行規則")]
    [SerializeField]
    GameObject Rule_Obj;
    [SerializeField]
    Text RuleTitle_Txt, RuleContent_Txt;
    [SerializeField]
    Button RuleMask_Btn, Rule_Btn, RuleClose_Btn;

    ObjPool objPool;

    /// <summary>
    /// 當前排名類型
    /// </summary>
    RankType currRankType;
    enum RankType
    {
        Integral,
        Cash,
        Golden,
    }

    /// <summary>
    /// 當前賽季類型
    /// </summary>
    SeasonType currSeasonType;
    enum SeasonType
    {
        Current,
        Previous,
    }

    private void Awake()
    {
        objPool = new ObjPool(transform, 50);

        ListenerEvent();
    }

    /// <summary>
    /// 事件聆聽
    /// </summary>
    private void ListenerEvent()
    {
        //顯示積分排行
        Integral_Tog.onValueChanged.AddListener((isOn) =>
        {
            if (isOn)
            {
                currRankType = RankType.Integral;
                SetRank();
            }
        });

        //顯示Cash排行
        Cash_Tog.onValueChanged.AddListener((isOn) =>
        {
            if (isOn)
            {
                currRankType = RankType.Cash;
                SetRank();
            }
        });

        //顯示Golden排行
        Golden_Tog.onValueChanged.AddListener((isOn) =>
        {
            if (isOn)
            {
                currRankType = RankType.Golden;
                SetRank();
            }
        });

        //更換賽季
        ChangeSeason_Btn.onClick.AddListener(() =>
        {
            if (DataManager.CurrRankSeason - 1 <= 0)
            {
                return;
            }

            currSeasonType = currSeasonType == SeasonType.Current ?
                             SeasonType.Previous :
                             SeasonType.Current;

            ChangeSeasonBtn_Txt.text = currSeasonType == SeasonType.Current ?
                                       "Previous" :
                                       "Current";

            Season_Txt.text = currSeasonType == SeasonType.Current ?
                              $"SEASON {DataManager.CurrRankSeason}" :
                              $"SEASON {DataManager.CurrRankSeason - 1}";

            SetRank();
        });

        //刷新排行
        RefreshRank_Btn.onClick.AddListener(() =>
        {
            Debug.Log("Ranl Refresh!");
        });

        #region 排行規則

        //規則說明
        Rule_Btn.onClick.AddListener(() =>
        {
            Rule_Obj.SetActive(true);
            switch (currRankType)
            {
                case RankType.Integral:
                    RuleTitle_Txt.text = "INTEGRAL RULE";
                    RuleContent_Txt.text = "INTEGRAL RULE 1\nINTEGRAL RULE 2\nINTEGRAL RULE 3";
                    break;

                case RankType.Cash:
                    RuleTitle_Txt.text = "CASH RULE";
                    RuleContent_Txt.text = "CASH RULE 1\nCASH RULE 2\nCASH RULE 3";
                    break;

                case RankType.Golden:
                    RuleTitle_Txt.text = "Golden RULE";
                    RuleContent_Txt.text = "Golden RULE 1\nGolden RULE 2\nGolden RULE 3";
                    break;
            }
        });

        //關閉規則
        RuleClose_Btn.onClick.AddListener(() =>
        {
            Rule_Obj.SetActive(false);
        });

        //規則遮罩按鈕
        RuleMask_Btn.onClick.AddListener(() =>
        {
            Rule_Obj.gameObject.SetActive(false);
        });

        #endregion
    }

    private void OnEnable()
    {
        Rule_Obj.gameObject.SetActive(false);
        Season_Txt.text = $"SEASON {DataManager.CurrRankSeason}";
        RankSampleObj.SetActive(false);
        currSeasonType = SeasonType.Current;
        currRankType = RankType.Integral;

        SetRank();
    }

    private void Update()
    {
        //賽季時間倒數
        if ((DataManager.RandEndDate - DateTime.Now).TotalSeconds > 0)
        {
            TimeSpan timeLest = DataManager.RandEndDate - DateTime.Now;
            TimeLest_Txt.text = $"{timeLest.Days }D : {timeLest.Hours}H : {timeLest.Minutes}M";
        }        
    }

    /// <summary>
    /// 設置排名
    /// </summary>
    private void SetRank()
    {
        List<RankData> rankDatas = null;
        string pointStr = "";

        RankData localUserData = new RankData();

        switch (currRankType)
        {
            case RankType.Integral:
                pointStr = "Points";
                rankDatas = currSeasonType == SeasonType.Current ?
                            DataManager.CurrSeasonIntegralRankList :
                            DataManager.PreSeasonIntegralRankList;
                localUserData = currSeasonType == SeasonType.Current ?
                                DataManager.LocalUserRankData[0] :
                                DataManager.LocalUserRankData[3];
                break;

            case RankType.Cash:
                pointStr = "Cash";
                rankDatas = currSeasonType == SeasonType.Current ?
                            DataManager.CurrSeasonCashRankList :
                            DataManager.PreSeasonCashRankList;
                localUserData = currSeasonType == SeasonType.Current ?
                                DataManager.LocalUserRankData[1] :
                                DataManager.LocalUserRankData[4];
                break;

            case RankType.Golden:
                pointStr = "Golden";
                rankDatas = currSeasonType == SeasonType.Current ?
                            DataManager.CurrSeasonGoldenRankList :
                            DataManager.PreSeasonGoldenRankList;
                localUserData = currSeasonType == SeasonType.Current ?
                                DataManager.LocalUserRankData[2] :
                                DataManager.LocalUserRankData[5];
                break;
        }

        if (rankDatas == null || rankDatas.Count == 0)
        {
            return;
        }

        //關閉舊排名
        for (int i = 1; i < RankContent.childCount; i++)
        {
            RankContent.GetChild(i).gameObject.SetActive(false);
        }

        //前三名設置
        for (int i = 0; i < TopThreeData.Length; i++)
        {
            TopThreeData[i].SetRankData(rankDatas[i], i + 1, pointStr);
        }

        //排名4名以後設置
        for (int i = TopThreeData.Length; i < rankDatas.Count; i++)
        {
            RankSample ranlSample = objPool.CreateObj<RankSample>(RankSampleObj, RankContent);
            ranlSample.SetRankData(rankDatas[i], i + 1, pointStr);
        }

        #region 本地玩家排名資料

        LocalUserRank_Txt.text = localUserData.rank > 999 ?
                                 $"My Rank   #999+" :
                                 $"My Rank   #{localUserData.rank}";
        LocalUserPoint_Txt.text = $"{localUserData.point} Points";
        LocalUserAward_Txt.text = $"{localUserData.award}";

        #endregion
    }
}
