using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetNicknameView : MonoBehaviour
{
    [SerializeField]
    Button Close_Btn, Subimt_Btn;
    [SerializeField]
    InputField SetNickname_If;
    [SerializeField]
    Text Error_Txt;

    private void Awake()
    {
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
            DataManager.UserNickname = "User Nickname";
            GameObject.FindAnyObjectByType<LobbyView>().UpdateUserInfo();
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
        SetNickname_If.Select();
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
            Error_Txt.text = "User Name Entered Incorrectly, Please Try Again.";
        }
        else
        {
            DataManager.UserNickname = SetNickname_If.text;
            GameObject.FindAnyObjectByType<LobbyView>().UpdateUserInfo();
            DataManager.ReciveRankData();
            Destroy(gameObject);
        }
    }
}
