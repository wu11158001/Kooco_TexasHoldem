using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class JoinRoomView : MonoBehaviour
{
    [SerializeField]
    Request_CreateCashRoom baseRequest;
    [SerializeField]
    Image SB_Img, BB_Img;
    [SerializeField]
    Slider BuyChips_Sli;
    [SerializeField]
    Button Cancel_Btn, Buy_Btn, BuyPlus_Btn, BuyMinus_Btn;
    [SerializeField]
    TextMeshProUGUI Title_Txt, BlindsTitle_Txt,
                    SB_Txt, BB_Txt, PreBuyChips_Txt,
                    MinBuyChips_Txt, MaxBuyChips_Txt,
                    CancelBtn_Txt, BuyBtn_Txt;

    double smallBlind;                  //小盲值
    TableTypeEnum tableType;            //房間類型

    /// <summary>
    /// 更新文本翻譯
    /// </summary>
    private void UpdateLanguage()
    {
        BlindsTitle_Txt.text = LanguageManager.Instance.GetText("Blinds");
        CancelBtn_Txt.text = LanguageManager.Instance.GetText("Cancel");
        BuyBtn_Txt.text = LanguageManager.Instance.GetText("Buy");
    }

    private void OnDestroy()
    {
        LanguageManager.Instance.RemoveLanguageFun(UpdateLanguage);
    }

    public void Awake()
    {
        LanguageManager.Instance.AddUpdateLanguageFunc(UpdateLanguage, gameObject);
        ListenerEvent();
    }

    /// <summary>
    /// 事件聆聽
    /// </summary>
    private void ListenerEvent()
    {
        //取消
        Cancel_Btn.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayCancelClick();
            gameObject.SetActive(false);
        });

        //購買
        Buy_Btn.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayConfirmClick();
            baseRequest.SendRequest_JoinRoom(tableType,
                                             smallBlind, 
                                             BuyChips_Sli.value, 
                                             1);
        });

        //購買Slider單位設定
        BuyChips_Sli.onValueChanged.AddListener((value) =>
        {
            float newRaiseValue = TexasHoldemUtil.SliderValueChange(BuyChips_Sli,
                                                                    value,
                                                                    (float)smallBlind * 2,
                                                                    BuyChips_Sli.minValue,
                                                                    BuyChips_Sli.maxValue);
            PreBuyChips_Txt.text = StringUtils.SetChipsUnit(newRaiseValue);
        });

        //購買+按鈕
        BuyPlus_Btn.onClick.AddListener(() =>
        {
            BuyChips_Sli.value += (float)smallBlind * 2;
        });

        //購買-按鈕
        BuyMinus_Btn.onClick.AddListener(() =>
        {
            BuyChips_Sli.value -= (float)smallBlind * 2;
        });
    }

    /// <summary>
    /// 設定創建房間介面
    /// </summary>
    /// <param name="tableType">遊戲桌類型</param>
    /// <param name="smallBlind">小盲值</param>
    public void SetCreatRoomViewInfo(TableTypeEnum tableType, double smallBlind)
    {
        this.smallBlind = smallBlind;
        this.tableType = tableType;

        string titleStr = "";
        string maxBuyChipsStr = "";
        switch (tableType)
        {
            //加密貨幣桌
            case TableTypeEnum.CryptoTable:
                titleStr = "CRYPTO TABLE";
                maxBuyChipsStr = $"{StringUtils.SetChipsUnit(DataManager.UserCryptoChips)}";
                SB_Img.sprite = AssetsManager.Instance.GetAlbumAsset(AlbumEnum.CurrencyAlbum).album[0];
                BB_Img.sprite = AssetsManager.Instance.GetAlbumAsset(AlbumEnum.CurrencyAlbum).album[0];
                break;

            //虛擬貨幣桌
            case TableTypeEnum.VCTable:
                titleStr = "VIRTUAL CURRENCY TABLE";
                maxBuyChipsStr = $"{StringUtils.SetChipsUnit(DataManager.UserVCChips)}";
                SB_Img.sprite = AssetsManager.Instance.GetAlbumAsset(AlbumEnum.CurrencyAlbum).album[1];
                BB_Img.sprite = AssetsManager.Instance.GetAlbumAsset(AlbumEnum.CurrencyAlbum).album[1];
                break;
        }
        Title_Txt.text = LanguageManager.Instance.GetText(titleStr);

        SB_Txt.text = $"{StringUtils.SetChipsUnit(smallBlind)}";
        BB_Txt.text = $"{StringUtils.SetChipsUnit(smallBlind * 2)}";

        TexasHoldemUtil.SetBuySlider(this.smallBlind, BuyChips_Sli, tableType);
        MinBuyChips_Txt.text = $"{StringUtils.SetChipsUnit(this.smallBlind * DataManager.MinMagnification)}";
        MaxBuyChips_Txt.text = maxBuyChipsStr;
    }
}
