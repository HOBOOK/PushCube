using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSystemManager : MonoBehaviour
{
    public static GameSystemManager Instance;
    public GameObject AlertYesNoPrefab;
    public GameObject AlertGetPrefab;
    public GameObject AlertTopPrefab;
    public Transform LoadImageTransform;
    Transform canvas;
    Queue<AlertResult> alertResults = new Queue<AlertResult>();
    int alertCount;
    float playTimeCheckTime=.0f;
    bool isQuitPopup;
    public bool isPlayerDataLoad = false;
    public bool isAdMobLoad=false;
    public bool isGPGSLoad=false;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Application.targetFrameRate = 60;
        isQuitPopup = false;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        StartCoroutine("LoadCompleted");
    }

    IEnumerator LoadCompleted()
    {
        float process = 0f;
        LoadImageTransform.GetChild(0).gameObject.SetActive(true);
        LoadImageTransform.GetChild(1).gameObject.SetActive(false);
        while (!isAdMobLoad||!isGPGSLoad)
        {
            LoadImageTransform.GetChild(0).GetComponent<Image>().fillAmount = process;

            if(process>=1.0f)
                LoadImageTransform.GetChild(0).GetComponent<Image>().fillClockwise = true;
            else if(process<=0.0f)
                LoadImageTransform.GetChild(0).GetComponent<Image>().fillClockwise = false;
            if (LoadImageTransform.GetChild(0).GetComponent<Image>().fillClockwise)
                process -= Time.deltaTime;
            else
                process += Time.deltaTime;
            yield return null;
        }
        LoadImageTransform.GetChild(0).gameObject.SetActive(false);
        yield return new WaitForFixedUpdate();
        LoadImageTransform.GetChild(1).gameObject.SetActive(true);
        LoadImageTransform.GetChild(1).GetComponent<Image>().fillAmount = 0;
        yield return new WaitForFixedUpdate();
        float time = 0f;
        while(time<1.0f)
        {
            LoadImageTransform.GetChild(1).GetComponent<Image>().fillAmount = time;
            time += 2 * Time.deltaTime;
            yield return null;
        }
        LoadImageTransform.GetChild(1).GetComponent<Image>().fillAmount = 1;
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadSceneAsync(1);
    }

    private void Update()
    {
        UpdateCheckPlayTime();
        UpdateAlertMessageReceiver();
        UpdateCheckExitButton();
    }

    private void OnApplicationQuit()
    {
        if(isPlayerDataLoad)
        {
            PlayerDataSystem.Save();
            ConfigurationDataSystem.Save();
        }
    }
    private void UpdateCheckExitButton()
    {
        if(Input.GetKeyDown(KeyCode.Escape)&&!isQuitPopup)
        {
            isQuitPopup = true;
            ShowAlertMessage(new AlertMessage(0, LocalizationManager.GetText("Alert_GameQuit"), null, null, GameQuit));
        }
    }
    private void GameQuit(bool isYes)
    {
        isQuitPopup = false;
        if (isYes)
            Application.Quit();
    }
    
    private void UpdateCheckPlayTime()
    {
        playTimeCheckTime += Time.deltaTime;
        if(playTimeCheckTime>30)
        {
            PlayerData.totalPlayTime += .5f;
            PlayerDataSystem.Save();
            playTimeCheckTime = .0f;
        }
    }

#region 알림창
    void UpdateAlertMessageReceiver()
    {
        if (alertResults.Count > 0)
        {
            int itemsInQueue = alertResults.Count;
            lock (alertResults)
            {
                for (int i = 0; i < itemsInQueue; i++)
                {
                    AlertResult alertResult = alertResults.Dequeue();
                    alertResult.callback(alertResult.result);
                }
            }
        }
    }
    public static void ShowAlertMessage(AlertMessage message)
    {
        if(Instance.alertCount<3)
        {
            ThreadStart threadStart = delegate
            {
                Instance.ShowAlert(message, Instance.FinishAlertMessage);
            };
            threadStart.Invoke();
        }
    }
    public void ShowAlert(AlertMessage message, UnityAction<AlertResult> callback=null)
    {
        if (canvas == null)
            canvas = GameObject.Find("Canvas").transform;
        alertCount++;
        if (message.type == 0)
        {
            GameObject alertYesNo = Instantiate(AlertYesNoPrefab, canvas);
            StartCoroutine(AlertReceiving(alertYesNo, message, callback));
        }
        else if(message.type==1)
        {
            GameObject alertGet = Instantiate(AlertGetPrefab, canvas);
            UI_AlertGet alertScript = alertGet.GetComponent<UI_AlertGet>();
            alertScript.image.sprite = message.sprite != null ? message.sprite : alertScript.image.sprite;
            if (message.prefab != null)
            {
                GameObject messagePrefab = Instantiate(message.prefab, alertScript.image.transform);
                messagePrefab.transform.localScale = new Vector3(200, 200, 200);
                messagePrefab.transform.localRotation = Quaternion.Euler(new Vector3(-20, 45, -20));
                messagePrefab.transform.localPosition = new Vector3(0, -50, -100);
                messagePrefab.SetActive(true);
            }
            alertScript.text.text = message.context;
            alertGet.gameObject.SetActive(true);
            StartCoroutine(AlertEnd(2.5f));
        }
        else if(message.type==2) // 탑 알림창
        {
            GameObject alertTop = Instantiate(AlertTopPrefab, canvas);
            UI_AlertTop alertScript = alertTop.GetComponent<UI_AlertTop>();
            alertScript.image.sprite = message.sprite != null ? message.sprite : alertScript.image.sprite;
            alertScript.text.text = message.context;
            alertTop.gameObject.SetActive(true);
            StartCoroutine(AlertEnd(2.5f));
        }
    }
    public IEnumerator AlertReceiving(GameObject alertObject, AlertMessage message, UnityAction<AlertResult> callback)
    {
        UI_AlertYesNo alertScript = alertObject.GetComponent<UI_AlertYesNo>();
        alertScript.image.sprite = message.sprite!=null?message.sprite : alertScript.image.sprite;
        alertScript.text.text = message.context;
        alertObject.SetActive(true);
        bool receiveResult = false;
        while(!alertScript.isSelectCompleted)
        {
            yield return null;
        }
        receiveResult = alertScript.result;
        StartCoroutine(alertScript.OnDisableAnimation());
        callback(new AlertResult(receiveResult, message.callback));
    }
    public IEnumerator AlertEnd(float delay)
    {
        yield return new WaitForSeconds(delay);
        alertCount = alertCount - 1 >= 0 ? alertCount - 1 : 0;
    }
    public void FinishAlertMessage(AlertResult result)
    {
        lock (alertResults)
        {
            alertResults.Enqueue(result);
            alertCount = alertCount - 1 >= 0 ? alertCount - 1 : 0;
        }
    }

    public void ShowReviewRequestAlert()
    {
        ShowAlertMessage(new AlertMessage(0, LocalizationManager.GetText("Alert_Rating"), null,null, MoveToAndroidMarket));
    }
    public void MoveToAndroidMarket(bool isYes)
    {
        if(isYes)
        {
            //Application.OpenURL("market://details?id=com.Company.Project");
            Debug.Log("안드로이드 마켓이동");
        }
    }
#endregion
}


public struct AlertMessage
{
    public int type;
    public string context;
    public Sprite sprite;
    public GameObject prefab;
    public UnityAction<bool> callback;
    public AlertMessage(int nType, string nContext, Sprite nSprite, GameObject nPrefab, UnityAction<bool> nCallback)
    {
        type = nType;
        context = nContext;
        sprite = nSprite;
        prefab = nPrefab;
        callback = nCallback;
    }
}

public struct AlertResult
{
    public bool result;
    public UnityAction<bool> callback;
    public AlertResult(bool nResult, UnityAction<bool> nCallback)
    {
        result = nResult;
        callback = nCallback;
    }
}