using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

public class PlayerDataSystem
{
    public static void Init()
    {
        string path = Application.persistentDataPath + "/player.pushcube";
        if (!File.Exists(path))
        {
            PlayerData.currentStage = 0;
            PlayerData.currentCharacter = 0;
            PlayerData.goldCube = 5;
            PlayerData.undoCount = 50;
            PlayerData.undoTryCount = 0;
            PlayerData.retryCount = 0;
            PlayerData.moveCount = 0;
            PlayerData.getGoldCubeCount = PlayerData.goldCube;
            PlayerData.totalPlayTime = 0;
            PlayerData.stageInfo = "";
            PlayerData.havingCharacters = "0,1";
            PlayerData.isRemoveAd = false;
            Debug.Log("유저 데이터 초기화");
            Save();
        }
    }
    public static void Save()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.pushcube";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerDataFormat data = new PlayerDataFormat();
        try
        {
            formatter.Serialize(stream, data);
        }
        catch (SerializationException e)
        {
            Debug.LogError("유저 데이터 저장 실패 > " + e.Message);
            throw;
        }
        finally
        {
            stream.Close();
            Debug.Log("유저 데이터 로컬 저장완료.");
        }
    }

    public static void Load()
    {
        string path = Application.persistentDataPath + "/player.pushcube";
        PlayerDataFormat data = null;
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            data = formatter.Deserialize(stream) as PlayerDataFormat;
            stream.Close();
        }
        if (data != null)
        {
            PlayerData.currentStage = data.currentStage;
            PlayerData.currentCharacter = data.currentCharacter;
            PlayerData.goldCube = data.goldCube;
            PlayerData.undoCount = data.undoCount;
            PlayerData.undoTryCount = data.undoTryCount;
            PlayerData.retryCount = data.retryCount;
            PlayerData.moveCount = data.moveCount;
            PlayerData.getGoldCubeCount = data.getGoldCubeCount;
            PlayerData.totalPlayTime = data.totalPlayTime;
            PlayerData.stageInfo = data.stageInfo;
            PlayerData.havingCharacters = data.havingCharacters;
            PlayerData.isRemoveAd = data.isRemoveAd;
        }
        else
        {
            Debug.LogWarning("유저 데이터를 찾을 수 없습니다. " + path);
            Init();
        }
    }
}

[System.Serializable]
public class PlayerData
{
    [SerializeField]
    public static int currentStage;
    [SerializeField]
    public static int currentCharacter;
    [SerializeField]
    public static int goldCube;
    [SerializeField]
    public static int undoCount;
    [SerializeField]
    public static int undoTryCount;
    [SerializeField]
    public static int retryCount;
    [SerializeField]
    public static int moveCount;
    [SerializeField]
    public static int getGoldCubeCount;
    [SerializeField]
    public static float totalPlayTime;
    [SerializeField]
    public static string stageInfo;
    [SerializeField]
    public static string havingCharacters;
    [SerializeField]
    public static bool isRemoveAd;

    public static List<ClearStageInfo> clearStageInfoList
    {
        get
        {
            if(!string.IsNullOrEmpty(stageInfo))
            {
                List<ClearStageInfo> clearStageList = new List<ClearStageInfo>();
                string[] clearStageData = stageInfo.Split(',');
                for(int i = 0; i < clearStageData.Length; i++)
                {
                    clearStageList.Add(new ClearStageInfo(clearStageData[i]));
                }
                return clearStageList;
            }
            else
            {
                return new List<ClearStageInfo>();
            }
        }
    }


    public static List<int> GetHavingCharacters()
    {
        List<int> charList = new List<int>();
        string[] chars = havingCharacters.Split(',');
        foreach (var c in chars)
        {
            charList.Add(int.Parse(c));
        }
        return charList;
    }
    public static bool IsHavingCharacter(int characterNumber)
    {
        var havingCharacterList = GetHavingCharacters();
        if (!havingCharacterList.Contains(characterNumber))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public static void AddCharacter(int characterNumber)
    {
        if(!IsHavingCharacter(characterNumber))
        {
            string havingCharacterDataString = havingCharacters;
            PlayerData.havingCharacters = havingCharacterDataString + "," + characterNumber.ToString();
            PlayerDataSystem.Save();
            Debug.Log(characterNumber + " 캐릭터 획득!");
        }
        else
        {
            Debug.Log("이미 가지고 있는 캐릭터!");
        }
    }
    public static void AddMoveCount(int count)
    {
        PlayerData.moveCount += count;
        PlayerDataSystem.Save();
    }

    public static void AddGoldCube(int count)
    {
        PlayerData.goldCube += count;
        PlayerData.getGoldCubeCount += count;
        PlayerDataSystem.Save();
        Debug.Log(count + "의 골큐획득! > " + goldCube);
    }

    public static void UseGoldCube(int count)
    {
        PlayerData.goldCube -= count;
        PlayerDataSystem.Save();
    }
    public static void AddUndoCount(int count)
    {
        undoCount += count;
        PlayerDataSystem.Save();
    }

    public static void UseUndoCount()
    {
        if(undoCount>0)
        {
            PlayerData.undoTryCount++;
            PlayerData.undoCount--;
            PlayerDataSystem.Save();
        }
    }
    public static void Retry()
    {
        PlayerData.retryCount++;
        PlayerDataSystem.Save();
    }
    public static void UpdateClearStageInfo(List<ClearStageInfo> clearStageList)
    {
        StringBuilder data = new StringBuilder();
        for(int i = 0; i < clearStageList.Count; i++)
        {
            if (i == 0)
                data.Append(clearStageList[i].Serialization());
            else
                data.Append("," + clearStageList[i].Serialization());
        }
        PlayerData.stageInfo = data.ToString();
        PlayerDataSystem.Save();
    }
}

[System.Serializable]
public class PlayerDataFormat
{
    public int currentStage;
    public int currentCharacter;
    public int goldCube;
    public int undoCount;
    public int undoTryCount;
    public int retryCount;
    public int moveCount;
    public int getGoldCubeCount;
    public float totalPlayTime;
    public string stageInfo;
    public string havingCharacters;
    public bool isRemoveAd;

    public PlayerDataFormat()
    {
        currentStage = PlayerData.currentStage;
        currentCharacter = PlayerData.currentCharacter;
        goldCube = PlayerData.goldCube;
        undoCount = PlayerData.undoCount;
        undoTryCount = PlayerData.undoTryCount;
        retryCount = PlayerData.retryCount;
        moveCount = PlayerData.moveCount;
        getGoldCubeCount = PlayerData.getGoldCubeCount;
        totalPlayTime = PlayerData.totalPlayTime;
        stageInfo = PlayerData.stageInfo;
        havingCharacters = PlayerData.havingCharacters;
        isRemoveAd = PlayerData.isRemoveAd;
    }
}

[System.Serializable]
public class ClearStageInfo
{
    public int stageNumber;
    public int totalMove;
    public int rank;
    public int clearCount;
    public float clearTime;

    public ClearStageInfo(int nStageNum, int nMove, int nRank, int nCount, float nTime)
    {
        stageNumber = nStageNum;
        totalMove = nMove;
        rank = nRank;
        clearCount = nCount;
        clearTime = nTime;
    }
    public string Serialization()
    {
        return string.Format("[{0}:{1}:{2}:{3}:{4}]",stageNumber,totalMove,rank,clearCount,clearTime);
    }
    public ClearStageInfo(string serialData)
    {
        string[] data = serialData.Replace("[", "").Replace("]", "").Split(':');
        if(data.Length==5)
        {
            stageNumber = int.Parse(data[0]);
            totalMove = int.Parse(data[1]);
            rank = int.Parse(data[2]);
            clearCount = int.Parse(data[3]);
            clearTime = float.Parse(data[4]);
        }
        else
        {
            Debug.LogWarning("데이터 복원에 실패 => " + serialData);
        }
    }
    public string GetClearTime()
    {
        float time = clearTime;
        string h = ((int)(time / 3600)).ToString("00");
        time -= ((int)time / 3600) * 3600;
        string m = ((int)(time / 60)).ToString("00");
        string s = (time % 60).ToString("00");
        return string.Format("{0}:{1}:{2}", h, m, s);
    }
}
    
