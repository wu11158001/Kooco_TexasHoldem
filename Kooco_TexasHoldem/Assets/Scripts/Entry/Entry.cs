using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

public class Entry : UnitySingleton<Entry>
{
    #region 測試
    public static GameServer CurrGameServer;
    public static class TestInfoData
    {      
        public static string LocalUserId = "LocalUser";
        public static string NickName = "LocalUserName";

        public static int newPlayerId = 10;
        public static double LocalUserCrypto = 11000;
        public static double LocalUserVirtual = 230200;

        public static DateTime foldTimd = DateTime.Now;
    }
    #endregion

    [Header("版本號")]
    public string version;
    [Header("解析度")]
    public Vector2 resolution;
    [Header("Debug工具")]
    public bool isUsingDebug;
    [SerializeField] 
    GameObject ReporterObj;


    public override void Awake()
    {
        base.Awake();

        #region 測試用

        DataManager.UserId = "LocalUser";
        DataManager.UserCryptoChips = 11000;
        DataManager.UserVCChips = 230200;
        DataManager.UserGoldChips = 65000;
        DataManager.UserStamina = 45;
        DataManager.UserOTProps = 8;

        #endregion
    }

    private IEnumerator Start()
    {
        if (isUsingDebug)
        {
            //Debug工具初始化
            Reporter.I.Initialize();
            Reporter.I.show = false;
        }

        LanguageManager.Instance.LoadLangageJson();

        yield return AssetsManager.Instance.ILoadAssets();
        AudioManager.Instance.StartLoadAudioAssets();

        LoadSceneManager.Instance.LoadScene(SceneEnum.Login);
    }

    private void Update()
    {
        #region 測試操作

        //NFT測試
        if (Input.GetKeyDown(KeyCode.RightControl))
        {
            DataManager.UserWalletAddress = "0xef279977cBC232C667082E06cfC252529513B738";
            NFTManager.Instance.UpdateNFT();
        }

        //移除手牌紀錄
        if (Input.GetKeyDown(KeyCode.F8))
        {
            HandHistoryManager.Instance.OnDeleteHistoryData();
        }

        /*//更換語言_英文
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            LanguageManager.Instance.ChangeLanguage(0);
        }
        //更換語言_繁體中文
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            LanguageManager.Instance.ChangeLanguage(1);
        }*/

        #endregion

        if (Input.GetKeyDown(KeyCode.K))
        {
            Test();
        }
    }

    public List<int> hands1;
    public List<int> hands2;
    public List<int> hands3;
    public List<int> hands4;
    public List<int> hands5;
    public List<int> hands6;
    public int num;
    public List<int> community;
    public int numSimulations;

    void Test()
    {
        Debug.Log("Start");
        System.DateTime startTime = DateTime.Now;
        PokerWinRateCalculator pokerWinRateCalculator = new PokerWinRateCalculator(hands1, community);
        pokerWinRateCalculator.CalculateWinRate((winRate) =>
        {
            Debug.LogError($"Time={(System.DateTime.Now - startTime).TotalSeconds}");
            Debug.LogError($"Rate={winRate}");
        });        
    }

    #region Instagram登入

    /// <summary>
    /// 接收獲取IG用戶訊息
    /// </summary>
    /// <param name="mail"></param>
    public void ReceiveIGInfo(string info)
    {
        Debug.Log($"Get IG Info:{info}");
        DataManager.IGIUserIdAndName = info;
    }

    [System.Serializable]
    public class IGUserInfo
    {
        public string id;
        public string username;
    }

    /// <summary>
    /// Instagram 登入回傳
    /// </summary>
    /// <param name="code"></param>
    public void OnIGLoginCallback(string code)
    {
        Debug.Log("Receive Code: " + code);
        StartCoroutine(GetIGAccessToken(code));
    }

    [System.Serializable]
    public class TokenResponse
    {
        public string access_token;
        public string token_type;
        public string refresh_token;
        public string expires_in;
        public string scope;
        public string id_token;
    }

    public IEnumerator GetIGAccessToken(string accessCode)
    {
        string tokenUrl = "https://api.instagram.com/oauth/access_token";
        WWWForm data = new WWWForm();
        data.AddField("client_id", DataManager.InstagramChannelID);
        data.AddField("client_secret", DataManager.InstagramChannelSecret);
        data.AddField("grant_type", "authorization_code");
        data.AddField("redirect_uri", DataManager.InstagramRedirectUri);
        data.AddField("code", accessCode);

        using (UnityWebRequest www = UnityWebRequest.Post(tokenUrl, data))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                // 解析访问令牌响应
                var tokenResponse = JsonUtility.FromJson<TokenResponse>(www.downloadHandler.text);
                string idToken = tokenResponse.id_token;

                StartCoroutine(IG_VerifyIdToken(idToken));
            }
        }
    }
    /// <summary>
    /// 驗證Token返回用戶資訊
    /// </summary>
    /// <param name="idToken"></param>
    /// <returns></returns>
    public IEnumerator IG_VerifyIdToken(string idToken)
    {
        string tokenUrl = "https://graph.instagram.com/";
        WWWForm data = new WWWForm();
        data.AddField("user_id", DataManager.InstagramChannelID);
        data.AddField("access_token", idToken);

        using (UnityWebRequest www = UnityWebRequest.Post(tokenUrl, data))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                var userProfile = JsonUtility.FromJson<IGUserInfo>(www.downloadHandler.text);

                Debug.Log("User ID: " + userProfile.id);
                Debug.Log("Name: " + userProfile.username);
            }
        }

    }

    #endregion

    #region LINE

    /// <summary>
    /// 接收獲取Line信箱
    /// </summary>
    /// <param name="mail"></param>
    public void ReceiveLineMail(string mail)
    {
        DataManager.LineMail = mail;
    }

    [System.Serializable]
    public class LineTokenResponse
    {
        public string access_token;
        public string token_type;
        public string refresh_token;
        public string expires_in;
        public string scope;
        public string id_token;
    }

    [System.Serializable]
    public class LineUserProfile
    {
        public string iss;
        public string sub;
        public string aud;
        public int exp;
        public int iat;
        public string nonce;
        public string[] amr;
        public string name;
        public string picture;
        public string email;
    }
    /// <summary>
    /// Line登入回傳
    /// </summary>
    /// <param name="code"></param>
    public void OnLineLoginCallback(string code)
    {
        Debug.Log("Recive Line Login Callback:" + code);
        StartCoroutine(IGetLineAccessToken(code));
    }
    /// <summary>
    /// 獲取LineToken
    /// </summary>
    /// <param name="authorizationCode"></param>
    /// <returns></returns>
    private IEnumerator IGetLineAccessToken(string authorizationCode)
    {
        string tokenUrl = "https://api.line.me/oauth2/v2.1/token";
        WWWForm form = new WWWForm();
        form.AddField("grant_type", "authorization_code");
        form.AddField("code", authorizationCode);
        form.AddField("redirect_uri", DataManager.RedirectUri);
        form.AddField("client_id", DataManager.LineChannelId);
        form.AddField("client_secret", DataManager.LineChannelSecret);

        using (UnityWebRequest www = UnityWebRequest.Post(tokenUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                // 解析访问令牌响应
                var tokenResponse = JsonUtility.FromJson<LineTokenResponse>(www.downloadHandler.text);
                string idToken = tokenResponse.id_token;

                // 验证 ID 令牌并获取用户信息
                StartCoroutine(LineVerifyIdToken(idToken));
            }
        }
    }
    /// <summary>
    /// Line驗證ID令牌並獲取用戶訊息
    /// </summary>
    /// <param name="idToken"></param>
    /// <returns></returns>
    private IEnumerator LineVerifyIdToken(string idToken)
    {
        string verifyUrl = "https://api.line.me/oauth2/v2.1/verify";
        WWWForm form = new WWWForm();
        form.AddField("id_token", idToken);
        form.AddField("client_id", DataManager.LineChannelId);

        using (UnityWebRequest www = UnityWebRequest.Post(verifyUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                // 解析用户信息响应
                var userProfile = JsonUtility.FromJson<LineUserProfile>(www.downloadHandler.text);
                Debug.Log("User ID: " + userProfile.sub);
                Debug.Log("Name: " + userProfile.name);
                Debug.Log("Picture: " + userProfile.picture);
                Debug.Log("Email: " + userProfile.email);

                DataManager.LineMail = userProfile.email;
            }
        }
    }

    #endregion

    #region 工具類 

    /// <summary>
    /// 網頁視窗失去焦點
    /// </summary>
    public void OnWindowBlur()
    {
        GameRoomManager.Instance.OnGamePause(true);
    }

    /// <summary>
    /// 網頁視窗獲得焦點
    /// </summary>
    public void OnWindowFocus()
    {

    }

    /// <summary>
    /// 是否為移動平台
    /// </summary>
    public void IsMobilePlatform(string isMobile)
    {
        DataManager.IsMobilePlatform = isMobile == "true";
        Debug.Log($"IsMobilePlatform:{isMobile}");
    }

    /// <summary>
    /// 是否在預設瀏覽器內
    /// </summary>
    public void IsDefaultBrowser(string isDefaultBrowser)
    {
        DataManager.IsDefaultBrowser = isDefaultBrowser == "true";
        Debug.Log($"isDefaultBrowser:{isDefaultBrowser}");
    }


    /// <summary>
    /// 是否在Coinbase瀏覽器內
    /// </summary>
    public void IsInCoinbase(string isCoinbaseBrowser)
    {
        DataManager.IsInCoinbase = isCoinbaseBrowser == "true";
        Debug.Log($"isDefaultBrowser:{isCoinbaseBrowser}");
    }

    /// <summary>
    /// 網頁debug
    /// </summary>
    /// <param name="str"></param>
    public void HtmlDebug(string str)
    {
        Debug.Log($"Browser Debug: {str}");
    }

    #endregion
}
