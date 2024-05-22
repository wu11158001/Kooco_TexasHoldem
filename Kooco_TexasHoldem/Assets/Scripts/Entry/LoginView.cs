using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Security.Cryptography;
using System.Text;
using ZXing;
using ZXing.QrCode;
using System;

public class LoginView : MonoBehaviour
{
    [SerializeField]
    Button metaMaskConnect_Btn, trustConnect_Btn, binanceConnect_Btn, okxConnect_Btn;

    [SerializeField]
    RawImage qrCodeImage;

    private void Awake()
    {
        ListenerEvent();
    }

    /// <summary>
    /// 事件聆聽
    /// </summary>
    private void ListenerEvent()
    {
        //MetaMask連接
        metaMaskConnect_Btn.onClick.AddListener(() =>
        {
            Web3WalletManager.Instance.ConnectAndSign(Web3Enum.MetaMask);
        });

        //TrustWallet連接
        trustConnect_Btn.onClick.AddListener(() =>
        {
            Web3WalletManager.Instance.ConnectAndSign(Web3Enum.TrustWallet);
        });

        //Binance連接
        binanceConnect_Btn.onClick.AddListener(() =>
        {
            Web3WalletManager.Instance.ConnectAndSign(Web3Enum.Binance);
        });

        //OKX連接
        okxConnect_Btn.onClick.AddListener(() =>
        {
            Web3WalletManager.Instance.ConnectAndSign(Web3Enum.OKX);
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            LoadSceneManager.Instance.LoadScene(SceneEnum.Lobby);
        }   
    }
}
