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
    private static extern string JS_GetBrowserInfo();                                            //獲取瀏覽器訊息
    [DllImport("__Internal")]
    private static extern void JS_LocationHref(string url);                                      //本地頁面跳轉
    [DllImport("__Internal")]
    private static extern void JS_WindowClose();                                                 //關閉頁面
    [DllImport("__Internal")]
    private static extern void JS_OpenNewBrowser(string mail, string igIdAndName);              //開啟新瀏覽器

    [Header("錢包連接")]
    [SerializeField]
    Button metaMaskConnect_Btn, trustConnect_Btn, binanceConnect_Btn, okxConnect_Btn, coinbaseConnect_Btn;

    [Header("綁定")]
    [SerializeField]
    Button line_Btn, ig_Btn;
    [SerializeField]
    Text lineMail_Txt, igIdAndName_Txt;

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
        //IG登入
        ig_Btn.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayConfirmClick();
            StartInstagram();
        });

        //Line登入
        line_Btn.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayConfirmClick();
            StartLineLogin();
        });

        //MetaMask連接
        metaMaskConnect_Btn.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayConfirmClick();
            StartConnect("Metamask");
        });

        //Trust連接
        trustConnect_Btn.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayConfirmClick();
            StartConnect("Metamask");
        });

        //OKX連接
        okxConnect_Btn.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayConfirmClick();
            StartConnect("Metamask");
        });

        //Binance連接
        binanceConnect_Btn.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayConfirmClick();
            if (DataManager.IsMobilePlatform)
            {
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
            AudioManager.Instance.PlayConfirmClick();
            StartConnect("Coinbase");
        });
    }

    private void Start()
    {
        _currentChainData = ThirdwebManager.Instance.supportedChains.Find(x => x.identifier == ThirdwebManager.Instance.activeChain);

        //查詢瀏覽器訊息
        if (DataManager.IsMobilePlatform)
        {
            //JS_GetBrowserInfo();
        }

        //已有Line Mail
        if (!string.IsNullOrEmpty(DataManager.LineMail))
        {
            line_Btn.interactable = false;
            lineMail_Txt.text = DataManager.LineMail;
        }

        //已有IG授權碼
        if (!string.IsNullOrEmpty(DataManager.IGIUserIdAndName))
        {
            ig_Btn.interactable = false;
            igIdAndName_Txt.text = DataManager.IGIUserIdAndName;
        }

        ///自動連接Coinbase
        if (!DataManager.IsNotFirstInLogin && 
            DataManager.IsInCoinbase)
        {
            Debug.Log("Aoto Connect Coinbase");
            StartConnect("Coinbase");
        }

        DataManager.IsNotFirstInLogin = true;
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
        if (DataManager.IsMobilePlatform)
        {
            try
            {
                string add = await ThirdwebManager.Instance.SDK.Wallet.GetAddress();
                var bal = await ThirdwebManager.Instance.SDK.Wallet.GetBalance();
                var balStr = $"{bal.value.ToEth()} {bal.symbol}";

                DataManager.UserWalletAddress = add;
                DataManager.UserWalletBalance = balStr;

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

    #region Instafram

    /// <summary>
    /// 開始Instagram登入
    /// </summary>
    public void StartInstagram()
    {
        string authUrl = $"https://api.instagram.com/oauth/authorize?client_id=" +
                         $"{DataManager.InstagramChannelID}&redirect_uri={DataManager.InstagramRedirectUri}" +
                         $"&scope=user_profile,user_media&response_type=code";
        Application.OpenURL(authUrl);
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
                         $"client_id={DataManager.LineChannelId}&" +
                         $"redirect_uri={DataManager.RedirectUri}&" +
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

    #region ThirdWallet

    /// <summary>
    /// 開始連接
    /// </summary>
    /// <param name="walletProviderStr">連接形式</param>
    public void StartConnect(string walletProviderStr)
    {
        if (DataManager.IsMobilePlatform && 
            DataManager.IsDefaultBrowser &&
            Application.platform != RuntimePlatform.IPhonePlayer)
        {
            //在預設瀏覽器內
            JS_OpenNewBrowser(DataManager.LineMail, DataManager.IGIUserIdAndName);
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
        DataManager.UserWalletAddress = _address;

        var bal = await ThirdwebManager.Instance.SDK.Wallet.GetBalance();
        var balStr = $"{bal.value.ToEth()} {bal.symbol}";
        DataManager.UserWalletBalance = balStr;

        Debug.Log($"Address:{DataManager.UserWalletAddress}");
        Debug.Log($"Balance:{DataManager.UserWalletBalance}");
        LoadSceneManager.Instance.LoadScene(SceneEnum.Lobby);
    }

    #endregion
}
