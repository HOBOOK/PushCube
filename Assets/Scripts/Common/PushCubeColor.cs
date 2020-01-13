using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushCubeColor
{
    public static Color32 ThemeMainColor
    {
        get
        {
            switch(ConfigurationData.theme)
            {
                default:
                    return Color.white;
                case 0:
                    return Color.white;
                case 1:
                    return Color.black;
            }
        }
    }
    public static Color32 ThemeBackColor
    {
        get
        {
            switch (ConfigurationData.theme)
            {
                default:
                    return Color.white;
                case 0:
                    return new Color32(40, 40, 40, 240);
                case 1:
                    return new Color32(230, 230, 230, 240);
            }
        }
    }
    public static Color32 ThemeTextColor
    {
        get
        {
            switch (ConfigurationData.theme)
            {
                default:
                    return Color.white;
                case 0:
                    return new Color32(200, 200, 200, 255);
                case 1:
                    return new Color32(40, 40, 40, 255);
            }
        }
    }

    public static Color32 SlotSelectColor
    {
        get
        {
            return new Color(1, 1, 0, 0.5f);
        }
    }
    public static Color32 SlotUnableColor
    {
        get
        {
            return new Color(0, 1, 0, 0.5f);
        }
    }
    public static Color32 SlotColor
    {
        get
        {
            return new Color(0, 0, 0, 0.196f);
        }
    }
}
