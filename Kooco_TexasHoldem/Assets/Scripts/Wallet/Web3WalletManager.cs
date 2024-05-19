using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using System.Runtime.InteropServices;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

using MetaMask.Unity;
using MetaMask.SocketIOClient;
using MetaMask.NativeWebSocket;
using MetaMask.Transports;
using MetaMask.Transports.Unity.UI;
using MetaMask;

public class Web3WalletManager : UnitySingleton<Web3WalletManager>
{
    [DllImport("__Internal")]
    private static extern void MetaMaskConnectAndSignFormWindows();                //登入電腦網頁MetaMask
    [DllImport("__Internal")]
    private static extern void TrustConnectAndSignFormWindows();                   //登入電腦網頁TrustWallet
    [DllImport("__Internal")]
    private static extern void TrustConnectAndSignFormMobile();                    //移動平台連接TrustWallet

    public override void Awake()
    {
        base.Awake();

        MetaMaskUnity.Instance.Initialize();
        MetaMaskUnity.Instance.Events.WalletAuthorized += OnWalletConnected;
        MetaMaskUnity.Instance.Events.WalletDisconnected += OnWalletDisconnected;
        MetaMaskUnity.Instance.Events.WalletReady += OnWalletReady;
        MetaMaskUnity.Instance.Events.WalletPaused += OnWalletPaused;
        MetaMaskUnity.Instance.Events.WalletUnauthorized += OnWalletUnauthorized;
        MetaMaskUnity.Instance.Wallet.EthereumRequestFailed += OnEthereumRequestFailed;
        MetaMaskUnity.Instance.Wallet.EthereumRequestResultReceived += OnEthereumRequestResultReceived;
        if (Application.isMobilePlatform && MetaMaskUnityUITransport.DefaultInstance.IsDeeplinkAvailable())
        {
            MetaMaskUnity.Instance.Events.WalletAuthorized += OnConnectedFeedback;
        }
    }

    private void OnDisable()
    {
        MetaMaskUnity.Instance.Events.WalletAuthorized -= OnWalletConnected;
        MetaMaskUnity.Instance.Events.WalletDisconnected -= OnWalletDisconnected;
        MetaMaskUnity.Instance.Events.WalletReady -= OnWalletReady;
        MetaMaskUnity.Instance.Events.WalletUnauthorized -= OnWalletUnauthorized;
        MetaMaskUnity.Instance.Events.WalletPaused -= OnWalletPaused;
        MetaMaskUnity.Instance.Wallet.EthereumRequestFailed -= OnEthereumRequestFailed;
        MetaMaskUnity.Instance.Wallet.EthereumRequestResultReceived -= OnEthereumRequestResultReceived;
        if (Application.isMobilePlatform && MetaMaskUnityUITransport.DefaultInstance.IsDeeplinkAvailable())
        {
            MetaMaskUnity.Instance.Events.WalletAuthorized -= OnConnectedFeedback;
        }
    }

    #region 外部接口

    /// <summary>
    /// 斷開連接
    /// </summary>
    public void DoWalletDisconnected()
    {
        MetaMaskUnity.Instance.Disconnect(true);
    }

    /// <summary>
    /// 移除權限
    /// </summary>
    public void RevokePermissions()
    {
        MetaMaskUnity.Instance.Wallet.Disconnect();
        MetaMaskUnity.Instance.Wallet.Dispose();
        WalletManager.Instance.DoRevokePermissions();
    }

    /// <summary>
    /// 連接與簽名
    /// </summary>
    /// <param name="web3Type">Web3類型</param>
    public void ConnectAndSign(Web3Enum web3Type)
    {
        RevokePermissions();
        DoWalletDisconnected();

        if (WalletManager.Instance.DoIsMobilePlatform())
        {
            switch (web3Type)
            {
                //MetaMask
                case Web3Enum.MetaMask:
                    MetaMaskUnity.Instance.ConnectAndSign("MetaMask MobilePlatform Sign Info");
                    break;

                //TrustWallet
                case Web3Enum.TrustWallet:
                    TrustConnectAndSignFormMobile();
                    break;
            }
        }
        else
        {
            ViewManager.Instance.OpenPartsView(PartsViewEnum.WaitingView);

            switch (web3Type)
            {
                //MetaMask
                case Web3Enum.MetaMask:
                    MetaMaskConnectAndSignFormWindows();
                    break;

                //TrustWallet
                case Web3Enum.TrustWallet:
                    TrustConnectAndSignFormWindows();
                    break;
            }
            
        }
    }

    #endregion

    #region 移動平台

    /// <summary>
    /// 移動平台錢包連線後呼叫
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnWalletConnected(object sender, EventArgs e)
    {
        UnityThread.executeInUpdate(() =>
        {
            Debug.Log("Wallet Connected!!!");
        });
    }

    /// <summary>
    /// 錢包已斷開
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnWalletDisconnected(object sender, EventArgs e)
    {
        Debug.Log("Wallet Disconnected!!");
    }

    /// <summary>
    /// 錢包已準備
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnWalletReady(object sender, EventArgs e)
    {
        UnityThread.executeInUpdate(() =>
        {
            Debug.Log("Wallet Ready!!");
        });
    }

    /// <summary>
    /// 錢包已暫停
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnWalletPaused(object sender, EventArgs e)
    {
        UnityThread.executeInUpdate(() =>
        {
            Debug.Log("Wallet Paused!!");
        });
    }

    /// <summary>
    /// 連接回饋
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnConnectedFeedback(object sender, EventArgs e)
    {
        Debug.Log("Wallet ConnectedFeedback"); ;
        StopAllCoroutines();
    }

    /// <summary>
    /// 錢包未授權
    /// </summary>
    private void OnWalletUnauthorized(object sender, EventArgs e)
    {
        Debug.Log("Wallet Unauthorized");
    }

    /// <summary>
    /// 請求失敗
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnEthereumRequestFailed(object sender, MetaMaskEthereumRequestFailedEventArgs e)
    {
        Debug.Log("Wallet EthereumRequestFailed!!!!");
    }

    /// <summary>
    /// 請求結果訊息
    /// </summary>
    [System.Serializable]
    public class EthereumRequestData
    {
        public string jsonrpc;
        public string id;
        public string method;
        public string result;
        public string[] resultArr;
        public double date;
    }
    /// <summary>
    /// 請求結果
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnEthereumRequestResultReceived(object sender, MetaMaskEthereumRequestResultEventArgs e)
    {
        Debug.Log("Wallet OnEthereumRequestResultReceived!!!!!" + e.Result);
        EthereumRequestData data = JsonUtility.FromJson<EthereumRequestData>(e.Result);

        switch (data.method)
        {
            //首次發起連接請求
            case "wallet_requestPermissions":
                if (data.resultArr != null &&
                    data.resultArr.Length != 0)
                {
                    
                }
                break;

            //連接請求
            case "eth_requestAccounts":

                break;

            //簽名結果
            case "metamask_connectSign":
                ViewManager.Instance.OpenPartsView(PartsViewEnum.WaitingView);
                if (data.result != "")
                {                    
                    Debug.Log($"Unity Singatrue:{data.result}");
                    ConnectSuccess();
                }
                else
                {
                    Debug.Log("Sign Not Agree!!!");
                }
                break;

            //鏈更改
            case "eth_chainId":
               
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// 連接成功
    /// </summary>
    async private void ConnectSuccess()
    {
        try
        {
            //獲取訊息
            string[] accounts = await MetaMaskUnity.Instance.Request<string[]>("eth_accounts");
            if (accounts.Length > 0)
            {
                string address = accounts[0];
                GameDataManager.UserWalletAddress = address;
                Debug.Log("Set Metamask Address: " + address);

                // Request account balance
                string balance = await MetaMaskUnity.Instance.Request<string>("eth_getBalance", new object[] { address, "latest" });
                //將十六進位字串轉換為 BigInteger
                BigInteger balanceWei;
                if (balance.StartsWith("0x"))
                {
                    balanceWei = BigInteger.Parse(balance.Substring(2), System.Globalization.NumberStyles.HexNumber);
                }
                else
                {
                    balanceWei = BigInteger.Parse(balance, System.Globalization.NumberStyles.HexNumber);
                }
                //將Wei轉換為乙太幣
                decimal balanceEth = (decimal)balanceWei / (decimal)Math.Pow(10, 18);

                GameDataManager.UserWalletBalance = balanceEth.ToString();
                Debug.Log("Set Metamask Balance: " + balanceEth.ToString());
            }
            else
            {
                Debug.LogError("No account found.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error retrieving account information: " + ex.Message);
        }

        Debug.Log($"IsConnected : {MetaMaskUnity.Instance.Wallet.IsConnected}");
        Debug.Log($"IsAuthorized : {MetaMaskUnity.Instance.Wallet.IsAuthorized}");
        if (MetaMaskUnity.Instance.Wallet.IsConnected &&
            MetaMaskUnity.Instance.Wallet.IsAuthorized)
        {
            LoadSceneManager.Instance.LoadScene(SceneEnum.Lobby);
        }
    }

    #endregion
}
