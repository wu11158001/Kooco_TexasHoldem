using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Thirdweb;

using RequestBuf;

public class Entry : UnitySingleton<Entry>
{
    #region 測試
    public static GameServer CurrGameServer;
    public static class TestInfoData
    {      
        public static string LocalUserId = "LocalUser";
        public static string NickName = "LocalUserName";

        public static int newPlayerId = 10;
        public static double LocalUserChips = 153000; 

        public static DateTime foldTimd = DateTime.Now;
    }
    #endregion

    [Header("Debug工具")]
    [SerializeField]
    GameObject debugObj;
    [SerializeField]
    bool isShowDebug;

    public override void Awake()
    {
        base.Awake();

        debugObj.SetActive(isShowDebug);
    }

    private IEnumerator Start()
    {
        LanguageManager.Instance.LoadLangageJson();

        yield return AssetsManager.Instance.ILoadAssets();

        LoadSceneManager.Instance.LoadScene(SceneEnum.Login);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            LanguageManager.Instance.ChangeLanguage(0);
        }

        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            LanguageManager.Instance.ChangeLanguage(1);
        }

        if (CurrGameServer != null && CurrGameServer.gameObject.activeSelf)
        {
            if ((DateTime.Now - TestInfoData.foldTimd).TotalSeconds > 0.2f)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    TestInfoData.foldTimd = DateTime.Now;
                    CurrGameServer.TextPlayerAction(RequestBuf.ActingEnum.Fold);
                }

                if (Input.GetKeyDown(KeyCode.R))
                {
                    TestInfoData.foldTimd = DateTime.Now;
                    CurrGameServer.TextPlayerAction(RequestBuf.ActingEnum.Raise);
                }

                if (Input.GetKeyDown(KeyCode.C))
                {
                    TestInfoData.foldTimd = DateTime.Now;
                    CurrGameServer.TextPlayerAction(RequestBuf.ActingEnum.Call);
                }

                if (Input.GetKeyDown(KeyCode.I))
                {
                    TestInfoData.foldTimd = DateTime.Now;
                    CurrGameServer.TextPlayerAction(RequestBuf.ActingEnum.AllIn);
                }

                if (Input.GetKeyDown(KeyCode.A))
                {
                    TestInfoData.foldTimd = DateTime.Now;
                    if (CurrGameServer.clientList.Count < CurrGameServer.maxRoomPeople)
                    {
                        MainPack pack = new MainPack();
                        pack.ActionCode = ActionCode.Request_PlayerInOutRoom;

                        PlayerInfoPack playerInfoPack = new PlayerInfoPack();
                        playerInfoPack.UserID = $"00000{TestInfoData.newPlayerId}";
                        playerInfoPack.NickName = $"Player{TestInfoData.newPlayerId}";
                        playerInfoPack.Chips = 2000;

                        PlayerInOutRoomPack playerInOutRoomPack = new PlayerInOutRoomPack();
                        playerInOutRoomPack.IsInRoom = true;
                        playerInOutRoomPack.PlayerInfoPack = playerInfoPack;

                        pack.PlayerInOutRoomPack = playerInOutRoomPack;
                        CurrGameServer.Request_PlayerInOutRoom(pack);

                        TestInfoData.newPlayerId++;
                    }
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    TestInfoData.foldTimd = DateTime.Now;
                    CurrGameServer.TextPlayerAction(RequestBuf.ActingEnum.Fold, true);
                }                

                if (Input.GetKeyDown(KeyCode.Keypad1))
                {
                    CurrGameServer.TestShowFoldPoker(0, 0);
                }
                if (Input.GetKeyDown(KeyCode.Keypad2))
                {
                    CurrGameServer.TestShowFoldPoker(1, 0);
                }
                if (Input.GetKeyDown(KeyCode.Keypad4))
                {
                    CurrGameServer.TestShowFoldPoker(0, 1);
                }
                if (Input.GetKeyDown(KeyCode.Keypad5))
                {
                    CurrGameServer.TestShowFoldPoker(1, 1);
                }
            }
        }
        
    }

    /// <summary>
    /// 網頁視窗失去焦點
    /// </summary>
    public void OnWindowBlur()
    {
        GameRoomManager.Instance.OnGamePause(true);
    }

    /// <summary>
    /// 網頁視窗獲得焦點
    /// </summary>
    public void OnWindowFocus()
    {
        
    }

    /// <summary>
    /// 是否為移動平台
    /// </summary>
    public void IsMobilePlatform(string isMobile)
    {
        GameDataManager.IsMobilePlatform = isMobile == "true";
        Debug.Log($"IsMobilePlatform:{isMobile}");
    }

    /// <summary>
    /// 網頁debug
    /// </summary>
    /// <param name="str"></param>
    public void HtmlDebug(string str)
    {
        Debug.Log($"Html Debug: {str}");
    }
}
