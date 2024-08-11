using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class GameControl : MonoBehaviour
{
    [SerializeField]
    GameView gameView;
    [SerializeField]
    RobotControl RobotControl;

    public string QueryRoomPath { get; set; }                   //查詢房間資料路徑
    public double SmallBlind { get; set; }                      //小盲值
    public TableTypeEnum RoomType { get; set; }                 //房間類型
    public int MaxRoomPeople { get; set; }                      //房間最大人數

    GameRoomData gameRoomData = new();                          //房間資料
    Coroutine cdCoroutine;                                      //倒數Coroutine


    int prePlayerCount { get; set; }                            //上個紀錄的遊戲人數
    bool isWaitingCreateRobot { get; set; }                     //是否等待產生機器人
    bool isGameStart { get; set; }                              //是否遊戲開始
    GameFlowEnum preUpdateGameFlow { get; set; }                //上個更新遊戲流程
    GameFlowEnum preLocalGameFlow { get; set; }                 //上個本地遊戲流程
    string preBetActionerId { get; set; }                       //上個下注玩家
    int preCD { get; set; }                                     //當前行動倒數時間


    private void Update()
    {
        #region 測試

        if (Entry.Instance.releaseType == ReleaseEnvironmentEnum.Test)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                CreateRobot();
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                RemoveRobot();
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                string id = gameRoomData.currActionerId;
                UpdateBetAction(id,
                                BetActingEnum.Call,
                                gameRoomData.currCallValue);
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                string id = gameRoomData.currActionerId;
                UpdateBetAction(id,
                                BetActingEnum.Check,
                                0);
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                string id = gameRoomData.currActionerId;
                UpdateBetAction(id,
                                BetActingEnum.Raise,
                                gameRoomData.currCallValue + gameRoomData.smallBlind);
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                string id = gameRoomData.currActionerId;
                UpdateBetAction(id,
                                BetActingEnum.Fold,
                                0);
            }
        }

        #endregion

        //初始遊戲開始
        if (isGameStart == false &&
            gameRoomData.playerDataDic.Count >= 2 )
        {
            isGameStart = true;
            StartCoroutine(IStartGameFlow(GameFlowEnum.Licensing));
        }
    }

    #region 起始/結束

    /// <summary>
    /// 離開遊戲
    /// </summary>
    public void ExitGame()
    {
        Debug.Log($"Curr Room Player Count:{gameRoomData.playerDataDic.Count}");

        //機器人數量
        int robotCount = gameRoomData.playerDataDic.Where(x => x.Value.userId.StartsWith(FirebaseManager.ROBOT_ID))
                                                         .Count();

        //停止監聽遊戲房間資料
        JSBridgeManager.Instance.StopListeningForDataChanges($"{QueryRoomPath}");
        //移除監測連線狀態
        JSBridgeManager.Instance.RemoveListenerConnectState($"{QueryRoomPath}/{FirebaseManager.PLAYER_DATA_LIST}/{DataManager.UserId}");

        //移除房間判斷
        if (gameRoomData.playerDataDic.Count - robotCount == 1)
        {
            Debug.Log("Remove Room!!!");
            //房間剩下1名玩家
            JSBridgeManager.Instance.RemoveDataFromFirebase($"{QueryRoomPath}");
        }
        else
        {
            //移除玩家
            Debug.Log("Remove Player!!!");
            RemovePlayer(DataManager.UserId);
        }

        //本地玩家房間關閉
        GameRoomManager.Instance.RemoveGameRoom(transform.name);
    }

    /// <summary>
    /// 房間啟動
    /// </summary>
    public void RoomStart()
    {
        //讀取房間資料
        JSBridgeManager.Instance.ReadDataFromFirebase($"{QueryRoomPath}",
                                                      gameObject.name,
                                                      nameof(ReadGameRoomDataCallback));
    }

    /// <summary>
    /// 讀取房間資料回傳
    /// </summary>
    /// <param name="jsonData"></param>
    public void ReadGameRoomDataCallback(string jsonData)
    {
        Debug.Log($"Read Game Room Data Callback:{jsonData}");
        var data = FirebaseManager.Instance.OnFirebaseDataRead<GameRoomData>(jsonData);
        gameRoomData = data;

        //更新房間玩家訊息
        gameView.UpdateGameRoomInfo(gameRoomData);

        //開始監聽遊戲房間資料
        JSBridgeManager.Instance.StartListeningForDataChanges($"{QueryRoomPath}",
                                                              gameObject.name,
                                                              nameof(GameRoomDataCallback));

        //開始監聽連線狀態
        JSBridgeManager.Instance.StartListenerConnectState($"{QueryRoomPath}/{FirebaseManager.PLAYER_DATA_LIST}/{DataManager.UserId}");

        //產生機器人
        if (isWaitingCreateRobot)
        {
            isWaitingCreateRobot = false;
            CreateRobot();
        }
    }

    #endregion

    #region 玩家進出房間

    /// <summary>
    /// 創建首個玩家
    /// </summary>
    /// <param name="carryChips">攜帶籌碼</param>
    /// <param name="seatIndex">遊戲座位</param>
    public void CreateFirstPlayer(double carryChips, int seatIndex)
    {
        isWaitingCreateRobot = true;

        var dataDic = new Dictionary<string, object>()
        {
            { FirebaseManager.USER_ID, DataManager.UserId},                         //用戶ID
            { FirebaseManager.NICKNAME, DataManager.UserNickname},                  //暱稱
            { FirebaseManager.AVATAR_INDEX, DataManager.UserAvatarIndex},           //頭像編號
            { FirebaseManager.CARRY_CHIPS, carryChips},                             //攜帶籌碼
            { FirebaseManager.GAME_SEAT, seatIndex},                                //遊戲座位
            { FirebaseManager.GAME_STATE, (int)PlayerStateEnum.Waiting},            //遊戲狀態(等待下局/遊戲中/All In/棄牌)
        };
        UpdataPlayerData(DataManager.UserId,
                         dataDic);

        RoomStart();
    }

    /// <summary>
    /// 新玩家加入房間
    /// </summary>
    /// <param name="carryChips">攜帶籌碼</param>
    /// <param name="seatIndex">遊戲座位</param>
    public void NewPlayerInRoom(double carryChips, int seatIndex)
    {
        //添加新玩家
        var dataDic = new Dictionary<string, object>()
        {
            { FirebaseManager.USER_ID, DataManager.UserId},                         //用戶ID
            { FirebaseManager.NICKNAME, DataManager.UserNickname},                  //暱稱
            { FirebaseManager.AVATAR_INDEX, DataManager.UserAvatarIndex},           //頭像編號
            { FirebaseManager.CARRY_CHIPS, carryChips},                             //攜帶籌碼
            { FirebaseManager.GAME_SEAT, seatIndex},                                //遊戲座位
            { FirebaseManager.GAME_STATE, (int)PlayerStateEnum.Waiting},            //遊戲狀態(等待下局/遊戲中/All In/棄牌)
        };
        UpdataPlayerData(DataManager.UserId,
                         dataDic);

        RoomStart();
    }

    /// <summary>
    /// 移除玩家
    /// </summary>
    /// <param name="id"></param>
    private void RemovePlayer(string id)
    {
        //玩家列表中移除
        if (gameRoomData.playerDataDic.ContainsKey(id))
        {
            JSBridgeManager.Instance.RemoveDataFromFirebase($"{QueryRoomPath}/{FirebaseManager.PLAYER_DATA_LIST}/{id}");
        }
        //遊戲中玩家列表忠移除
        if (gameRoomData.playingPlayersIdList.Contains(id))
        {
            JSBridgeManager.Instance.RemoveDataFromFirebase($"{QueryRoomPath}/{FirebaseManager.PLAYING_PLAYER_ID}/{id}");
        }
        //AllIn列表中移除
        if (gameRoomData.allInDataDic.ContainsKey(id))
        {
            JSBridgeManager.Instance.RemoveDataFromFirebase($"{QueryRoomPath}/{FirebaseManager.ALL_IN_PLAYER_DATA}/{id}");
        }

        JSBridgeManager.Instance.RemoveDataFromFirebase($"{QueryRoomPath}/{FirebaseManager.PLAYER_DATA_LIST}/{id}");
    }

    #endregion

    #region 機器人

    /// <summary>
    /// 產生機器人
    /// </summary>
    private void CreateRobot()
    {
        //設置座位
        int robotSeat = TexasHoldemUtil.SetGameSeat(gameRoomData);

        //機器人暱稱
        string[] names = {
            "Oliver", "Amelia", "William", "Emma", "James", "Olivia", "Benjamin", "Ava",
            "Lucas", "Sophia", "Henry", "Isabella", "Alexander", "Mia", "Michael", "Charlotte",
            "Elijah", "Harper", "Daniel", "Evelyn", "Matthew", "Abigail", "Joseph", "Emily",
            "David", "Ella", "Jackson", "Lily", "Samuel", "Grace", "Sebastian", "Chloe",
            "Owen", "Victoria", "Jack", "Riley", "Aiden", "Aria", "John", "Scarlett",
            "Luke", "Zoey", "Gabriel", "Lillian", "Anthony", "Aubrey", "Isaac", "Addison",
            "Dylan", "Eleanor", "Wyatt", "Nora", "Carter", "Hannah", "Julian", "Stella",
            "Levi", "Bella", "Isaiah", "Lucy", "Nolan", "Ellie", "Hunter", "Paisley",
            "Caleb", "Audrey", "Christian", "Claire", "Josiah", "Skylar", "Andrew", "Camila",
            "Thomas", "Penelope", "Nathan", "Layla", "Eli", "Anna", "Aaron", "Aaliyah",
            "Charles", "Gabriella", "Connor", "Madelyn", "Jeremiah", "Alice", "Ezekiel", "Ariana",
            "Colton", "Ruby", "Jordan", "Eva", "Cameron", "Serenity", "Nicholas", "Autumn",
            "Adrian", "Quinn", "Grayson", "Peyton"
        };
        string robotName = names[UnityEngine.Random.Range(0, names.Length)];
        while (gameRoomData.playerDataDic.Values.Any(x => x.nickname == robotName))
        {
            robotName = names[UnityEngine.Random.Range(0, names.Length)];
        }

        //機器人頭像
        int avatarLength = AssetsManager.Instance.GetAlbumAsset(AlbumEnum.AvatarAlbum).album.Length;
        int robotAvatar = UnityEngine.Random.Range(0, avatarLength);

        //機器人攜帶籌碼
        double robotCarryChips = (SmallBlind * 2) * 80;

        //機器人ID
        string robotId = $"{FirebaseManager.ROBOT_ID}{gameRoomData.robotIndex + 1}";

        //添加機器人
        var dataDic = new Dictionary<string, object>()
        {
            { FirebaseManager.USER_ID, robotId},                             //用戶ID
            { FirebaseManager.NICKNAME, robotName},                          //暱稱
            { FirebaseManager.AVATAR_INDEX, robotAvatar },                   //頭像編號
            { FirebaseManager.CARRY_CHIPS, robotCarryChips},                 //攜帶籌碼
            { FirebaseManager.GAME_SEAT, robotSeat},                         //遊戲座位
            { FirebaseManager.GAME_STATE, PlayerStateEnum.Waiting},          //遊戲狀態(等待下局/遊戲中/All In/棄牌)
        };
        UpdataPlayerData(robotId,
                         dataDic);

        //更新房間機器人編號
        var updateDataDic = new Dictionary<string, object>()
        {
            { FirebaseManager.ROBOT_INDEX, gameRoomData.robotIndex + 1},
        };
        JSBridgeManager.Instance.UpdateDataFromFirebase($"{QueryRoomPath}",
                                                        updateDataDic);
    }

    /// <summary>
    /// 移除機器人
    /// </summary>
    private void RemoveRobot()
    {
        string robotId = gameRoomData.playerDataDic.Values.Where(x => x.userId.StartsWith(FirebaseManager.ROBOT_ID))
                                                          .FirstOrDefault()
                                                          .userId;

        if (!string.IsNullOrEmpty(robotId))
        {
            RemovePlayer(robotId);
        }
    }

    #endregion

    #region 斷線判斷

    /// <summary>
    /// 判斷房主
    /// </summary>
    private void JudgeHost()
    {
        //房主離開/斷線
        GameRoomPlayerData host = gameRoomData.playerDataDic.Where(x => x.Value.userId == gameRoomData.hostId)
                                                              .FirstOrDefault()
                                                              .Value;
        if (host == null ||
            host.online == false)
        {
            string oldHostId = gameRoomData.hostId;

            //尋找下位房主
            string newHostID = "";
            foreach (var player in gameRoomData.playerDataDic.Values)
            {
                if (!player.userId.StartsWith(FirebaseManager.ROBOT_ID) &&
                    player.online == true)
                {
                    newHostID = player.userId;
                    Debug.Log($"Change Host:{player.userId}");
                    break;
                }
            }

            //尋找新房主錯誤
            if (string.IsNullOrEmpty(newHostID))
            {
                Debug.LogError("New Host Is Null");
                return;
            }

            //新房主是本地端
            if (newHostID == DataManager.UserId)
            {
                //更新房主
                var dataDic = new Dictionary<string, object>()
                {
                     { FirebaseManager.ROOM_HOST_ID, DataManager.UserId},
                };
                JSBridgeManager.Instance.UpdateDataFromFirebase($"{QueryRoomPath}",
                                                                dataDic);

                //舊房主斷線
                if (host != null &&
                    host.online == false)
                {
                    //移除舊房主
                    RemovePlayer(oldHostId);
                }
            }
        }
    }

    #endregion

    #region 遊戲流程控制

    /// <summary>
    /// 開始遊戲流程
    /// </summary>
    /// <param name="gameFlow">遊戲流程</param>
    private IEnumerator IStartGameFlow(GameFlowEnum gameFlow)
    {
        if (preUpdateGameFlow == gameFlow ||
            gameRoomData.hostId != DataManager.UserId)
        {
            yield break;
        }
        preUpdateGameFlow = gameFlow;
        Debug.Log($"Start Game Flow:{gameFlow}");

        //重製房間資料
        var roomData = new Dictionary<string, object>()
        {
            { FirebaseManager.CURR_ACTIONER_ID, ""},                                        //當前行動玩家ID
            { FirebaseManager.CURR_CALL_VALUE, gameRoomData.smallBlind * 2},                //當前跟注值
            { FirebaseManager.ACTIONP_PLAYER_COUNT, 0},                                     //當前流程行動玩家次數
            { FirebaseManager.ACTION_CD, 0},                                                //行動倒數時間
        };
        UpdateGameRoomData(roomData);

        //重製所有玩家
        foreach (var item in gameRoomData.playerDataDic.Values)
        {
            var playerData = new Dictionary<string, object>()
            {
                { FirebaseManager.CURR_ALL_BET_CHIPS, 0},             //該回合總下注籌碼
                { FirebaseManager.IS_BET, false},                     //該流程是否已下注
            };
            JSBridgeManager.Instance.UpdateDataFromFirebase($"{QueryRoomPath}/{FirebaseManager.PLAYER_DATA_LIST}/{item.userId}",
                                                            playerData);
        }

        //重製下注行為
        preBetActionerId = "";
        var betActionData = new Dictionary<string, object>()
        {
            { FirebaseManager.BET_ACTIONER_ID, ""},
            { FirebaseManager.BET_ACTION, 0},
            { FirebaseManager.BET_ACTION_VALUE, 0},
            { FirebaseManager.UPDATE_CARRY_CHIPS, 0},
        };
        JSBridgeManager.Instance.UpdateDataFromFirebase($"{QueryRoomPath}/{FirebaseManager.BET_ACTION_DATA}",
                                                        betActionData);

        var data = new Dictionary<string, object>();
        switch (gameFlow)
        {
            //發牌
            case GameFlowEnum.Licensing:

                //遊戲資料初始化
                GameDataInit();

                yield return new WaitForSeconds(1);

                //更新遊戲流程
                data = new Dictionary<string, object>()
                {
                    { FirebaseManager.CURR_GAME_FLOW, (int)GameFlowEnum.Licensing},         //當前遊戲流程
                };
                UpdateGameRoomData(data);

                break;

            //大小盲
            case GameFlowEnum.SetBlind:

                //更新遊戲流程
                data = new Dictionary<string, object>()
                {
                    { FirebaseManager.CURR_GAME_FLOW, (int)GameFlowEnum.SetBlind},           //當前遊戲流程
                };
                UpdateGameRoomData(data);
                break;

            //翻牌
            case GameFlowEnum.Flop:

                //更新公共牌翻牌流程
                UpdateCommunityFlopSeason(GameFlowEnum.Flop,
                                          3);
                break;

            //轉牌
            case GameFlowEnum.Turn:

                //更新公共牌翻牌流程
                UpdateCommunityFlopSeason(GameFlowEnum.Turn,
                                          4);
                break;

            //河牌
            case GameFlowEnum.River:

                //更新公共牌翻牌流程
                UpdateCommunityFlopSeason(GameFlowEnum.River,
                                          5);
                break;

            //遊戲結果
            case GameFlowEnum.PotResult:

                break;
        }
    }

    /// <summary>
    /// 更新公共牌翻牌流程
    /// </summary>
    /// <param name="inGameFlow">進入流程</param>
    /// <param name="takeCommunityPoker">顯示的公共牌數量</param>
    private void UpdateCommunityFlopSeason(GameFlowEnum inGameFlow, int takeCommunityPoker)
    {
        //首位行動玩家=Button座位
        List<GameRoomPlayerData> players = GetPlayingPlayer();
        string nextPlayerId = players.Where(x => x.gameSeat == gameRoomData.buttonSeat)
                                     .FirstOrDefault()
                                     .userId;

        //更新遊戲流程
        var data = new Dictionary<string, object>()
        {
            { FirebaseManager.CURR_GAME_FLOW, (int)inGameFlow},                                                 //當前遊戲流程
            { FirebaseManager.CURR_COMMUNITY_POKER, gameRoomData.communityPoker.Take(takeCommunityPoker)},      //當前顯示公共牌
            { FirebaseManager.CURR_ACTIONER_ID, nextPlayerId},                                                  //當前行動玩家Id
        };
        UpdateGameRoomData(data);
    }

    /// <summary>
    /// 監聽遊戲房間資料回傳
    /// </summary>
    /// <param name="jsonData"></param>
    public void GameRoomDataCallback(string jsonData)
    {
        Debug.Log($"Game Room Data Callback:{jsonData}");
        //同步資料
        var data = FirebaseManager.Instance.OnFirebaseDataRead<GameRoomData>(jsonData);
        gameRoomData = data;

        gameView.SetTotalPot = gameRoomData.potChips;

        //判斷房主
        JudgeHost();

        //人數有變化更新房間玩家訊息
        if (gameRoomData.playerDataDic.Count != prePlayerCount)
        {
            prePlayerCount = gameRoomData.playerDataDic.Count;
            gameView.UpdateGameRoomInfo(gameRoomData);
        }

        //遊戲流程回傳
        LocalGameFlowBehavior();

        //下注行為演出
        ShowBetAction();

        //行動倒數
        CountDown();
    }

    /// <summary>
    /// 遊戲流程回傳
    /// </summary>
    private void LocalGameFlowBehavior()
    {
        if (preLocalGameFlow == (GameFlowEnum)gameRoomData.currGameFlow)
        {
            return;
        }

        //本地遊戲流程行為
        StartCoroutine(ILocalGameFlowBehavior());
    }
    /// <summary>
    /// 遊戲流程回傳
    /// </summary>
    private IEnumerator ILocalGameFlowBehavior()
    {
        Debug.Log("Game Flow Callback!");
        preLocalGameFlow = (GameFlowEnum)gameRoomData.currGameFlow;

        yield return gameView.IGameStage(gameRoomData,
                                         SmallBlind);

        var data = new Dictionary<string, object>();
        switch ((GameFlowEnum)gameRoomData.currGameFlow)
        {
            //發牌
            case GameFlowEnum.Licensing:

                gameView.OnLicensingFlow(gameRoomData);

                //房主執行
                if (gameRoomData.hostId == DataManager.UserId)
                {
                    StartCoroutine(IStartGameFlow(GameFlowEnum.SetBlind));
                }
                break;

            //大小盲
            case GameFlowEnum.SetBlind:

                gameView.OnBlindFlow(gameRoomData);

                yield return new WaitForSeconds(1);

                //房主執行
                if (gameRoomData.hostId == DataManager.UserId)
                {
                    //設置下位行動玩家
                    UpdateNextPlayer();
                }
                break;

            //翻牌
            case GameFlowEnum.Flop:

                //開始公共牌翻牌流程
                yield return IStartCommunityFlopSeason();

                break;

            //轉牌
            case GameFlowEnum.Turn:

                //開始公共牌翻牌流程
                yield return IStartCommunityFlopSeason();

                break;

            //河牌
            case GameFlowEnum.River:

                //開始公共牌翻牌流程
                yield return IStartCommunityFlopSeason();

                break;

            //遊戲結果
            case GameFlowEnum.PotResult:

                break;
        }
    }

    /// <summary>
    /// 開始公共牌翻牌流程
    /// </summary>
    /// <returns></returns>
    private IEnumerator IStartCommunityFlopSeason()
    {
        if (cdCoroutine != null) StopCoroutine(cdCoroutine);

        //翻開公共牌
        yield return gameView.IFlopCommunityPoker(gameRoomData.currCommunityPoker,
                                                  gameRoomData);

        yield return new WaitForSeconds(1);

        //房主執行
        if (gameRoomData.hostId == DataManager.UserId)
        {
            var data = new Dictionary<string, object>()
            {
                { FirebaseManager.ACTION_CD, DataManager.StartCountDownTime},           //行動倒數時間
            };
            UpdateGameRoomData(data);
        }
    }

    /// <summary>
    /// 行動倒數
    /// </summary>
    public void CountDown()
    {
        if ((preCD < DataManager.StartCountDownTime && preCD == gameRoomData.actionCD) ||
            gameRoomData.actionCD <= 0)
        {
            return;
        }

        if (string.IsNullOrEmpty(gameRoomData.currActionerId))
        {
            return;
        }

        preCD = gameRoomData.actionCD;

        //行動倒數
        if (cdCoroutine != null) StopCoroutine(cdCoroutine);
        cdCoroutine = StartCoroutine(ICountdown());
    }
    /// <summary>
    /// 行動倒數
    /// </summary>
    private IEnumerator ICountdown()
    {
        GamePlayerInfo player = gameView.GetPlayer(gameRoomData.playerDataDic[gameRoomData.currActionerId].userId);
        if (gameRoomData.actionCD == DataManager.StartCountDownTime)
        {
            yield return new WaitForSeconds(2);

            Debug.Log("Local Player Start Action!!!");
            player.InitCountDown();

            if (player.UserId == DataManager.UserId)
            {
                gameView.LocalPlayerRound(gameRoomData);
            }
        }

        if (gameRoomData.actionCD <= 0)
        {
            yield break;
        }

        player.ActionFrame = true;
        player.CountDown(DataManager.StartCountDownTime,
                         gameRoomData.actionCD);

        Debug.Log($"Action Countdown:{gameRoomData.actionCD}");

        yield return new WaitForSeconds(1);

        if (gameRoomData.actionCD <= 0)
        {
            yield break;
        }

        //房主執行
        if (gameRoomData.hostId == DataManager.UserId)
        {
            gameRoomData.actionCD -= 1;

            if (gameRoomData.actionCD < 0)
            {
                //超過時間棄牌
                string id = gameRoomData.currActionerId;
                UpdateBetAction(id,
                                BetActingEnum.Fold,
                                0);
            }
            else
            {
                //機器人動作
                if (player.UserId.StartsWith(FirebaseManager.ROBOT_ID) &&
                    gameRoomData.actionCD == 12)
                {
                    RobotControl.RobotBet(gameRoomData);
                }

                //更新倒數
                var data = new Dictionary<string, object>()
                {
                    { FirebaseManager.ACTION_CD, gameRoomData.actionCD},              //行動倒數時間
                };
                UpdateGameRoomData(data);
            }
        }
    }

    /// <summary>
    /// 下注行為演出
    /// </summary>
    public void ShowBetAction()
    {
        if (string.IsNullOrEmpty(gameRoomData.betActionDataDic.betActionerId) ||
            preBetActionerId == gameRoomData.betActionDataDic.betActionerId)
        {
            return;
        }
        preBetActionerId = gameRoomData.betActionDataDic.betActionerId;

        gameView.GetPlayerAction(gameRoomData);
        if (cdCoroutine != null) StopCoroutine(cdCoroutine);

        StartCoroutine(IJudgeNextSeason());
    }

    /// <summary>
    /// 判斷是否進入下個流程
    /// </summary>
    /// <returns></returns>
    private IEnumerator IJudgeNextSeason()
    {
        //房主執行
        if (gameRoomData.hostId == DataManager.UserId)
        {
            //判斷是否進入下流程
            List<GameRoomPlayerData> playingPlayers = GetPlayingPlayer();
            bool isAllBet = playingPlayers.All(x => x.isBet == true);
            bool isBetValueEqual = playingPlayers.All(x => x.currAllBetChips == playingPlayers[0].currAllBetChips);

            if (isAllBet == true &&
                isBetValueEqual == true)
            {
                yield return new WaitForSeconds(2);

                int nextFlowIndex = (gameRoomData.currGameFlow + 1) % Enum.GetValues(typeof(GameFlowEnum)).Length;
                GameFlowEnum nextFlow = (GameFlowEnum)Mathf.Max(1, nextFlowIndex);
                yield return IStartGameFlow(nextFlow);
                
                yield break;
            }

            //設置下位行動玩家
            UpdateNextPlayer();
        }
    }

    #endregion

    #region 遊戲資料更新

    /// <summary>
    /// 遊戲資料初始化
    /// </summary>
    private void GameDataInit()
    {
        //遊戲中玩家
        List<string> playingPlayersId = new();
        foreach (var player in gameRoomData.playerDataDic)
        {
            playingPlayersId.Add(player.Key);
        }

        //設置Button座位
        SetButtonSeat();

        //更新房間資料
        var data = new Dictionary<string, object>()
        {
            { FirebaseManager.POT_CHIPS, 0},                                        //底池
            { FirebaseManager.PLAYING_PLAYER_ID, playingPlayersId},                 //遊戲中玩家ID
            { FirebaseManager.COMMUNITY_POKER, SetPoker()},                         //公共牌
            { FirebaseManager.BUTTON_SEAT, gameRoomData.buttonSeat},                //Button座位
        };
        UpdateGameRoomData(data);

        //AllIn資料
        if (gameRoomData.allInDataDic?.Count > 0)
        {
            JSBridgeManager.Instance.RemoveDataFromFirebase($"{QueryRoomPath}/{FirebaseManager.ALL_IN_PLAYER_DATA}");
        }

        Debug.Log("Game Data Init Finish !");
    }

    /// <summary>
    /// 更新玩家個人資料
    /// </summary>
    /// <param name="id">玩家ID</param>
    /// <param name="dataDic">更新資料</param>
    public void UpdataPlayerData(string id, Dictionary<string, object> dataDic)
    {
        JSBridgeManager.Instance.UpdateDataFromFirebase($"{QueryRoomPath}/{FirebaseManager.PLAYER_DATA_LIST}/{id}",
                                                        dataDic);
    }

    /// <summary>
    /// 更新AllIn玩家資料
    /// </summary>
    /// <param name="id">玩家ID</param>
    /// <param name="dataDic">更新資料</param>
    public void UpdataAllInPlayerData(string id, Dictionary<string, object> dataDic)
    {
        JSBridgeManager.Instance.UpdateDataFromFirebase($"{QueryRoomPath}/{FirebaseManager.ALL_IN_PLAYER_DATA}/{id}",
                                                       dataDic);
    }

    /// <summary>
    ///更新遊戲房間資料
    /// </summary>
    /// <param name="data"></param>
    public void UpdateGameRoomData(Dictionary<string, object> data)
    {
        JSBridgeManager.Instance.UpdateDataFromFirebase($"{QueryRoomPath}",
                                                        data);
    }

    /// <summary>
    /// 設置下位行動玩家
    /// </summary>
    private void UpdateNextPlayer()
    {
        //尋找下位玩家
        List<GameRoomPlayerData> playingPlayers = GetPlayingPlayer();

        int currActionPlayerIndex = playingPlayers.Select(((v, i) => (v, i)))
                                                  .Where(x => x.v.userId == gameRoomData.currActionerId)
                                                  .FirstOrDefault()
                                                  .i;
        string nextPlayerID = playingPlayers[(currActionPlayerIndex + 1) % playingPlayers.Count()].userId;

        //更新資料
        if (cdCoroutine != null) StopCoroutine(cdCoroutine);
        var data = new Dictionary<string, object>()
        {
            { FirebaseManager.CURR_ACTIONER_ID, nextPlayerID},                      //當前行動玩家Id
            { FirebaseManager.ACTION_CD, DataManager.StartCountDownTime},           //行動倒數時間
        };
        UpdateGameRoomData(data);
    }

    /// <summary>
    /// 更新下注行為
    /// </summary>
    /// <param name="id">玩家ID</param>
    /// <param name="betActing">下注動作</param>
    /// <param name="betValue">下注值</param>
    public void UpdateBetAction(string id, BetActingEnum betActing, double betValue)
    {
        Debug.Log($"Update Bet Action:{id}/{betActing}:{betValue}");
        if (cdCoroutine != null) StopCoroutine(cdCoroutine);

        if (betActing == BetActingEnum.Check && gameRoomData.currCallValue > 0)
        {
            betValue = gameRoomData.currCallValue;
        }

        //更新玩家資料
        GameRoomPlayerData roomPlayerData = gameRoomData.playerDataDic[id];
        double currAllBetChips = roomPlayerData.currAllBetChips + (betValue - roomPlayerData.currAllBetChips);                  //該回合總下注籌碼
        double allBetChips = roomPlayerData.allBetChips + (betValue - roomPlayerData.currAllBetChips);                          //該局總下注籌碼
        double carryChips = roomPlayerData.carryChips - (betValue - roomPlayerData.currAllBetChips);     //攜帶籌碼
        var playerData = new Dictionary<string, object>()
        {
            { FirebaseManager.CURR_ALL_BET_CHIPS, currAllBetChips},             //該回合總下注籌碼
            { FirebaseManager.ALL_BET_CHIPS, allBetChips},                      //該局總下注籌碼
            { FirebaseManager.CARRY_CHIPS, carryChips},                         //攜帶籌碼
            { FirebaseManager.IS_BET, true},                                    //該流程是否已下注
        };
        JSBridgeManager.Instance.UpdateDataFromFirebase($"{QueryRoomPath}/{FirebaseManager.PLAYER_DATA_LIST}/{id}",
                                                        playerData);

        //更新下注行為
        var betActionData = new Dictionary<string, object>()
        {
            { FirebaseManager.BET_ACTIONER_ID, id},
            { FirebaseManager.BET_ACTION, (int)betActing},
            { FirebaseManager.BET_ACTION_VALUE, betValue},
            { FirebaseManager.UPDATE_CARRY_CHIPS, carryChips},
        };
        JSBridgeManager.Instance.UpdateDataFromFirebase($"{QueryRoomPath}/{FirebaseManager.BET_ACTION_DATA}",
                                                        betActionData);

        //更新底池
        double totalPot = gameRoomData.potChips + betValue;
        double currCallValue = Math.Max(betValue, gameRoomData.currCallValue);
        double actionPlayerCount = gameRoomData.actionPlayerCount + 1;
        if (gameRoomData.actionPlayerCount == 0 &&
            betActing == BetActingEnum.Check)
        {
            actionPlayerCount = 0;
        }

        var data = new Dictionary<string, object>()
        {
            { FirebaseManager.POT_CHIPS, totalPot },                                 //底池
            { FirebaseManager.CURR_CALL_VALUE, currCallValue },                      //當前跟注值
            { FirebaseManager.ACTIONP_PLAYER_COUNT, actionPlayerCount },             //當前流程行動玩家次數
        };
        UpdateGameRoomData(data);
    }

    #endregion

    #region 遊戲工具類

    /// <summary>
    /// 設定玩家手牌與公共牌
    /// </summary>
    public List<int> SetPoker()
    {
        int poker;
        //52張撲克
        List<int> pokerList = new List<int>();
        for (int i = 0; i < 52; i++)
        {
            pokerList.Add(i);
        }

        //公共牌
        List<int> community = new();
        for (int i = 0; i < 5; i++)
        {
            poker = Licensing();
            community.Add(poker);
        }

        //玩家手牌
        foreach (var player in gameRoomData.playerDataDic)
        {
            int[] handPoker = new int[2];
            for (int i = 0; i < 2; i++)
            {
                poker = Licensing();
                handPoker[i] = poker;
            }

            //更新玩家資料
            Dictionary<string, object> dataDic = new Dictionary<string, object>()
            {
                { FirebaseManager.HAND_POKER, handPoker.ToList()},              //手牌
                { FirebaseManager.CURR_ALL_BET_CHIPS, 0},                       //該回合總下注籌碼
                { FirebaseManager.ALL_BET_CHIPS, 0},                            //該局總下注籌碼
                { FirebaseManager.GAME_STATE, PlayerStateEnum.Playing},         //遊戲狀態(等待/遊戲中/棄牌/All In)
            };
            JSBridgeManager.Instance.UpdateDataFromFirebase($"{QueryRoomPath}/{FirebaseManager.PLAYER_DATA_LIST}/{player.Key}",
                                                            dataDic);
        }

        return community;

        //發牌
        int Licensing()
        {
            int index = new System.Random().Next(0, pokerList.Count);
            int poker = pokerList[index];
            pokerList.RemoveAt(index);
            return poker;
        }
    }

    /// <summary>
    /// 設置Botton座位
    /// </summary>
    private void SetButtonSeat()
    {
        gameRoomData.buttonSeat = (gameRoomData.buttonSeat + 1) % MaxRoomPeople;
        bool isHave = gameRoomData.playerDataDic.Any(x => x.Value.gameSeat == gameRoomData.buttonSeat);
        if (isHave == false)
        {
            SetButtonSeat();
        }
    }

    /// <summary>
    /// 獲取本地玩家
    /// </summary>
    /// <returns></returns>
    public GameRoomPlayerData GetLocalPlayer()
    {
        return gameRoomData.playerDataDic.Where(x => x.Value.userId == DataManager.UserId)
                                         .FirstOrDefault()
                                         .Value;
    }

    /// <summary>
    /// 獲取遊戲中玩家
    /// </summary>
    /// <returns></returns>
    private List<GameRoomPlayerData> GetPlayingPlayer()
    {
        return gameRoomData.playerDataDic.OrderBy(x => x.Value.gameSeat)
                                         .Where(x => (PlayerStateEnum)x.Value.gameState == PlayerStateEnum.Playing)
                                         .Select(x => x.Value)
                                         .ToList();
    }

    #endregion
}
