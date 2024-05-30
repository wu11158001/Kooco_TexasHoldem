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
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

public class LoginView : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern string JS_GetBrowserInfo();                   //獲取瀏覽器訊息
    [DllImport("__Internal")]
    private static extern void JS_LocationHref(string url);             //本地頁面跳轉
    [DllImport("__Internal")]
    private static extern void JS_WindowClose();                        //關閉頁面
    [DllImport("__Internal")]
    private static extern void JS_OpenNewBrowser(string mail);          //開啟新瀏覽器

    [Header("錢包連接")]
    [SerializeField]
    Button metaMaskConnect_Btn, trustConnect_Btn, binanceConnect_Btn, okxConnect_Btn, coinbaseConnect_Btn;

    [Header("綁定")]
    [SerializeField]
    Button line_Btn;
    [SerializeField]
    Text lineMail_Txt;

    ChainData _currentChainData;
    string _address;

    private void Awake()
    {
        ListenerEvent();
    }

    public Button tt;

    /// <summary>
    /// 事件聆聽
    /// </summary>
    private void ListenerEvent()
    {
        //Line登入
        line_Btn.onClick.AddListener(() =>
        {
            StartLineLogin();
        });

        //MetaMask連接
        metaMaskConnect_Btn.onClick.AddListener(() =>
        {
            StartConnect("Metamask");
        });

        //Trust連接
        trustConnect_Btn.onClick.AddListener(() =>
        {
            StartConnect("Metamask");
        });

        //OKX連接
        okxConnect_Btn.onClick.AddListener(() =>
        {
            StartConnect("Metamask");
        });

        //Binance連接
        binanceConnect_Btn.onClick.AddListener(() =>
        {
            if (GameDataManager.IsMobilePlatform)
            {
                Debug.Log("Binance~");
                StartConnect("Metamask");
            }
            else
            {
                StartConnect("WalletConnect");
            }                

            InvokeRepeating(nameof(TryBinanceConnect), 8, 3);
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

        if (GameDataManager.IsMobilePlatform)
        {
            JS_GetBrowserInfo();
        }

        //已有Line Mail
        if (!string.IsNullOrEmpty(GameDataManager.LineMail))
        {
            line_Btn.interactable = false;
            lineMail_Txt.text = GameDataManager.LineMail;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            LoadSceneManager.Instance.LoadScene(SceneEnum.Lobby);
        }   
    }

    /// <summary>
    /// 嘗試連接Binance
    /// </summary>
    async public void TryBinanceConnect()
    {
        if (GameDataManager.IsMobilePlatform)
        {
            try
            {
                string add = await ThirdwebManager.Instance.SDK.Wallet.GetAddress();
                var bal = await ThirdwebManager.Instance.SDK.Wallet.GetBalance();
                var balStr = $"{bal.value.ToEth()} {bal.symbol}";

                GameDataManager.UserWalletAddress = add;
                GameDataManager.UserWalletBalance = balStr;

                CancelInvoke(nameof(TryBinanceConnect));
                ViewManager.Instance.ClosePartsView(PartsViewEnum.WaitingView);
                LoadSceneManager.Instance.LoadScene(SceneEnum.Lobby);
            }
            catch (Exception)
            {
                Debug.LogError("Try Connect Fail!!!");
            }
        }
    }

    #region ThirdWallet

    /// <summary>
    /// 開始連接
    /// </summary>
    /// <param name="walletProviderStr">連接形式</param>
    public void StartConnect(string walletProviderStr)
    {
        if (GameDataManager.IsMobilePlatform && 
            GameDataManager.IsDefaultBrowser &&
            Application.platform != RuntimePlatform.IPhonePlayer)
        {
            //在預設瀏覽器內
            JS_OpenNewBrowser(GameDataManager.LineMail);
            return;
        }

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
            CancelInvoke(nameof(TryBinanceConnect));
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

    #region LINE

    /// <summary>
    /// 開始Line登入
    /// </summary>
    public void StartLineLogin()
    {
        string state = GenerateRandomString();
        string nonce = GenerateRandomString();
        string authUrl = $"https://access.line.me/oauth2/v2.1/authorize?response_type=code&" +
                         $"client_id={GameDataManager.LineChannelId}&" +
                         $"redirect_uri={GameDataManager.RedirectUri}&" +
                         $"state={state}&" +
                         $"scope=profile%20openid%20email&nonce={nonce}";


        JS_LocationHref(authUrl);
    }
    private string GenerateRandomString(int length = 16)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new System.Random();
        return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
    }

    #endregion
}
