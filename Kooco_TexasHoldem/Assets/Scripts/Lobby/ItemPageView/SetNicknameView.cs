using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SetNicknameView : MonoBehaviour
{
    [SerializeField]
    Button Close_Btn, Subimt_Btn;
    [SerializeField]
    TMP_InputField SetNickname_If;
    [SerializeField]
    TextMeshProUGUI SetNicknameTitle_Txt, Title_Txt, SetNicknameIf_Placeholder,
                    Error_Txt, SubimtBtn_Txt;

    /// <summary>
    /// 更新文本翻譯
    /// </summary>
    private void UpdateLanguage()
    {
        SetNicknameTitle_Txt.text = LanguageManager.Instance.GetText("Set Your Nickname");
        Title_Txt.text = LanguageManager.Instance.GetText("Nickname");
        SetNicknameIf_Placeholder.text = LanguageManager.Instance.GetText("Enter nickname");
        SubimtBtn_Txt.text = LanguageManager.Instance.GetText("SUBMIT");
    }

    private void OnDestroy()
    {
        LanguageManager.Instance.RemoveLanguageFun(UpdateLanguage);
    }

    private void Awake()
    {
        LanguageManager.Instance.AddUpdateLanguageFunc(UpdateLanguage, gameObject);

        Error_Txt.text = "";

        ListenerEvent();
    }

    /// <summary>
    /// 事件聆聽
    /// </summary>
    private void ListenerEvent()
    {
        //關閉
        Close_Btn.onClick.AddListener(() =>
        {
            DataManager.UserNickname = "User3ab457";
            GameObject.FindAnyObjectByType<LobbyView>().UpdateUserInfo();
            DataManager.ReciveRankData();
            Destroy(gameObject);
        });

        //提交
        Subimt_Btn.onClick.AddListener(() =>
        {
            OnSubmit();
        });
    }

    private void Start()
    {
        if (!DataManager.IsMobilePlatform)
        {
            SetNickname_If.Select();
        }        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            OnSubmit();
        }
    }

    /// <summary>
    /// 提交
    /// </summary>
    private void OnSubmit()
    {
        if (SetNickname_If.text.Length <= 0)
        {
            Error_Txt.text = LanguageManager.Instance.GetText("User Name Entered Incorrectly, Please Try Again.");
        }
        else
        {
            string nickname = SetNickname_If.text.Trim();

            DataManager.UserNickname = nickname;
            GameObject.FindAnyObjectByType<LobbyView>().UpdateUserInfo();
            DataManager.ReciveRankData();
            Destroy(gameObject);
        }
    }
}
