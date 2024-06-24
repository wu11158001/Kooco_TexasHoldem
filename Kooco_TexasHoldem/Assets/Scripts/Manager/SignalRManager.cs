using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

// https://docs.microsoft.com/zh-cn/dotnet/api/microsoft.aspnetcore.signalr.client?view=aspnetcore-5.0
public class SignalRManager : UnitySingleton<SignalRManager>
{
    HubConnection connection;

    private bool isConnected => connection.State == HubConnectionState.Connected;

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
    
    void Start()
    {
        connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5000/TaskHub")
            .WithAutomaticReconnect(new ConnectRetryPolicy(10)) //10S重连
            .Build();

        //斷線重連處理
        connection.Closed += async (error) =>
        {
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

        //要處理的SignalR消息
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

        //開始連接
        try
        {
            connection.StartAsync();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }

    private void Update()
    {
        Debug.Log(isConnected);
    }

    public void SendMassage()
    {
        //connection.InvokeAsync("FuncName", "value");
    }

    private void OnDisable()
    {
        connection.StopAsync();
    }
}