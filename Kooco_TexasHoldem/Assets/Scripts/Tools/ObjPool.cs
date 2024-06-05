using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjPool
{
    /// <summary>
    /// 物件池(主物件, (父物件, 已創建物件)
    /// </summary>
    Dictionary<GameObject, (Transform, List<GameObject>)> poolDic;

    Transform parent;       //物件池父物件
    int cleanNum;           //達到數量清理未使用物件

    public ObjPool(Transform parent, int cleanNum = 0)
    {
        this.parent = parent;
        this.cleanNum = cleanNum;
        poolDic = new Dictionary<GameObject, (Transform, List<GameObject>)>();
    }

    /// <summary>
    /// 物件池創建物件
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public GameObject CreateObj(GameObject obj)
    {
        if (poolDic.ContainsKey(obj))
        {
            //物件池已有物件
            foreach (var item in poolDic[obj].Item2)
            {
                //有物件未使用
                if (!item.activeSelf)
                {
                    item.SetActive(true);

                    //清理物件池
                    if (cleanNum > 0 && 
                        poolDic[obj].Item2.Count >= cleanNum)
                    {
                        List<GameObject> cleanList = new List<GameObject>();
                        foreach (var usingObj in poolDic[obj].Item2)
                        {
                            if(!usingObj.activeSelf)
                            {
                                cleanList.Add(usingObj);                                
                            }
                        }

                        foreach (var cleanObj in cleanList)
                        {
                            poolDic[obj].Item2.Remove(cleanObj);
                            GameObject.Destroy(cleanObj);
                        }
                    }

                    return item;
                }
            }

            //增加新物件
            GameObject addObj = GameObject.Instantiate(obj, poolDic[obj].Item1);
            poolDic[obj].Item2.Add(addObj);
            return addObj;
        }

        //創建新物件池
        GameObject parentObj = new GameObject();
        parentObj.name = $"{obj.name}Pool";
        parentObj.transform.SetParent(parent);
        GameObject newObj = GameObject.Instantiate(obj, parentObj.transform);
        poolDic.Add(obj, (parentObj.transform, new List<GameObject>() { newObj}));
        return newObj;
    }
}
