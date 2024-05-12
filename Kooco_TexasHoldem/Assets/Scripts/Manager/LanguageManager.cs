using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public static class LanguageManager
{
    /// <summary>
    /// 紀錄翻譯文本
    /// </summary>
    static Dictionary<string, string[]> languageDic = new Dictionary<string, string[]>();

    /// <summary>
    /// 翻譯攔名稱
    /// </summary>
    static string[] languageId =
    {
        "English",      //英文
        "zh_TW",        //繁體中文        
    };

    /// <summary>
    /// 當前語言Index
    /// </summary>
    public static int CurrLanguage { get; set; }

    /// <summary>
    /// 載入翻譯文本
    /// </summary>
    public static void LoadLangageJson()
    {
        string jsonFile = Resources.Load<TextAsset>("Language/Language").text;
        JsonData jsonData = JsonMapper.ToObject<JsonData>(jsonFile);

        for (int i = 0; i < jsonData.Count; i++)
        {            
            string serchName = jsonData[i]["ID"].ToString();
            string[] lang = new string[languageId.Length];
            for (int j = 0; j < languageId.Length; j++)
            {
                lang[j] = jsonData[i][languageId[j]].ToString();
            }

            languageDic.Add(serchName, lang);
        }
    }

    /// <summary>
    /// 獲取文本
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static string GetText(string id)
    {
        if (languageDic.ContainsKey(id))
        {
            Debug.LogError(languageDic[id][CurrLanguage]);
            return languageDic[id][CurrLanguage];
        }
        else
        {
            Debug.LogError($"{id}:翻譯文本不存在");
            return "";
        }
    }
}
