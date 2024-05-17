using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RequestBuf;

abstract public class BaseRequest : MonoBehaviour
{
    public GameServer gameServer;

    protected List<ActionCode> requestDic;
    protected List<ActionCode> roomBroadcastDic;

    public virtual void Awake()
    {
        //RegisterRequest();
        //RegisterRoomBroadcast();
    }

    /// <summary>
    /// 注冊協議
    /// </summary>
    private void RegisterRequest()
    {
        foreach (var request in requestDic)
        {
            RequestManager.Instance.RegisterRequest(request, HandleRequest);
        }
    }

    /// <summary>
    /// 注冊房間廣播協議
    /// </summary>
    private void RegisterRoomBroadcast()
    {
        foreach (var broadCast in roomBroadcastDic)
        {
            RequestManager.Instance.RegisterRoomBroadcast(broadCast, HandleRoomBroadcast);
        }
    }

    /// <summary>
    /// 發送協議
    /// </summary>
    /// <param name="pack"></param>
    protected void SendRequest(MainPack pack)
    {
        RequestManager.Instance.Send(pack, HandleRequest, gameServer);
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

    public virtual void OnDestroy()
    {
        /*foreach (var request in requestDic)
        {
            RequestManager.Instance.RemoveRequest(request);
        }

        foreach (var broadCast in roomBroadcastDic)
        {
            RequestManager.Instance.RemoveRoomBroadcast(broadCast);
        }*/
    }
}
