using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopSample : MonoBehaviour
{
    [SerializeField]
    Image ItemIcon;

    [SerializeField]
    Text Buff_Text,CostAmount_Text;

    [SerializeField]
    Button BuyBtn;

    /// <summary>
    /// 設置商店物品資料
    /// </summary>
    /// <param name="shopData">商店資料</param>
    /// <param name="index">圖集index</param>
    /// <param name="albumEnum">圖集枚舉</param>
    public void SetShopItemData(ShopData shopData,int index,AlbumEnum albumEnum)
    {
        ItemIcon.sprite = AssetsManager.Instance.GetAlbumAsset(albumEnum).album[index];
        Buff_Text.text = $"+ {shopData.BuffAmount} {shopData.BuffName}";
        CostAmount_Text.text = $"{shopData.CostCoin}";
    }
    
    /// <summary>
    /// 購買商品事件
    /// </summary>
    /// <param name="shopView">商店物件</param>
    /// <param name="MallMsg">購買訊息彈窗</param>
    /// <param name="shopData">商品資料</param>
    /// <param name="img">商品Icon</param>
    /// <param name="info">購買訊息欄位</param>
    /// <param name="ItemName">商品名</param>
    public void OnBuyAddListener(LobbyShopView shopView, GameObject MallMsg, ShopData shopData, Image img, Text info,string ItemName)
    {
        BuyBtn.onClick.AddListener(() =>
        {
            MallMsg.SetActive(!MallMsg.activeSelf);
            img.sprite = ItemIcon.sprite;
            info.text = $"purchase {shopData.BuffAmount} {ItemName}";

            shopView.GetComponent<LobbyShopView>().OnBuyingPopupUI(this,shopData,ItemName);
        });
    }

    /// <summary>
    /// 餘額不足介面更新
    /// </summary>
    /// <param name="img">關閉商品icon</param>
    /// <param name="info">顯示餘額不足訊息</param>
    public void InsufficientBalance(Image img, Text info)
    {
        img.gameObject.SetActive(false);
        info.text = $"Insufficient balance.\nPlease proceed with collateralization.";
    }

}
