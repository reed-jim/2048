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

    [Header("REFERENCE")] [SerializeField] private GameManager gameManager;

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

        SetTextFontSize(title, 0.1f);
        title.text = title.text.ToUpper();

        title.rectTransform.sizeDelta = new Vector2(title.preferredWidth, title.preferredHeight);
        title.rectTransform.localPosition = new Vector3(0, 0.45f * (container.sizeDelta.y - title.preferredHeight), 0);

        closeButtonRT.localPosition = new Vector3(0.45f * (container.sizeDelta.x - closeButtonRT.sizeDelta.x),
            title.rectTransform.localPosition.y, 0);
     
        container.gameObject.SetActive(false);
    }

    public virtual void ShowPopup()
    {
        gameManager.Pause();

        container.anchoredPosition = new Vector2(0, screenSize.y);

        container.gameObject.SetActive(true);
 
        Tween.UIAnchoredPosition(container, Vector2.zero, duration: 0.4f);
        // Tween.UIAnchoredPosition(container, new Vector2(0, (Screen.safeArea.height - Screen.currentResolution.height) / 2), duration: 0.4f);
    }

    public virtual void ClosePopup()
    {
        gameManager.UnPause();

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
}