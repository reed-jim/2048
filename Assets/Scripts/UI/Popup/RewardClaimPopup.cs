using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardClaimPopup : Popup
{
    [Header("UI")] [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text numGemRewardText;
    [SerializeField] private Image gemImage;

    protected override void InitUI()
    {
        base.InitUI();

        SetTextFontSize(description, 0.05f);
        SetTextFontSize(numGemRewardText, 0.07f);

        description.rectTransform.sizeDelta = new Vector2(description.preferredWidth, description.preferredHeight);
        description.rectTransform.localPosition = new Vector3(0, 0.1f * container.sizeDelta.y, 0);

        gemImage.rectTransform.sizeDelta =
            new Vector2(1.1f * numGemRewardText.preferredHeight, numGemRewardText.preferredHeight);
    }

    public void ShowPopup(int numGemReward)
    {
        base.ShowPopup();

        numGemRewardText.text = "+" + numGemReward;
        numGemRewardText.rectTransform.sizeDelta =
            new Vector2(numGemRewardText.preferredWidth, numGemRewardText.preferredHeight);

        numGemRewardText.rectTransform.localPosition = new Vector3(-0.05f * container.sizeDelta.x, 0, 0);
        gemImage.rectTransform.localPosition =
            new Vector3(
                numGemRewardText.rectTransform.localPosition.x +
                0.6f * (numGemRewardText.preferredWidth + gemImage.rectTransform.sizeDelta.x), 0, 0);
    }
}