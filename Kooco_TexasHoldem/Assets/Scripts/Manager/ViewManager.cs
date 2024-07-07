using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewManager : UnitySingleton<ViewManager>
{
    Canvas mainCanvas;

    public override void Awake()
    {
        base.Awake();

        Init();
    }

    /// <summary>
    /// 獲取Canvas
    /// </summary>
    /// <returns></returns>
    public Canvas GetCanvas()
    {
        return mainCanvas;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        mainCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }

    /// <summary>
    /// 初始化介面
    /// </summary>
    /// <param name="view"></param>
    /// <param name="viewName"></param>
    public void InitViewTr(RectTransform view, string viewName = "")
    {
        view.anchorMax = new Vector2(0.5f, 1);
        view.anchorMin = new Vector2(0.5f, 0);
        view.offsetMax = Vector2.zero;
        view.offsetMin = Vector2.zero;
        view.sizeDelta = new Vector2(Entry.Instance.resolution.x, 0);
        view.anchoredPosition = Vector2.zero;
        view.localScale = Vector3.one;
        view.eulerAngles = Vector3.zero;
        view.name = viewName;
        view.SetSiblingIndex(100);
    }

    /// <summary>
    /// 創建View在主畫面
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public T CreateViewInMainCanvas<T>(GameObject obj) where T : Component
    {
        RectTransform rt = Instantiate(obj).GetComponent<RectTransform>();
        rt.SetParent(mainCanvas.transform);
        InitViewTr(rt, rt.name.Replace("(Clone)", ""));

        return rt.GetComponent<T>();
    }
}
