using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using TMPro;
public class LoadSceneManager : UnitySingleton<LoadSceneManager>
{
    [SerializeField]
    RectTransform lodingView;
    [SerializeField]
    Image Progress_Img;
    [SerializeField]
    TextMeshProUGUI Loading_Txt, Progress_Txt;

    [Header("開啟介面")]
    [SerializeField]
    GameObject LoginViewObj, LobbyViewObj;

    DateTime startYieldTime;

    public override void Awake()
    {
        base.Awake();

        Loading_Txt.text = "Now Loading...";
        lodingView.gameObject.SetActive(false);
    }

    /// <summary>
    /// 載入場景
    /// </summary>
    /// <param name="sceneEnum">進入場景</param>
    public void LoadScene(SceneEnum sceneEnum)
    {
        if (SceneManager.GetActiveScene().name != "Entry")
        {
            StartCoroutine(ILoadScene(sceneEnum));
        }
        else
        {
            StartCoroutine(IEntryInToLogin(sceneEnum));
        }
    }

    /// <summary>
    /// Entry載入登入場景
    /// </summary>
    /// <returns></returns>
    private IEnumerator IEntryInToLogin(SceneEnum sceneEnum)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneEnum.ToString());
        asyncLoad.allowSceneActivation = false;

        // 等待加载完成
        while (!asyncLoad.isDone)
        {            
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
                yield return new WaitForSeconds(0.1f);

                DataManager.CurrScene = sceneEnum;
                ViewManager.Instance.Init();
                JudgeIntoScene(sceneEnum);
            }

            yield return null;
        }
    }

    /// <summary>
    /// 載入場景
    /// </summary>
    /// <param name="sceneEnum">進入場景</param>
    /// <returns></returns>
    private IEnumerator ILoadScene(SceneEnum sceneEnum)
    {
        lodingView.gameObject.SetActive(true);
        Progress_Img.fillAmount = 0;

        // 异步加载场景
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneEnum.ToString());
        asyncLoad.allowSceneActivation = false;

        // 等待加载完成
        while (!asyncLoad.isDone)
        {
            startYieldTime = DateTime.Now;
            while (Progress_Img.fillAmount < 0.9f)
            {
                float progress = (float)(DateTime.Now - startYieldTime).TotalSeconds / 0.8f;
                Progress_Img.fillAmount = asyncLoad.progress < progress?
                                          progress :
                                          Mathf.Lerp(0, 0.9f, progress);
                Progress_Txt.text = $"{(Progress_Img.fillAmount * 100):F0}%";

                yield return null;
            }

            if (asyncLoad.progress >= 0.9f && Progress_Img.fillAmount >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;

                DataManager.CurrScene = sceneEnum;

                startYieldTime = DateTime.Now;
                while ((DateTime.Now - startYieldTime).TotalSeconds < 0.5f)
                {
                    float progress = (float)(DateTime.Now - startYieldTime).TotalSeconds / 0.5f;
                    Progress_Img.fillAmount = Mathf.Lerp(0.9f, 1, progress);
                    Progress_Txt.text = $"{(Progress_Img.fillAmount * 100):F0}%";
                    yield return null;
                }

                ViewManager.Instance.Init();
                JudgeIntoScene(sceneEnum);

                yield return new WaitForSeconds(0.5f);
                lodingView.gameObject.SetActive(false);
            }

            yield return null;
        }
    }

    /// <summary>
    /// 判斷進入場景
    /// </summary>
    /// <param name="sceneEnum">進入場景</param>
    private void JudgeIntoScene(SceneEnum sceneEnum)
    {
        switch (sceneEnum)
        {
            case SceneEnum.Login:
                NFTManager.Instance.CancelUpdate();
                ViewManager.Instance.CreateViewInMainCanvas<LoginView>(LoginViewObj);
                break;

            case SceneEnum.Lobby:
                DataManager.ReciveRankData();
                ViewManager.Instance.CreateViewInMainCanvas<LobbyView>(LobbyViewObj);
                break;

            case SceneEnum.Game:
                break;
        }
    }
}
