using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

public class LobbyMinePageView : MonoBehaviour
{
    [Header("用戶訊息")]
    [SerializeField]
    GameObject UserPorfile_Obj;
    [SerializeField]
    Button EditorAvatar_Btn, CopyWalletAddress_Btn;
    [SerializeField]
    Text Nickname_Txt, WalletAddress_Txt;

    [Header("更換頭像")]
    [SerializeField]
    RectTransform ChangeAvatar_Tr, AvatarListParent_Tr, SelectAvatarIcon_Tr;
    [SerializeField]
    GameObject AvatarSapmle;
    [SerializeField]
    Button CloseChangeAvatar_Btn, ChangeAvatarSubmit_Btn;

    const float expandTIme = 0.1f;      //內容展開時間

    /// <summary>
    /// (展開物件, 初始高度)
    /// </summary>
    Dictionary<RectTransform, float> expandObjDic = new();
    List<Button> avatarBtnList;                                                 //頭像按鈕

    int tempAvatarIndex;                                                        //零時頭像index

    private void Awake()
    {
        //紀錄展開物件
        List<RectTransform> expandObjList = new()
        {
            ChangeAvatar_Tr,        //更換頭像
        };
        foreach (var expandObj in expandObjList)
        {
            expandObjDic.Add(expandObj, expandObj.rect.height);
            expandObj.gameObject.SetActive(false);
        }

        ListenerEvent();
    }

    /// <summary>
    /// 事件聆聽
    /// </summary>
    private void ListenerEvent()
    {
        #region 用戶訊息

        //開啟更換頭像
        EditorAvatar_Btn.onClick.AddListener(() =>
        {
            UserPorfile_Obj.SetActive(false);
            StartCoroutine(ISwitchContent(true, ChangeAvatar_Tr, () =>
            {
                int avatar = DataManager.UserAvatar;
                SelectAvatarIcon_Tr.SetParent(avatarBtnList[avatar].transform);
                SelectAvatarIcon_Tr.anchoredPosition = Vector2.zero;
            }));
        });

        //複製錢包地址
        CopyWalletAddress_Btn.onClick.AddListener(() =>
        {
            StringUtils.CopyText(DataManager.UserWalletAddress);
        });

        //關閉選擇頭像
        CloseChangeAvatar_Btn.onClick.AddListener(() =>
        {
            StartCoroutine(ISwitchContent(false, ChangeAvatar_Tr, () =>
            {
                UserPorfile_Obj.SetActive(true);
            }));
        });

        //提交更換頭像
        ChangeAvatarSubmit_Btn.onClick.AddListener(() =>
        {
            DataManager.UserAvatar = tempAvatarIndex;
            EditorAvatar_Btn.image.sprite = AssetsManager.Instance.GetAlbumAsset(AlbumEnum.AvatarAlbum).album[DataManager.UserAvatar];
            GameObject.FindAnyObjectByType<LobbyView>().UpdateUserInfo();

            StartCoroutine(ISwitchContent(false, ChangeAvatar_Tr, () =>
            {
                UserPorfile_Obj.SetActive(true);
            }));
        });

        #endregion
    }

    private void OnEnable()
    {
        SetUserInfo();
    }

    /// <summary>
    /// 設置用戶訊息
    /// </summary>
    private void SetUserInfo()
    {
        //頭像
        EditorAvatar_Btn.image.sprite = AssetsManager.Instance.GetAlbumAsset(AlbumEnum.AvatarAlbum).album[DataManager.UserAvatar];

        //暱稱
        Nickname_Txt.text = DataManager.UserNickname;

        //錢包地址
        StringUtils.StrExceedSize(DataManager.UserWalletAddress, WalletAddress_Txt);

        //產生選擇的頭像
        avatarBtnList = new List<Button>();
        AvatarSapmle.gameObject.SetActive(false);
        Sprite[] avatars = AssetsManager.Instance.GetAlbumAsset(AlbumEnum.AvatarAlbum).album;
        for (int i = 0; i < avatars.Length; i++)
        {
            Button avatarBtn = Instantiate(AvatarSapmle, AvatarListParent_Tr).GetComponent<Button>();
            avatarBtn.gameObject.SetActive(true);
            avatarBtn.image.sprite = avatars[i];
            int index = i;

            avatarBtn.onClick.AddListener(() =>
            {
                SelectAvatarIcon_Tr.SetParent(avatarBtn.transform);
                SelectAvatarIcon_Tr.anchoredPosition = Vector2.zero;
                tempAvatarIndex = index;
            });

            avatarBtnList.Add(avatarBtn);
        }
    }

    /// <summary>
    /// 內容顯示開關
    /// </summary>
    /// <param name="isExpand">是否展開</param>
    /// <param name="rt">展開內容物件</param>
    /// <param name="completeCallback">完成回傳</param>
    /// <returns></returns>
    private IEnumerator ISwitchContent(bool isExpand, RectTransform rt, UnityAction completeCallback = null)
    {
        if (expandObjDic.ContainsKey(rt))
        {
            float initHeight = isExpand == true ?
                               0 :
                               rt.rect.height ;
            float targetHeight = isExpand == true ?
                                 expandObjDic[rt] :
                                 0;

            rt.gameObject.SetActive(true);
            rt.sizeDelta = new Vector2(rt.rect.width, initHeight);

            DateTime startTime = DateTime.Now;
            while ((DateTime.Now - startTime).TotalSeconds < expandTIme) 
            {
                float progress = (float)(DateTime.Now - startTime).TotalSeconds / expandTIme;
                float height = Mathf.Lerp(initHeight, targetHeight, progress);
                rt.sizeDelta = new Vector2(rt.rect.width, height);

                yield return null;
            }

            rt.sizeDelta = new Vector2(rt.rect.width, targetHeight);
            rt.gameObject.SetActive(isExpand);

            completeCallback?.Invoke();
        }
    }
}
