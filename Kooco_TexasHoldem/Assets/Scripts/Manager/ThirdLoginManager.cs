using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

public class ThirdLoginManager : UnitySingleton<ThirdLoginManager>
{
    public override void Awake()
    {
        base.Awake();
    }

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
    /// 獲取Line授權Token
    /// </summary>
    /// <param name="authorizationCode"></param>
    /// <returns></returns>
    private IEnumerator IGetLineAccessToken(string authorizationCode)
    {
        string tokenUrl = "https://api.line.me/oauth2/v2.1/token";
        WWWForm form = new WWWForm();
        form.AddField("grant_type", "authorization_code");
        form.AddField("code", authorizationCode);
        form.AddField("redirect_uri", DataManager.GetRedirectUri());
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

                Debug.Log($"Line Token:{idToken}");
                DataManager.GetLineToken = idToken;
            }
        }
    }
    /// <summary>
    /// Line驗證ID令牌並獲取用戶訊息
    /// </summary>
    /// <param name="idToken">line token</param>
    /// <param name="callback">資料獲取後回傳</param>
    /// <returns></returns>
    public IEnumerator LineVerifyIdToken(string idToken, UnityAction callback)
    {
        string verifyUrl = DataManager.LineVerifyUrl;

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
                DataManager.LinePicture = userProfile.picture;
            }
        }
    }

    #endregion
}
