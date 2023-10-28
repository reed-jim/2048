using System;
using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlockRecordPopup : Popup
{
    [Header("UI")]
    [SerializeField] private RectTransform innerContainer;
    [SerializeField] private Image blockImage;
    [SerializeField] private Image crownImage;

    [SerializeField] private TMP_Text blockText;

    [SerializeField] private Button X4Button;
    private RectTransform _x4ButtonRT;

    [Header("REFERENCE")]
    [SerializeField] private AdManager adManager;
    [SerializeField] private DataManager dataManager;

    public delegate void OnX2RewardedAdCompleted();

    protected override void InitUI()
    {
        base.InitUI();

        _x4ButtonRT = X4Button.GetComponent<RectTransform>();

        container.GetComponent<Image>().color -= new Color(0, 0, 0, 0.02f);

        SetUIElementSizeToParent(innerContainer, container, new Vector2(0.8f, 0.4f));

        SetTextFontSize(title, 0.06f);
        SetTextPreferredSize(title);
        SetLocalPositionY(title.rectTransform, 0.45f * (innerContainer.sizeDelta.y - title.preferredHeight));

        SetLocalPosition(closeButtonRT, 0.45f * (innerContainer.sizeDelta.x - closeButtonRT.sizeDelta.x),
            title.rectTransform.localPosition.y);

        SetTextFontSize(blockText, 0.06f);
        SetSquareSize(blockImage.rectTransform, 0.25f * innerContainer.sizeDelta.x);
        SetSize(crownImage.rectTransform, 0.1f * innerContainer.sizeDelta.x, 0.08f * innerContainer.sizeDelta.y);
        SetLocalPositionY(crownImage.rectTransform,
            0.5f * (blockImage.rectTransform.sizeDelta.y + crownImage.rectTransform.sizeDelta.y));

        SetUIElementSizeToParent(_x4ButtonRT, innerContainer, new Vector2(0.5f, 0.13f));
        SetLocalPositionY(_x4ButtonRT, -0.4f * (innerContainer.sizeDelta.y - _x4ButtonRT.sizeDelta.y));
    }

    public void ShowPopup(float blockNumber, char? blockLetter, int colorIndex)
    {
        blockImage.color = Constants.AllBlockColors[colorIndex];
        blockText.text = blockNumber.ToString("F0") + blockLetter;
        blockText.color = Constants.AllBlockTextColors[colorIndex];

        X4Button.onClick.RemoveAllListeners();
        X4Button.onClick.AddListener(() => ShowAdForMultiplyBlockValue());

        base.ShowPopup();
    }

    private void ShowAdForMultiplyBlockValue()
    {
        if (dataManager.IsAdRemoved)
        {
            gameManager.X2BlockValue();
        }
        else
        {
            adManager.ShowRewardedAd(onRewardedAdCompleted: gameManager.X2BlockValue);
        }
    }

    public void UpdateBestBlock(float blockNumber, char? blockLetter)
    {
        Tween.Scale(blockImage.rectTransform, 1.1f, duration: 0.2f)
            .OnComplete(() =>
            {
                blockText.text = blockNumber.ToString("F0") + blockLetter;
                Tween.Scale(blockImage.rectTransform, 1f, duration: 0.2f);
            });
    }
}