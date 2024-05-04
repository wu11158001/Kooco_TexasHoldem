using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : UnitySingleton<UIManager>
{
    static Canvas mainCanvas;

    private Dictionary<ViewName, RectTransform> viewDic = new Dictionary<ViewName, RectTransform>();
    private Dictionary<ViewName, RectTransform> partsViewDic = new Dictionary<ViewName, RectTransform>();
    private Stack<RectTransform> viewStack = new Stack<RectTransform>();

    public override void Awake()
    {
        base.Awake();

        mainCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }

    /// <summary>
    /// 初始化介面
    /// </summary>
    /// <param name="view"></param>
    private void InitViewTr(RectTransform view)
    {
        view.anchorMax = Vector2.one;
        view.anchorMin = Vector2.zero;
        view.anchoredPosition = Vector2.zero;
        view.localScale = Vector3.one;
        view.eulerAngles = Vector3.zero;
        view.offsetMax = Vector2.zero;
        view.offsetMin = Vector2.zero;
    }

    /// <summary>
    /// 開啟View
    /// </summary>
    /// <param name="viewName"></param>
    public RectTransform OpenView(ViewName viewName)
    {
        if (viewStack.Count > 0)
        {
            viewStack.Peek().gameObject.SetActive(false);
        }

        RectTransform view = null;
        if (viewDic.ContainsKey(viewName))
        {
            view = viewDic[viewName];
            view.gameObject.SetActive(true);
        }
        else
        {
            GameObject gameViewObj = Resources.Load<GameObject>($"View/{viewName}");
            view = CreateUIObj(gameViewObj);
            InitViewTr(view);
            viewDic.Add(viewName, view);
        }

        viewStack.Push(view);

        return view;
    }

    /// <summary>
    /// 開啟零件類View
    /// </summary>
    /// <param name="viewName"></param>
    public RectTransform OpenPartsView(ViewName viewName)
    {
        RectTransform view = null;
        if (partsViewDic.ContainsKey(viewName))
        {
            view = partsViewDic[viewName];
            view.gameObject.SetActive(true);
        }
        else
        {
            GameObject gameViewObj = Resources.Load<GameObject>($"PartsView/{viewName}");
            view = CreateUIObj(gameViewObj);
            InitViewTr(view);
            partsViewDic.Add(viewName, view);
        }

        return view;
    }

    /// <summary>
    /// 創建UI物件
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public RectTransform CreateUIObj(GameObject obj)
    {
        RectTransform rt = Instantiate(obj).GetComponent<RectTransform>();
        rt.SetParent(mainCanvas.transform);

        return rt;
    }
}
