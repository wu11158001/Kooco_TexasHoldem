using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VCTableBtnSample : MonoBehaviour
{
    [SerializeField]
    Button Launch_Btn;
    [SerializeField]
    GameObject JoinRoomViewObj;
    [SerializeField]
    TextMeshProUGUI BlindsStr_Txt, Blinds_Txt,
                    MinBuyStr_Txt, MinBuy_Txt,
                    LaunchBtn_Txt;

    /// <summary>
    /// 更新文本翻譯
    /// </summary>
    private void UpdateLanguage()
    {
        BlindsStr_Txt.text = LanguageManager.Instance.GetText("Blinds");
        MinBuyStr_Txt.text = LanguageManager.Instance.GetText("Min Buy-In");
        LaunchBtn_Txt.text = LanguageManager.Instance.GetText("LAUNCH");
    }

    private void OnDestroy()
    {
        LanguageManager.Instance.RemoveLanguageFun(UpdateLanguage);
    }

    private void Awake()
    {
        LanguageManager.Instance.AddUpdateLanguageFunc(UpdateLanguage, gameObject);
    }

    /// <summary>
    /// 設定虛擬貨幣桌按鈕訊息
    /// </summary>
    /// <param name="smallBlind">小盲</param>
    /// <param name="lobbyView">大廳</param>
    public void SetVCTableBtnInfo(double smallBlind, LobbyView lobbyView)
    {
        Blinds_Txt.text = $"{StringUtils.SetChipsUnit(smallBlind)} / {StringUtils.SetChipsUnit(smallBlind * 2)}";
        MinBuy_Txt.text = $"{StringUtils.SetChipsUnit(smallBlind * DataManager.MinMagnification)}";

        Launch_Btn.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayConfirmClick();
            if (GameRoomManager.Instance.JudgeIsCanBeCreateRoom())
            {
                JoinRoomView joinRoomView = ViewManager.Instance.CreateViewInCurrCanvas<JoinRoomView>(JoinRoomViewObj);
                joinRoomView.SetCreatRoomViewInfo(TableTypeEnum.VCTable, smallBlind);
            }
            else
            {
                lobbyView.ShowMaxRoomTip();
            }
        });
    }
}
