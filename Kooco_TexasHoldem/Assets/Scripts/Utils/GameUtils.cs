using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class GameUtils
{
    /// <summary>
    /// 更換檢查圖樣
    /// </summary>
    /// <param name="isTrue"></param>
    /// <param name="img"></param>
    /// <returns></returns>
    public static bool CnahgeCheckIcon(bool isTrue, Image img)
    {
        string mistakeColor = "#CF5A5A";
        string correctColor = "#87CF5A";

        img.sprite = isTrue ?
                     AssetsManager.Instance.GetAlbumAsset(AlbumEnum.PasswordCheckAlbum).album[1] :
                     AssetsManager.Instance.GetAlbumAsset(AlbumEnum.PasswordCheckAlbum).album[0];
        string colorStr = isTrue ?
                          correctColor :
                          mistakeColor;
        if (ColorUtility.TryParseHtmlString(colorStr, out Color color))
        {
            img.color = color;
        }

        return isTrue;
    }
}
