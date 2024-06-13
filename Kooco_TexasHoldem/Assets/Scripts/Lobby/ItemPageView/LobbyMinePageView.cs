using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMinePageView : MonoBehaviour
{
    [Header("用戶訊息")]
    [SerializeField]
    Button Avatar_Btn, CopyWalletAddress_Btn;
    [SerializeField]
    Text NickName_Txt, WalletAddress_Txt;

    private void Awake()
    {
        ListenerEvent();
    }

    /// <summary>
    /// 事件聆聽
    /// </summary>
    private void ListenerEvent()
    {
        #region 用戶訊息

        //複製錢包地址
        CopyWalletAddress_Btn.onClick.AddListener(() =>
        {
            StringUtils.CopyText("123");
        });

        #endregion
    }

    /// <summary>
    /// 設置用戶訊息
    /// </summary>
    private void SetUserInfo()
    {
        //頭像
        Avatar_Btn.image.sprite = AssetsManager.Instance.GetAlbumAsset(AlbumEnum.AvatarAlbum).album[DataManager.UserAvatar];

        //錢包地址
        string walletAddress = DataManager.UserWalletAddress.Substring(0, 22) + "...";
        WalletAddress_Txt.text = walletAddress;
    }
}
