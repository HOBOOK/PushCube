using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_GooglePlayButton : MonoBehaviour
{
    public void OnClickLeaderboard()
    {
        GooglePlayManager.Instance.OnShowLeaderBoard();
    }
    public void OnClickAchivement()
    {
        GooglePlayManager.Instance.OnShowAchievement();
    }
}
