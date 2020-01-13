using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_GetGoldCubeButton : MonoBehaviour
{
    public Sprite alertSprite;
    public void OnClickButton()
    {
        GameSystemManager.ShowAlertMessage(new AlertMessage(0, string.Format("{0}",LocalizationManager.GetText("Alert_BuyGoldcube")), alertSprite,null, PurchaseGoldCube));
    }

    void PurchaseGoldCube(bool isYes)
    {
        if (isYes)
        {
            IAPManager.Instance.Purchase(1);
        }
        else
            Debug.Log("구매 거절");
    }
}
