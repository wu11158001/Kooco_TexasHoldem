using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadingView : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI Loading_Txt;

    /// <summary>
    /// 更新文本翻譯
    /// </summary>
    private void UpdateLanguage()
    {
        Loading_Txt.text = $"{LanguageManager.Instance.GetText("Loading")}...";
    }

    private void OnDestroy()
    {
        LanguageManager.Instance.RemoveLanguageFun(UpdateLanguage);
    }

    private void Awake()
    {
        LanguageManager.Instance.AddUpdateLanguageFunc(UpdateLanguage);
    }
}
