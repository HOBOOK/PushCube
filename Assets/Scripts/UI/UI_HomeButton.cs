using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_HomeButton : MonoBehaviour
{
    public void OnClickButton()
    {
        SceneManager.LoadSceneAsync(1);
    }
}
