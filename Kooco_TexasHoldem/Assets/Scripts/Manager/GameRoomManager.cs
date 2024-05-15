using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

using RequestBuf;

public class GameRoomManager : UnitySingleton<GameRoomManager>
{
    [SerializeField]
    GameObject gameServerObj;

    [Header("Canvas")]
    [SerializeField]
    Canvas gameRoomCanvas, swtichBtnCanvas;


    [Header("房間")]
    [SerializeField]
    RectTransform gameRoomList_Tr, switchBtnParent;
    [SerializeField]
    GameObject switchBtnSample;

    [Header("新增房間按鈕")]
    [SerializeField]
    Button addRoom_Btn;
    [SerializeField]
    RectTransform addRoomBtn_Tr;

    private ThisData thisData;
    public class ThisData
    {
        public int currRoomIndex;   //當前房間編號(已開啟房間數量)

        /// <summary>
        /// (房間名, (房間View, 切換按鈕))
        /// </summary>
        public Dictionary<string, (RectTransform, SwitchRoomBtn)> roomDic;
    }

    /// <summary>
    /// 獲取房間數量
    /// </summary>
    public int GetRoomCount
    {
        get
        {
            return gameRoomList_Tr.childCount;
        }
    }

    /// <summary>
    /// 顯示遊戲房間
    /// </summary>
    public bool IsShowGameRoom
    {
        set
        {
            gameRoomCanvas.sortingOrder = value == true ?
                                          50 :
                                          -1;
        }
    }

    public override void Awake()
    {
        base.Awake();

        Init();
        ListenerEnent();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    private void Init()
    {
        thisData = new ThisData();
        thisData.roomDic = new Dictionary<string, (RectTransform, SwitchRoomBtn)>();

        switchBtnSample.gameObject.SetActive(false);
        StartCoroutine(IJudgeShowSwitchBtn());
    }

    /// <summary>
    /// 事件聆聽
    /// </summary>
    private void ListenerEnent()
    {
        //新增房間按鈕(返回大廳)
        addRoom_Btn.onClick.AddListener(() =>
        {
            IsShowGameRoom = false;
            CloseAllBtnFrame();
        });
    }

    /// <summary>
    /// 關閉所有切換按鈕選擇框
    /// </summary>
    public void CloseAllBtnFrame()
    {
        foreach (var room in thisData.roomDic.Values)
        {
            room.Item2.SetSelectFrameActive = false;
        }
    }

    /// <summary>
    /// 獲取房間名稱
    /// </summary>
    /// <param name="roomType"></param>
    /// <returns></returns>
    private string GetRoomName(GameRoomEnum roomType)
    {
        string str = "";

        switch (roomType)
        {
            //積分房
            case GameRoomEnum.BattleRoomView:
                str = "Battle";
                break;

            //現金房
            case GameRoomEnum.CashRoomView:
                str = "Cash";
                break;
            default:
                break;
        }

        return str;
    }

    /// <summary>
    /// 判斷顯示切換房間按鈕
    /// </summary>
    private IEnumerator IJudgeShowSwitchBtn()
    {
        yield return null;
        swtichBtnCanvas.sortingOrder = GetRoomCount > 0 ?
                                       50 :
                                       -1;
    }

    /// <summary>
    /// 創建遊戲房間
    /// </summary>
    /// <param name="pack"></param>
    /// <param name="roomType">房間類型</param>
    public void CerateGameRoom(MainPack pack, GameRoomEnum roomType, double smallBlind)
    {
        IsShowGameRoom = true;
        thisData.currRoomIndex++;

        //創建房間介面
        GameObject roomObj = Resources.Load<GameObject>($"GameRoom/{roomType}");
        RectTransform room = Instantiate(roomObj).GetComponent<RectTransform>();
        room.gameObject.SetActive(true);
        room.SetParent(gameRoomList_Tr);
        string roomName = $"{roomType}{thisData.currRoomIndex}";
        ViewManager.Instance.InitViewTr(room, roomName);

        //假Server
        GameServer gameServer = room.GetComponent<GameServer>();
        gameServer.SmallBlind = smallBlind;
        gameServer.ServerStart(roomType);
        gameServer.Request_PlayerInOutRoom(pack);

        GameView gameView = room.GetComponent<GameView>();
        gameView.SendRequest_UpdateRoomInfo();

        //關閉其他切換房間按鈕框
        CloseAllBtnFrame();

        //創建切換房間按鈕
        RectTransform switchBtnObj = Instantiate(switchBtnSample).GetComponent<RectTransform>();
        switchBtnObj.gameObject.SetActive(true);
        switchBtnObj.SetParent(switchBtnParent);
        SwitchRoomBtn switchRoomBtn = switchBtnObj.GetComponent<SwitchRoomBtn>();
        switchRoomBtn.SetSwitchBtnInfo(room, GetRoomName(roomType));
        switchRoomBtn.SetSelectFrameActive = true;

        thisData.roomDic.Add(roomName, (room, switchRoomBtn));

        addRoomBtn_Tr.SetSiblingIndex(GetRoomCount + 1);

        StartCoroutine(IJudgeShowSwitchBtn());
    }

    /// <summary>
    /// 移除房間
    /// </summary>
    /// <param name="roomName">房間名</param>
    public void RemoveGameRoom(string roomName)
    {
        if (thisData.roomDic.ContainsKey(roomName))
        {
            Destroy(thisData.roomDic[roomName].Item1.gameObject);
            Destroy(thisData.roomDic[roomName].Item2.gameObject);


            thisData.roomDic.Remove(roomName); 
        }
        else
        {
            Debug.LogError($"{roomName}:移除房間出錯");
        }

        //開啟最後一個房間切換按鈕
        if (thisData.roomDic.Count > 0)
        {
            SwitchRoomBtn switchRoom = thisData.roomDic.Last().Value.Item2;
            switchRoom.SetSelectFrameActive = true;
        }

        StartCoroutine(IJudgeShowSwitchBtn());
    }
}
