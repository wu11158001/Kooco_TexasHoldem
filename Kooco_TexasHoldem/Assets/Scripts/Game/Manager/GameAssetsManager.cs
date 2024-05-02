using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssetsManager : UnitySingleton<GameAssetsManager>
{
    ScriptableObject_SpriteData pokerSpriteData, pokerBackSpriteData;

    /// <summary>
    /// 玩家訊息物件
    /// </summary>
    public GameObject GamePlayerInfoObj { get; set; }  

    /// <summary>
    /// 獲勝籌碼物件
    /// </summary>
    public GameObject WinChipsObj { get; set; }

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
        //撲克數字圖
        ResourceRequest request = Resources.LoadAsync<ScriptableObject_SpriteData>("ScriptableObject/PokerSpriteData");
        yield return request;
        pokerSpriteData = request.asset as ScriptableObject_SpriteData;

        //撲克背面圖
        request = Resources.LoadAsync<ScriptableObject_SpriteData>("ScriptableObject/pokerBackSpriteData");
        yield return request;
        pokerBackSpriteData = request.asset as ScriptableObject_SpriteData;

        //玩家訊息物件
        request = Resources.LoadAsync<GameObject>("ComponentPrefab/GamePlayerInfo");
        yield return request;
        GamePlayerInfoObj = request.asset as GameObject;

        //獲勝籌碼物件
        request = Resources.LoadAsync<GameObject>("ComponentPrefab/WinChips");
        yield return request;
        WinChipsObj = request.asset as GameObject;
    }

    /// <summary>
    /// 獲取撲克數字圖
    /// </summary>
    /// <param name="pokerNum"></param>
    /// <returns></returns>
    public Sprite GetPokerNum(int pokerNum)
    {
        return pokerSpriteData.sprites.spritesList[pokerNum];
    }

    /// <summary>
    /// 獲取撲克背面圖
    /// </summary>
    /// <param name="backIndex"></param>
    /// <returns></returns>
    public Sprite GetPokerBack(int backIndex)
    {
        return pokerBackSpriteData.sprites.spritesList[backIndex];
    }

    private void OnDestroy()
    {
        Resources.UnloadUnusedAssets();
    }
}
