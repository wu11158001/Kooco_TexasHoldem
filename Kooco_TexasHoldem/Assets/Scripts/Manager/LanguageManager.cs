using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using UnityEngine.Events;

public class LanguageManager
{
    private static readonly object lockObject = new object();
    private static LanguageManager language = null;
    public static LanguageManager Instance
    {
        get
        {
            if (language == null)
            {
                lock (lockObject)
                {
                    if (language == null)
                    {
                        language = new LanguageManager();
                    }
                }
            }
            return language;
        }
    }
    private LanguageManager() 
    {
        thisData = new ThisData();
    }

    /// <summary>
    /// 紀錄翻譯文本
    /// </summary>
    Dictionary<string, string[]> languageDic = new Dictionary<string, string[]>();

    /// <summary>
    /// 紀錄更新語言方法
    /// </summary>
    List<UnityAction> updateLanguageFuncList = new List<UnityAction>();

    /// <summary>
    /// 翻譯攔名稱
    /// </summary>
    string[] languageId =
    {
        "English",      //英文
        "zh_TW",        //繁體中文        
    };

    /// <summary>
    /// 翻譯資料
    /// </summary>
    private class TranslationData
    {
        public string ID { get; set; }
        public string English { get; set; }
        public string zh_TW { get; set; }

        public string GetId(string language)
        {
            switch (language)
            {
                case "English":
                    return English;

                case "zh_TW":
                    return zh_TW;

                default:
                    return "";
            }
        }
    }

    private ThisData thisData;
    public class ThisData
    {
        public int CurrLanguageIndex;       //當前語言Index
    }

    /// <summary>
    /// 載入翻譯文本
    /// </summary>
    public void LoadLangageJson()
    {
        string jsonFile = Resources.Load<TextAsset>("Language/Language").text;
        List<TranslationData> translations = JsonMapper.ToObject<List<TranslationData>>(jsonFile);

        foreach (TranslationData data in translations)
        {
            string serchName = data.ID;
            string[] lang = new string[languageId.Length];

            for (int j = 0; j < languageId.Length; j++)
            {
                lang[j] = data.GetId(languageId[j]);
            }

            languageDic.Add(serchName, lang);
        }
    }

    /// <summary>
    /// 獲取文本
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public string GetText(string id)
    {
        if (languageDic.ContainsKey(id))
        {
            return languageDic[id][thisData.CurrLanguageIndex];
        }
        else
        {
            Debug.LogError($"{id}:翻譯文本不存在");
            return "";
        }
    }

    /// <summary>
    /// 添加更新語言方法
    /// </summary>
    /// <param name="updateFunc"></param>
    public void AddUpdateLanguageFunc(UnityAction updateFunc)
    {
        updateFunc();
        updateLanguageFuncList.Add(updateFunc);
    }

    /// <summary>
    /// 移除更新語言方法
    /// </summary>
    /// <param name="func"></param>
    public void RemoveLanguageFun(UnityAction func)
    {
        if (updateLanguageFuncList.Contains(func))
        {
            updateLanguageFuncList.Remove(func);
        }
        else
        {
            Debug.LogError("未找到移除更新語言方法!!!");
        }    
    }

    /// <summary>
    /// 更新翻譯
    /// </summary>
    private void UpdateLanguage()
    {
        Debug.Log($"Change Language:{languageId[thisData.CurrLanguageIndex]}");
        foreach (var func in updateLanguageFuncList)
        {
            func?.Invoke();
        }
    }

    /// <summary>
    /// 更換語言
    /// </summary>
    /// <param name="index"></param>
    public void ChangeLanguage(int index)
    {
        if (index >= languageId.Length || 
            index < 0)
        {
            Debug.LogError($"Change Language Index:{index}:Error!!!");
            return;
        }

        thisData.CurrLanguageIndex = index;
        UpdateLanguage();
    }
}
