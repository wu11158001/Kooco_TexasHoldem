using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewManager : UnitySingleton<ViewManager>
{
    Canvas mainCanvas;

    Dictionary<ViewEnum, RectTransform> viewDic;
    Dictionary<PartsViewEnum, RectTransform> partsViewDic;
    Stack<RectTransform> viewStack;

    public override void Awake()
    {
        base.Awake();

        Init();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        viewDic = new Dictionary<ViewEnum, RectTransform>();
        partsViewDic = new Dictionary<PartsViewEnum, RectTransform>();
        viewStack = new Stack<RectTransform>();

        mainCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }

    /// <summary>
    /// 初始化介面
    /// </summary>
    /// <param name="view"></param>
    /// <param name="viewName"></param>
    public void InitViewTr(RectTransform view, string viewName)
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
    }

    /// <summary>
    /// 開啟View
    /// </summary>
    /// <param name="viewName"></param>
    public RectTransform OpenView(ViewEnum viewName)
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
            InitViewTr(view, viewName.ToString());
            viewDic.Add(viewName, view);
        }

        viewStack.Push(view);
        return view;
    }

    /// <summary>
    /// 開啟零件類View
    /// </summary>
    /// <param name="partsView"></param>
    /// <param name="parent">父物件</param>
    /// <returns></returns>
    public RectTransform OpenPartsView(PartsViewEnum partsView, Transform parent = null)
    {
        if (parent == null)
        {
            parent = mainCanvas.transform;
        }

        RectTransform view = null;
        if (partsViewDic.ContainsKey(partsView))
        {
            view = partsViewDic[partsView];
            view.SetParent(parent);
            view.gameObject.SetActive(true);
        }
        else
        {
            GameObject gameViewObj = Resources.Load<GameObject>($"PartsView/{partsView}");
            view = Instantiate(gameViewObj).GetComponent<RectTransform>();
            view.SetParent(parent);
            view.gameObject.SetActive(true);
            InitViewTr(view, partsView.ToString());
            partsViewDic.Add(partsView, view);
        }

        return view;
    }

    /// <summary>
    /// 關閉零件類View
    /// </summary>
    /// <param name="partsView"></param>
    public void ClosePartsView(PartsViewEnum partsView)
    {
        if (partsViewDic.ContainsKey(partsView))
        {
            partsViewDic[partsView].gameObject.SetActive(false);
        }
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
