using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_MenuMyInformation : MonoBehaviour
{
    public Text titleText;
    public Text currentVersionText;
    public Text categoryText;
    public Text categoryValueText;
    public Button restoreButton;

    private void OnEnable()
    {
        RefreshUI();
    }

    void RefreshUI()
    {
        titleText.color = PushCubeColor.ThemeMainColor;
        currentVersionText.color = PushCubeColor.ThemeTextColor;
        categoryText.color = PushCubeColor.ThemeTextColor;
        categoryValueText.color = PushCubeColor.ThemeTextColor;
        restoreButton.GetComponent<Image>().color = PushCubeColor.ThemeMainColor;
        restoreButton.GetComponentInChildren<Text>().color = PushCubeColor.ThemeMainColor;

        currentVersionText.text = string.Format("{0} {1}", LocalizationManager.GetText("StageScene_MenuMyInformation_Version"),Application.version);
        categoryValueText.text = string.Format("{0}\r\n{1}\r\n{2}\r\n{3}\r\n{4}\r\n{5}\r\n{6}\r\n{7}",GetTotalClear(),GetTotalPerfectClear(),GetTotalPlayTime(),GetTotalMoveCount(),GetTotalUndoCount(),GetTotalRetryCount(),GetTotalGoldCubeCount(),GetTotalCharacterCount());
    }

    string GetTotalClear()
    {
        return PlayerData.clearStageInfoList.Sum(x => x.clearCount).ToString("N0");
    }
    string GetTotalPerfectClear()
    {
        return PlayerData.clearStageInfoList.FindAll(x=>x.stageNumber>0&&x.totalMove==int.Parse(MapDataManager.GetMapClearCount(x.stageNumber))).Count.ToString("N0");
    }
    string GetTotalPlayTime()
    {
        float totalPlayTime = PlayerData.totalPlayTime;
        string h = ((int)(totalPlayTime / 60)).ToString("0");
        totalPlayTime -= ((int)totalPlayTime / 60)*60;
        string m = ((int)totalPlayTime).ToString("00");
        return ConfigurationData.language == 0 ? string.Format("{0}시간 {1}분", h, m) : string.Format("{0}H {1}M", h, m);
    }
    string GetTotalMoveCount()
    {
        return PlayerData.moveCount.ToString("N0");
    }
    string GetTotalUndoCount()
    {
        return PlayerData.undoTryCount.ToString("N0");
    }
    string GetTotalRetryCount()
    {
        return PlayerData.retryCount.ToString("N0");
    }
    string GetTotalGoldCubeCount()
    {
        return PlayerData.getGoldCubeCount.ToString("N0");
    }
    string GetTotalCharacterCount()
    {
        return PlayerData.GetHavingCharacters().Count.ToString("N0");
    }
}
