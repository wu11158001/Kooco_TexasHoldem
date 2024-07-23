using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Transfers_AnteView : MonoBehaviour
{
    [SerializeField]
    Button CloseBtn;

    [Header("Mask滑塊")]
    [SerializeField]
    GameObject MaskObj;

    [Header("質押介面")]
    [SerializeField]
    Button AnteBtn;
    [SerializeField]
    Button SumbitBtn;
    [SerializeField]
    GameObject AnteView, AnteSuccessView;
    [SerializeField]
    TextMeshProUGUI AnteAmount;

    [Header("贖回介面")]
    [SerializeField]
    Button RedeemBtn;
    [SerializeField]
    TextMeshProUGUI EstimatedAmount;



    private void Awake()
    {
        CloseBtn.onClick.AddListener(() =>
        {
            Destroy(this.gameObject);
        });

        AnteBtn.onClick.AddListener(() =>
        {
            
        });

        RedeemBtn.onClick.AddListener(() =>
        {


        });


        SumbitBtn.onClick.AddListener(() =>
        {
            AnteView.SetActive(!AnteView.activeSelf);
            AnteSuccessView.SetActive(!AnteSuccessView.activeSelf);
            EstimatedAmount.text = AnteAmount.text;
        });

    }

}
