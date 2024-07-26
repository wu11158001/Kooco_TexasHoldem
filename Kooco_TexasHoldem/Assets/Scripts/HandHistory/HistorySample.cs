using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HistorySample : MonoBehaviour
{
    [SerializeField]
    Image Avatar_Img;
    [SerializeField]
    Poker[] HandPokers;
    [SerializeField]
    Poker[] CommunityPokers;
    [SerializeField]
    Button Play_Btn;
    [SerializeField]
    TextMeshProUGUI Index_Txt, TypeAndBlind_Txt, Nicaname_Txt, WinChips_Txt;

    ResultHistoryData tempResultHistory;                                                //紀錄資料
    int tempIndex;                                                                      //紀錄顯示的筆數

    /// <summary>
    /// 更新文本翻譯
    /// </summary>
    private void UpdateLanguage()
    {
        SetData(tempResultHistory, tempIndex);
    }

    private void OnDestroy()
    {
        LanguageManager.Instance.RemoveLanguageFun(UpdateLanguage);
    }

    private void Awake()
    {
        LanguageManager.Instance.AddUpdateLanguageFunc(UpdateLanguage, gameObject);
    }

    /// <summary>
    /// 設置手牌紀錄資料
    /// </summary>
    /// <param name="resultHistory"></param>
    /// <param name="index"></param>
    public void SetData(ResultHistoryData resultHistory, int index)
    {
        if (resultHistory == null)
        {
            return;
        }

        tempResultHistory = resultHistory;
        tempIndex = index;

        string roomName = LanguageManager.Instance.GetText(resultHistory.RoomType);

        Index_Txt.text = $"{LanguageManager.Instance.GetText("Hand")}{index + 1}";
        TypeAndBlind_Txt.text = $"<color=#FFFFFF><size=12>{roomName}</size></color>" +
                                $"  <color=#C0C0C0><size=11>{StringUtils.SetChipsUnit(resultHistory.SmallBlind)}/{StringUtils.SetChipsUnit(resultHistory.SmallBlind * 2)}</size></color>";
        Avatar_Img.sprite = AssetsManager.Instance.GetAlbumAsset(AlbumEnum.AvatarAlbum).album[resultHistory.Avatar];
        Nicaname_Txt.text = resultHistory.NickName;
        WinChips_Txt.text = StringUtils.SetChipsUnit(resultHistory.WinChips);
        HandPokers[0].PokerNum = resultHistory.HandPokers[0];
        HandPokers[1].PokerNum = resultHistory.HandPokers[1];

        foreach (var common in CommunityPokers)
        {
            common.PokerNum = -1;
        }
        for (int i = 0; i < resultHistory.CommunityPoker.Count; i++)
        {
            CommunityPokers[i].PokerNum = resultHistory.CommunityPoker[i];
        }

        Play_Btn.onClick.AddListener(() =>
        {
            HandHistoryManager.Instance.PlayVideo(index);
        });
    }
}
