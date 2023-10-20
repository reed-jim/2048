using System;
using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    [SerializeField] protected RectTransform container;
    [SerializeField] protected TMP_Text title;
    [SerializeField] protected Button closeButton;

    protected RectTransform closeButtonRT;

    protected Vector2 screenSize;

    private void Awake()
    {
        screenSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);

        closeButtonRT = closeButton.GetComponent<RectTransform>();

        closeButton.onClick.AddListener(ClosePopup);

        InitUI();
    }

    protected virtual void InitUI()
    {
        container.sizeDelta = screenSize;

        title.rectTransform.localPosition = new Vector3(0, 0.45f * (container.sizeDelta.y - title.preferredHeight), 0);

        closeButtonRT.localPosition = new Vector3(0.45f * (container.sizeDelta.x - closeButtonRT.sizeDelta.x),
            title.rectTransform.localPosition.y, 0);

        container.gameObject.SetActive(false);
    }

    public virtual void ShowPopup()
    {
        container.anchoredPosition = new Vector2(0, screenSize.y);

        container.gameObject.SetActive(true);

        Tween.UIAnchoredPosition(container, Vector2.zero, duration: 0.4f);
    }

    public virtual void ClosePopup()
    {
        Tween.UIAnchoredPosition(container, new Vector2(0, screenSize.y), duration: 0.4f)
            .OnComplete(() => container.gameObject.SetActive(false));
    }

    protected void SetTextFontSize(TMP_Text text, float proportion)
    {
        text.fontSize = proportion * screenSize.x;
    }
}