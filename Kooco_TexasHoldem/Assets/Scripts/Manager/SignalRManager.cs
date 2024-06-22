using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

// https://docs.microsoft.com/zh-cn/dotnet/api/microsoft.aspnetcore.signalr.client?view=aspnetcore-5.0
public class SignalRManager : UnitySingleton<SignalRManager>
{
    public override void Awake()
    {
        base.Awake();
    }

    private class ConnectRetryPolicy : IRetryPolicy
    {
        private int secondDelay = 1;
        public ConnectRetryPolicy(int secondDelay)
        {
            this.secondDelay = secondDelay;
        }
        public TimeSpan? NextRetryDelay(RetryContext retryContext)
        {
            return TimeSpan.FromSeconds(secondDelay);
        }
    }
    HubConnection connection;
    void Start()
    {
        connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5000/TaskHub")
            .WithAutomaticReconnect(new ConnectRetryPolicy(10)) // 10S重连
            .Build();
        // 增加连接关闭后的处理
        connection.Closed += async (error) =>
        {
            // 其实这里不用，因为上面的自动重连会处理
            await Task.Delay(TimeSpan.FromSeconds(5));
            await connection.StartAsync();
        };
        connection.Reconnecting += async (error) =>
        {
            Debug.Log("Reconnecting");
        };

        connection.Reconnected += async (error) =>
        {
            Debug.Log("Reconnected");
        };
        // 关联要处理的SignalR消息
        connection.On<int>("Add", (newTaskId) =>
        {
            Debug.Log("Add:" + newTaskId);
        });
        connection.On<string[]>("BatchDelete", (canceledTaskIds) =>
        {
            foreach (string id in canceledTaskIds)
            {
                Debug.Log("BatchDelete:" + id);
            }
        });
        connection.On<int>("Edit", (changedTaskId) =>
        {
            Debug.Log("Edit:" + changedTaskId);
        });
        // 开始连接
        try
        {
            connection.StartAsync();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }

    private bool isConnected => connection.State == HubConnectionState.Connected;

    private void Update()
    {
        Debug.Log(isConnected);
    }

    private void OnDisable()
    {
        connection.StopAsync();
    }
}