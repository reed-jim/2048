using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "banner_ad_unit", menuName = "ScriptableObjects/BannerAdUnit")]
public class BannerAdUnit : ScriptableObject
{
    private string _adUnitId;
    private bool _isAdLoaded;

    public string AdUnitId { set => _adUnitId = value; }

    [SerializeField] private ScriptableEventNoParam onBannerAdLoaded;

    public void InitializeBannerAds()
    {
        // Banners are automatically sized to 320×50 on phones and 728×90 on tablets
        // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
        MaxSdk.CreateBanner(_adUnitId, MaxSdkBase.BannerPosition.BottomCenter);

        // // Set background or background color for banners to be fully functional
        // MaxSdk.SetBannerBackgroundColor(bannerAdUnitId,  < YOUR_BANNER_BACKGROUND_COLOR >);

        MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdLoadFailedEvent;
        MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;
        MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnBannerAdExpandedEvent;
        MaxSdkCallbacks.Banner.OnAdCollapsedEvent += OnBannerAdCollapsedEvent;
    }

    public void Show()
    {
        if (_isAdLoaded)
        {
            MaxSdk.ShowBanner(_adUnitId);
        }
    }

    public void Hide()
    {
        if (_isAdLoaded)
        {
            MaxSdk.HideBanner(_adUnitId);
        }
    }

    public void Destroy()
    {
        if (_isAdLoaded)
        {
            MaxSdk.DestroyBanner(_adUnitId);
        }
    }

    private void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        _isAdLoaded = true;
        Show();
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
}
