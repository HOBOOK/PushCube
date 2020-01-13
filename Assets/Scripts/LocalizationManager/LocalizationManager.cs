﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum LanguageType
{
    ko,
    en,
    zh
}

public class LocalizationManager
{
    public static Dictionary<String, String> Fields { get; private set; }

    static LocalizationManager()
    {
        LoadLanguage(ConfigurationData.language);
    }

    public static string GetText(string key)
    {
        string txt = "";
        if (key == "ISOCode")
            txt = LocalizationManager.GetLanguage();
        else
        {
            try
            {
                txt = LocalizationManager.Fields[key];
            }
            catch (KeyNotFoundException e)
            {
                Debug.LogWarning(key + " 의 로컬라이징 텍스트를 발견하지못함" + e);
            }
        }
        return txt;
    }
    public static void RedrawLanguage()
    {
        foreach (var txt in GameObject.FindObjectsOfType<LocalizationText>())
        {
            txt.ReDraw();
        }
    }

    public static void LoadLanguage(int type = 0)
    {
        if (Fields == null)
            Fields = new Dictionary<string, string>();
        Fields.Clear();
        string[] langType = { "ko", "en", "zh" };
        string lang = langType[type];
        if (string.IsNullOrEmpty(lang))
            lang = Get2LetterISOCodeFromSystemLanguage().ToLower();
        //lang = "de";
        var textAsset = Resources.Load(@"Localization/" + lang); //no .txt needed
        string allTexts = "";
        if (textAsset == null)
            textAsset = Resources.Load(@"Localization/en") as TextAsset; //no .txt needed
        if (textAsset == null)
            Debug.LogError("언어 텍스트 파일을 찾을 수 없습니다. I18n: Assets/Resources/Localization/" + lang + ".txt");
        allTexts = (textAsset as TextAsset).text;
        string[] lines = allTexts.Split(new string[] { "\r\n", "\n" },
            StringSplitOptions.None);
        string key, value;
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].IndexOf("=") >= 0 && !lines[i].StartsWith("#"))
            {
                key = lines[i].Substring(0, lines[i].IndexOf("="));
                value = lines[i].Substring(lines[i].IndexOf("=") + 1,
                        lines[i].Length - lines[i].IndexOf("=") - 1).Replace("\\n", Environment.NewLine);
                Fields.Add(key, value);
            }
        }
        RedrawLanguage();
        Debug.Log(lang + " 매니저에서 언어변경완료");
    }


    public static string GetLanguage()
    {
        return Get2LetterISOCodeFromSystemLanguage().ToLower();
    }

    public static string Get2LetterISOCodeFromSystemLanguage()
    {
        SystemLanguage lang = Application.systemLanguage;
        string res = "EN";
        switch (lang)
        {
            case SystemLanguage.Afrikaans: res = "AF"; break;
            case SystemLanguage.Arabic: res = "AR"; break;
            case SystemLanguage.Basque: res = "EU"; break;
            case SystemLanguage.Belarusian: res = "BY"; break;
            case SystemLanguage.Bulgarian: res = "BG"; break;
            case SystemLanguage.Catalan: res = "CA"; break;
            case SystemLanguage.Chinese: res = "ZH"; break;
            case SystemLanguage.ChineseSimplified: res = "ZH"; break;
            case SystemLanguage.ChineseTraditional: res = "ZH"; break;
            case SystemLanguage.Czech: res = "CS"; break;
            case SystemLanguage.Danish: res = "DA"; break;
            case SystemLanguage.Dutch: res = "NL"; break;
            case SystemLanguage.English: res = "EN"; break;
            case SystemLanguage.Estonian: res = "ET"; break;
            case SystemLanguage.Faroese: res = "FO"; break;
            case SystemLanguage.Finnish: res = "FI"; break;
            case SystemLanguage.French: res = "FR"; break;
            case SystemLanguage.German: res = "DE"; break;
            case SystemLanguage.Greek: res = "EL"; break;
            case SystemLanguage.Hebrew: res = "IW"; break;
            case SystemLanguage.Hungarian: res = "HU"; break;
            case SystemLanguage.Icelandic: res = "IS"; break;
            case SystemLanguage.Indonesian: res = "IN"; break;
            case SystemLanguage.Italian: res = "IT"; break;
            case SystemLanguage.Japanese: res = "JA"; break;
            case SystemLanguage.Korean: res = "KO"; break;
            case SystemLanguage.Latvian: res = "LV"; break;
            case SystemLanguage.Lithuanian: res = "LT"; break;
            case SystemLanguage.Norwegian: res = "NO"; break;
            case SystemLanguage.Polish: res = "PL"; break;
            case SystemLanguage.Portuguese: res = "PT"; break;
            case SystemLanguage.Romanian: res = "RO"; break;
            case SystemLanguage.Russian: res = "RU"; break;
            case SystemLanguage.SerboCroatian: res = "SH"; break;
            case SystemLanguage.Slovak: res = "SK"; break;
            case SystemLanguage.Slovenian: res = "SL"; break;
            case SystemLanguage.Spanish: res = "ES"; break;
            case SystemLanguage.Swedish: res = "SV"; break;
            case SystemLanguage.Thai: res = "TH"; break;
            case SystemLanguage.Turkish: res = "TR"; break;
            case SystemLanguage.Ukrainian: res = "UK"; break;
            case SystemLanguage.Unknown: res = "EN"; break;
            case SystemLanguage.Vietnamese: res = "VI"; break;
        }
        //		Debug.Log ("Lang: " + res);
        return res;
    }
}
