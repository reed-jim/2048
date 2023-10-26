using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using PrimeTween;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "rewarded_ad_unit", menuName = "ScriptableObjects/RewaredAdUnit")]
public class RewardedAdUnit : ScriptableObject
{
    private int _retryAttempt;
    private string _adUnitId;

    public string AdUnitId { set => _adUnitId = value; }

    public delegate void OnRewardedAdCompleted();

    // private OnRewardedAdCompleted _onRewardedAdCompleted;

    private Action<string, MaxSdk.Reward, MaxSdkBase.AdInfo> _cachedOnRewardedAdCompleted;

    public void InitializeRewardedAds()
    {
        // Attach callback
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        // MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

        // Load the first rewarded ad
        LoadRewardedAd();
    }

    private void LoadRewardedAd()
    {
        MaxSdk.LoadRewardedAd(_adUnitId);
    }

    public void Show(Action<string, MaxSdk.Reward, MaxSdkBase.AdInfo> onRewardedAdCompleted)
    {
        Debug.Log("test50: " + (_cachedOnRewardedAdCompleted == null));
        if (_cachedOnRewardedAdCompleted == null)
        {
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += onRewardedAdCompleted;
        }
        else
        {
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent -= _cachedOnRewardedAdCompleted;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += onRewardedAdCompleted;
        }

        _cachedOnRewardedAdCompleted = onRewardedAdCompleted;
        Debug.Log("test51: " + _cachedOnRewardedAdCompleted.Method.Name);

        if (MaxSdk.IsRewardedAdReady(_adUnitId))
        {
            MaxSdk.ShowRewardedAd(_adUnitId);
        }
    }

    public void UnregisterEvent()
    {
        Debug.Log("test14");
        if (_cachedOnRewardedAdCompleted != null)
        {
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent -= _cachedOnRewardedAdCompleted;
        }
    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is ready for you to show. MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.

        // Reset retry attempt
        _retryAttempt = 0;
    }

    private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).

        _retryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, _retryAttempt));

        Tween.Delay((float)retryDelay).OnComplete(() => LoadRewardedAd());
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
        LoadRewardedAd();
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is hidden. Pre-load the next ad
        LoadRewardedAd();
    }

    // private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    // {
    //     Debug.Log("test6: " + _onRewardedAdCompleted.Method.Name);
    //     // The rewarded ad displayed and the user should receive the reward.
    //     _onRewardedAdCompleted();
    // }

    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Ad revenue paid. Use this callback to track user revenue.
    }
}
