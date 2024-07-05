using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyHandHistoryView : MonoBehaviour
{
    [SerializeField]
    Button Back_Btn;

    private void Awake()
    {
        //返回按鈕
        Back_Btn.onClick.AddListener(() =>
        {
            Destroy(gameObject);
        });
    }
}
