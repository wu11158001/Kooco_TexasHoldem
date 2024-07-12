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
    /// 創建View在當前Canvas畫面
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public T CreateViewInCurrCanvas<T>(GameObject obj) where T : Component
    {
        Transform parent = mainCanvas.transform;
        if (GameRoomManager.Instance.GetGameRoomCanvas().sortingOrder > 0)
        {
            parent = GameRoomManager.Instance.GetGameRoomCanvas().transform;
        }

        GameRoomManager.Instance.IsCanMoveSwitch = false;

        RectTransform rt = Instantiate(obj).GetComponent<RectTransform>();
        rt.SetParent(parent);
        InitViewTr(rt, rt.name.Replace("(Clone)", ""));

        if (rt.TryGetComponent<T>(out T component))
        {
            return component;
        }
        else
        {
            Debug.LogError("Not Get Component");
            return null;
        }
    }

    /// <summary>
    /// 開啟確認介面
    /// </summary>
    /// <returns></returns>
    public ConfirmView OpenConfirmView()
    {
        GameObject obj = Resources.Load<GameObject>("Prefab/CommonView/ConfirmView");
        return CreateViewInCurrCanvas<ConfirmView>(obj);
    }
}
