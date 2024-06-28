using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class JSBridgeManager : UnitySingleton<JSBridgeManager>
{
    [DllImport("__Internal")]
    private static extern string JS_GetBrowserInfo();                                            //獲取瀏覽器訊息
    [DllImport("__Internal")]
    private static extern void JS_LocationHref(string url);                                      //本地頁面跳轉
    [DllImport("__Internal")]
    private static extern void JS_WindowClose();                                                 //關閉頁面
    [DllImport("__Internal")]
    private static extern void JS_OpenNewBrowser(string mail, string igIdAndName);               //開啟新瀏覽器
    [DllImport("__Internal")]
    private static extern void JS_CopyText(string copyStr);                                      //複製文字

    public override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// 獲取瀏覽器訊息
    /// </summary>
    public void GetBrowserInfo()
    {
        JS_GetBrowserInfo();
    }

    /// <summary>
    /// 本地頁面跳轉
    /// </summary>
    /// <param name="url"></param>
    public void LocationHref(string url)
    {
        JS_LocationHref(url);
    }

    /// <summary>
    /// 關閉頁面
    /// </summary>
    public void WindowClose()
    {
        JS_WindowClose();
    }

    /// <summary>
    /// 開啟新瀏覽器
    /// </summary>
    /// <param name="mail"></param>
    /// <param name="igIdAndName"></param>
    public void OpenNewBrowser(string mail, string igIdAndName)
    {
        JS_OpenNewBrowser(mail, igIdAndName);
    }

    /// <summary>
    /// Webgl複製文字
    /// </summary>
    /// <param name="copyStr"></param>
    public void CopyText(string copyStr)
    {
        JS_CopyText(copyStr);
    }
}
