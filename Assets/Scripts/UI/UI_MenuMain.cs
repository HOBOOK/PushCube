using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MenuMain : MonoBehaviour
{
    private void OnEnable()
    {
        foreach (Text txt in GetComponentsInChildren<Text>())
        {
            if (txt.name.Equals("title"))
            {
                txt.color = PushCubeColor.ThemeMainColor;
                continue;
            }
            txt.color = PushCubeColor.ThemeTextColor;
        }
    }
}
