using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMainPageView : MonoBehaviour
{
    [Header("加密貨幣桌")]
    [SerializeField]
    GameObject CryptoTableBtnSample;
    [SerializeField]
    RectTransform CryptoTableParent;

    [Header("虛擬貨幣桌")]
    [SerializeField]
    GameObject VCTableBtnSample;
    [SerializeField]
    RectTransform VCTableParent;

    LobbyView lobbyView;

    private void Start()
    {
        lobbyView = GameObject.FindAnyObjectByType<LobbyView>();

        CreateRoomBtn();
    }

    /// <summary>
    /// 創建房間按鈕
    /// </summary>
    private void CreateRoomBtn()
    {
        //加密貨幣桌        
        CryptoTableBtnSample.SetActive(false);
        float cryptoSpacing = CryptoTableParent.GetComponent<HorizontalLayoutGroup>().spacing;
        Rect cryptoRect = CryptoTableBtnSample.GetComponent<RectTransform>().rect;
        CryptoTableParent.sizeDelta = new Vector2((cryptoRect.width + cryptoSpacing) * DataManager.CryptoSmallBlindList.Count, cryptoRect.height);
        foreach (var smallBlind in DataManager.CryptoSmallBlindList)
        {
            RectTransform rt = Instantiate(CryptoTableBtnSample).GetComponent<RectTransform>();
            rt.gameObject.SetActive(true);
            rt.SetParent(CryptoTableParent);
            rt.GetComponent<CryptoTableBtnSample>().SetCryptoTableBtnInfo(smallBlind, lobbyView);
            rt.localScale = Vector3.one;
        }
        CryptoTableParent.anchoredPosition = Vector2.zero;

        //虛擬貨幣桌
        VCTableBtnSample.SetActive(false);
        float vcSpacing = VCTableParent.GetComponent<HorizontalLayoutGroup>().spacing;
        Rect vcRect = VCTableBtnSample.GetComponent<RectTransform>().rect;
        VCTableParent.sizeDelta = new Vector2((vcRect.width + vcSpacing) * DataManager.VCSmallBlindList.Count, vcRect.height);
        foreach (var smallBlind in DataManager.VCSmallBlindList)
        {
            RectTransform rt = Instantiate(VCTableBtnSample).GetComponent<RectTransform>();
            rt.gameObject.SetActive(true);
            rt.SetParent(VCTableParent);
            rt.GetComponent<VCTableBtnSample>().SetVCTableBtnInfo(smallBlind, lobbyView);
            rt.localScale = Vector3.one;
        }
        VCTableParent.anchoredPosition = Vector2.zero;
    }
}
