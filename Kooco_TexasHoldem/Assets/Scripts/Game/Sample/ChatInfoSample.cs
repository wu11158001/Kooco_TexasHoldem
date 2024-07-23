using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatInfoSample : MonoBehaviour
{
    [SerializeField]
    RectTransform thisRt;

    [SerializeField]
    Image Avatar_Img;
    [SerializeField]
    TextMeshProUGUI Nickname_Txt, Content_Txt;
    [SerializeField]
    RectTransform ContentBg_Tr;

    int Avatar { get; set; }
    string Nickname { get; set; }
    string Content { get; set; }

    private void OnEnable()
    {
        if (!string.IsNullOrEmpty(Content))
        {
            SetChatInfo(Avatar,
                        Nickname,
                        Content);
        }
    }

    /// <summary>
    /// 設置聊天內容
    /// </summary>
    /// <param name="avatar"></param>
    /// <param name="nickname"></param>
    /// <param name="content"></param>
    public void SetChatInfo(int avatar, string nickname, string content)
    {
        this.Avatar = avatar;
        this.Nickname = nickname;
        this.Content = content;

        float initThisHeight = thisRt.rect.height;                                      //初始物件高度
        float maxContentTxtWidth = Content_Txt.rectTransform.rect.width;                //內容文字最大寬度
        float initContentTxtHeight = Content_Txt.rectTransform.rect.height;             //初始內容文字高度

        //資料顯示
        Avatar_Img.sprite = AssetsManager.Instance.GetAlbumAsset(AlbumEnum.AvatarAlbum).album[avatar];
        Nickname_Txt.text = nickname;
        Content_Txt.text = content;

        //內容區域
        float txtWidth = Mathf.Min(maxContentTxtWidth,
                                   Content_Txt.preferredWidth);
        float txtHeight = Content_Txt.preferredHeight;

        Content_Txt.rectTransform.sizeDelta = new Vector2(txtWidth,
                                                          txtHeight);
        ContentBg_Tr.sizeDelta = new Vector2(txtWidth + 10,
                                             txtHeight + 10);

        //物件高度
        float thisRtAddHeight = txtHeight - initContentTxtHeight;
        thisRt.sizeDelta = new Vector2(thisRt.rect.width,
                                       initThisHeight + thisRtAddHeight);
    }
}
