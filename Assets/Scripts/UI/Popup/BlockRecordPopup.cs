using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlockRecordPopup : Popup
{
    [SerializeField] private RectTransform innerContainer;
    [SerializeField] private Image blockImage;
    [SerializeField] private Image crownImage;

    [SerializeField] private TMP_Text blockText;
    
    [SerializeField] private Button X4Button;

    private RectTransform _x4ButtonRT;

    [Header("REFERENCE")] [SerializeField] private AdManager adManager;

    protected override void InitUI()
    {
        base.InitUI();
        
        _x4ButtonRT = X4Button.GetComponent<RectTransform>();

        container.GetComponent<Image>().color -= new Color(0, 0, 0, 0.02f);

        SetUIElementSizeToParent(innerContainer, container, new Vector2(0.8f, 0.4f));

        SetTextFontSize(title, 0.06f);
        title.rectTransform.sizeDelta = new Vector2(title.preferredWidth, title.preferredHeight);
        title.rectTransform.localPosition =
            new Vector3(0, 0.45f * (innerContainer.sizeDelta.y - title.preferredHeight), 0);
        
        SetTextFontSize(blockText, 0.06f);
        
        closeButtonRT.localPosition = new Vector3(0.45f * (innerContainer.sizeDelta.x - closeButtonRT.sizeDelta.x),
            title.rectTransform.localPosition.y, 0);

        blockImage.rectTransform.sizeDelta =
            0.25f * new Vector2(innerContainer.sizeDelta.x, innerContainer.sizeDelta.x);

        crownImage.rectTransform.sizeDelta =
            new Vector2(0.1f * innerContainer.sizeDelta.x, 0.08f * innerContainer.sizeDelta.y);

        crownImage.rectTransform.localPosition = new Vector3(0,
            0.55f * (blockImage.rectTransform.sizeDelta.y + crownImage.rectTransform.sizeDelta.y), 0);

        SetUIElementSizeToParent(_x4ButtonRT, innerContainer, new Vector2(0.5f, 0.1f));

        _x4ButtonRT.localPosition = new Vector3(0, -0.4f * (innerContainer.sizeDelta.y - _x4ButtonRT.sizeDelta.y), 0);

        X4Button.onClick.AddListener(ShowAdForMultiplyBlockValue);
    }

    public void ShowPopup(float blockNumber, char? blockLetter, int colorIndex)
    {
        blockImage.color = Constants.AllBlockColors[colorIndex];
        blockText.text = blockNumber.ToString("F0") + blockLetter;
        
        base.ShowPopup();
    }
    
    private void ShowAdForMultiplyBlockValue()
    {
        adManager.ShowRewardedAd(() => { Debug.Log("X4 Value"); });
    }
}