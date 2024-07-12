using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AssetsManager : UnitySingleton<AssetsManager>
{
    [Header("圖集資源")]
    Dictionary<AlbumEnum, SpriteAlbum> albumAssetsDic = new Dictionary<AlbumEnum, SpriteAlbum>();                  //圖集資源(物件名稱, 物件資源)

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
        //圖集資源
        foreach (var album in Enum.GetValues(typeof(AlbumEnum)))
        {
            ResourceRequest request = Resources.LoadAsync<SpriteAlbum>($"SpriteAlbum/{album}");
            yield return request;
            SpriteAlbum obj = request.asset as SpriteAlbum;
            albumAssetsDic.Add((AlbumEnum)album, obj);
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
