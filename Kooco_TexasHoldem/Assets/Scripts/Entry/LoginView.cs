using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class LoginView : MonoBehaviour
{
    [SerializeField]
    Button metaMaskConnect_Btn, t_Btn, t2_Btn;

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
            MetaMaskManager.Instance.MetaMaskConnectAndSign();
        });

        t_Btn.onClick.AddListener(() =>
        {
            MetaMaskManager.Instance.DoRevokePermissions();
        });

        t2_Btn.onClick.AddListener(() =>
        {
            MetaMaskManager.Instance.DoReload();
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            LoadSceneManager.Instance.LoadScene(SceneEnum.Lobby);
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            LanguageManager.Instance.CurrLanguage = 0;
            Debug.LogError(LanguageManager.Instance.GetText("Test1"));
            UIManager.Instance.ClosePartsView(PartsViewEnum.WaitingView);
        }

        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            LanguageManager.Instance.CurrLanguage = 1;
            Debug.LogError(LanguageManager.Instance.GetText("Test1"));
            UIManager.Instance.OpenPartsView(PartsViewEnum.WaitingView);
        }
    }
}
