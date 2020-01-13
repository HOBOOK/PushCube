using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MenuButton : MonoBehaviour
{
    public Transform MenuTransform;
    UI_Menu menuScript;
    public Sprite OnSprite;
    public Sprite OffSprite;
    Image buttonImage;
    bool isOff;
    bool isAnimating;
    private void Awake()
    {
        buttonImage = this.GetComponent<Image>();
        menuScript = MenuTransform.GetComponent<UI_Menu>();
        isOff = false;
        isAnimating = false;
        buttonImage.color = Color.white;
    }
    public void InitUI()
    {
        isOff = false;
        StartCoroutine("ClickAnimation");
    }

    public void OnClickMenuButton()
    {
        if(!isAnimating)
        {
            if (MenuTransform.gameObject.activeSelf)
            {
                if(IsMainMenu)
                {
                    SoundManager.PlaySound(SoundManager.Sound.MenuOpen);
                    isOff = false;
                    StartCoroutine("ClickAnimation");
                    MenuTransform.GetComponent<UI_Menu>().CloseUI();
                    GameManager.instance.GamePause(false);
                }
                else
                {
                    menuScript.OnClickMenu(0);
                }
            }
            else
            {
                SoundManager.PlaySound(SoundManager.Sound.MenuOpen);
                isOff = true;
                StartCoroutine("ClickAnimation");
                MenuTransform.gameObject.SetActive(true);
                MenuTransform.gameObject.SetActive(true);
                GameManager.instance.GamePause(true);
                IsMainMenu = true;
            }
        }
    }

    IEnumerator ClickAnimation()
    {
        isAnimating = true;
        float time = .0f;
        while(time<0.25f)
        {
            buttonImage.color = ConfigurationData.theme==0||isOff? new Color(1, 1, 1, (1 - time * 4)) : new Color(.15f,.15f,.15f,(1-time*4));
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        if (isOff)
            buttonImage.sprite = OffSprite;
        else
            buttonImage.sprite = OnSprite;
        time = .0f;
        while (time < 0.25f)
        {
            buttonImage.color = ConfigurationData.theme == 0|| !isOff ? new Color(1, 1, 1, (time * 4)):new Color(.15f, .15f, .15f, (time * 4));
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        buttonImage.color = ConfigurationData.theme==0||!isOff? Color.white : new Color(.15f, .15f, .15f, 1);
        isAnimating = false;
    }

    bool IsMainMenu
    {
        get
        {
            return menuScript.menuContainer.transform.GetChild(0).gameObject.activeSelf;
        }
        set { }
    }
}
