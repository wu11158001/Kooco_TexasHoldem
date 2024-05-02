using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using RequestBuf;

public class Entry : UnitySingleton<Entry>
{
    public GameServer gameServer;
    public static GameView gameView { get; set; }

    public static bool isPause;

    public static class TestInfoData
    {      
        public static string LocalUserId = "001";
        public static string NickName = "LocalUserName";

        public static int newPlayerId = 10;

        public static DateTime foldTimd = DateTime.Now;
    }

    public override void Awake()
    {
        base.Awake();

        gameObject.AddComponent<UnityMainThreadDispatcher>();
        gameObject.AddComponent<UIManager>();
        gameObject.AddComponent<RequestManager>();
    }

    private IEnumerator Start()
    {
        gameServer.gameObject.SetActive(false);

        //遊戲資源
        GameObject gameAssetsManagerObj = new GameObject();
        gameAssetsManagerObj.name = "gameAssetsManager";
        GameAssetsManager gameAssetsManager = gameAssetsManagerObj.AddComponent<GameAssetsManager>();
        yield return gameAssetsManager.ILoadAssets();

        UIManager.Instance.OpenView(ViewName.JoinRoomView);
    }

    private void Update()
    {
        if (gameView != null && gameView.gameObject.activeSelf)
        {
            if ((DateTime.Now - TestInfoData.foldTimd).TotalSeconds > 1f)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    TestInfoData.foldTimd = DateTime.Now;
                    gameServer.TextPlayerAction(RequestBuf.ActingEnum.Fold);
                }

                if (Input.GetKeyDown(KeyCode.R))
                {
                    TestInfoData.foldTimd = DateTime.Now;
                    gameServer.TextPlayerAction(RequestBuf.ActingEnum.Raise);
                }

                if (Input.GetKeyDown(KeyCode.C))
                {
                    TestInfoData.foldTimd = DateTime.Now;
                    gameServer.TextPlayerAction(RequestBuf.ActingEnum.Call);
                }

                if (Input.GetKeyDown(KeyCode.I))
                {
                    TestInfoData.foldTimd = DateTime.Now;
                    gameServer.TextPlayerAction(RequestBuf.ActingEnum.AllIn);
                }

                if (Input.GetKeyDown(KeyCode.A))
                {
                    TestInfoData.foldTimd = DateTime.Now;
                    if (gameServer.clientList.Count < gameServer.maxRoomPeople)
                    {
                        MainPack pack = new MainPack();
                        pack.ActionCode = ActionCode.Request_PlayerInOutRoom;

                        PlayerInfoPack playerInfoPack = new PlayerInfoPack();
                        playerInfoPack.UserID = $"00000{TestInfoData.newPlayerId}";
                        playerInfoPack.NickName = $"Player{TestInfoData.newPlayerId}";
                        playerInfoPack.Chips = 100000;

                        PlayerInOutRoomPack playerInOutRoomPack = new PlayerInOutRoomPack();
                        playerInOutRoomPack.IsInRoom = true;
                        playerInOutRoomPack.PlayerInfoPack = playerInfoPack;

                        pack.PlayerInOutRoomPack = playerInOutRoomPack;
                        gameServer.Request_PlayerInOutRoom(pack);

                        TestInfoData.newPlayerId++;
                    }
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    TestInfoData.foldTimd = DateTime.Now;
                    gameServer.TextPlayerAction(RequestBuf.ActingEnum.Fold, true);
                }
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                isPause = !isPause;
                Time.timeScale = isPause == true ? 0 : 1;
            }
        }
        
    }

    /// <summary>
    /// 網頁視窗失去焦點
    /// </summary>
    public void OnWindowBlur()
    {
        Debug.Log("OnWindowBlur");
        if (gameView != null && gameView.gameObject.activeSelf)
        {
            isPause = true;
            gameView.GamePause = true;
        }
    }

    /// <summary>
    /// 網頁視窗獲得焦點
    /// </summary>
    public void OnWindowFocus()
    {
        Debug.Log("OnWinndowFocus");
    }

    /// <summary>
    /// 網頁視窗縮小
    /// </summary>
    public void OnWindowResize()
    {
        Debug.Log("OnWinndowResize");
    }
}
