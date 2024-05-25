using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Thirdweb.Redcode.Awaiting;
using System.Numerics;
using RotaryHeart.Lib.SerializableDictionary;
using Thirdweb;
using System;
using System.Runtime.InteropServices;
using MetaMask.Unity;

public class LoginView : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void JS_ConnectWalletFromWindow(int walletIndex);           //電腦網頁_連接錢包

    [Header("錢包連接")]
    [SerializeField]
    Button metaMaskConnect_Btn, trustConnect_Btn, binanceConnect_Btn, okxConnect_Btn, coinbaseConnect_Btn;

    ChainData _currentChainData;
    string _address;

    private void Awake()
    {
        ListenerEvent();
    }

    /// <summary>
    /// 事件聆聽
    /// </summary>
    private void ListenerEvent()
    {
        //MetaMask連接
        metaMaskConnect_Btn.onClick.AddListener(() =>
        {
            StartConnect("Metamask");
        });

        //Trust連接
        trustConnect_Btn.onClick.AddListener(() =>
        {
            if (GameDataManager.IsMobilePlatform)
            {
                StartConnect("WalletConnect");
            }
            else
            {
                StartConnect("WalletConnect");
                //ConnectWalletFromWindow(1);
            }
        });

        //OKX連接
        okxConnect_Btn.onClick.AddListener(() =>
        {
            if (GameDataManager.IsMobilePlatform)
            {
                StartConnect("WalletConnect");
            }
            else
            {
                StartConnect("WalletConnect");
                //ConnectWalletFromWindow(2);
            }
        });

        //Binance連接
        binanceConnect_Btn.onClick.AddListener(() =>
        {
            StartConnect("WalletConnect");
        });

        //Coonbase連接
        coinbaseConnect_Btn.onClick.AddListener(() =>
        {
            StartConnect("Coinbase");
        });
    }

    private void Start()
    {
        _currentChainData = ThirdwebManager.Instance.supportedChains.Find(x => x.identifier == ThirdwebManager.Instance.activeChain);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            LoadSceneManager.Instance.LoadScene(SceneEnum.Lobby);
        }   
    }

#region 電腦網頁平台

    /// <summary>
    /// 電腦網頁連接錢包
    /// </summary>
    /// <param name="walletIndex"></param>
    private void ConnectWalletFromWindow(int walletIndex)
    {
        ViewManager.Instance.OpenPartsView(PartsViewEnum.WaitingView);
        JS_ConnectWalletFromWindow(walletIndex);
    }

    /// <summary>
    /// 電腦網頁連接失敗
    /// </summary>
    public void WindowConnectFail()
    {
        ViewManager.Instance.ClosePartsView(PartsViewEnum.WaitingView);
    }

    /// <summary>
    /// 電腦網頁連接成功
    /// </summary>
    public void WindowConnectSuccess()
    {
        LoadSceneManager.Instance.LoadScene(SceneEnum.Lobby);
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

    #region ThirdWallet

    /// <summary>
    /// 開始連接
    /// </summary>
    /// <param name="walletProviderStr">連接形式</param>
    public void StartConnect(string walletProviderStr)
    {
        ViewManager.Instance.OpenPartsView(PartsViewEnum.WaitingView);

        var wc = new WalletConnection(provider: Enum.Parse<WalletProvider>(walletProviderStr), chainId: BigInteger.Parse(_currentChainData.chainId));
        Connect(wc);
    }

    /// <summary>
    /// 連接錢包
    /// </summary>
    /// <param name="wc"></param>
    private async void Connect(WalletConnection wc)
    {
        try
        {
            _address = await ThirdwebManager.Instance.SDK.Wallet.Connect(wc);
        }
        catch (Exception e)
        {
            ViewManager.Instance.ClosePartsView(PartsViewEnum.WaitingView);

            _address = null;
            Debug.LogError($"Failed to connect: {e}");
            return;
        }

        PostConnect(wc);
    }

    /// <summary>
    /// 連接完成
    /// </summary>
    /// <param name="wc"></param>
    private async void PostConnect(WalletConnection wc = null)
    {
        Debug.Log($"Connected to {_address}");

        var addy = _address.ShortenAddress();
        GameDataManager.UserWalletAddress = _address;

        var bal = await ThirdwebManager.Instance.SDK.Wallet.GetBalance();
        var balStr = $"{bal.value.ToEth()} {bal.symbol}";
        GameDataManager.UserWalletBalance = balStr;

        Debug.Log($"Address:{GameDataManager.UserWalletAddress}");
        Debug.Log($"Balance:{GameDataManager.UserWalletBalance}");
        LoadSceneManager.Instance.LoadScene(SceneEnum.Lobby);
    }

#endregion
}
