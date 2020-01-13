using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MenuDeveloper : MonoBehaviour
{
    private void OnEnable()
    {
        foreach(Text txt in GetComponentsInChildren<Text>())
        {
            if(txt.name.Equals("title"))
            {
                txt.color = PushCubeColor.ThemeMainColor;
                continue;
            }
            txt.color = PushCubeColor.ThemeTextColor;
        }
        foreach (Image img in GetComponentsInChildren<Image>())
        {
            if (img.name.Equals("logo")) continue;
            img.color = PushCubeColor.ThemeMainColor;
        }
    }
    public void OnClickBlog()
    {
        Application.OpenURL("http://blog.naver.com/pkh879");
    }
    public void OnClickWebsite()
    {
        Application.OpenURL("http://www.hobookgames.site");
    }
}
