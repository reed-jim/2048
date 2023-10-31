using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class PausePopup : Popup
{
    [SerializeField] private Button quitButton;
    [SerializeField] private Button replayButton;
    [SerializeField] private Button continueButton;

    [SerializeField] private RectTransform quitButtonRT;
    [SerializeField] private RectTransform replayButtonRT;
    [SerializeField] private RectTransform continueButtonRT;

    [SerializeField] private Button muteButton;
    [SerializeField] private Button themeButton;
    [SerializeField] private Button removeAdButton;

    private RectTransform _muteButtonRT;
    private RectTransform _themeButtonRT;
    private RectTransform _removeAdButtonRT;

    [Header("SPRITE")]
    [SerializeField] private Sprite muteSprite;
    [SerializeField] private Sprite unmuteSprite;

    [Header("TEXT")]
    [SerializeField]
    private TMP_Text quitText;

    [SerializeField] private TMP_Text replayText;
    [SerializeField] private TMP_Text continueText;

    [Header("REFERENCE")]
    [SerializeField] private DataManager dataManager;
    [SerializeField] private IAPManager iapManager;

    [Header("POPUP")]
    [SerializeField] private ThemePickerPopup themePickerPopup;

    private void Start()
    {
        _muteButtonRT = muteButton.GetComponent<RectTransform>();
        _themeButtonRT = themeButton.GetComponent<RectTransform>();
        _removeAdButtonRT = removeAdButton.GetComponent<RectTransform>();

        closeButton.onClick.AddListener(ClosePopup);
        quitButton.onClick.AddListener(QuitToHome);
        replayButton.onClick.AddListener(Replay);
        continueButton.onClick.AddListener(ClosePopup);
        muteButton.onClick.AddListener(Mute);
        themeButton.onClick.AddListener(ShowThemePickerPopup);
        removeAdButton.onClick.AddListener(OnRemoveAdButtonPressed);

        InitUI();
    }

    private void InitUI()
    {
        SetUIElementSizeToParent(quitButtonRT, container, new Vector2(0.8f, 0.08f));
        SetUIElementSizeToParent(replayButtonRT, container, new Vector2(0.8f, 0.08f));
        SetUIElementSizeToParent(continueButtonRT, container, new Vector2(0.8f, 0.08f));

        quitButtonRT.localPosition = new Vector3(0, 0.1f * container.sizeDelta.y, 0);
        replayButtonRT.localPosition =
            new Vector3(0, 0.1f * container.sizeDelta.y - 1 * 1.1f * quitButtonRT.sizeDelta.y, 0);
        continueButtonRT.localPosition =
            new Vector3(0, 0.1f * container.sizeDelta.y - 2 * 1.1f * quitButtonRT.sizeDelta.y, 0);

        _muteButtonRT.sizeDelta = 0.15f * new Vector2(container.sizeDelta.x, container.sizeDelta.x);
        _themeButtonRT.sizeDelta = _muteButtonRT.sizeDelta;
        _removeAdButtonRT.sizeDelta = _muteButtonRT.sizeDelta;

        _muteButtonRT.localPosition = new Vector3(-1.1f * _muteButtonRT.sizeDelta.x,
            -0.3f * (container.sizeDelta.y - _muteButtonRT.sizeDelta.y), 0);
        SetLocalPosition(_themeButtonRT, 0, _muteButtonRT.localPosition.y);
        SetLocalPosition(_removeAdButtonRT, 1.1f * _removeAdButtonRT.sizeDelta.x, _muteButtonRT.localPosition.y);

        SetTextSize(quitText, 0.07f);
        SetTextSize(replayText, 0.07f);
        SetTextSize(continueText, 0.07f);
    }

    private void SetTextSize(TMP_Text text, float proportion)
    {
        text.fontSize = (int)(proportion * screenSize.x);
    }

    private void QuitToHome()
    {
        Addressables.LoadSceneAsync("Menu");
    }

    private void Replay()
    {
        dataManager.ResetGameplayData();

        Addressables.LoadSceneAsync("Gameplay");
    }

    private void Mute()
    {
        muteButton.GetComponent<Image>().sprite = muteSprite;
        AudioListener.pause = true;

        muteButton.onClick.RemoveAllListeners();
        muteButton.onClick.AddListener(Unmute);
    }

    private void Unmute()
    {
        muteButton.GetComponent<Image>().sprite = unmuteSprite;
        AudioListener.pause = false;

        muteButton.onClick.RemoveAllListeners();
        muteButton.onClick.AddListener(Mute);
    }

    private void ShowThemePickerPopup()
    {
        themePickerPopup.ShowPopup();
    }

    private void OnRemoveAdButtonPressed()
    {
        AudioManager.Instance.PlayPopupSound();

        iapManager.BuyProducts(dataManager.ProductIds[0]);
    }
}