using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Menu : MonoBehaviour
{
    public RectTransform backgroundRectTransform;
    public Transform menuContainer;
    List<Transform> menuItemTransform;
    public Transform currentMenuTransform;

    private void Awake()
    {
        StartMenuItemUI();
        this.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        RefreshUI();
        StopCoroutine("ClosingUI");
        StartCoroutine("StartUI");
    }

    void RefreshUI()
    {
        backgroundRectTransform.GetComponent<Image>().color = PushCubeColor.ThemeBackColor;
    }

    IEnumerator StartUI()
    {
        float startX = 2500;
        float time = .0f;
        backgroundRectTransform.anchoredPosition = new Vector3(startX, 0, 0);
        Color startColor = backgroundRectTransform.GetComponent<Image>().color;
        StartMenuItemUI();
        while (time<0.5f)
        {
            backgroundRectTransform.anchoredPosition = new Vector3(startX-(time*2000), 0, 0);
            backgroundRectTransform.GetComponent<Image>().color = new Color(startColor.r, startColor.g, startColor.b, (0.27f + (time*2) * 0.7f));
            time += Time.deltaTime*1.5f;
            yield return new WaitForEndOfFrame();
        }
        StartCoroutine(StartMenuUIAnimation(currentMenuTransform));
        backgroundRectTransform.GetComponent<Image>().color = startColor;
        backgroundRectTransform.anchoredPosition = new Vector3(1500, 0, 0);
    }
    public void CloseUI()
    {
        StopCoroutine("StartUI");
        StartCoroutine("ClosingUI");
    }

    IEnumerator ClosingUI()
    {
        float startX = 1500;
        float time = .0f;
        backgroundRectTransform.anchoredPosition = new Vector3(startX, 0, 0);
        Color startColor = backgroundRectTransform.GetComponent<Image>().color;
        StartCoroutine(CloseMenuUIAnimation(currentMenuTransform));
        while (time < 0.5f)
        {
            backgroundRectTransform.anchoredPosition = new Vector3(startX + (time * 2000), 0, 0);
            backgroundRectTransform.GetComponent<Image>().color = new Color(startColor.r, startColor.g, startColor.b, 1-(0.27f + (time * 1.4f)));
            time += Time.deltaTime*1.5f;
            yield return new WaitForEndOfFrame();
        }
        backgroundRectTransform.GetComponent<Image>().color = startColor;
        backgroundRectTransform.anchoredPosition = new Vector3(2500, 0, 0);
        this.gameObject.SetActive(false);
    }

    IEnumerator StartMenuUIAnimation(Transform menuItem)
    {
        menuItem.gameObject.SetActive(true);
        float time = .0f;
        while(time<0.5f)
        {
            foreach (var child in menuItem.GetComponentsInChildren<Text>())
            {
                child.color = new Color(child.color.r, child.color.g, child.color.b, (0.2f + (time * 2) * 0.8f));
            }
            time += Time.deltaTime * 1.5f;
            yield return new WaitForEndOfFrame();
        }
    }
    IEnumerator CloseMenuUIAnimation(Transform menuItem)
    {
        float time = .0f;
        while (time < 0.5f)
        {
            foreach (var child in menuItem.GetComponentsInChildren<Text>())
            {
                child.color = new Color(child.color.r, child.color.g, child.color.b, 1 - (0.2f + (time * 1.6f)));
            }
            time += Time.deltaTime * 1.5f;
            yield return new WaitForEndOfFrame();
        }
        menuItem.gameObject.SetActive(false);
    }

    void StartMenuItemUI()
    {
        menuItemTransform = new List<Transform>();
        for (int i = 0; i < menuContainer.childCount; i++)
        {
            menuItemTransform.Add(menuContainer.GetChild(i));
            menuContainer.GetChild(i).gameObject.SetActive(false);
        }
        currentMenuTransform = menuItemTransform[0];
    }

    public void OnClickMenu(int index)
    {
        SoundManager.PlaySound(SoundManager.Sound.MenuClick);
        currentMenuTransform.gameObject.SetActive(false);
        currentMenuTransform = menuItemTransform[index];
        StartCoroutine(StartMenuUIAnimation(currentMenuTransform));
    }
}
