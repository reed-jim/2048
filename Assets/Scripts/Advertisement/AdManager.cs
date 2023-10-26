using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;

public class AdManager : MonoBehaviour
{
    [Header("AD UNIT")]
    [SerializeField] private InterstitialAdUnit interstitialAdUnit;
    [SerializeField] private RewardedAdUnit rewardedAdUnit;

    [Header("REFERENCE")][SerializeField] private DataManager dataManager;
    [SerializeField] private UIManager uiManager;

    public delegate void OnRewardedAdFinished();

    void Start()
    {
        SetAdUnitIds();

        if (!MaxSdk.IsInitialized())
        {
#if UNITY_EDITOR

#elif UNITY_ANDROID
            InitAppLovin();
#endif
        }
    }

    void OnDestroy()
    {
        rewardedAdUnit.UnregisterEvent();
    }

#if UNITY_IOS
string bannerAdUnitId = "YOUR_IOS_BANNER_AD_UNIT_ID"; // Retrieve the ID from your account
#else // UNITY_ANDROID
    string _bannerAdUnitId = "51a60ebef89540e0"; // Retrieve the ID from your account
    private string _interstitialAdUnitId = "1590d1c6dba78b2e";
    private string rewardedAdUnitId = "05cdd2745d999c36";
#endif

    private void SetAdUnitIds()
    {
        interstitialAdUnit.AdUnitId = _interstitialAdUnitId;
        rewardedAdUnit.AdUnitId = rewardedAdUnitId;
    }

    private void InitAppLovin()
    {
        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
        {
            InitializeBannerAds();
            interstitialAdUnit.InitializeInterstitialAds();
            rewardedAdUnit.InitializeRewardedAds();
        };

        MaxSdk.SetSdkKey("lIQyTJDgFJTFiih870xxIKPO6Wfu0KqlHCiw6g13ATdwaVPCnfQldwRQLI07awbBwN6_HT9KmzcKEcsOzM4IXL");
        // MaxSdk.SetUserId("USER_ID");
        MaxSdk.InitializeSdk();
    }

    public void InitializeBannerAds()
    {
        // Banners are automatically sized to 320×50 on phones and 728×90 on tablets
        // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
        MaxSdk.CreateBanner(_bannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);

        // // Set background or background color for banners to be fully functional
        // MaxSdk.SetBannerBackgroundColor(bannerAdUnitId,  < YOUR_BANNER_BACKGROUND_COLOR >);

        MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdLoadFailedEvent;
        MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;
        MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnBannerAdExpandedEvent;
        MaxSdkCallbacks.Banner.OnAdCollapsedEvent += OnBannerAdCollapsedEvent;
    }

    public void ShowInterstitialAd()
    {
        interstitialAdUnit.ShowInterstitialAd();
    }

    public void ShowRewardedAd(Action<string, MaxSdk.Reward, MaxSdkBase.AdInfo> onRewardedAdCompleted)
    {
        rewardedAdUnit.Show(onRewardedAdCompleted);
    }

    private void ShowBannerAd()
    {
        MaxSdk.ShowBanner(_bannerAdUnitId);
    }

    private void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        ShowBannerAd();
    }

    private void OnBannerAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
    }

    private void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
    }

    private void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
    }

    private void OnBannerAdExpandedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
    }

    private void OnBannerAdCollapsedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
    }

    //
    // #if UNITY_ANDROID
    //     private string _bannerUnitId = "ca-app-pub-3940256099942544/6300978111";
    // #elif UNITY_IPHONE
    //   private string _bannerUnitId = "ca-app-pub-3940256099942544/2934735716";
    // #else
    //   private string _bannerUnitId = "unused";
    // #endif
    //
    // #if UNITY_ANDROID
    //     private string _rewardedUnitId = "ca-app-pub-3940256099942544/5224354917";
    // #elif UNITY_IPHONE
    //   private string _rewardedUnitId = "ca-app-pub-3940256099942544/1712485313";
    // #else
    //   private string _rewardedUnitId = "unused";
    // #endif
    //
    // #if UNITY_ANDROID
    //     private string _interstitialUnitId = "ca-app-pub-3940256099942544/1033173712";
    // #elif UNITY_IPHONE
    //   private string _interstitialUnitId = "ca-app-pub-3940256099942544/4411468910";
    // #else
    //   private string _interstitialUnitId = "unused";
    // #endif
    //
    //     public void CreateBannerView()
    //     {
    //         if (_bannerView != null)
    //         {
    //             DestroyBannerView();
    //         }
    //
    //         _bannerView = new BannerView(_bannerUnitId, AdSize.Banner, AdPosition.Bottom);
    //
    //         ListenToAdEvents();
    //     }
    //
    //     public void LoadAd()
    //     {
    //         if (_bannerView == null)
    //         {
    //             CreateBannerView();
    //         }
    //
    //         var adRequest = new AdRequest();
    //
    //         _bannerView.LoadAd(adRequest);
    //     }
    //
    //     public void DestroyBannerView()
    //     {
    //         if (_bannerView != null)
    //         {
    //             _bannerView.Destroy();
    //             _bannerView = null;
    //         }
    //     }
    //
    //     private void ListenToAdEvents()
    //     {
    //         // Raised when an ad is loaded into the banner view.
    //         _bannerView.OnBannerAdLoaded += () => { };
    //         // Raised when an ad fails to load into the banner view.
    //         _bannerView.OnBannerAdLoadFailed += (LoadAdError error) => { };
    //         // Raised when the ad is estimated to have earned money.
    //         _bannerView.OnAdPaid += (AdValue adValue) => { };
    //         // Raised when an impression is recorded for an ad.
    //         _bannerView.OnAdImpressionRecorded += () => { };
    //         // Raised when a click is recorded for an ad.
    //         _bannerView.OnAdClicked += () => { };
    //         // Raised when an ad opened full screen content.
    //         _bannerView.OnAdFullScreenContentOpened += () => { };
    //         // Raised when the ad closed full screen content.
    //         _bannerView.OnAdFullScreenContentClosed += () => { };
    //     }
    //
    //     public void LoadRewardedAd()
    //     {
    //         if (_rewardedAd != null)
    //         {
    //             _rewardedAd.Destroy();
    //             _rewardedAd = null;
    //         }
    //
    //         var adRequest = new AdRequest();
    //
    //         RewardedAd.Load(_rewardedUnitId, adRequest,
    //             (RewardedAd ad, LoadAdError error) =>
    //             {
    //                 if (error != null || ad == null)
    //                 {
    //                     return;
    //                 }
    //
    //                 Debug.Log("test0: rewarded ad " + error);
    //                 _rewardedAd = ad;
    //
    //                 RegisterEventHandlers(_rewardedAd);
    //                 RegisterReloadHandler(_rewardedAd);
    //             });
    //
    //         Debug.Log("test01: rewarded ad " + _rewardedAd);
    //     }
    //
    //     public void ShowRewardedAd(OnRewardedAdFinished onRewardedAdFinished)
    //     {
    //         Debug.Log("test  " + _rewardedAd != null + " / " + _rewardedAd.CanShowAd());
    //         if (_rewardedAd != null && _rewardedAd.CanShowAd())
    //         {
    //             _rewardedAd.Show((Reward reward) =>
    //             {
    //                 // TODO: Reward the user.
    //                 onRewardedAdFinished();
    //             });
    //         }
    //     }
    //
    //     private void RegisterEventHandlers(RewardedAd ad)
    //     {
    //         // Raised when the ad is estimated to have earned money.
    //         ad.OnAdPaid += (AdValue adValue) => { };
    //         // Raised when an impression is recorded for an ad.
    //         ad.OnAdImpressionRecorded += () => { };
    //         // Raised when a click is recorded for an ad.
    //         ad.OnAdClicked += () => { };
    //         // Raised when an ad opened full screen content.
    //         ad.OnAdFullScreenContentOpened += () => { };
    //         // Raised when the ad closed full screen content.
    //         ad.OnAdFullScreenContentClosed += () => { };
    //         // Raised when the ad failed to open full screen content.
    //         ad.OnAdFullScreenContentFailed += (AdError error) => { };
    //     }
    //
    //     private void RegisterReloadHandler(RewardedAd ad)
    //     {
    //         // Raised when the ad closed full screen content.
    //         ad.OnAdFullScreenContentClosed += () =>
    //         {
    //             // Reload the ad so that we can show another as soon as possible.
    //             LoadRewardedAd();
    //         };
    //         // Raised when the ad failed to open full screen content.
    //         ad.OnAdFullScreenContentFailed += (AdError error) =>
    //         {
    //             // Reload the ad so that we can show another as soon as possible.
    //             LoadRewardedAd();
    //         };
    //     }
    //
    //     public void LoadInterstitialAd()
    //     {
    //         if (_interstitialAd != null)
    //         {
    //             _interstitialAd.Destroy();
    //             _interstitialAd = null;
    //         }
    //
    //         var adRequest = new AdRequest();
    //
    //         InterstitialAd.Load(_interstitialUnitId, adRequest,
    //             (InterstitialAd ad, LoadAdError error) =>
    //             {
    //                 if (error != null || ad == null)
    //                 {
    //                     return;
    //                 }
    //
    //                 _interstitialAd = ad;
    //
    //                 RegisterReloadHandler(_interstitialAd);
    //             });
    //     }
    //
    //     public void ShowInterstitialAd()
    //     {
    //         if (_interstitialAd != null && _interstitialAd.CanShowAd())
    //         {
    //             _interstitialAd.Show();
    //         }
    //         else
    //         {
    //         }
    //     }
    //
    //     private void RegisterReloadHandler(InterstitialAd interstitialAd)
    //     {
    //         // Raised when the ad closed full screen content.
    //         interstitialAd.OnAdFullScreenContentClosed += () =>
    //         {
    //             // Reload the ad so that we can show another as soon as possible.
    //             LoadInterstitialAd();
    //         };
    //         // Raised when the ad failed to open full screen content.
    //         interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
    //         {
    //             // Reload the ad so that we can show another as soon as possible.
    //             LoadInterstitialAd();
    //         };
    //     }
}