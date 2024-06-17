using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetsManager : UnitySingleton<AssetsManager>
{
    [Header("ObjType 資源")]
    Dictionary<ObjTypeEnum, GameObject> objTypeAssetsDic = new Dictionary<ObjTypeEnum, GameObject>();             //物件類型資源(物件名稱, 物件資源)
    readonly Dictionary<ObjTypeEnum, string> objTypeAssetsPath = new Dictionary<ObjTypeEnum, string>()            //物件類型資源路徑
    {
        {ObjTypeEnum.Poker, "ObjType/Game/Poker"},                              //撲克牌物件
        {ObjTypeEnum.WinChips, "ObjType/Game/WinChips"},                        //獲勝籌碼物件
    };

    [Header("圖集資源")]
    Dictionary<AlbumEnum, SpriteAlbum> albumAssetsDic = new Dictionary<AlbumEnum, SpriteAlbum>();       //圖集資源(物件名稱, 物件資源)
    readonly Dictionary<AlbumEnum, string> albumAssetsPath = new Dictionary<AlbumEnum, string>()        //圖集資源路徑
    {
        {AlbumEnum.PokerNum, "SpriteAlbum/PokerNumAlbum"},                                              //撲克牌數字圖集
        {AlbumEnum.PokerBack, "SpriteAlbum/pokerBackAlbum"},                                            //撲克牌背面圖集
        {AlbumEnum.WalletLogo, "SpriteAlbum/WalletLogoAlbum"},                                          //錢包Logo圖集
        {AlbumEnum.PasswordEye, "SpriteAlbum/PasswordEyeAlbum" },                                       //密碼顯示眼睛圖集
        {AlbumEnum.PasswordCheck, "SpriteAlbum/CheckPasswordAlbum" },                                   //檢查密碼正確性圖集
        {AlbumEnum.AvatarAlbum, "SpriteAlbum/AvatarAlbum" },                                            //頭像圖集
        {AlbumEnum.ArrowAlbum, "SpriteAlbum/ArrowAlbum" },                                              //箭頭圖集
        {AlbumEnum.LinkAlbum, "SpriteAlbum/LinkAlbum" },                                                //連接圖集
        {AlbumEnum.TransactionStatusAlbum, "SpriteAlbum/TransactionStatusAlbum" },                      //交易狀態圖集
    };

    public override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// 載入資源
    /// </summary>
    /// <returns></returns>
    public IEnumerator ILoadAssets()
    {
        //ObjType物件
        foreach (var objtype in objTypeAssetsPath)
        {
            ResourceRequest request = Resources.LoadAsync<GameObject>(objtype.Value);
            yield return request;
            GameObject obj = request.asset as GameObject;
            objTypeAssetsDic.Add(objtype.Key, obj);
        }

        //圖集資源
        foreach (var album in albumAssetsPath)
        {
            ResourceRequest request = Resources.LoadAsync<SpriteAlbum>(album.Value);
            yield return request;
            SpriteAlbum obj = request.asset as SpriteAlbum;
            albumAssetsDic.Add(album.Key, obj);
        }
    }

    /// <summary>
    /// 獲取物件類遊戲物件
    /// </summary>
    /// <param name="objTypeEnum"></param>
    /// <returns></returns>
    public GameObject GetObjtypeAsset(ObjTypeEnum objTypeEnum)
    {
        if (objTypeAssetsDic.ContainsKey(objTypeEnum))
        {
            return objTypeAssetsDic[objTypeEnum];
        }
        else
        {
            Debug.LogError("無法找到 ObjType 物件");
            return null;
        }
    }

    /// <summary>
    /// 獲取圖集
    /// </summary>
    /// <param name="albumEnum"></param>
    /// <returns></returns>
    public SpriteAlbum GetAlbumAsset(AlbumEnum albumEnum)
    {
        if (albumAssetsDic.ContainsKey(albumEnum))
        {
            return albumAssetsDic[albumEnum];
        }
        else
        {
            Debug.LogError("無法找到 ScriptableObject_SpriteData 物件");
            return null;
        }
    }

    private void OnDestroy()
    {
        Resources.UnloadUnusedAssets();
    }
}
