using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdMobManager : MonoBehaviour
{
    private BannerView banner;
    private InterstitialAd interstitial;

    private int interstitialCount;
    public bool IsTest;
    public static AdMobManager Instance;

    private void Awake()
    {
        Instance = this;
#if UNITY_ANDROID
        string appId = "ca-app-pub-3940256099942544~3347511713";
#elif UNITY_IPHONE
            string appId = "ca-app-pub-1654715490901132~3803966061";
#else
            string appId = "unexpected_platform";
#endif
        MobileAds.Initialize(appId);
        this.RequestBanner();
        this.RequestInterstitial();
        GameSystemManager.Instance.isAdMobLoad = true;

    }
    public void StartAdMob()
    {
        if (!PlayerData.isRemoveAd)
        {
            interstitialCount = 0;
            ShowBanner();
        }
        else
            HideBanner();
    }

    #region 배너 광고
    private void RequestBanner()
    {
#if UNITY_ANDROID
        string AdUnitID = IsTest ? "ca-app-pub-3940256099942544/6300978111" : "ca-app-pub-1654715490901132/9895312291";
#else
        string AdUnitID = IsTest ? "ca-app-pub-3940256099942544/2934735716" : "ca-app-pub-1654715490901132/4401079474";
#endif

        banner = new BannerView(AdUnitID, AdSize.SmartBanner, AdPosition.Top);

        //// Called when an ad request has successfully loaded.
        //banner.OnAdLoaded += HandleOnAdLoaded_banner;
        //// Called when an ad request failed to load.
        //banner.OnAdFailedToLoad += HandleOnAdFailedToLoad_banner;
        //// Called when an ad is clicked.
        //banner.OnAdOpening += HandleOnAdOpened_banner;
        //// Called when the user returned from the app after an ad click.
        //banner.OnAdClosed += HandleOnAdClosed_banner;
        //// Called when the ad click caused the user to leave the application.
        //banner.OnAdLeavingApplication += HandleOnAdLeavingApplication_banner;

        AdRequest request = new AdRequest.Builder().Build();
        banner.LoadAd(request);
        HideBanner();
    }

    public void ShowBanner()
    {
        if(!PlayerData.isRemoveAd)
            banner.Show();
    }

    public void HideBanner()
    {
        banner.Hide();
    }
    #endregion

    #region 전면 광고
    private void RequestInterstitial()
    {
#if UNITY_ANDROID
        string adUnitId = IsTest ? "ca-app-pub-3940256099942544/1033173712" : "ca-app-pub-1654715490901132/5545289155";
#elif UNITY_IPHONE
        string adUnitId = IsTest ? "ca-app-pub-3940256099942544/4411468910" : "ca-app-pub-1654715490901132/1092389854";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Initialize an InterstitialAd.
        this.interstitial = new InterstitialAd(adUnitId);
        // 전면광고
        //// Called when an ad request has successfully loaded.
        //this.interstitial.OnAdLoaded += HandleOnAdLoaded;
        //// Called when an ad request failed to load.
        //this.interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        //// Called when an ad is shown.
        //this.interstitial.OnAdOpening += HandleOnAdOpened;
        //// Called when the ad is closed.
        //this.interstitial.OnAdClosed += HandleOnAdClosed;
        //// Called when the ad click caused the user to leave the application.
        //this.interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);
    }
    public void ShowInterstitial()
    {
        if (PlayerData.isRemoveAd)
            return;
        if (interstitialCount > 0)
        {
            interstitialCount--;
            return;
        }
        interstitialCount = 2;
        if (this.interstitial.IsLoaded())
        {

            this.interstitial.Show();
        }
        else
        {
            Debug.Log("NOT Loaded Interstitial");
            RequestInterstitial();
        }
    }
    #endregion

    #region 이벤트 함수
    public void HandleOnAdLoaded_banner(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received_banner");
    }

    public void HandleOnAdFailedToLoad_banner(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("HandleFailedToReceiveAd_banner event received with message: "
                            + args.Message);
    }

    public void HandleOnAdOpened_banner(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received_banner");
    }

    public void HandleOnAdClosed_banner(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received_banner");
    }

    public void HandleOnAdLeavingApplication_banner(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLeavingApplication event received_banner");
    }
    #endregion
}
