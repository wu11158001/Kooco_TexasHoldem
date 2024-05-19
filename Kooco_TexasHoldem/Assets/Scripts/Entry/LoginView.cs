using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginView : MonoBehaviour
{
    [SerializeField]
    Button metaMaskConnect_Btn, trustConnect_Btn;

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
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            LoadSceneManager.Instance.LoadScene(SceneEnum.Lobby);
        }   
    }
}
