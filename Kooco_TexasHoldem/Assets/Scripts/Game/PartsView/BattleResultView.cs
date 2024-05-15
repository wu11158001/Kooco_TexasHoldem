using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleResultView : MonoBehaviour
{
    [SerializeField]
    Text result_Txt;
    [SerializeField]
    Button confirm_Btn;

    private void Awake()
    {
        ListenerEvent();
    }

    /// <summary>
    /// 事件聆聽
    /// </summary>
    private void ListenerEvent()
    {
        //確認
        confirm_Btn.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);           
            LoadSceneManager.Instance.LoadScene(SceneEnum.Lobby);
        });
    }

    /// <summary>
    /// 設定結果
    /// </summary>
    /// <param name="isWin"></param>
    public void OnSetResult(bool isWin)
    {
        result_Txt.text = isWin ? "Win" : "Fail";
    }
}
