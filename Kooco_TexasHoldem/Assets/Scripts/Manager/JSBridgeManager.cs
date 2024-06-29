using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class JSBridgeManager : UnitySingleton<JSBridgeManager>
{
    public override void Awake()
    {
        base.Awake();
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
}
