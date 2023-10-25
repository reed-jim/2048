using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPopup : Popup
{
    [SerializeField] private TMP_Text description;
    [SerializeField] private Slider loadingBar;
    private RectTransform _loadingBarRT;

    protected override void InitUI()
    {
        base.InitUI();
        
        _loadingBarRT = loadingBar.GetComponent<RectTransform>();

        SetTextFontSize(description, 0.05f);

        _loadingBarRT.sizeDelta = new Vector2(0.7f * container.sizeDelta.x, 0.02f * container.sizeDelta.y);
        _loadingBarRT.localPosition = new Vector3(0, -0.4f * (container.sizeDelta.y - _loadingBarRT.sizeDelta.y), 0);

        description.rectTransform.sizeDelta = new Vector2(description.preferredWidth, description.preferredHeight);
        description.rectTransform.localPosition = new Vector3(0,
            _loadingBarRT.localPosition.y + 0.7f * (_loadingBarRT.sizeDelta.y + description.preferredHeight), 0);
        
        closeButton.gameObject.SetActive(false);
    }

    public override void ShowPopup()
    {
        container.anchoredPosition = new Vector2(0, screenSize.y);

        container.gameObject.SetActive(true);

        Tween.UIAnchoredPosition(container, Vector2.zero, duration: 0.3f)
            .OnComplete(() => OnShowPopupCompleted());
    }

    public override void ClosePopup()
    {
        Tween.UIAnchoredPosition(container, new Vector2(0, -0.5f * (screenSize.y + container.sizeDelta.y)),
                duration: 0.4f)
            .OnComplete(() => container.gameObject.SetActive(false));
    }

    private void OnShowPopupCompleted()
    {
        Tween.Custom(loadingBar.value, 1, duration: 1f, onValueChange: newVal => loadingBar.value = newVal);
    }
}