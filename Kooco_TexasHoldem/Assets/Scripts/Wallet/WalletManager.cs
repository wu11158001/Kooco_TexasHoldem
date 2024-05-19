using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class WalletManager : UnitySingleton<WalletManager>
{
    [DllImport("__Internal")]
    private static extern bool IsMobilePlatform();                                  //檢測瀏覽器是否在移動平台

    [DllImport("__Internal")]
    private static extern void Reload();                                            //重新整理頁面

    [DllImport("__Internal")]
    private static extern void RevokePermissions();                                 //移除權限

    [DllImport("__Internal")]
    private static extern void Disconnect();                                        //斷開連接

    public override void Awake()
    {
        base.Awake();
    }

    #region 外部工具

    /// <summary>
    /// 檢測瀏覽器是否在移動平台
    /// </summary>
    public bool DoIsMobilePlatform()
    {
       return  IsMobilePlatform();
    }

    /// <summary>
    /// 重新整理頁面
    /// </summary>
    public void DoReload()
    {
        Reload();
    }

    /// <summary>
    /// 移除權限
    /// </summary>
    public void DoRevokePermissions()
    {
        RevokePermissions();
    }

    /// <summary>
    /// 斷開連接
    /// </summary>
    public void DoDisconnect()
    {
        Disconnect();
    }


    #endregion

    #region 電腦網頁

    /// <summary>
    /// 電腦網頁登入成功
    /// </summary>
    /// <param name="signature">簽名結果</param>
    public void WindowConnectSuccess(string signature)
    {
        Debug.Log($"Signature:{signature}");
        LoadSceneManager.Instance.LoadScene(SceneEnum.Lobby);
    }

    /// <summary>
    /// 電腦網頁登入失敗
    /// </summary>
    public void WindowConnectFail()
    {
        ViewManager.Instance.ClosePartsView(PartsViewEnum.WaitingView);
    }

    /// <summary>
    /// 網頁登入設置錢包地址
    /// </summary>
    /// <param name="address"></param>
    public void SetAddress(string address)
    {
        Debug.Log($"Set Wallet Address:{address}");
        GameDataManager.UserWalletAddress = address;
    }

    /// <summary>
    /// 網頁登入設置ETH餘額
    /// </summary>
    /// <param name="eth"></param>
    public void SetEthBlance(string eth)
    {
        Debug.Log($"Set Wallet EthBlance:{eth}");
        GameDataManager.UserWalletBalance = eth;
    }

    #endregion
}
