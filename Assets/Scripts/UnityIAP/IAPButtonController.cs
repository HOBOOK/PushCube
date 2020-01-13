using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAPButtonController : MonoBehaviour
{
    private void Start()
    {
#if UNITY_ANDROID
        this.gameObject.SetActive(false);
#elif UNITY_IPHONE
        this.gameObject.SetActive(true);
#else
        this.gameObject.SetActive(false);
#endif
    }
    public void Restore()
    {
        SoundManager.PlaySound(SoundManager.Sound.MenuClick);
        IAPManager.Instance.Restore();
    }
}
