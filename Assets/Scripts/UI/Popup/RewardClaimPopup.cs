using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardClaimPopup : Popup
{
    [Header("UI")][SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text numGemRewardText;
    [SerializeField] private Image gemImage;
    [SerializeField] private Button x2Button;
    [SerializeField] private TMP_Text x2ButtonText;

    [Header("REFERENCE")]
    [SerializeField] DataManager dataManager;
    [SerializeField] AdManager adManager;

    [Header("EVENT")]
    [SerializeField] ScriptableEventNoParam onNumGemUpdatedEvent;

    private RectTransform _x2ButtonRT;

    private int _numGemClaim;

    protected override void InitUI()
    {
        base.InitUI();

        _x2ButtonRT = x2Button.GetComponent<RectTransform>();

        SetTextFontSize(description, 0.05f);
        SetTextFontSize(numGemRewardText, 0.07f);

        description.rectTransform.sizeDelta = new Vector2(description.preferredWidth, description.preferredHeight);
        description.rectTransform.localPosition = new Vector3(0, 0.1f * container.sizeDelta.y, 0);

        gemImage.rectTransform.sizeDelta =
            new Vector2(1.1f * numGemRewardText.preferredHeight, numGemRewardText.preferredHeight);

        SetSize(_x2ButtonRT, 0.37f * container.sizeDelta.x, (0.37f / 2.55f) * container.sizeDelta.x);
        SetLocalPositionY(_x2ButtonRT,
            numGemRewardText.rectTransform.localPosition.y - 1f * (numGemRewardText.preferredHeight + _x2ButtonRT.sizeDelta.y));
        SetTextFontSize(x2ButtonText, 0.05f);

        x2Button.onClick.AddListener(OnX2ButtonPressed);
    }

    public void ShowPopup(int numGemReward)
    {
        base.ShowPopup();

        SetNumGemClaimText(numGemReward);

        _numGemClaim = numGemReward;
    }

    private void SetNumGemClaimText(int numGemReward)
    {
        numGemRewardText.text = "+" + numGemReward;
        SetTextPreferredSize(numGemRewardText);

        numGemRewardText.rectTransform.localPosition = new Vector3(-0.05f * container.sizeDelta.x, 0, 0);
        gemImage.rectTransform.localPosition =
            new Vector3(
                numGemRewardText.rectTransform.localPosition.x +
                0.6f * (numGemRewardText.preferredWidth + gemImage.rectTransform.sizeDelta.x), 0, 0);
    }

    private void OnX2ButtonPressed()
    {
        if (dataManager.IsAdRemoved)
        {
            HandleOnRewardedAdCompleted();
            
            x2Button.gameObject.SetActive(false);
        }
        else
        {
            adManager.ShowRewardedAd(onRewardedAdCompleted: HandleOnRewardedAdCompleted);
        }
    }

    private void HandleOnRewardedAdCompleted()
    {
        dataManager.NumGem += _numGemClaim;
        dataManager.SaveIAPData();

        SetNumGemClaimText(2 * _numGemClaim);

        onNumGemUpdatedEvent.Raise();
    }

    private void HandleOnRewardedAdCompleted(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        HandleOnRewardedAdCompleted();
    }
}