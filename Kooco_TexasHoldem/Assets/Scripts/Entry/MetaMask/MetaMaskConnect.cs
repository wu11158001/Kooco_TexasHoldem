using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

using MetaMask.Unity;
using MetaMask.SocketIOClient;
using MetaMask.NativeWebSocket;
using MetaMask.Transports;
using MetaMask.Transports.Unity.UI;
using MetaMask;

public class MetaMaskConnect : MonoBehaviour
{
    [SerializeField]
    Button metaMaskConnect_Btn;

    private void Awake()
    {
        MetaMaskUnity.Instance.Initialize();
        MetaMaskUnity.Instance.Events.WalletAuthorized += walletConnected;
        MetaMaskUnity.Instance.Events.WalletDisconnected += walletDisconnected;
        MetaMaskUnity.Instance.Events.WalletReady += walletReady;
        MetaMaskUnity.Instance.Events.WalletPaused += walletPaused;
        if (Application.isMobilePlatform && MetaMaskUnityUITransport.DefaultInstance.IsDeeplinkAvailable())
        {
            MetaMaskUnity.Instance.Events.WalletAuthorized += ConnectedFeedback;
        }

        ListenerEvent();
    }

    private void OnDisable()
    {
        MetaMaskUnity.Instance.Events.WalletAuthorized -= walletConnected;
        MetaMaskUnity.Instance.Events.WalletDisconnected -= walletDisconnected;
        MetaMaskUnity.Instance.Events.WalletReady -= walletReady;
        MetaMaskUnity.Instance.Events.WalletPaused -= walletPaused;
        if (Application.isMobilePlatform && MetaMaskUnityUITransport.DefaultInstance.IsDeeplinkAvailable())
        {
            MetaMaskUnity.Instance.Events.WalletAuthorized -= ConnectedFeedback;
        }
    }

    /// <summary>
    /// 事件聆聽
    /// </summary>
    private void ListenerEvent()
    {
        //MataMask連接
        metaMaskConnect_Btn.onClick.AddListener(() =>
        {
            MetaMaskUnity.Instance.Connect();
        });
    }

    /// <summary>
    /// 錢包連線後呼叫
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void walletConnected(object sender, EventArgs e)
    {
        UnityThread.executeInUpdate(() =>
        {
            Debug.Log("Wallet Connect Success!!");
            LoadSceneManager.Instance.LoadScene(SceneEnum.Lobby);
        });
    }

    /// <summary>
    /// 錢包已斷開
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void walletDisconnected(object sender, EventArgs e)
    {
        Debug.Log("Wallet Disconnected!!");
    }

    /// <summary>
    /// 錢包已準備
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void walletReady(object sender, EventArgs e)
    {
        UnityThread.executeInUpdate(() =>
        {
            Debug.Log("Wallet Ready!!");
        });
    }

    /// <summary>
    /// 錢包已暫停
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void walletPaused(object sender, EventArgs e)
    {
        UnityThread.executeInUpdate(() =>
        {
            Debug.Log("Wallet Paused!!");
        });
    }

    /// <summary>
    /// 連接回饋
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ConnectedFeedback(object sender, EventArgs e)
    {
        Debug.Log("Wallet ConnectedFeedback");
        StopAllCoroutines();
    }
}
