using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MenuStage : MonoBehaviour
{
    public Transform PanelMenuTransform;
    public ScrollRect stageScrollView;
    public Transform stageSelectCheckPanelTransform;
    public Text currentStageText;
    Transform stageParentTransform;
    public GameObject slotStagePrefab;
    List<Transform> stageSlotList;
    int selectStage;
    int currentPlayingStageNumber;
    public UI_MenuButton menuButtonScript;

    private void Awake()
    {
        stageParentTransform = stageScrollView.GetComponentInChildren<ContentSizeFitter>().transform;
        stageSlotList = new List<Transform>();
        for (int i = 0; i < 100; i++)
        {
            GameObject slot = Instantiate(slotStagePrefab, stageParentTransform);
            slot.GetComponentInChildren<Text>().text = (i + 1).ToString();
            int slotStageNumber = i + 1;
            slot.GetComponent<Button>().onClick.RemoveAllListeners();
            slot.GetComponent<Button>().onClick.AddListener(delegate
            {
                OnSelectStage(slotStageNumber);
            });
            slot.SetActive(true);
            stageSlotList.Add(slot.transform);
        }
    }

    private void OnEnable()
    {
        RefreshUI();
    }

    void RefreshUI()
    {
        stageSelectCheckPanelTransform.gameObject.SetActive(true);
        for (int i = 0; i < stageSlotList.Count; i++)
        {
            if ((i + 1) > PlayerData.currentStage)
                stageSlotList[i].GetChild(0).GetComponent<Image>().color = PushCubeColor.SlotUnableColor;
            else if ((i + 1) < PlayerData.currentStage)
            {
                stageSlotList[i].GetChild(0).GetComponent<Image>().color = PushCubeColor.SlotColor;
                if (PlayerData.clearStageInfoList[i + 1].totalMove == int.Parse(MapDataManager.GetMapClearCount(i + 1)))
                    stageSlotList[i].GetChild(2).gameObject.SetActive(true);
                else
                    stageSlotList[i].GetChild(2).gameObject.SetActive(false);
            }
            else
            {
                stageSlotList[i].GetChild(0).GetComponent<Image>().color = PushCubeColor.SlotColor;
            }
        }
        foreach (Text txt in GetComponentsInChildren<Text>())
        {
            if(txt.name.Equals("title"))
            {
                txt.color = PushCubeColor.ThemeMainColor;
                continue;
            }
            txt.color = PushCubeColor.ThemeTextColor;
        }

        stageSelectCheckPanelTransform.gameObject.SetActive(false);
        currentPlayingStageNumber = GameManager.instance.StageNumber;
        currentStageText.text = LocalizationManager.GetText("StageScene_MenuStage_Current") + " " + currentPlayingStageNumber;
        stageParentTransform.GetComponent<RectTransform>().anchoredPosition= new Vector3(Mathf.Clamp(-(256 * (currentPlayingStageNumber - 3)),-256*stageSlotList.Count,0),0,0);
        if(currentPlayingStageNumber>0)
            StartCoroutine(CurrentStageAnimation(currentPlayingStageNumber));
    }

    void OnSelectStage(int stage)
    {
        if (selectStage > 0)
        {
            stageSlotList[selectStage - 1].GetChild(0).GetComponent<Image>().color = PushCubeColor.SlotColor;
            selectStage = 0;
        }
        if (stage>PlayerData.currentStage)
        {
            SoundManager.PlaySound(SoundManager.Sound.MenuMiss);
            stageSelectCheckPanelTransform.GetChild(0).GetComponent<Text>().enabled = false;
            stageSelectCheckPanelTransform.GetChild(1).GetComponent<Text>().text = string.Format("{0}",LocalizationManager.GetText("StageScene_MenuStage_UnableStage"));
            stageSelectCheckPanelTransform.GetChild(2).gameObject.SetActive(false);
            stageSelectCheckPanelTransform.gameObject.SetActive(true);
        }
        else if(stage == currentPlayingStageNumber)
        {
            stageSelectCheckPanelTransform.gameObject.SetActive(false);
        }
        else
        {
            SoundManager.PlaySound(SoundManager.Sound.MenuClick);
            selectStage = stage;

            ClearStageInfo clearStageInfo = PlayerData.clearStageInfoList.Find(x => x.stageNumber == selectStage);
            if(clearStageInfo!=null)
            {
                stageSelectCheckPanelTransform.GetChild(0).GetComponent<Text>().enabled = true;
                stageSelectCheckPanelTransform.GetChild(0).GetComponent<Text>().text = ConfigurationData.language == 0 ? string.Format("<b>최고 기록</b>   시간 <color='#FF8364'><size='45'>{0}</size></color>     이동 <color='#FF8364'><size='45'>{1}</size></color>     완료 <color='#FF8364'><size='45'>{2}</size></color>", clearStageInfo.GetClearTime(), clearStageInfo.totalMove, clearStageInfo.clearCount) : string.Format("<b>Best Record</b>   Time <color='#FF8364'><size='45'>{0}</size></color>     Move <color='#FF8364'><size='45'>{1}</size></color>     Clear <color='#FF8364'><size='45'>{2}</size></color>", clearStageInfo.GetClearTime(), clearStageInfo.totalMove, clearStageInfo.clearCount);
            }
            else
            {
                stageSelectCheckPanelTransform.GetChild(0).GetComponent<Text>().enabled = false;
            }
            stageSelectCheckPanelTransform.GetChild(1).GetComponent<Text>().text = ConfigurationData.language == 0 ? string.Format("{0} {1} ?", selectStage, LocalizationManager.GetText("StageScene_MenuStage_StartStage")) : string.Format("{0} {1} ?", LocalizationManager.GetText("StageScene_MenuStage_StartStage"), selectStage);
            stageSelectCheckPanelTransform.GetChild(2).gameObject.SetActive(true);
            stageSelectCheckPanelTransform.gameObject.SetActive(true);
            stageSlotList[selectStage - 1].GetChild(0).GetComponent<Image>().color = PushCubeColor.SlotSelectColor;
        }
    }

    public void OnClickSelectStageYes()
    {
        if(selectStage>0)
        {
            SoundManager.PlaySound(SoundManager.Sound.MenuClick);
            stageSlotList[selectStage - 1].GetChild(0).GetComponent<Image>().color = PushCubeColor.SlotColor;
            PanelMenuTransform.gameObject.SetActive(false);
            menuButtonScript.InitUI();
            GameManager.instance.GameStart(selectStage);
        }
    }
    public void OnClickSelectStageNo()
    {
        SoundManager.PlaySound(SoundManager.Sound.MenuClick);
        stageSelectCheckPanelTransform.gameObject.SetActive(false);
        stageSlotList[selectStage - 1].GetChild(0).GetComponent<Image>().color = PushCubeColor.SlotColor;
        selectStage = 0;
    }

    IEnumerator CurrentStageAnimation(int stage)
    {
        float sclae = 1f;
        bool flag = false;
        while(this.gameObject.activeSelf)
        {
            if (sclae > 1.2f)
                flag = true;
            else if (sclae < 1.0f)
                flag = false;
            stageSlotList[stage - 1].localScale = new Vector3(sclae, sclae, sclae);
            if (flag)
                sclae -= Time.deltaTime*.5f;
            else
                sclae += Time.deltaTime*.5f;
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
}
