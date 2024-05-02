using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class BaseView : MonoBehaviour
{
    protected BaseViewRequest requestView;

    public virtual void Awake()
    {
        string requestViewName = $"Request_{this.GetType().Name}";
        Type requestScriptType = Type.GetType(requestViewName);
        requestView = gameObject.AddComponent(requestScriptType) as BaseViewRequest;
        requestView.SetBaseView(this);
        SetRequestView();
    }

    /// <summary>
    /// 設置協議腳本
    /// </summary>
    abstract public void SetRequestView();
}
