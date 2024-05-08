using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : UnitySingleton<LoadSceneManager>
{
    public override void Awake()
    {
        base.Awake();
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

                yield return null;

                UIManager.Instance.Init();
                JudgeIntoScene(sceneEnum);
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
            case SceneEnum.Entry:
                break;

            case SceneEnum.Lobby:
                UIManager.Instance.OpenView(ViewEnum.LobbyView);
                break;

            case SceneEnum.Game:
                UIManager.Instance.OpenView(ViewEnum.GameView);
                break;
        }
    }
}
