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
        Index_Txt.text = $"Hand{index + 1}";
        TypeAndBlind_Txt.text = $"<color=#FFFFFF><size=12>{resultHistory.RoomType}</size></color>" +
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
