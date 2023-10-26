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

    [Header("REFERENCE")] [SerializeField] protected GameManager gameManager;

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

        SetTextFontSize(title, 0.08f);
        title.text = title.text.ToUpper();

        title.rectTransform.sizeDelta = new Vector2(title.preferredWidth, title.preferredHeight);
        title.rectTransform.localPosition = new Vector3(0, 0.45f * (container.sizeDelta.y - title.preferredHeight), 0);

        closeButtonRT.localPosition = new Vector3(0.45f * (container.sizeDelta.x - closeButtonRT.sizeDelta.x),
            title.rectTransform.localPosition.y, 0);

        container.gameObject.SetActive(false);
    }

    public virtual void ShowPopup()
    {
        if (gameManager != null) gameManager.Pause();

        container.anchoredPosition = new Vector2(0, screenSize.y);

        container.gameObject.SetActive(true);

        Tween.UIAnchoredPosition(container, Vector2.zero, duration: 0.4f);
        // Tween.UIAnchoredPosition(container, new Vector2(0, (Screen.safeArea.height - Screen.currentResolution.height) / 2), duration: 0.4f);
    }

    public virtual void ClosePopup()
    {
        if (gameManager != null) gameManager.UnPause();

        Tween.UIAnchoredPosition(container, new Vector2(0, screenSize.y), duration: 0.4f)
            .OnComplete(() => container.gameObject.SetActive(false));
    }

    protected void SetUIElementSizeToParent(RectTransform target, RectTransform parent, Vector2 proportional)
    {
        target.sizeDelta = new Vector2(proportional.x * parent.sizeDelta.x, proportional.y * parent.sizeDelta.y);
    }

    protected void SetTextFontSize(TMP_Text text, float proportion)
    {
        text.fontSize = proportion * screenSize.x;
    }

    protected void SetTextPreferredSize(TMP_Text text)
    {
        text.rectTransform.sizeDelta = new Vector2(text.preferredWidth, text.preferredHeight);
    }

    protected void SetSize(RectTransform target, float width, float height)
    {
        target.sizeDelta = new Vector2(width, height);
    }
    
    protected void SetSquareSize(RectTransform target, float width)
    {
        SetSize(target, width, width);
    }

    protected void SetLocalPosition(RectTransform target, float x, float y)
    {
        target.localPosition = new Vector3(x, y, 0);
    }

    protected void SetLocalPositionX(RectTransform target, float x)
    {
        SetLocalPosition(target, x, 0);
    }

    protected void SetLocalPositionY(RectTransform target, float y)
    {
        SetLocalPosition(target, 0, y);
    }
}