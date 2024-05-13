using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

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
    private LanguageManager() { }

    /// <summary>
    /// 紀錄翻譯文本
    /// </summary>
    Dictionary<string, string[]> languageDic = new Dictionary<string, string[]>();

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

    /// <summary>
    /// 當前語言Index
    /// </summary>
    public int CurrLanguage { get; set; }

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
            return languageDic[id][CurrLanguage];
        }
        else
        {
            Debug.LogError($"{id}:翻譯文本不存在");
            return "";
        }
    }
}
