using System;
using PrimeTween;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class PausePopup : Popup
{
    [SerializeField] private Button changeThemeBigButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button replayButton;
    [SerializeField] private Button continueButton;

    [SerializeField] private RectTransform changeThemeBigButtonRT;
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
    [SerializeField] TMP_Text themeText;
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
        closeButton.onClick.AddListener(ClosePopup);
        changeThemeBigButton.onClick.AddListener(ShowThemePickerPopup);
        quitButton.onClick.AddListener(QuitToHome);
        replayButton.onClick.AddListener(Replay);
        continueButton.onClick.AddListener(ClosePopup);
        muteButton.onClick.AddListener(Mute);
        themeButton.onClick.AddListener(ShowThemePickerPopup);
        removeAdButton.onClick.AddListener(OnRemoveAdButtonPressed);
    }

    protected override void InitUI()
    {
        base.InitUI();

        _muteButtonRT = muteButton.GetComponent<RectTransform>();
        _themeButtonRT = themeButton.GetComponent<RectTransform>();
        _removeAdButtonRT = removeAdButton.GetComponent<RectTransform>();

        SetUIElementSizeToParent(changeThemeBigButtonRT, container, new Vector2(0.8f, 0.08f));
        SetUIElementSizeToParent(quitButtonRT, container, new Vector2(0.8f, 0.08f));
        SetUIElementSizeToParent(replayButtonRT, container, new Vector2(0.8f, 0.08f));
        SetUIElementSizeToParent(continueButtonRT, container, new Vector2(0.8f, 0.08f));

        SetLocalPositionY(changeThemeBigButtonRT, 0.2f * container.sizeDelta.y);
        SetLocalPositionY(quitButtonRT, 0.2f * container.sizeDelta.y - 1 * 1.1f * quitButtonRT.sizeDelta.y);
        SetLocalPositionY(replayButtonRT, 0.2f * container.sizeDelta.y - 2 * 1.1f * quitButtonRT.sizeDelta.y);
        SetLocalPositionY(continueButtonRT, 0.2f * container.sizeDelta.y - 3 * 1.1f * quitButtonRT.sizeDelta.y);

        _muteButtonRT.sizeDelta = 0.15f * new Vector2(container.sizeDelta.x, container.sizeDelta.x);
        _themeButtonRT.sizeDelta = _muteButtonRT.sizeDelta;
        _removeAdButtonRT.sizeDelta = _muteButtonRT.sizeDelta;

        SetLocalPosition(_muteButtonRT, -1.1f * _muteButtonRT.sizeDelta.x, -0.3f * (container.sizeDelta.y - _muteButtonRT.sizeDelta.y));
        SetLocalPosition(_themeButtonRT, 0, _muteButtonRT.localPosition.y);
        SetLocalPosition(_removeAdButtonRT, 1.1f * _removeAdButtonRT.sizeDelta.x, _muteButtonRT.localPosition.y);

        SetTextFontSize(themeText, 0.07f, isSetPreferredSize: true);
        SetTextFontSize(quitText, 0.07f, isSetPreferredSize: true);
        SetTextFontSize(replayText, 0.07f, isSetPreferredSize: true);
        SetTextFontSize(continueText, 0.07f, isSetPreferredSize: true);
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
        Tween.Delay(0.1f).OnComplete(() => ClosePopupNotUnpause());
        themePickerPopup.ShowPopup();
    }

    private void OnRemoveAdButtonPressed()
    {
        AudioManager.Instance.PlayPopupSound();

        iapManager.BuyProducts(dataManager.ProductIds[0]);
    }
}