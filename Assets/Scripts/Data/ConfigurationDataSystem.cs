using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;


public static class ConfigurationDataSystem
{
    public static void Init()
    {
        string path = Application.persistentDataPath + "/config.pushcube";
        if (!File.Exists(path))
        {
            ConfigurationData.bgmSound = true;
            ConfigurationData.efxSound = true;
            ConfigurationData.vibrate = true;
            ConfigurationData.language = LocalizationManager.GetLanguage() == "ko" ? 0 : LocalizationManager.GetLanguage() == "zh" ? 2 : 1;
            ConfigurationData.theme = 0;
            Debug.Log("설정 데이터 초기화");
            Save();
        }
    }
    public static void Save()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/config.pushcube";
        FileStream stream = new FileStream(path, FileMode.Create);

        ConfigurationDataFormat data = new ConfigurationDataFormat();
        try
        {
            formatter.Serialize(stream, data);
        }
        catch (SerializationException e)
        {
            Debug.LogError("설정 데이터 저장 실패 > " + e.Message);
            throw;
        }
        finally
        {
            stream.Close();
        }
        Debug.Log("설정 데이터 로컬 저장완료.");
    }

    public static void Load()
    {
        string path = Application.persistentDataPath + "/config.pushcube";
        ConfigurationDataFormat data = null;
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            data = formatter.Deserialize(stream) as ConfigurationDataFormat;
            stream.Close();
        }
        if (data != null)
        {
            ConfigurationData.bgmSound = data.bgmSound;
            ConfigurationData.efxSound = data.efxSound;
            ConfigurationData.vibrate = data.vibrate;
            ConfigurationData.language = data.language;
            ConfigurationData.theme = data.theme;
        }
        else
        {
            Debug.LogWarning("설정 데이터를 찾을 수 없습니다. " + path);
            Init();
        }
    }
}

[System.Serializable]
public class ConfigurationDataFormat
{
    public bool bgmSound;
    public bool efxSound;
    public bool vibrate;
    public int language; //0:한국어 1:영어 2: 일본어 3:중국어
    public int theme;

    public ConfigurationDataFormat()
    {
        bgmSound = ConfigurationData.bgmSound;
        efxSound = ConfigurationData.efxSound;
        vibrate = ConfigurationData.vibrate;
        language = ConfigurationData.language;
        theme = ConfigurationData.theme;
    }
}

[System.Serializable]
public class ConfigurationData
{
    [SerializeField]
    public static bool bgmSound;
    [SerializeField]
    public static bool efxSound;
    [SerializeField]
    public static bool vibrate;
    [SerializeField]
    public static int language; //0:한국어 1:영어 2: 일본어 3:중국어
    [SerializeField]
    public static int theme;
}
