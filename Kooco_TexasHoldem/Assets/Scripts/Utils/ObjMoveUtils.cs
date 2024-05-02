using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;
using UnityEngine.Events;

public static class ObjMoveUtils
{
    private static float defaultScreenWidth = 540;
    private static float defaultScreenHeight = 960;

    /// <summary>
    /// 獲取解析度變化量
    /// </summary>
    /// <returns></returns>
    public static float GetResolutionVariation()
    {
        float currWidth = (float)Screen.width;
        float currHeight = (float)Screen.height;

        return Math.Min(currWidth / defaultScreenWidth, currHeight / defaultScreenHeight);
    }

    /// <summary>
    /// 物件移動至目標位置
    /// </summary>
    /// <param name="moveObj">移動物件</param>
    /// <param name="targetPos">目標位置</param>
    /// <param name="during">移動時間</param>
    /// <param name="complete">移動結束CallBack</param>
    async public static void ObjMoveToTarget(Transform moveObj, Vector2 targetPos, float during, UnityAction complete = null)
    {
        float duration = 0.5f;//效果時間

        Vector2 startPos = moveObj.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (moveObj == null) return;

            float process = elapsedTime / duration;
            float posX = Mathf.Lerp(startPos.x, targetPos.x, process);
            float posY = Mathf.Lerp(startPos.y, targetPos.y, process);
            moveObj.position = new Vector2(posX, posY);
            elapsedTime += Time.deltaTime;
            await Task.Yield();
        }

        complete();
    }

    /// <summary>
    /// 物件朝目標移動
    /// </summary>
    /// <param name="moveObj">移動物件</param>
    /// <param name="targetPos">目標位置</param>
    /// <param name="duration">移動時間</param>
    /// <param name="speed">移動速度</param>
    async public static void ObjMoveTowardsTarget(Transform moveObj, Vector2 targetPos, float duration, float speed)
    {
        float variation = ObjMoveUtils.GetResolutionVariation();
        float moveSpeed = speed * variation;//移動速度
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (moveObj == null) return;
          
            moveObj.position = Vector2.MoveTowards(moveObj.position, targetPos, moveSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            await Task.Yield();
        }
    }
}
