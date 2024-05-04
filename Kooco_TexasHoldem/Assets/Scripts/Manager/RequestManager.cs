using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Reflection;

using RequestBuf;

public class RequestManager : UnitySingleton<RequestManager>
{
    private Dictionary<ActionCode, UnityAction<MainPack>> requestDic = new Dictionary<ActionCode, UnityAction<MainPack>>();
    private Dictionary<ActionCode, UnityAction<MainPack>> roomBroadcastDic = new Dictionary<ActionCode, UnityAction<MainPack>>();

    public override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// 注冊協議
    /// </summary>
    /// <param name="actionCode"></param>
    /// <param name="callback"></param>
    public void RegisterRequest(ActionCode actionCode, UnityAction<MainPack> callback)
    {
        requestDic.Add(actionCode, callback);
    }

    /// <summary>
    /// 移除協議監聽
    /// </summary>
    /// <param name="actionCode"></param>
    public void RemoveRequest(ActionCode actionCode)
    {
        requestDic.Remove(actionCode);
    }

    /// <summary>
    /// 註冊房間廣播協議
    /// </summary>
    /// <param name="actionCode"></param>
    /// <param name="callback"></param>
    public void RegisterRoomBroadcast(ActionCode actionCode, UnityAction<MainPack> callback)
    {
        roomBroadcastDic.Add(actionCode, callback);
    }

    /// <summary>
    /// 移除房間廣播監聽
    /// </summary>
    /// <param name="actionCode"></param>
    public void RemoveRoomBroadcast(ActionCode actionCode)
    {
        roomBroadcastDic.Remove(actionCode);
    }

    /// <summary>
    /// 發送協議
    /// </summary>
    /// <param name="pack"></param>
    /// <param name="callback"></param>
    public void Send(MainPack pack, UnityAction<MainPack> callback)
    {
        string metname = pack.ActionCode.ToString();
        MethodInfo method = Entry.Instance.gameServer.GetType().GetMethod(metname);
        if (method != null)
        {
            object[] objs = new object[] { pack };
            StartCoroutine(InvokeMethod(method, Entry.Instance.gameServer, objs));
        }
        else
        {
            Debug.LogError($"無法找到 {metname} 方法。");
        }
    }

    /// <summary>
    /// 處理協議
    /// </summary>
    /// <param name="pack"></param>
    public void HandleRequest(MainPack pack)
    {
        if (pack.SendModeCode == SendModeCode.RoomBroadcast)
        {
            //房間廣播
            if (roomBroadcastDic.ContainsKey(pack.ActionCode))
            {
                UnityMainThreadDispatcher.Instance.Enqueue(() =>
                {
                    roomBroadcastDic[pack.ActionCode](pack);
                });                
            }
            else
            {
                Debug.LogError("沒有相關房間廣播協議:" + pack.ActionCode);
            }
        }
        else
        {
            //一般協議
            if (requestDic.ContainsKey(pack.ActionCode))
            {
                UnityMainThreadDispatcher.Instance.Enqueue(() =>
                {
                    requestDic[pack.ActionCode](pack);
                });                
            }
            else
            {
                Debug.LogWarning("沒有相關協議:" + pack.ActionCode);
            }
        }
    }

    private IEnumerator InvokeMethod(MethodInfo method, object target, object[] parameters)
    {
        yield return method.Invoke(target, parameters);
    }
}
