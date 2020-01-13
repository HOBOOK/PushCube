using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_AlertYesNo : MonoBehaviour
{
    public Image image;
    public Text text;
    public Button buttonYes;
    public Button buttonNo;
    public bool isSelectCompleted = false;
    public bool result = false;

    RectTransform alertRectTransform;

    private void Awake()
    {
        alertRectTransform = this.transform.GetChild(0).GetComponent<RectTransform>();
    }
    private void OnEnable()
    {
        isSelectCompleted = false;

        StartCoroutine("OnEnableAnimation");
    }

    IEnumerator OnEnableAnimation()
    {
        SoundManager.PlaySound(SoundManager.Sound.AlertPopup);
        float time = .0f;
        while(time < .3f)
        {
            alertRectTransform.anchoredPosition = new Vector3(0, -100 + (time*300), 0);
            foreach(Image image in alertRectTransform.GetComponentsInChildren<Image>())
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, time * 3.3f);
            }
            foreach (Text text in alertRectTransform.GetComponentsInChildren<Text>())
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, time * 3.3f);
            }
            time += Time.deltaTime*1.5f;
            yield return new WaitForEndOfFrame();
        }
        foreach (Image image in alertRectTransform.GetComponentsInChildren<Image>())
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b,1);
        }
        foreach (Text text in alertRectTransform.GetComponentsInChildren<Text>())
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        }
        alertRectTransform.anchoredPosition = Vector3.zero;
    }

    public IEnumerator OnDisableAnimation()
    {
        float time = .0f;
        while (time < .3f)
        {
            alertRectTransform.anchoredPosition = new Vector3(0, -(time * 300), 0);
            foreach (Image image in alertRectTransform.GetComponentsInChildren<Image>())
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, 1-(time * 3.3f));
            }
            foreach (Text text in alertRectTransform.GetComponentsInChildren<Text>())
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, 1-(time * 3.3f));
            }
            time += Time.deltaTime*1.5f;
            yield return new WaitForEndOfFrame();
        }
        foreach (Image image in alertRectTransform.GetComponentsInChildren<Image>())
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b,0);
        }
        foreach (Text text in alertRectTransform.GetComponentsInChildren<Text>())
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
        }
        Destroy(this.gameObject);
    }

    public void OnClickYes()
    {
        SoundManager.PlaySound(SoundManager.Sound.AlertYes);
        result = true;
        isSelectCompleted = true;
    }

    public void OnClickNo()
    {
        SoundManager.PlaySound(SoundManager.Sound.AlertNo);
        result = false;
        isSelectCompleted = true;
    }
}
