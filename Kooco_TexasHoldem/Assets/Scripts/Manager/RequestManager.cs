using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Reflection;

using RequestBuf;

public class RequestManager : UnitySingleton<RequestManager>
{
    private Dictionary<ActionCode, UnityAction<MainPack>> requsetDic = new Dictionary<ActionCode, UnityAction<MainPack>>();
    private Dictionary<ActionCode, UnityAction<MainPack>> broadcastDic = new Dictionary<ActionCode, UnityAction<MainPack>>();

    public override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// 註冊廣播事件
    /// </summary>
    /// <param name="actionCode"></param>
    /// <param name="callback"></param>
    public void RegisterBroadcast(ActionCode actionCode, UnityAction<MainPack> callback)
    {
        broadcastDic.Add(actionCode, callback);
    }

    /// <summary>
    /// 發送協議
    /// </summary>
    /// <param name="pack"></param>
    /// <param name="callback"></param>
    public void Send(MainPack pack, UnityAction<MainPack> callback)
    {
        if (!requsetDic.ContainsKey(pack.ActionCode))
        {
            requsetDic.Add(pack.ActionCode, callback);
        }

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
            if (broadcastDic.ContainsKey(pack.ActionCode))
            {
                UnityMainThreadDispatcher.Instance.Enqueue(() =>
                {
                    broadcastDic[pack.ActionCode](pack);
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
            if (requsetDic.ContainsKey(pack.ActionCode))
            {
                UnityMainThreadDispatcher.Instance.Enqueue(() =>
                {
                    requsetDic[pack.ActionCode](pack);
                    requsetDic.Remove(pack.ActionCode);
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
