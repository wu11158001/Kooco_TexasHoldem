using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Thirdweb;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System.Threading.Tasks;

public class MyNFTView : MonoBehaviour
{
    [SerializeField]
    Button Back_Btn, Direction_Btn;
    [SerializeField]
    Transform HorixontalContent, VerticalContent;
    [SerializeField]
    GameObject Horizontal_Sv, Vertical_Sv, NFTHorizontalSample, NFTVerticalSample, NoNFT_Obj;

    /// <summary>
    /// 顯示方向
    /// </summary>
    protected enum ShowDriection
    {
        Horizontal,
        Vertical,
    }

    /// <summary>
    /// //當前顯示方向
    /// </summary>
    protected ShowDriection currDirction;
    protected ShowDriection CurrShowDirction
    {
        get
        {
            return currDirction;
        }
        set
        {
            currDirction = value;
            Horizontal_Sv.SetActive(value == ShowDriection.Horizontal);
            Vertical_Sv.SetActive(value == ShowDriection.Vertical);

            DisplayNFT();
        }
    }

    private void Awake()
    {
        //返回
        Back_Btn.onClick.AddListener(() =>
        {
            Destroy(gameObject);
        });


        //顯示方向切換
        Direction_Btn.onClick.AddListener(() =>
        {
            ShowDriection dir = currDirction == ShowDriection.Horizontal ?
                                ShowDriection.Vertical :
                                ShowDriection.Horizontal;

            CurrShowDirction = dir;
        });
    }

    private void Start()
    {
        NoNFT_Obj.SetActive(false);
        NFTHorizontalSample.SetActive(false);
        NFTVerticalSample.SetActive(false);

        CurrShowDirction = ShowDriection.Horizontal;
    }

    /// <summary>
    /// 顯示NFT
    /// </summary>
    public void DisplayNFT()
    {
        switch (currDirction)
        {
            //顯示橫向
            case ShowDriection.Horizontal:
                CreateNFT(HorixontalContent, NFTHorizontalSample);
                break;

            //顯示直向
            case ShowDriection.Vertical:
                CreateNFT(VerticalContent, NFTVerticalSample);
                break;
        }
    }

    /// <summary>
    /// 創建NFT
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="sample"></param>
    private void CreateNFT(Transform parent, GameObject sample)
    {
        for (int i = 1; i < parent.childCount; i++)
        {
            Destroy(parent.GetChild(i).gameObject);
        }

        NoNFT_Obj.SetActive(NFTManager.Instance.NFTDataList == null || NFTManager.Instance.NFTDataList.Count == 0);

        if (NFTManager.Instance.NFTDataList != null)
        {
            int index = 0;
            foreach (var data in NFTManager.Instance.NFTDataList)
            {
                NFTSample nftSample = Instantiate(sample, parent).GetComponent<NFTSample>();
                nftSample.gameObject.SetActive(true);
                nftSample.SetNFT(data, index);

                index++;
            }
        }        
    }
}

[System.Serializable]
public class NFTAssets
{
    public NFTData[] nfts;
}

[System.Serializable]
public class NFTData
{
    public string name;
    public string updated_at;
    public string description;
    public string display_image_url;
    public string identifier;                   //tokenId
    public string rarity = "0";
}