using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : UnitySingleton<UIManager>
{
    static Canvas mainCanvas;

    private Dictionary<ViewEnum, RectTransform> viewDic = new Dictionary<ViewEnum, RectTransform>();
    private Dictionary<ViewEnum, RectTransform> partsViewDic = new Dictionary<ViewEnum, RectTransform>();
    private Stack<RectTransform> viewStack = new Stack<RectTransform>();

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
        viewDic.Clear();
        partsViewDic.Clear();
        viewStack.Clear();

        mainCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }

    /// <summary>
    /// 初始化介面
    /// </summary>
    /// <param name="view"></param>
    /// <param name="viewName"></param>
    private void InitViewTr(RectTransform view, string viewName)
    {
        view.anchorMax = new Vector2(0.5f, 1);
        view.anchorMin = new Vector2(0.5f, 0);
        view.offsetMax = Vector2.zero;
        view.offsetMin = new Vector2(-540, 0);
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
    /// <param name="viewName"></param>
    public RectTransform OpenPartsView(ViewEnum viewName)
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
            InitViewTr(view, viewName.ToString());
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
