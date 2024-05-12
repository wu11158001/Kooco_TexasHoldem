using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class LoginView : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            LoadSceneManager.Instance.LoadScene(SceneEnum.Lobby);
        }

        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            LanguageManager.CurrLanguage = 0;
            LanguageManager.GetText("Test1");
        }

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            LanguageManager.CurrLanguage = 1;
            LanguageManager.GetText("Test1");
        }
    }
}
