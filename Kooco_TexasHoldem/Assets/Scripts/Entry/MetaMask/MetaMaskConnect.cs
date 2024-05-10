using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using System.Runtime.InteropServices;
using System.Numerics;

using MetaMask.Unity;
using MetaMask.SocketIOClient;
using MetaMask.NativeWebSocket;
using MetaMask.Transports;
using MetaMask.Transports.Unity.UI;
using MetaMask;


public class MetaMaskConnect : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern bool IsMobilePlatform();//檢測瀏覽器是否在移動平台

    [DllImport("__Internal")]
    private static extern void WindowsLoginMetaMask();//登入電腦網頁MetaMask

    [SerializeField]
    Button metaMaskConnect_Btn;

    private void Awake()
    {
        MetaMaskUnity.Instance.Initialize();
        MetaMaskUnity.Instance.Events.WalletAuthorized += OnWalletConnected;
        MetaMaskUnity.Instance.Events.WalletDisconnected += OnWalletDisconnected;
        MetaMaskUnity.Instance.Events.WalletReady += OnWalletReady;
        MetaMaskUnity.Instance.Events.WalletPaused += OnWalletPaused;
        if (Application.isMobilePlatform && MetaMaskUnityUITransport.DefaultInstance.IsDeeplinkAvailable())
        {
            MetaMaskUnity.Instance.Events.WalletAuthorized += OnConnectedFeedback;
        }

        ListenerEvent();
    }

    private void OnDisable()
    {
        MetaMaskUnity.Instance.Events.WalletAuthorized -= OnWalletConnected;
        MetaMaskUnity.Instance.Events.WalletDisconnected -= OnWalletDisconnected;
        MetaMaskUnity.Instance.Events.WalletReady -= OnWalletReady;
        MetaMaskUnity.Instance.Events.WalletPaused -= OnWalletPaused;
        if (Application.isMobilePlatform && MetaMaskUnityUITransport.DefaultInstance.IsDeeplinkAvailable())
        {
            MetaMaskUnity.Instance.Events.WalletAuthorized -= OnConnectedFeedback;
        }
    }

    /// <summary>
    /// 事件聆聽
    /// </summary>
    private void ListenerEvent()
    {
        //MataMask連接
        metaMaskConnect_Btn.onClick.AddListener(() =>
        {
             Debug.Log($"IsMobilePlatform :{IsMobilePlatform()}");

             if (IsMobilePlatform())
             {
                 MetaMaskUnity.Instance.Connect();
             }
             else
             {
                 WindowsLoginMetaMask();
             }        
        });
    }

    /// <summary>
    /// 獲取錢包地址
    /// </summary>
    /// <param name="address"></param>
    public void GetAddress(string address)
    {
        Debug.Log($"GetAddress:{address}");
        GameDataManager.UserWalletAddress = address;
    }

    /// <summary>
    /// 獲取ETH餘額
    /// </summary>
    /// <param name="eth"></param>
    public void GetEthBlance(string eth)
    {
        Debug.Log($"GetEthBlance:{eth}");
        GameDataManager.UserWalletBalance = eth;
    }

    /// <summary>
    /// 電腦網頁登入成功
    /// </summary>
    /// <param name="address">錢包地址</param>
    /// <param name="blance">ETH餘額</param>
    public void WindowConnectSuccess()
    {        
        LoadSceneManager.Instance.LoadScene(SceneEnum.Lobby);
    }

    /// <summary>
    /// 移動平台錢包連線後呼叫
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnWalletConnected(object sender, EventArgs e)
    {
        UnityThread.executeInUpdate(() =>
        {
            Debug.Log("Wallet Connect Success!!");
            RequestAccountInfo();            
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
        Debug.Log("Wallet ConnectedFeedback");
        StopAllCoroutines();
    }

    /// <summary>
    /// 請求錢包訊息
    /// </summary>
    private async void RequestAccountInfo()
    {
        try
        {
            //獲取訊息
            string[] accounts = await MetaMaskUnity.Instance.Request<string[]>("eth_accounts");
            if (accounts.Length > 0)
            {
                string address = accounts[0];
                GameDataManager.UserWalletAddress = address;
                Debug.Log("Metamask Address: " + address);

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
                Debug.Log("Metamask Balance: " + balanceEth.ToString());
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

        LoadSceneManager.Instance.LoadScene(SceneEnum.Lobby);
    }
}
