using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Transfers_AnteView : MonoBehaviour
{
    [SerializeField]
    Button CloseBtn;

    [Header("Button滑塊")]
    [SerializeField]
    GameObject MaskObject;
    TextMeshProUGUI MaskText;

    [Header("質押介面")]
    [SerializeField]
    Button AnteBtn,AnteAgainBtn;
    [SerializeField]
    Button Ante_SumbitBtn;
    [SerializeField]
    GameObject AnteViewParent,AnteView;
    [SerializeField]
    TextMeshProUGUI AnteAmount,Ante_Currency;
    [SerializeField]
    TMP_Dropdown Ante_NetWorkDropdown,Ante_CurrencyDropdown;

    [Header("質押成功介面")]
    [SerializeField]
    GameObject AnteSuccessView;
    [SerializeField]
    TextMeshProUGUI EstimatedAmount;

    [Space(10)]
    [SerializeField]
    Image Ante_NetWorkIcon;
    [SerializeField]
    TextMeshProUGUI Ante_NetWorkText;

    [Space(10)]
    [SerializeField]
    Image Ante_OrderIcon;
    [SerializeField]
    TextMeshProUGUI Ante_OrderAmountText,Ante_OrderCoin;

    [Header("贖回介面")]
    [SerializeField]
    Button RedeemBtn;
    [SerializeField]
    Button Redeem_SumbitBtn,Redeem_AgainBtn;
    [SerializeField]
    GameObject RedeemViewParent,RedeemView;
    [SerializeField]
    TextMeshProUGUI EnterAmount,Redeem_Currency;
    [SerializeField]
    TMP_Dropdown Redeem_NetWorkDropdown, Redeem_CurrencyDropdown;

    [Header("贖回成功介面")]
    [SerializeField]
    GameObject RedeemSuccessView;

    [Space(10)]
    [SerializeField]
    Image Redeem_NetWorkIcon;
    [SerializeField]
    TextMeshProUGUI Redeem_NetWorkText;

    [Space(10)]
    [SerializeField]
    Image Redeem_OrderIcon;
    [SerializeField]
    TextMeshProUGUI Redeem_OrderAmountText,Redeem_OrderCoin;

    private void Awake()
    {
        MaskText = MaskObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        CloseBtn.onClick.AddListener(() =>
        {
            Destroy(this.gameObject);
        });

        AnteBtn.onClick.AddListener(() =>
        {
            if (!AnteViewParent.gameObject.activeSelf)
            {
                AnteViewParent.gameObject.SetActive(true);
                RedeemViewParent.gameObject.SetActive(false);

                MaskMove(AnteBtn.gameObject);
            }
        });

        AnteAgainBtn.onClick.AddListener(() =>
        {
            AnteView.SetActive(!AnteView.activeSelf);
            AnteSuccessView.SetActive(!AnteSuccessView.activeSelf);

        });


        RedeemBtn.onClick.AddListener(() =>
        {
            if (!RedeemViewParent.gameObject.activeSelf)
            {
                AnteViewParent.gameObject.SetActive(false);
                RedeemViewParent.gameObject.SetActive(true);

                MaskMove(RedeemBtn.gameObject);
            }
        });

        Redeem_AgainBtn.onClick.AddListener(() =>
        {
            RedeemView.SetActive(!RedeemView.activeSelf);
            RedeemSuccessView.SetActive(!RedeemSuccessView.activeSelf);
        });

        Ante_SumbitBtn.onClick.AddListener(() =>
        {
            AnteView.SetActive(!AnteView.activeSelf);
            AnteSuccessView.SetActive(!AnteSuccessView.activeSelf);
            EstimatedAmount.text = AnteAmount.text;

            var Netdrop = Ante_NetWorkDropdown.GetComponent<TMP_Dropdown>();
            Ante_NetWorkIcon.sprite = Netdrop.captionImage.sprite;
            Ante_NetWorkText.text = Netdrop.captionText.text;

            var Currdrop = Ante_CurrencyDropdown.GetComponent<TMP_Dropdown>();
            Ante_OrderIcon.sprite = Currdrop.captionImage.sprite;
            Ante_OrderAmountText.text = EstimatedAmount.text;
            Ante_OrderCoin.text = Currdrop.captionText.text;


        });

        Redeem_SumbitBtn.onClick.AddListener(() =>
        {
            RedeemView.SetActive(!RedeemView.activeSelf);
            RedeemSuccessView.SetActive(!RedeemSuccessView.activeSelf);

            var Netdrop = Redeem_NetWorkDropdown.GetComponent<TMP_Dropdown>();
            Redeem_NetWorkIcon.sprite = Netdrop.captionImage.sprite;
            Redeem_NetWorkText.text = Netdrop.captionText.text;

            var Currdrop = Redeem_CurrencyDropdown.GetComponent<TMP_Dropdown>();
            Redeem_OrderIcon.sprite = Currdrop.captionImage.sprite;
            Redeem_OrderAmountText.text = EnterAmount.text;
            Redeem_OrderCoin.text = Currdrop.captionText.text;
        });


        //  更改輸入欄位幣種
        Ante_CurrencyDropdown.onValueChanged.AddListener(delegate{ CurrencyValueChange(Ante_CurrencyDropdown,Ante_Currency); });
        Redeem_CurrencyDropdown.onValueChanged.AddListener(delegate { CurrencyValueChange(Redeem_CurrencyDropdown, Redeem_Currency); });
    }

    /// <summary>
    /// 更改輸入欄位幣種
    /// </summary>
    /// <param name="dropdown">下拉式選單</param>
    /// <param name="CurrencyText">輸入欄位幣種</param>
    public void CurrencyValueChange(TMP_Dropdown dropdown, TextMeshProUGUI CurrencyText)
    {
        CurrencyText.text = dropdown.captionText.text;
    }


    /// <summary>
    /// 滑塊遮罩移動
    /// </summary>
    /// <param name="Mask">滑塊物件</param>
    public void MaskMove(GameObject Mask)
    {
        RectTransform Maskrect = MaskObject.GetComponent<RectTransform>();
        RectTransform TransfersMaskRect = Mask.GetComponent<RectTransform>();

        Maskrect.localPosition = TransfersMaskRect.localPosition;
        MaskText.text = Mask.name;
    }

}
