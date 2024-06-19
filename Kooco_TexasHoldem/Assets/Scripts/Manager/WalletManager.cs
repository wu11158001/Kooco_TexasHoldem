using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using Thirdweb;
using System;
using UnityEngine.SceneManagement;

public class WalletManager : UnitySingleton<WalletManager>
{
    [DllImport("__Internal")]
    private static extern void JS_WindowDisconnect();                                 //電腦網頁_斷開連接
    [DllImport("__Internal")]
    private static extern void JS_RevokePermissions();                                //電腦網頁_撤銷權限

    public override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// 錢包斷開
    /// </summary>
    async public void OnWalletDisconnect()
    {
        Debug.Log("Disconnecting...");
        try
        {
            await ThirdwebManager.Instance.SDK.Wallet.Disconnect(true);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to disconnect: {e}");
        }

        if (!DataManager.IsMobilePlatform)
        {
            JS_WindowDisconnect();
        }
    }

    /// <summary>
    /// 開始偵測連線
    /// </summary>
    public void StartCheckConnect()
    {
        InvokeRepeating(nameof(CheckConnect), 7, 4);
    }

    /// <summary>
    /// 取消偵測連線
    /// </summary>
    public void CancelCheckConnect()
    {
        CancelInvoke(nameof(CheckConnect));
    }

    /// <summary>
    /// 偵測連線
    /// </summary>
    async private void CheckConnect()
    {
        bool isConnect = await ThirdwebManager.Instance.SDK.Wallet.IsConnected();

        if (!isConnect && SceneManager.GetActiveScene().name != "Login")
        {
            DataManager.UserWalletAddress = "";
            LoadSceneManager.Instance.LoadScene(SceneEnum.Login);
        }
    }
}
