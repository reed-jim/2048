using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;

public class SwapModePopup : Popup
{
    [SerializeField] private TMP_Text description;

    protected override void InitUI()
    {
        SetTextFontSize(title, 0.09f);
        SetTextFontSize(description, 0.05f);
        
        container.sizeDelta = new Vector2(screenSize.x, 0.35f * screenSize.y);

        title.rectTransform.sizeDelta = new Vector2(title.preferredWidth, title.preferredHeight);
        description.rectTransform.sizeDelta = new Vector2(description.preferredWidth, description.preferredHeight);
        
        title.rectTransform.localPosition = new Vector3(0, 0.3f * (container.sizeDelta.y - title.preferredHeight), 0);

        closeButtonRT.localPosition = new Vector3(0.45f * (container.sizeDelta.x - closeButtonRT.sizeDelta.x),
            title.rectTransform.localPosition.y, 0);

        description.rectTransform.localPosition = new Vector3(0,
            title.rectTransform.localPosition.y - 1f * (title.preferredHeight + description.preferredHeight), 0);

        container.gameObject.SetActive(false);
    }

    public override void ShowPopup()
    {
        container.anchoredPosition = new Vector2(0, -0.5f * (screenSize.y + container.sizeDelta.y));

        container.gameObject.SetActive(true);

        Tween.UIAnchoredPosition(container, new Vector2(0, -0.5f * (screenSize.y - container.sizeDelta.y)),
            duration: 0.4f);
    }

    public override void ClosePopup()
    {
        Tween.UIAnchoredPosition(container, new Vector2(0, -0.5f * (screenSize.y + container.sizeDelta.y)),
                duration: 0.4f)
            .OnComplete(() => container.gameObject.SetActive(false));
    }
}