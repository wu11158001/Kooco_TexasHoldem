using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using RequestBuf;

public class JoinRoomView : MonoBehaviour
{
    readonly List<int> smallblindList = new List<int>
    {
        50,
        200,
        400,
    };

    [SerializeField]
    Dropdown blind_Dr;
    [SerializeField]
    Slider carryChips_Sli;
    [SerializeField]
    Text carryChips_Txt;
    [SerializeField]
    Button start_Btn;

    private void Awake()
    {
        blind_Dr.onValueChanged.AddListener((value) =>
        {
            carryChips_Sli.minValue = smallblindList[value] * 40;
            carryChips_Sli.maxValue = smallblindList[value] * 200;
            carryChips_Sli.value = carryChips_Sli.minValue;
        });

        carryChips_Sli.onValueChanged.AddListener((value) =>
        {
            float stepSize = smallblindList[blind_Dr.value] * 2;
            float newRaiseValue = Mathf.Round(value / stepSize) * stepSize;
            carryChips_Sli.value = newRaiseValue >= carryChips_Sli.maxValue ? carryChips_Sli.maxValue : newRaiseValue;
            carryChips_Txt.text = StringUtils.SetChipsUnit(newRaiseValue);
        });

        start_Btn.onClick.AddListener(() =>
        {
            Entry.Instance.gameServer.SmallBlind = (int)smallblindList[blind_Dr.value];
            Entry.Instance.gameServer.gameObject.SetActive(true);
            
            
            MainPack pack = new MainPack();
            pack.ActionCode = ActionCode.Request_PlayerInOutRoom;

            PlayerInfoPack playerInfoPack = new PlayerInfoPack();
            playerInfoPack.UserID = Entry.TestInfoData.LocalUserId;
            playerInfoPack.NickName = Entry.TestInfoData.NickName;
            playerInfoPack.Chips = (int)carryChips_Sli.value;

            PlayerInOutRoomPack playerInOutRoomPack = new PlayerInOutRoomPack();
            playerInOutRoomPack.IsInRoom = true;
            playerInOutRoomPack.PlayerInfoPack = playerInfoPack;

            pack.PlayerInOutRoomPack = playerInOutRoomPack;
            Entry.Instance.gameServer.Request_PlayerInOutRoom(pack);

            UIManager.Instance.OpenView(ViewName.GameView);
        });
    }

    private void Start()
    {
        blind_Dr.value = 1;
    }
}
