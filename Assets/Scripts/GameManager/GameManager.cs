using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public MapManager mapManager;
    public Controller controller;
    public TutorialManager tutorialManager;

    public int StageNumber;
    public int MoveCount;
    public int GoldCubeCount;
    public float StageTime;

    public Text StageNumberText;
    public Text BestMoveCountText;
    public Text MoveCountText;
    public Text UndoCountText;
    public Text TimeText;
    public Image Fade;
    public RectTransform TopUITransform;

    public GameObject ClearEffectPrefab;
    GameObject[] ClearEffect;

    float gameTime;
    string h, m, s;

    bool isPause;
    bool isStart;
    bool isClear;
    bool isRestart;
    bool isGoldCubeClear;
    bool isUndo;

    Stack<PlayingRecord> playingRecords;
    
    public bool IsPlaying
    {
        get { return isStart && !isPause; }
    }

    private void Awake()
    {
        instance = this;
        controller = GetComponent<Controller>();
        ClearEffect = new GameObject[3];
        for (int i = 0; i < 3; i++)
        {
            ClearEffect[i] = Instantiate(ClearEffectPrefab, this.transform);
            ClearEffect[i].gameObject.SetActive(false);
        }

        GetPlayerData();
    }
    private void Start()
    {
        RenderSettings.skybox.SetColor("_TintColor", new Color32(200, 230, 50, 255));
        SetTopUIPosition();
        StartCoroutine("GameStartAnimation");
        GameStart(PlayerData.currentStage);
        if(PlayerData.currentStage==0)
        {
            tutorialManager = GetComponent<TutorialManager>();
            tutorialManager.StartTutorial();
        }
        AdMobManager.Instance.ShowBanner();
    }
    private void Update()
    {
        if(IsPlaying)
        {
            UpdateTime();
        }
    }

    void SetTopUIPosition()
    {
        if(PlayerData.isRemoveAd)
        {
            TopUITransform.anchoredPosition = Vector3.zero;
        }
        else
        {
            TopUITransform.anchoredPosition = new Vector3(0, -150, 0);
        }
    }

    void GetPlayerData()
    {
        StageNumber = PlayerData.currentStage;
    }

    public void GameStart(int stage)
    {
        SoundManager.PlayLoopSound(SoundManager.Sound.Loop1);
        ResetScore(stage);
        mapManager.CreateMap(stage);
        isPause = false;
        isStart = true;
        isClear = false;
        isGoldCubeClear = false;
    }

    public void GameRestart()
    {
        if(!isClear&&!isRestart)
        {
            if(PlayerData.currentStage>0)
                AdMobManager.Instance.ShowInterstitial();
            StartCoroutine("RestartDelay");
            PlayerData.Retry();
            ResetScore(StageNumber);
            mapManager.CreateMap(StageNumber);
            isGoldCubeClear = false;
            isPause = false;
            isStart = true;
            isClear = false;
        }
    }

    IEnumerator GameStartAnimation()
    {
        Fade.gameObject.SetActive(true);
        Color startcolor = PushCubeColor.ThemeMainColor;
        float alpha = 0f;
        while (alpha < 1f)
        {
            Fade.color = new Color(startcolor.r, startcolor.g, startcolor.b, 1-alpha);
            alpha += 1 * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Fade.gameObject.SetActive(false);
    }

    public void GameUndo()
    {
        if(!isUndo&&!isRestart&&IsPlaying)
        {
            if(PlayerData.undoCount>0)
            {
                StartCoroutine("UndoDelay");
                if (playingRecords.Count == 0)
                {
                    Debug.Log("원지점입니다.");
                    GameSystemManager.ShowAlertMessage(new AlertMessage(2, LocalizationManager.GetText("Alert_WarningUndoStart"), null,null, null));
                }
                else
                {
                    PlayingRecord record = playingRecords.Pop();
                    controller.UndoMove(record);
                    PlayerData.UseUndoCount();
                    UpdateUI();
                }
            }
            else
            {
                GameSystemManager.ShowAlertMessage(new AlertMessage(2, LocalizationManager.GetText("Alert_WarningUndo"), null,null, null));
            }
        }
    }

    public void GamePause(bool pause)
    {
        isPause = pause;
    }

    public void UpdateUI()
    {
        StageNumberText.text = string.Format("{0} {1}", LocalizationManager.GetText("StageScene_StageText"), StageNumber);
        MoveCountText.text = string.Format("{0} {1}",LocalizationManager.GetText("StageScene_MoveText"), MoveCount);
        UndoCountText.text = string.Format("{0} {1}", LocalizationManager.GetText("StageScene_UndoText"),PlayerData.undoCount);
    }

    void UpdateTime()
    {
        gameTime += Time.deltaTime;
        if(gameTime>1.0f)
        {
            StageTime++;
            h = ((int)(StageTime / 3600)).ToString("00");
            StageTime -= ((int)StageTime / 3600) * 3600;
            m = ((int)(StageTime / 60)).ToString("00");
            s = (StageTime % 60).ToString("00");
            TimeText.text = string.Format("{0}:{1}:{2}", h, m, s);
            gameTime = .0f;
        }
    }

    public void ResetScore(int stage)
    {
        playingRecords = new Stack<PlayingRecord>();
        StageNumber = stage;
        StageNumberText.text = string.Format("{0} {1}",LocalizationManager.GetText("StageScene_StageText"), StageNumber);
        BestMoveCountText.text = MapDataManager.GetMapClearCount(stage);
        PlayerData.AddMoveCount(MoveCount);
        MoveCount = 0;
        StageTime = 0;
        UpdateUI();
    }

    public void AddMoveCount()
    {
        MoveCount++;
        UpdateUI();
    }
    public void AddPushCount()
    {
        UpdateUI();
        StageClear();
    }
    public void RemoveMoveCount()
    {
        MoveCount--;
        UpdateUI();
    }
    void StageClear()
    {
        if(mapManager.CheckClearable())
        {
            GoldCubeCount++;
            isStart = false;
            isClear = true;
            Debug.Log("스테이지 클리어!");
            if ((StageNumber==5||StageNumber==50)&& IsReviewPopupAble)
                GameSystemManager.Instance.ShowReviewRequestAlert();

            StartCoroutine("StageClearing");
        }
    }

    IEnumerator StageClearing()
    {
        SoundManager.PlaySound(SoundManager.Sound.StageClear);
        string bestClearCountString = MapDataManager.GetMapClearCount(StageNumber);
        int bestClearCount = bestClearCountString=="?" ? 999 : int.Parse(bestClearCountString);
        if (bestClearCount == MoveCount)
            StartCoroutine(StageClearEffect(3));
        else if(MoveCount > bestClearCount && MoveCount <= bestClearCount * 1.5f)
            StartCoroutine(StageClearEffect(2));
        else if(MoveCount > bestClearCount * 1.5f && MoveCount < bestClearCount * 5)
            StartCoroutine(StageClearEffect(1));

        StageClearInfoSave();
        if (isGoldCubeClear)
        {
            isGoldCubeClear = false;
            if (bestClearCount == MoveCount)
                PlayerData.AddGoldCube(2);
            else
                PlayerData.AddGoldCube(1);
        }
        StageNumber = StageNumber<100? StageNumber+1 : 100;
        if (StageNumber > PlayerData.currentStage)
        {
            PlayerData.currentStage = StageNumber;
            PlayerDataSystem.Save();
        }
        yield return new WaitForSeconds(.5f);
        controller.Jump();
        yield return new WaitForSeconds(3.0f);
        if(StageNumber!=1)
        {
            // if(StageNumber==50)
            //     GooglePlayManager.Instance.OnAddAchievement(2);
            GameStart(StageNumber);
            AdMobManager.Instance.ShowInterstitial();
        }
    }
    IEnumerator StageClearEffect(int clearPoint)
    {
        for(int i = 0; i < clearPoint; i++)
        {
            ClearEffect[i].transform.position = controller.Target.transform.position + new Vector3(0,2,0);
            ClearEffect[i].gameObject.SetActive(true);
            ClearEffect[i].GetComponent<ParticleSystem>().Play();
            SoundManager.PlaySound(SoundManager.Sound.GameClearFirework);
            yield return new WaitForSeconds(0.2f);
        }
        yield return new WaitForSeconds(2.0f);
        for (int i = 0; i < clearPoint; i++)
        {
            ClearEffect[i].gameObject.SetActive(false);
        }
    }

    IEnumerator RestartDelay()
    {
        isRestart = true;
        yield return new WaitForSeconds(2.0f);
        isRestart = false;
    }

    IEnumerator UndoDelay()
    {
        isUndo = true;
        yield return new WaitForSeconds(.3f);
        isUndo = false;
    }

    public void ResetGoldCubeCount()
    {
        isGoldCubeClear = true;
        GoldCubeCount = 0;
    }

    public void RecordPlay(MapManager.Direction dir,Vector3 currentPos, Vector3 targetPos, Vector3 searchPos)
    {
        playingRecords.Push(new PlayingRecord(playingRecords.Count, (int)dir, currentPos, targetPos, searchPos));
    }

    void StageClearInfoSave()
    {
        List<ClearStageInfo> clearInfo = PlayerData.clearStageInfoList;
        ClearStageInfo currentStageInfo = clearInfo.Find(x => x.stageNumber == StageNumber);
        if(currentStageInfo!=null)
        {
            currentStageInfo.clearCount++;
            if (MoveCount < currentStageInfo.totalMove)
                currentStageInfo.totalMove = MoveCount;
            if (StageTime < currentStageInfo.clearTime)
                currentStageInfo.clearTime = StageTime;
        }
        else
        {
            ClearStageInfo newClearStageInfo = new ClearStageInfo(StageNumber, MoveCount, 0, 1, StageTime);
            clearInfo.Add(newClearStageInfo);
        }
        PlayerData.UpdateClearStageInfo(clearInfo);
    }

    bool IsReviewPopupAble
    {
        get
        {
            if (PlayerPrefs.HasKey("ReviewPopup"))
            {
                TimeSpan timediff = DateTime.Now - DateTime.Parse(PlayerPrefs.GetString("ReviewPopup"));
                if (timediff.Days > 15)
                {
                    PlayerPrefs.SetString("ReviewPopup", DateTime.Now.ToShortDateString());
                    return true;
                }
                return false;
            }
            else
            {
                PlayerPrefs.SetString("ReviewPopup", DateTime.Now.ToShortDateString());
                return true;
            }
        }
    }

}

public struct PlayingRecord
{
    public int order;
    public int direction;
    public Vector3 currentPos;
    public Vector3 targetPos;
    public Vector3 searchPos;
    public PlayingRecord(int nOrder, int nDir, Vector3 nCurrentPos, Vector3 nTargetPos, Vector3 nSearchPos)
    {
        order = nOrder;
        direction = nDir;
        currentPos = nCurrentPos;
        targetPos = nTargetPos;
        searchPos = nSearchPos;
    }
}
