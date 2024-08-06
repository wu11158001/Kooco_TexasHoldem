using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

public class JSBridgeManager : UnitySingleton<JSBridgeManager>
{
    public override void Awake()
    {
        base.Awake();
    }

    #region reCAPTCHA

    [DllImport("__Internal")]
    private static extern bool JS_SetupRecaptchaVerifier();
    /// <summary>
    /// 設置Recaptcha驗證監聽
    /// </summary>
    /// <returns></returns>
    public void SetupRecaptchaVerifier()
    {
        JS_SetupRecaptchaVerifier();
    }

    [DllImport("__Internal")]
    private static extern bool JS_OpenRecaptchaTool();
    /// <summary>
    /// 開啟Recaptcha小工具
    /// </summary>
    /// <returns></returns>
    public void OpenRecaptchaTool()
    {
        JS_OpenRecaptchaTool();
    }

    [DllImport("__Internal")]
    private static extern bool JS_CloseRecaptchaTool();
    /// <summary>
    /// 關閉Recaptcha小工具
    /// </summary>
    /// <returns></returns>
    public void CloseRecaptchaTool()
    {
        JS_CloseRecaptchaTool();
    }

    [DllImport("__Internal")]
    private static extern bool JS_TriggerRecaptcha(string phoneNumber);
    /// <summary>
    /// 觸發Recaptcha驗證
    /// </summary>
    /// <returns></returns>
    public void TriggerRecaptcha(string phoneNumber)
    {
        JS_TriggerRecaptcha(phoneNumber);
    }

    #endregion

    #region Firebase

    [DllImport("__Internal")]
    private static extern bool JS_FirebaseVerifyCode(string code, string objNamePtr, string callbackFunPtr);
    /// <summary>
    /// 驗證OTP
    /// </summary>
    /// <param name="code">OTP Code</param>
    /// <param name="objNamePtr">回傳物件名</param>
    /// <param name="callbackFunPtr">回傳方法名</param>
    public void FirebaseVerifyCode(string code, string objNamePtr, string callbackFunPtr)
    {
        JS_FirebaseVerifyCode(code,
                              objNamePtr,
                              callbackFunPtr);
    }

    [DllImport("__Internal")]
    private static extern bool JS_StartListeningForDataChanges(string pathPtr, string objNamePtr, string callbackFunPtr);
    /// <summary>
    /// 開始監聽資料
    /// </summary>
    /// <param name="pathPtr">資料路徑</param>
    /// <param name="objNamePtr">回傳物件名</param>
    /// <param name="callbackFunPtr">回傳方法名</param>
    public void StartListeningForDataChanges(string pathPtr, string objNamePtr, string callbackFunPtr)
    {
        JS_StartListeningForDataChanges(pathPtr,
                                        objNamePtr,
                                        callbackFunPtr);
    }

    [DllImport("__Internal")]
    private static extern bool JS_StopListeningForDataChanges(string pathPtr);
    /// <summary>
    /// 停止監聽資料
    /// </summary>
    /// <param name="pathPtr">資料路徑</param>
    public void StopListeningForDataChanges(string pathPtr)
    {
        JS_StopListeningForDataChanges(pathPtr);
    }

    [DllImport("__Internal")]
    private static extern bool JS_WriteDataFromFirebase(string refPathPtr, string jsonDataPtr, string objNamePtr = null, string callbackFunPtr = null);
    /// <summary>
    /// 寫入資料
    /// </summary>
    /// <param name="refPathPtr">資料路徑</param>
    /// <param name="data">資料</param>
    public void WriteDataFromFirebase(string refPathPtr, Dictionary<string, object> data, string objNamePtr = null, string callbackFunPtr = null)
    {
        string jsonData = JsonConvert.SerializeObject(data);
        JS_WriteDataFromFirebase(refPathPtr,
                                 jsonData,
                                 objNamePtr,
                                 callbackFunPtr);
    }

    [DllImport("__Internal")]
    private static extern bool JS_UpdateDataFromFirebase(string refPathPtr, string jsonDataPtr);
    /// <summary>
    /// 修改與擴充資料
    /// </summary>
    /// <param name="refPathPtr">資料路徑</param>
    /// <param name="data">資料</param>
    public void UpdateDataFromFirebase(string refPathPtr, Dictionary<string, object> data)
    {
        string jsonData = JsonConvert.SerializeObject(data);
        JS_UpdateDataFromFirebase(refPathPtr,
                                  jsonData);
    }

    [DllImport("__Internal")]
    private static extern bool JS_ReadDataFromFirebase(string refPathPtr, string objNamePtr, string callbackFunPtr);
    /// <summary>
    /// 讀取資料
    /// </summary>
    /// <param name="refPathPtr">資料路徑</param>
    /// <param name="objNamePtr">回傳物件名</param>
    /// <param name="callbackFunPtr">回傳方法名</param>
    public void ReadDataFromFirebase(string refPathPtr, string objNamePtr, string callbackFunPtr)
    {
        JS_ReadDataFromFirebase(refPathPtr,
                                objNamePtr,
                                callbackFunPtr);
    }

    [DllImport("__Internal")]
    private static extern bool JS_RemoveDataFromFirebase(string refPathPtr, string objNamePtr, string callbackFunPtr);
    /// <summary>
    /// 移除資料
    /// </summary>
    /// <param name="refPathPtr">資料路徑</param>
    /// <param name="objNamePtr">回傳物件名</param>
    /// <param name="callbackFunPtr">回傳方法名</param>
    public void RemoveDataFromFirebase(string refPathPtr, string objNamePtr, string callbackFunPtr)
    {
        JS_RemoveDataFromFirebase(refPathPtr,
                                  objNamePtr,
                                  callbackFunPtr);
    }

    [DllImport("__Internal")]
    private static extern bool JS_CheckUserDataExist(string keyPtr, string valuePtr, string releaseTypePtr, string objNamePtr, string callbackFunPtr);
    /// <summary>
    /// 檢查用戶資料是否已存在
    /// </summary>
    /// <param name="keyPtr">資料路徑</param>
    /// <param name="valuePtr">查詢的值</param>
    /// <param name="objNamePtr">回傳物件名</param>
    /// <param name="callbackFunPtr">回傳方法名</param>
    public void CheckUserDataExist(string keyPtr, string valuePtr, string objNamePtr, string callbackFunPtr)
    {
        JS_CheckUserDataExist(keyPtr,
                              valuePtr,
                              $"{Entry.Instance.releaseType}",
                              objNamePtr,
                              callbackFunPtr);
    }

    #endregion

    #region 錢包

    [DllImport("__Internal")]
    private static extern bool JS_WindowCheckWallet(string walletName);
    /// <summary>
    /// Window檢查錢包擴充是否安裝
    /// </summary>
    /// <param name="wallet"></param>
    /// <returns></returns>
    public bool WindowCheckWallet(WalletEnum wallet)
    {
        return JS_WindowCheckWallet(wallet.ToString());
    }

    [DllImport("__Internal")]
    private static extern bool JS_OpenDownloadWallet(string walletName);
    /// <summary>
    /// 開啟下載錢包分頁
    /// </summary>
    /// <param name="wallet"></param>
    /// <returns></returns>
    public bool OpenDownloadWallet(WalletEnum wallet)
    {
        return JS_OpenDownloadWallet(wallet.ToString());
    }

    #endregion

    #region 工具

    [DllImport("__Internal")]
    private static extern bool JS_ClearUrlQueryString();
    /// <summary>
    /// 清除URL資料
    /// </summary>
    public void ClearUrlQueryString()
    {
        JS_ClearUrlQueryString();
    }

    [DllImport("__Internal")]
    private static extern bool JS_Share(string title, string content, string url);
    /// <summary>
    /// 分享
    /// </summary>
    public void Share(string title, string content, string url)
    {
        JS_Share(title, content, url);
    }

    [DllImport("__Internal")]
    private static extern bool JS_Reload();
    /// <summary>
    /// 瀏覽器重新整理
    /// </summary>
    public void Reload()
    {
        JS_Reload();
    }

    [DllImport("__Internal")]
    private static extern string JS_GetBrowserInfo();
    /// <summary>
    /// 獲取瀏覽器訊息
    /// </summary>
    public void GetBrowserInfo()
    {
        JS_GetBrowserInfo();
    }

    [DllImport("__Internal")]
    private static extern void JS_LocationHref(string url);
    /// <summary>
    /// 本地頁面跳轉
    /// </summary>
    /// <param name="url"></param>
    public void LocationHref(string url)
    {
        JS_LocationHref(url);
    }

    [DllImport("__Internal")]
    private static extern void JS_WindowClose();
    /// <summary>
    /// 關閉頁面
    /// </summary>
    public void WindowClose()
    {
        JS_WindowClose();
    }

    [DllImport("__Internal")]
    private static extern void JS_OpenNewBrowser(string mail, string igIdAndName);
    /// <summary>
    /// 開啟新瀏覽器
    /// </summary>
    /// <param name="mail"></param>
    /// <param name="igIdAndName"></param>
    public void OpenNewBrowser(string mail, string igIdAndName)
    {
        JS_OpenNewBrowser(mail, igIdAndName);
    }

    [DllImport("__Internal")]
    private static extern void JS_CopyString(string copyStr);
    /// <summary>
    /// Webgl複製文字
    /// </summary>
    /// <param name="copyStr"></param>
    public void CopyString(string copyStr)
    {
        JS_CopyString(copyStr);
    }

    #endregion

    //  Line加客服好友
    [DllImport("__Internal")]
    private static extern void JS_LineService(string url);

    public void onLineService(string url)
    {
        JS_LineService(url);
    }

}
