using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Thirdweb;
using System.Threading.Tasks;

public class LobbyView : MonoBehaviour
{
    [SerializeField]
    Request_LobbyView baseRequest;

    [Header("斷開連結")]
    [SerializeField]
    Button disconnect_Btn;
    [SerializeField]
    Text disconnectBtn_Txt;

    [Header("用戶訊息")]
    [SerializeField]
    Text walletAddress_Txt, balanceETH_Txt;

    [Header("現金房")]
    [SerializeField]
    GameObject cashRoomBtnSample;
    [SerializeField]
    RectTransform cashRoomParent;
    [SerializeField]
    Text cashRoomTital_Txt;

    [Header("積分房")]
    [SerializeField]
    Button battle_Btn;
    [SerializeField]
    Text battleBtn_Txt;

    [Header("提示")]
    [SerializeField]
    Text tip_Txt;

    Coroutine tipCorutine;

    /// <summary>
    /// 更新文本翻譯
    /// </summary>
    private void UpdateLanguage()
    {
        battleBtn_Txt.text = LanguageManager.Instance.GetText("BattleRoom");
        cashRoomTital_Txt.text = LanguageManager.Instance.GetText("CashRoom");
        disconnectBtn_Txt.text = LanguageManager.Instance.GetText("WalletDisconnect");

        #region 用戶訊息

        walletAddress_Txt.text = $"{LanguageManager.Instance.GetText("WalletAddress")}:{DataManager.UserWalletAddress}";
        balanceETH_Txt.text = $"{LanguageManager.Instance.GetText("WalletBalance")}:{DataManager.UserWalletBalance}";

        #endregion
    }

    private void Awake()
    {
        battleData = new BattleData();

        LanguageManager.Instance.AddUpdateLanguageFunc(UpdateLanguage);
        ListenerEvent();
    }

    /// <summary>
    /// 事件聆聽
    /// </summary>
    private void ListenerEvent()
    {
        //斷開連接
        disconnect_Btn.onClick.AddListener(() =>
        {
            WalletManager.Instance.OnWalletDisconnect();
            LoadSceneManager.Instance.LoadScene(SceneEnum.Login);
        });

        //積分房
        battle_Btn.onClick.AddListener(() =>
        {  
            if (battleData.isPairing)
            {
                //正在配對取消配對
                EndPair();
            }
            else
            {
                if (GameRoomManager.Instance.JudgeIsCanBeCreateRoom())
                {
                    //開始配對
                    battleData.isPairing = true;
                    battleData.startPairTime = DateTime.Now;
                }
                else
                {
                    //房間數已達上限
                    ShowMaxRoomTip();                    
                }                
            }
        });
    }

    private void OnEnable()
    {
        Color tipColor = tip_Txt.color;
        tipColor.a = 0;
        tip_Txt.color = tipColor;

#if !UNITY_EDITOR
        WalletManager.Instance.StartCheckConnect();
#endif
    }

    private void Start()
    {
        cashRoomBtnSample.SetActive(false);
        CreateRoom();
    }

    private void Update()
    {
        //等待計時器
        if (battleData.isPairing)
        {
            TimeSpan waitingTime = DateTime.Now - battleData.startPairTime;
            battleBtn_Txt.text = $"{LanguageManager.Instance.GetText("Pairing")}:{(int)waitingTime.TotalMinutes} : {waitingTime.Seconds:00}";

            if (waitingTime.Seconds >= 3)
            {
                baseRequest.SendRequest_InBattleRoom();
                EndPair();
            }
        }
    }

    /// <summary>
    /// 創建房間
    /// </summary>
    private void CreateRoom()
    {
        //現金桌        
        float cashRoomSpacing = cashRoomParent.GetComponent<HorizontalLayoutGroup>().spacing;
        Rect cashRoomRect = cashRoomBtnSample.GetComponent<RectTransform>().rect;
        cashRoomParent.sizeDelta = new Vector2((cashRoomRect.width + cashRoomSpacing) * DataManager.CashRoomSmallBlindList.Count, cashRoomRect.height);
        foreach (var smallBlind in DataManager.CashRoomSmallBlindList)
        {
            RectTransform rt = Instantiate(cashRoomBtnSample).GetComponent<RectTransform>();
            rt.gameObject.SetActive(true);
            rt.SetParent(cashRoomParent);
            rt.GetComponent<CashRoomBtn>().SetCashRoomBtnInfo(smallBlind, this);
            rt.localScale = Vector3.one;
        }
        cashRoomParent.anchoredPosition = Vector2.zero;
    }

    /// <summary>
    /// 積分房資料
    /// </summary>
    private BattleData battleData;
    public class BattleData
    {
        public bool isPairing;              //是否正在配對中
        public DateTime startPairTime;      //開始配對時間
    }

    /// <summary>
    /// 結束配對
    /// </summary>
    private void EndPair()
    {
        battleBtn_Txt.text = LanguageManager.Instance.GetText("BattleRoom");
        battleData.isPairing = false;
    }

    /// <summary>
    /// 顯示已達房間數量提示
    /// </summary>
    public void ShowMaxRoomTip()
    {
        if(tipCorutine != null) StopCoroutine(tipCorutine);
        tipCorutine = StartCoroutine(IShowTip(LanguageManager.Instance.GetText("MaxRoomTip")));
    }

    /// <summary>
    /// 顯示提示
    /// </summary>
    /// <param name="tipContent">提示內容</param>
    /// <returns></returns>
    public IEnumerator IShowTip(string tipContent)
    {
        float showTime = 0.5f;

        tip_Txt.text = tipContent;
        Color tipColor = tip_Txt.color;
        tipColor.a = 0;

        DateTime startTime = DateTime.Now;

        while ((DateTime.Now - startTime).TotalSeconds < showTime)
        {
            float progress = (float)(DateTime.Now - startTime).TotalSeconds / showTime;
            float alpha = Mathf.Lerp(0, 1, progress);
            tipColor.a = alpha;
            tip_Txt.color = tipColor;

            yield return null;
        }

        tipColor.a = 1;
        tip_Txt.color = tipColor;

        yield return new WaitForSeconds(2.5f);

        startTime = DateTime.Now;

        while ((DateTime.Now - startTime).TotalSeconds < showTime)
        {
            float progress = (float)(DateTime.Now - startTime).TotalSeconds / showTime;
            float alpha = Mathf.Lerp(1, 0, progress);
            tipColor.a = alpha;
            tip_Txt.color = tipColor;

            yield return null;
        }

        tipColor.a = 0;
        tip_Txt.color = tipColor;
    }

    private void OnDestroy()
    {
        LanguageManager.Instance.RemoveLanguageFun(UpdateLanguage);
        WalletManager.Instance.CancelCheckConnect();
    }
}
