using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class Lang
{
    public static readonly string VN = "vn";
    public static readonly string EN = "en";

    private static string langCode = "vn";
    private static JSONObject langJson = null;

    public static string LANG
    {
        get
        {
            return langCode;
        }

        set
        {
            langCode = value;
            PlayerPrefs.SetString("lang_setup", langCode);
            PlayerPrefs.Save();
            Observer.Instance.Notify(ObserverKey.LanguageChanged, langCode);
        }
    }

    public static string GetText(string key)
    {
        if (string.IsNullOrEmpty(key)) return string.Empty;
        if (langJson == null)
        {
            TextAsset resLangs = Resources.Load("Data/Lang/text_languages") as TextAsset;
            langJson = JSON.Parse(resLangs.text).AsObject;
            langCode = PlayerPrefs.GetString("lang_setup", EN);
        }
        string value = langJson[key][langCode].ToString();
        if (string.IsNullOrEmpty(value))
            return key + "-" + langCode;
        else
        {
            value = value.Replace("\"", "");
            value = value.Replace("\\n",
                Environment.NewLine);
            return value;
        }
    }

    public static string GetText(string lang, string key)
    {
        if (langJson == null)
        {
            TextAsset resLangs = Resources.Load("Data/Lang/text_languages") as TextAsset;
            langJson = JSON.Parse(resLangs.text).AsObject;
            lang = PlayerPrefs.GetString("lang_setup", EN);
        }
        return langJson[lang][key].ToString();
    }
}
