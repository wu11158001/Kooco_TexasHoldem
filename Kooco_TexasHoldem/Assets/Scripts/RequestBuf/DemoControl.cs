using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using RequestBuf;

public class DemoControl : MonoBehaviour
{
    [SerializeField]
    Button Switch_Btn,
           AddNewPlayer_Btn, Exit_Btn, Fold_Btn, Raise_Btn, CallAndCheck_Btn, AllIn_Btn, Chat_Btn;

    [SerializeField]
    GameServer gameServer;

    [SerializeField]
    GameObject OtherControl_Tr;

    private void Awake()
    {
        OtherControl_Tr.SetActive(false);
        IsShowDemoControl(false);
    }

    private void Start()
    {
        Switch_Btn.onClick.AddListener(() =>
        {
            OtherControl_Tr.SetActive(!OtherControl_Tr.activeSelf);
        });


        AddNewPlayer_Btn.onClick.AddListener(() =>
        {
            if (gameServer.clientList.Count < gameServer.maxRoomPeople)
            {
                MainPack pack = new MainPack();
                pack.ActionCode = ActionCode.Request_PlayerInOutRoom;

                PlayerInfoPack playerInfoPack = new PlayerInfoPack();
                playerInfoPack.UserID = $"00000{Entry.TestInfoData.newPlayerId}";
                playerInfoPack.NickName = $"Player{Entry.TestInfoData.newPlayerId}";
                playerInfoPack.Avatar = UnityEngine.Random.Range(0, 3);
                playerInfoPack.Chips = 2000;

                PlayerInOutRoomPack playerInOutRoomPack = new PlayerInOutRoomPack();
                playerInOutRoomPack.IsInRoom = true;
                playerInOutRoomPack.PlayerInfoPack = playerInfoPack;

                pack.PlayerInOutRoomPack = playerInOutRoomPack;
                gameServer.Request_PlayerInOutRoom(pack);

                Entry.TestInfoData.newPlayerId++;

                IsShowDemoControl(false);
            }
        });

        Exit_Btn.onClick.AddListener(() =>
        {
            gameServer.TextPlayerAction(ActingEnum.Fold, true);
            IsShowDemoControl(false);
        });

        Fold_Btn.onClick.AddListener(() =>
        {
            gameServer.TextPlayerAction(ActingEnum.Fold);
            IsShowDemoControl(false);
        });

        Raise_Btn.onClick.AddListener(() =>
        {
            gameServer.TextPlayerAction(ActingEnum.Raise);
            IsShowDemoControl(false);
        });

        CallAndCheck_Btn.onClick.AddListener(() =>
        {
            gameServer.TextPlayerAction(ActingEnum.Call);
            IsShowDemoControl(false);
        });

        AllIn_Btn.onClick.AddListener(() =>
        {
            gameServer.TextPlayerAction(ActingEnum.AllIn);
            IsShowDemoControl(false);
        });

        Chat_Btn.onClick.AddListener(() =>
        {
            gameServer.TextChat();
        });
    }

    public void IsShowDemoControl(bool isShow)
    {
        //OtherControl_Tr.SetActive(false);
        //OtherControl_Tr.SetActive(isShow);
    }
}
