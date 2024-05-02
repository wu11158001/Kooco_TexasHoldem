using RequestBuf;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

abstract public class BaseViewRequest : MonoBehaviour
{
    protected BaseView thisBaseView;

    public virtual void Awake()
    {
        RegisterBroadcast();
    }

    /// <summary>
    /// 設置客戶端介面
    /// </summary>
    /// <param name="baseView"></param>
    public void SetBaseView(BaseView baseView)
    {
        thisBaseView = baseView;
        SetThisView();
    }

    /// <summary>
    /// 設置客戶端介面
    /// </summary>
    abstract public void SetThisView();

    /// <summary>
    /// 注冊廣播事件
    /// </summary>
    abstract public void RegisterBroadcast();

    /// <summary>
    /// 發送協議
    /// </summary>
    /// <param name="pack"></param>
    public virtual void SendRequest(MainPack pack)
    {
        RequestManager.Instance.Send(pack, HandleRequest);
    }

    /// <summary>
    /// 處理協議
    /// </summary>
    /// <param name="pack"></param>
    public virtual void HandleRequest(MainPack pack)
    {

    }

    /// <summary>
    /// 處理房間廣播協議
    /// </summary>
    /// <param name="pack"></param>
    public virtual void HandleRoomBroadcast(MainPack pack)
    {

    }
}
