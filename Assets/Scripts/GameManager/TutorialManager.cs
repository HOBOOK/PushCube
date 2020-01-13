using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    GameManager gameManager;
    Controller controller;
    Transform Character;
    public Transform TopUI;
    public Transform BottomUI;
    public Transform ContextUI;
    Text contextText;
    public GameObject Gesture;

    bool isTutorialPlay = false;
    int[] GestureOrder =  { 1,4,2,3};
    string[] GestureText = new string[4];

    private void Awake()
    {
        contextText = ContextUI.GetComponentInChildren<Text>();
    }
    public void StartTutorial()
    {
        gameManager = GameManager.instance;
        controller = gameManager.controller;
        Character = controller.Target;
        for(int i = 0; i < 4; i++)
        {
            GestureText[i] = LocalizationManager.GetText("Tutorial_Move" + (i + 1));
        }
        if (!isTutorialPlay)
        {
            isTutorialPlay = true;
            StartCoroutine("StartingTutorial");
        }
    }
    IEnumerator StartingTutorial()
    {
        // 튜토리얼 시작
        TopUI.gameObject.SetActive(false);
        BottomUI.gameObject.SetActive(false);
        Gesture.SetActive(false);
        gameManager.GamePause(true);
        ContextUI.gameObject.SetActive(false);

        yield return StartCoroutine("IntroTutorial");
        yield return new WaitForSeconds(1f);
        // 조작 튜토리얼 시작
        yield return StartCoroutine("ControlTutorial");
        yield return new WaitForSeconds(.5f);
        yield return StartCoroutine(ShowingTutorialText(LocalizationManager.GetText("Tutorial_ControlTutorialClear1")));
        yield return StartCoroutine(ShowingTutorialText(LocalizationManager.GetText("Tutorial_ControlTutorialClear2")));
        // UI 튜토리얼 시작
        yield return StartCoroutine(ShowingTutorialText(LocalizationManager.GetText("Tutorial_ControlTutorialClear3")));
        yield return StartCoroutine("UITutorial");
        yield return new WaitForSeconds(1f);
        // 클리어까지 대기
        gameManager.GamePause(false);
        while(PlayerData.currentStage<1)
            yield return null;

        yield return StartCoroutine("TutorialClear");

    }
    IEnumerator IntroTutorial()
    {
        string[] scenarios = {LocalizationManager.GetText("Tutorial_IntroTutorial1"),LocalizationManager.GetText("Tutorial_IntroTutorial2"),
            LocalizationManager.GetText("Tutorial_IntroTutorial3") };
        Text text = ContextUI.GetComponentInChildren<Text>();
        for (var i = 0; i < scenarios.Length; i++)
        {
            yield return StartCoroutine(ShowingTutorialText(scenarios[i]));
        }
        ContextUI.gameObject.SetActive(false);
        yield return null;
    }
    #region 튜토리얼 텍스트 관련
    IEnumerator ShowingTutorialText(string context)
    {
        yield return StartCoroutine("OnEnableAnimation");
        ContextUI.transform.GetChild(1).GetComponent<Image>().gameObject.SetActive(false);
        for (int j = 0; j < context.Length; j++)
        {
            contextText.text = context.Substring(0, j);
            yield return new WaitForSeconds(.05f);
        }
        ContextUI.transform.GetChild(1).GetComponent<Image>().gameObject.SetActive(true);
        yield return new WaitForEndOfFrame();
        yield return StartCoroutine("TouchReady");
        ContextUI.transform.GetChild(1).GetComponent<Image>().gameObject.SetActive(false);
        yield return StartCoroutine("OnDisableAnimation");
        yield return new WaitForSeconds(.5f);
    }

    IEnumerator OnEnableAnimation()
    {
        SoundManager.PlaySound(SoundManager.Sound.AlertPopup);
        ContextUI.gameObject.SetActive(true);
        float time = .0f;
        RectTransform rectTransform = ContextUI.GetComponent<RectTransform>();
        while (time < .3f)
        {
            rectTransform.anchoredPosition = new Vector3(0, -100 + (time * 300), 0);
            foreach (Image image in ContextUI.GetComponentsInChildren<Image>())
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, time * 3.3f);
            }
            foreach (Text text in ContextUI.GetComponentsInChildren<Text>())
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, time * 3.3f);
            }
            time += Time.deltaTime * 1.5f;
            yield return new WaitForEndOfFrame();
        }
        foreach (Image image in ContextUI.GetComponentsInChildren<Image>())
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
        }
        foreach (Text text in ContextUI.GetComponentsInChildren<Text>())
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        }
        rectTransform.anchoredPosition = Vector3.zero;
    }

    public IEnumerator OnDisableAnimation()
    {
        float time = .0f;
        RectTransform rectTransform = ContextUI.GetComponent<RectTransform>();
        while (time < .3f)
        {
            rectTransform.anchoredPosition = new Vector3(0, -(time * 300), 0);
            foreach (Image image in ContextUI.GetComponentsInChildren<Image>())
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, 1 - (time * 3.3f));
            }
            contextText.color = new Color(contextText.color.r, contextText.color.g, contextText.color.b, 1 - (time * 3.3f));
            time += Time.deltaTime * 1.5f;
            yield return new WaitForEndOfFrame();
        }
        foreach (Image image in ContextUI.GetComponentsInChildren<Image>())
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        }
        contextText.text = "";
        ContextUI.gameObject.SetActive(false);
    }

    IEnumerator TouchReady()
    {
        while(!Input.GetMouseButtonDown(0))
        {
            yield return null;
        }
        SoundManager.PlaySound(SoundManager.Sound.MenuClick);
        yield return null;
    }
    #endregion

    IEnumerator ControlTutorial()
    {
        gameManager.GamePause(false);
        Gesture.SetActive(true);
        Gesture.SetActive(true);
        MapManager.Direction dir;
        for (int i = 0; i < 4; i++)
        {
            dir = (MapManager.Direction)GestureOrder[i];
            controller.swipeController.SwipeRestrict((int)dir);
            GestureAnimationStart(dir);
            while (controller.direction != dir)
            {
                yield return null;
            }
            yield return null;
        }
        SoundManager.PlaySound(SoundManager.Sound.StageClear);
        Gesture.SetActive(false);
        controller.swipeController.SwipeRestrictClear();
        gameManager.GamePause(true);
        yield return null;
    }

    IEnumerator UITutorial()
    {
        gameManager.GamePause(false);
        TopUI.gameObject.SetActive(true);
        BottomUI.gameObject.SetActive(true);
        float time = 0f;
        while(time<1.0f)
        {
            foreach (Text child in TopUI.GetComponentsInChildren<Text>())
            {
                child.color = new Color(child.color.r, child.color.r, child.color.b, time);
            }
            TopUI.GetComponent<RectTransform>().anchoredPosition = PlayerData.isRemoveAd ? new Vector3(0, 50 - time * 50, 0) : new Vector3(0, -100 - time * 50, 0);
            time += 2 * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        TopUI.GetComponent<RectTransform>().anchoredPosition = PlayerData.isRemoveAd ?  Vector3.zero : new Vector3(0,-150,0);
        gameManager.GamePause(true);
        yield return null;
    }

    void GestureAnimationStart(MapManager.Direction dir)
    {
        SoundManager.PlaySound(SoundManager.Sound.ImagePopup);
        Gesture.GetComponentInChildren<Animator>().SetInteger("Direction", ((int)dir-1));
        Gesture.GetComponentInChildren<Text>().text = GestureText[(int)dir - 1];
    }

    IEnumerator TutorialClear()
    {
        TopUI.gameObject.SetActive(true);
        BottomUI.gameObject.SetActive(true);
        Gesture.SetActive(false);
        ContextUI.gameObject.SetActive(false);
        gameManager.GamePause(false);
        yield return StartCoroutine(ShowingTutorialText(LocalizationManager.GetText("Tutorial_Clear1")));
        yield return StartCoroutine(ShowingTutorialText(LocalizationManager.GetText("Tutorial_Clear2")));
        GameManager.instance.GameStart(1);
        GooglePlayManager.Instance.OnAddAchievement(0);
    }


}
