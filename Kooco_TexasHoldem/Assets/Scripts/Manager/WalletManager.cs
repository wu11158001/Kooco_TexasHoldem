using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using Thirdweb;
using System;

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
#if !UNITY_EDITOR
        Debug.Log("Disconnecting...");
        try
        {
            await ThirdwebManager.Instance.SDK.Wallet.Disconnect(true);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to disconnect: {e}");
        }

        if (!GameDataManager.IsMobilePlatform)
        {
            JS_WindowDisconnect();
        }
#endif
    }
}
