using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HistorySample : MonoBehaviour
{
    [SerializeField]
    Text Index_Txt, TypeAndBlind_Txt, Nicaname_Txt, WinChips_Txt;
    [SerializeField]
    Image Avatar_Img;
    [SerializeField]
    Poker[] HandPokers;
    [SerializeField]
    Poker[] CommunityPokers;
    [SerializeField]
    Button Play_Btn;

    public void SetData(ResultHistoryData resultHistory, int index)
    {
        Index_Txt.text = $"Hand{index}";
        TypeAndBlind_Txt.text = resultHistory.TypeAndBlindStr;
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
            Debug.Log($"Play{index}");
        });
    }
}
