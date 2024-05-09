using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class LoginView : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void LoginMetaMask();//登入MetaMask

    [DllImport("__Internal")]
    private static extern void TestFunc();//登入MetaMask


    [SerializeField]
    Button metaMaskLogin_Btn, tt;


    private void Awake()
    {
        ListenerEvent();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            LoginSuccess("Test123");
        }
    }

    /// <summary>
    /// 事件聆聽
    /// </summary>
    private void ListenerEvent()
    {
        //MetaMask登入
        metaMaskLogin_Btn.onClick.AddListener(() =>
        {
            LoginMetaMask();
        });


        //MetaMask登入
        tt.onClick.AddListener(() =>
        {
            TestFunc();
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
