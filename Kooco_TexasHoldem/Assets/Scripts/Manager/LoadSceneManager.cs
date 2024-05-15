using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneManager : UnitySingleton<LoadSceneManager>
{
    [SerializeField]
    RectTransform lodingView;

    public override void Awake()
    {
        base.Awake();

        lodingView.gameObject.SetActive(false);
    }

    /// <summary>
    /// 載入場景
    /// </summary>
    /// <param name="sceneEnum">進入場景</param>
    public void LoadScene(SceneEnum sceneEnum)
    {
        StartCoroutine(ILoadScene(sceneEnum));
    }

    /// <summary>
    /// 載入場景
    /// </summary>
    /// <param name="sceneEnum">進入場景</param>
    /// <returns></returns>
    private IEnumerator ILoadScene(SceneEnum sceneEnum)
    {
        ViewManager.Instance.ClosePartsView(PartsViewEnum.WaitingView);
        lodingView.gameObject.SetActive(true);

        // 异步加载场景
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneEnum.ToString());
        asyncLoad.allowSceneActivation = false;

        // 等待加载完成
        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
                GameDataManager.CurrScene = sceneEnum;

                yield return new WaitForSeconds(0.5f);

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
        Debug.Log($"In To Scene:{sceneEnum}");
        switch (sceneEnum)
        {
            case SceneEnum.Entry:
                break;

            case SceneEnum.Lobby:
                ViewManager.Instance.OpenView(ViewEnum.LobbyView);
                break;

            case SceneEnum.Game:

               /* switch (GameDataManager.CurrRoomType)
                {
                    //積分房
                    case GameRoomEnum.BattleRoomView:
                        ViewManager.Instance.OpenView(ViewEnum.BattleRoomView);
                        break;
                        
                    //現金房
                    case GameRoomEnum.CashRoomView:
                        ViewManager.Instance.OpenView(ViewEnum.CashRoomView);
                        break;
                }*/
                break;
        }
    }
}
