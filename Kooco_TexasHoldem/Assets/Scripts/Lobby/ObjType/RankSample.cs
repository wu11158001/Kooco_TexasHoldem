using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankSample : MonoBehaviour
{
    [SerializeField]
    Image Avatar_Img;
    [SerializeField]
    Text Nickname_Txt, Point_Txt, Award_Txt, Rank_Txt;

    /// <summary>
    /// 設置排名資料
    /// </summary>
    /// <param name="rankData"></param>
    /// <param name="rank">排名</param>
    /// <param name="pointStr">點數文字</param>
    public void SetRankData(RankData rankData, int rank, string pointStr)
    {
        Avatar_Img.sprite = AssetsManager.Instance.GetAlbumAsset(AlbumEnum.AvatarAlbum).album[rankData.avatar];
        Nickname_Txt.text = rankData.nickname;
        Point_Txt.text = $"{rankData.point.ToString()} {pointStr}";
        Award_Txt.text = rankData.award.ToString();
        Rank_Txt.text = rank.ToString();
    }
}
