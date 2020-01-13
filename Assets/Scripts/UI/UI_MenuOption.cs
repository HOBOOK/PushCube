using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MenuOption : MonoBehaviour
{
    public Transform MenuContainerTransform;
    public Transform MenuBackgroundTransform;
    public Transform MenuButtonTransform;
    public Text titleText; 
    public Text bgmOptionText;
    public Text efxOptionText;
    public Text vibrateOptionText;
    public List<Text> languageTextList;
    public List<Text> themTextList;

    bool isClicking = false;

    private void OnEnable()
    {
        RefreshUI();
    }
    void RefreshUI()
    {
        MenuButtonTransform.GetComponent<Image>().color = PushCubeColor.ThemeMainColor;
        MenuBackgroundTransform.GetComponent<Image>().color = PushCubeColor.ThemeBackColor;
        titleText.color = PushCubeColor.ThemeMainColor;
        bgmOptionText.color = PushCubeColor.ThemeTextColor;
        efxOptionText.color = PushCubeColor.ThemeTextColor;
        vibrateOptionText.color = PushCubeColor.ThemeTextColor;
        bgmOptionText.text = ConfigurationData.bgmSound ? LocalizationManager.GetText("StageScene_MenuOption_Bgm") + "  On" : LocalizationManager.GetText("StageScene_MenuOption_Bgm") + "<color='grey'>  Off</color>";
        efxOptionText.text = ConfigurationData.efxSound ? LocalizationManager.GetText("StageScene_MenuOption_Efx") + "  On" : LocalizationManager.GetText("StageScene_MenuOption_Efx") + "<color='grey'>  Off</color>";
        vibrateOptionText.text = ConfigurationData.vibrate ? LocalizationManager.GetText("StageScene_MenuOption_Vibrate") + "  On" : LocalizationManager.GetText("StageScene_MenuOption_Vibrate") + "<color='grey'>  Off</color>";
        for (int i = 0; i < languageTextList.Count; i++)
        {
            if (i == ConfigurationData.language)
            {
                languageTextList[i].color = PushCubeColor.ThemeTextColor;
                continue;
            }
            languageTextList[i].color = Color.gray;
        }
        for (int i = 0; i < themTextList.Count; i++)
        {
            if (i == ConfigurationData.theme)
            {
                themTextList[i].color = PushCubeColor.ThemeTextColor;
                continue;
            }
            themTextList[i].color = Color.gray;
        }
    }
    public void OnClickBgmSound()
    {
        if (!isClicking)
        {
            SoundManager.PlaySound(SoundManager.Sound.MenuClick);
            StartCoroutine("ClickDelay");
            ConfigurationData.bgmSound = !ConfigurationData.bgmSound;
            ConfigurationDataSystem.Save();
            SoundManager.RePlayLoopSound();
            RefreshUI();
        }
    }
    public void OnClickEfxSound()
    {
        if (!isClicking)
        {
            SoundManager.PlaySound(SoundManager.Sound.MenuClick);
            StartCoroutine("ClickDelay");
            ConfigurationData.efxSound = !ConfigurationData.efxSound;
            ConfigurationDataSystem.Save();
            RefreshUI();
        }
    }
    public void OnClickVibrate()
    {
        if (!isClicking)
        {
            SoundManager.PlaySound(SoundManager.Sound.MenuClick);
            StartCoroutine("ClickDelay");
            ConfigurationData.vibrate = !ConfigurationData.vibrate;
            ConfigurationDataSystem.Save();
            RefreshUI();
        }
    }
    public void OnClickLanguage(int language)
    {
        if(!isClicking)
        {
            SoundManager.PlaySound(SoundManager.Sound.MenuClick);
            StartCoroutine("ClickDelay");
            ConfigurationData.language = language;
            ConfigurationDataSystem.Save();
            LocalizationManager.LoadLanguage(ConfigurationData.language);
            LocalizationManager.RedrawLanguage();
            Debug.Log(ConfigurationData.language);
            RefreshUI();
            GameManager.instance.UpdateUI();
        }
    }

    public void OnClickTheme(int theme)
    {
        SoundManager.PlaySound(SoundManager.Sound.MenuClick);
        ConfigurationData.theme = theme;
        ConfigurationDataSystem.Save();
        RefreshUI();
    }

    IEnumerator ClickDelay()
    {
        isClicking = true;
        yield return new WaitForSeconds(.3f);
        isClicking = false;
    }
}
