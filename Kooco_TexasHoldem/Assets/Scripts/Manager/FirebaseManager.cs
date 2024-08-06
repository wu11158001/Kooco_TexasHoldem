using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

public class FirebaseManager : UnitySingleton<FirebaseManager>
{
    [Header("用戶資料路徑名稱")]
    public const string USER_DATA_PATH = "user/";                                           //Database用戶資料路徑

    [Header("用戶資料內容路徑名稱")]
    public const string USER_ID = "userId";                                                 //用戶ID
    public const string PHONE_NUMBER = "phoneNumber";                                       //登入手機號
    public const string PASSWORD = "password";                                              //登入密碼
    public const string NICKNAME = "nickname";                                              //暱稱
    public const string AVATAR_INDEX = "avatarIndex";                                       //頭像編號
    public const string INVITATION_CODE = "invitationCode";                                 //邀請碼
    public const string BOUND_INVITER_ID = "boundInviterId";                                //綁定的邀請者Id
    public const string LINE_TOKEN = "lineToken";                                           //Line Token

    public override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// 讀取資料回傳
    /// </summary>
    /// <param name="jsonData"></param>
    public T OnFirebaseDataRead<T>(string jsonData) where T : class
    {
        var data = JsonConvert.DeserializeObject<T>(jsonData);

        if (data == null)
        {
            Debug.LogError("Firebase read error or data is null.");
            return default;
        }
        else
        {
            Debug.Log("Firebase data read: " + JsonUtility.ToJson(data, true));
            return data;
        }
    }


    /// <summary>
    /// 移除資料
    /// </summary>
    [Serializable]
    public class RemoveData
    {
        public bool success;
        public string error;
    }
    /// <summary>
    /// 移除資料回傳
    /// </summary>
    /// <param name="jsonData"></param>
    public void OnRemoveDataCallback(string jsonData)
    {
        var data = JsonUtility.FromJson<RemoveData>(jsonData);

        if (data.error != null)
        {
            Debug.LogError("Firebase delete error: " + data.error);
        }
        else
        {
            Debug.Log("Firebase data deleted successfully.");
        }
    }
}

/// <summary>
/// 用戶資料查詢
/// </summary>
[Serializable]
public class CheckUserData
{
    public string exists;                   //差尋結果(true/false)
    public string phoneNumber;              //資料符合的用戶手機號
    public string error;                    //錯誤訊息
}

/// <summary>
/// 用戶資料
/// </summary>
[Serializable]
public class AccountData
{
    public string userId;                   //用戶ID
    public string phoneNumber;              //登入手機號
    public string password;                 //登入密碼
    public string nickname;                 //暱稱
    public int avatarIndex;                 //頭像編號
    public string invitationCode;           //邀請碼
    public string boundInviterId;           //綁定的邀請者Id
    public string lineToken;                //Line Token
}
