using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_NoAdButton : MonoBehaviour
{
    public Sprite alertSprite;
    private void Start()
    {
        if (PlayerData.isRemoveAd)
            this.gameObject.SetActive(false);
        else
            this.gameObject.SetActive(true);
    }
    public void OnClickButton()
    {
        if(!PlayerData.isRemoveAd)
        {
            GameSystemManager.ShowAlertMessage(new AlertMessage(0, string.Format("{0}", LocalizationManager.GetText("Alert_BuyRemoveAd")), alertSprite, null, PurchaseRemoveAd));
        }

    }

    void PurchaseRemoveAd(bool isYes)
    {
        if (isYes)
        {
            IAPManager.Instance.Purchase(0);
            if (PlayerData.isRemoveAd)
                this.gameObject.SetActive(false);
        }
        else
            Debug.Log("구매 거절");
    }
}
