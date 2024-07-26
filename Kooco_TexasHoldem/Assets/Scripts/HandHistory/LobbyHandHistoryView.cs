using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyHandHistoryView : MonoBehaviour
{
    [SerializeField]
    Button Back_Btn;
    [SerializeField]
    Image Tip_Img;
    [SerializeField]
    TextMeshProUGUI Title_Txt, Tip_Txt;

    /// <summary>
    /// 更新文本翻譯
    /// </summary>
    private void UpdateLanguage()
    {
        Title_Txt.text = LanguageManager.Instance.GetText("Hand History");
        Tip_Txt.text = LanguageManager.Instance.GetText("Show last 20 hands");
        StringUtils.TextInFrontOfImageFollow(Tip_Txt,
                                     Tip_Img);
    }

    private void OnDestroy()
    {
        LanguageManager.Instance.RemoveLanguageFun(UpdateLanguage);
    }

    private void Awake()
    {
        LanguageManager.Instance.AddUpdateLanguageFunc(UpdateLanguage, gameObject);

        //返回按鈕
        Back_Btn.onClick.AddListener(() =>
        {
            Destroy(gameObject);
        });
    }
}
