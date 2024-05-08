using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class LoginView : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void LoginMetaMask();//登入MetaMask

    [Header("本地測試")]
    public bool isLocalTest;

    [SerializeField]
    Button metaMaskLogin_Btn;


    private void Awake()
    {
        ListenerEvent();
    }

    /// <summary>
    /// 事件聆聽
    /// </summary>
    private void ListenerEvent()
    {
        //MetaMask登入
        metaMaskLogin_Btn.onClick.AddListener(() =>
        {
            if (isLocalTest)
            {
                LoginSuccess("Test123");
            }
            else
            {
                LoginMetaMask();
            }            
        });
    }

    /// <summary>
    /// 登入成功
    /// </summary>
    /// <param name="walletId">錢包ID</param>
    /// <returns></returns>
    public void LoginSuccess(string walletId)
    {
        GameDataManager.UserWalletId = walletId;
        LoadSceneManager.Instance.LoadScene(SceneEnum.Lobby);
    }
}
