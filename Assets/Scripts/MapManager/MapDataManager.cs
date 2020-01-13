using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapDataManager
{
    public static Dictionary<String, String> Fields { get; private set; }

    static MapDataManager()
    {
        LoadMapData();
    }
    public static string GetStartSceneMapData()
    {
        string txt = "";
        try
        {
            txt = MapDataManager.Fields["StageStart"];
        }
        catch (KeyNotFoundException e)
        {
            Debug.LogWarning(e);
        }
        return txt;
    }

    public static string GetMapData(int stage)
    {
        string txt = "";
        try
        {
            txt = MapDataManager.Fields["Stage"+stage];
        }
        catch (KeyNotFoundException e)
        {
            Debug.LogWarning(stage + " 의 맵 텍스트 발견못함" + e);
        }
        return txt;
    }

    public static string GetMapClearCount(int stage)
    {
        string txt = "";
        try
        {
            txt = MapDataManager.Fields["StageClearCount" + stage];
        }
        catch (KeyNotFoundException e)
        {
            Debug.LogWarning(stage + " 의 맵 카운트 텍스트 발견못함" + e);
        }
        return txt;
    }

    public static void LoadMapData()
    {
        if (Fields == null)
            Fields = new Dictionary<string, string>();

        Fields.Clear();
        var textAsset = Resources.Load(@"MapData/stageData"); //no .txt needed
        string allTexts = "";
        if (textAsset == null)
            Debug.LogError("File not found for I18n: Assets/Resources/MapData/stageData.txt");
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
    }
}
