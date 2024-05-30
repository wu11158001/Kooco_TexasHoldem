using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Thirdweb;

using RequestBuf;
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
        public static double LocalUserChips = 153000; 

        public static DateTime foldTimd = DateTime.Now;
    }
    #endregion

    [Header("Debug工具")]
    [SerializeField]
    GameObject debugObj;
    [SerializeField]
    bool isShowDebug;

    public override void Awake()
    {
        base.Awake();

        debugObj.SetActive(isShowDebug);
    }

    private IEnumerator Start()
    {
        LanguageManager.Instance.LoadLangageJson();

        yield return AssetsManager.Instance.ILoadAssets();

        LoadSceneManager.Instance.LoadScene(SceneEnum.Login);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            LanguageManager.Instance.ChangeLanguage(0);
        }

        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            LanguageManager.Instance.ChangeLanguage(1);
        }

        if (CurrGameServer != null && CurrGameServer.gameObject.activeSelf)
        {
            if ((DateTime.Now - TestInfoData.foldTimd).TotalSeconds > 0.2f)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    TestInfoData.foldTimd = DateTime.Now;
                    CurrGameServer.TextPlayerAction(RequestBuf.ActingEnum.Fold);
                }

                if (Input.GetKeyDown(KeyCode.R))
                {
                    TestInfoData.foldTimd = DateTime.Now;
                    CurrGameServer.TextPlayerAction(RequestBuf.ActingEnum.Raise);
                }

                if (Input.GetKeyDown(KeyCode.C))
                {
                    TestInfoData.foldTimd = DateTime.Now;
                    CurrGameServer.TextPlayerAction(RequestBuf.ActingEnum.Call);
                }

                if (Input.GetKeyDown(KeyCode.I))
                {
                    TestInfoData.foldTimd = DateTime.Now;
                    CurrGameServer.TextPlayerAction(RequestBuf.ActingEnum.AllIn);
                }

                if (Input.GetKeyDown(KeyCode.A))
                {
                    TestInfoData.foldTimd = DateTime.Now;
                    if (CurrGameServer.clientList.Count < CurrGameServer.maxRoomPeople)
                    {
                        MainPack pack = new MainPack();
                        pack.ActionCode = ActionCode.Request_PlayerInOutRoom;

                        PlayerInfoPack playerInfoPack = new PlayerInfoPack();
                        playerInfoPack.UserID = $"00000{TestInfoData.newPlayerId}";
                        playerInfoPack.NickName = $"Player{TestInfoData.newPlayerId}";
                        playerInfoPack.Chips = 6600;

                        PlayerInOutRoomPack playerInOutRoomPack = new PlayerInOutRoomPack();
                        playerInOutRoomPack.IsInRoom = true;
                        playerInOutRoomPack.PlayerInfoPack = playerInfoPack;

                        pack.PlayerInOutRoomPack = playerInOutRoomPack;
                        CurrGameServer.Request_PlayerInOutRoom(pack);

                        TestInfoData.newPlayerId++;
                    }
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    TestInfoData.foldTimd = DateTime.Now;
                    CurrGameServer.TextPlayerAction(RequestBuf.ActingEnum.Fold, true);
                }                

                if (Input.GetKeyDown(KeyCode.Keypad1))
                {
                    CurrGameServer.TestShowFoldPoker(0, 0);
                }
                if (Input.GetKeyDown(KeyCode.Keypad2))
                {
                    CurrGameServer.TestShowFoldPoker(1, 0);
                }
                if (Input.GetKeyDown(KeyCode.Keypad4))
                {
                    CurrGameServer.TestShowFoldPoker(0, 1);
                }
                if (Input.GetKeyDown(KeyCode.Keypad5))
                {
                    CurrGameServer.TestShowFoldPoker(1, 1);
                }
            }
        }
        
    }

    #region LINE

    /// <summary>
    /// 獲取Line信箱
    /// </summary>
    /// <param name="mail"></param>
    public void GetLineMail(string mail)
    {
        GameDataManager.LineMail = mail;
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

    [System.Serializable]
    public class UserProfile
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
        StartCoroutine(GetAccessToken(code));
    }
    private IEnumerator GetAccessToken(string authorizationCode)
    {
        string tokenUrl = "https://api.line.me/oauth2/v2.1/token";
        WWWForm form = new WWWForm();
        form.AddField("grant_type", "authorization_code");
        form.AddField("code", authorizationCode);
        form.AddField("redirect_uri", GameDataManager.RedirectUri);
        form.AddField("client_id", GameDataManager.LineChannelId);
        form.AddField("client_secret", GameDataManager.LineChannelSecret);

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
                var tokenResponse = JsonUtility.FromJson<TokenResponse>(www.downloadHandler.text);
                string idToken = tokenResponse.id_token;

                // 验证 ID 令牌并获取用户信息
                StartCoroutine(VerifyIdToken(idToken));
            }
        }
    }
    /// <summary>
    /// 驗證ID令牌並獲取用戶訊息
    /// </summary>
    /// <param name="idToken"></param>
    /// <returns></returns>
    private IEnumerator VerifyIdToken(string idToken)
    {
        string verifyUrl = "https://api.line.me/oauth2/v2.1/verify";
        WWWForm form = new WWWForm();
        form.AddField("id_token", idToken);
        form.AddField("client_id", GameDataManager.LineChannelId);

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
                var userProfile = JsonUtility.FromJson<UserProfile>(www.downloadHandler.text);
                Debug.Log("User ID: " + userProfile.sub);
                Debug.Log("Name: " + userProfile.name);
                Debug.Log("Picture: " + userProfile.picture);
                Debug.Log("Email: " + userProfile.email);

                GameDataManager.LineMail = userProfile.email;
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
        GameDataManager.IsMobilePlatform = isMobile == "true";
        Debug.Log($"IsMobilePlatform:{isMobile}");
    }

    /// <summary>
    /// 是否在預設瀏覽器內
    /// </summary>
    public void IsDefaultBrowser(string isDefaultBrowser)
    {
        GameDataManager.IsDefaultBrowser = isDefaultBrowser == "true";
        Debug.Log($"isDefaultBrowser:{isDefaultBrowser}");
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
