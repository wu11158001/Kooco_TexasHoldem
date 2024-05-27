using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

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
    RectTransform gameRoomList_Tr;

    [Header("房間按鈕")]
    [SerializeField]
    RectTransform switchBtnSample;
    [SerializeField]
    Button addRoom_Btn;
    [SerializeField]
    RectTransform switchBtnParent, addRoomBtn_Tr;

    public readonly int maxRoomCount = 2;
    public readonly float moveTargetDictance = 108;     //移動房間所需移動距離

    public int CurrRoomIndex { get; set; }      //當前顯示房間編號

    private ThisData thisData;
    public class ThisData
    {
        public int CurrRoomIndex;   //當前房間編號(已開啟房間數量)

        /// <summary>
        /// (房間名, (房間View, 切換按鈕))
        /// </summary>
        public Dictionary<string, (RectTransform, SwitchRoomBtn)> RoomDic;

        public float AddSwitchBtnParnetWidth;   //切換按鈕父物件每單位寬度
        public bool IsRoomMoving;               //是否房間正在移動
        public Vector2 MouseStartPos;           //滑鼠按下起始位置
    }

    public override void Awake()
    {
        base.Awake();

        Init();
        ListenerEnent();
    }

    private void Update()
    {
        if (GetRoomCount > 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                thisData.MouseStartPos = Input.mousePosition;
            }
            if (Input.GetMouseButtonUp(0) && !thisData.IsRoomMoving)
            {
                //房間右移
                if (Input.mousePosition.x < thisData.MouseStartPos.x)
                {
                    if (gameRoomList_Tr.anchoredPosition.x < (-moveTargetDictance * (CurrRoomIndex + 1)) &&
                        GetRoomCount >= CurrRoomIndex + 2)
                    {
                        CurrRoomIndex++;
                        ChangeRoom(CurrRoomIndex);
                    }
                    else
                    {
                        ChangeRoom(CurrRoomIndex);
                    }
                }

                //房間左移
                if (Input.mousePosition.x > thisData.MouseStartPos.x)
                {
                    if (gameRoomList_Tr.anchoredPosition.x < (-moveTargetDictance * (CurrRoomIndex - 1)) &&
                        CurrRoomIndex > 0)
                    {
                        CurrRoomIndex--;
                        ChangeRoom(CurrRoomIndex);
                    }
                    else if (CurrRoomIndex == 0 && gameRoomList_Tr.anchoredPosition.x > moveTargetDictance)
                    {
                        CurrRoomIndex = -1;
                        OnGoLobby();
                    }
                    else
                    {
                        ChangeRoom(CurrRoomIndex);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 初始化
    /// </summary>
    private void Init()
    {
        thisData = new ThisData();
        thisData.RoomDic = new Dictionary<string, (RectTransform, SwitchRoomBtn)>();
        thisData.AddSwitchBtnParnetWidth = switchBtnSample.rect.width + (switchBtnParent.GetComponent<HorizontalLayoutGroup>().spacing * 2);

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
            OnGoLobby();
        });
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
    /// 移動到大廳
    /// </summary>
    private void OnGoLobby()
    {
        IsShowGameRoom = false;
        CloseAllBtnFrame();
    }

    /// <summary>
    /// 顯示遊戲房間
    /// </summary>
    public bool IsShowGameRoom
    {
        set
        {
            if (value == true)
            {
                gameRoomCanvas.sortingOrder = 50;
                addRoom_Btn.interactable = true;
            }
            else
            {
                gameRoomCanvas.sortingOrder = -1;
                addRoom_Btn.interactable = false;
            }
        }
    }

    /// <summary>
    /// 判斷是否可以創建房間
    /// </summary>
    /// <returns></returns>
    public bool JudgeIsCanBeCreateRoom()
    {
        return GetRoomCount < maxRoomCount;
    }

    /// <summary>
    /// 關閉所有切換按鈕選擇框
    /// </summary>
    public void CloseAllBtnFrame()
    {
        foreach (var room in thisData.RoomDic.Values)
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
    /// 創建遊戲房間
    /// </summary>
    /// <param name="pack"></param>
    /// <param name="roomType">房間類型</param>
    public void CerateGameRoom(MainPack pack, GameRoomEnum roomType, double smallBlind)
    {
        IsShowGameRoom = true;
        thisData.CurrRoomIndex++;

        if (GetRoomCount > maxRoomCount)
        {
            Debug.LogError("Room Count Max!!!");
            return;
        }

        //創建房間介面
        GameObject roomObj = Resources.Load<GameObject>($"GameRoom/{roomType}");
        RectTransform room = Instantiate(roomObj).GetComponent<RectTransform>();
        room.gameObject.SetActive(true);
        room.SetParent(gameRoomList_Tr);
        string roomName = $"{roomType}{thisData.CurrRoomIndex}";
        ViewManager.Instance.InitViewTr(room, roomName);
        room.sizeDelta = new Vector2(540, 960);

        //假Server
        GameServer gameServer = room.GetComponent<GameServer>();
        gameServer.SmallBlind = smallBlind;
        gameServer.RoomType = roomType;
        gameServer.ServerStart(roomType);
        gameServer.Request_PlayerInOutRoom(pack);
        Entry.CurrGameServer = gameServer;

        GameView gameView = room.GetComponent<GameView>();
        gameView.SendRequest_UpdateRoomInfo();
        gameView.RoomType = roomType;

        //關閉其他切換房間按鈕框
        CloseAllBtnFrame();

        //創建切換房間按鈕
        RectTransform switchBtnObj = Instantiate(switchBtnSample.gameObject).GetComponent<RectTransform>();
        switchBtnObj.gameObject.SetActive(true);
        switchBtnObj.SetParent(switchBtnParent);
        switchBtnObj.localScale = Vector3.one;
        SwitchRoomBtn switchRoomBtn = switchBtnObj.GetComponent<SwitchRoomBtn>();
        switchRoomBtn.SetSwitchBtnInfo(GetRoomCount - 1, GetRoomName(roomType));
        switchRoomBtn.SetSelectFrameActive = true;

        thisData.RoomDic.Add(roomName, (room, switchRoomBtn));

        addRoomBtn_Tr.SetSiblingIndex(GetRoomCount + 1);

        StartCoroutine(IJudgeShowSwitchBtn());
    }

    /// <summary>
    /// 移除房間
    /// </summary>
    /// <param name="roomName">房間名</param>
    public void RemoveGameRoom(string roomName)
    {
        if (thisData.RoomDic.ContainsKey(roomName))
        {
            Destroy(thisData.RoomDic[roomName].Item1.gameObject);
            Destroy(thisData.RoomDic[roomName].Item2.gameObject);


            thisData.RoomDic.Remove(roomName); 
        }
        else
        {
            Debug.LogError($"{roomName}:移除房間出錯");
        }

        //開啟最後一個房間切換按鈕
        if (thisData.RoomDic.Count > 0)
        {
            SwitchRoomBtn switchRoom = thisData.RoomDic.Last().Value.Item2;
            switchRoom.SetSelectFrameActive = true;
        }

        StartCoroutine(IJudgeShowSwitchBtn());
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

        //切換按鈕空間大小
        float sizeX = (GetRoomCount + 1) * thisData.AddSwitchBtnParnetWidth;
        switchBtnParent.sizeDelta = new Vector2(sizeX, switchBtnParent.sizeDelta.y);

        //房間列表
        gameRoomList_Tr.sizeDelta = new Vector2(540 * GetRoomCount, 960);
        ChangeRoom(GetRoomCount - 1);
    }

    /// <summary>
    /// 更換遊戲房間
    /// </summary>
    /// <param name="roomIndex"></param>
    public void ChangeRoom(int roomIndex)
    {
        if (roomIndex < 0) return;

        CurrRoomIndex = roomIndex;
        CloseAllBtnFrame();
        IsShowGameRoom = true;

        StartCoroutine(IRoomMove(roomIndex));
    }

    /// <summary>
    /// 房間移動
    /// </summary>
    /// <param name="roomIndex"></param>
    /// <returns></returns>
    private IEnumerator IRoomMove(int roomIndex)
    {
        thisData.IsRoomMoving = true;

        float moveTime = 0.1f;
        DateTime startTime = DateTime.Now;
        while ((DateTime.Now - startTime).TotalSeconds < moveTime)
        {
            float progress = (float)(DateTime.Now - startTime).TotalSeconds / moveTime;
            float x = Mathf.Lerp(gameRoomList_Tr.anchoredPosition.x, -540 * roomIndex, progress);
            gameRoomList_Tr.anchoredPosition = new Vector2(x, gameRoomList_Tr.anchoredPosition.y);
            yield return null;
        }

        gameRoomList_Tr.anchoredPosition = new Vector2(-540 * roomIndex, gameRoomList_Tr.anchoredPosition.y);
        thisData.RoomDic.ElementAt(roomIndex).Value.Item2.SetSelectFrameActive = true;
        thisData.IsRoomMoving = false;
    }

    /// <summary>
    /// 遊戲暫停/繼續
    /// </summary>
    /// <param name="isPause"></param>
    public void OnGamePause(bool isPause)
    {
        if (thisData.RoomDic.Count > 0)
        {
            Time.timeScale = isPause == true ? 0 : 1;
        }        

        foreach (var room in thisData.RoomDic)
        {
            room.Value.Item1.GetComponent<GameView>().GamePause = isPause;
        }
    }
}
