using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class PausePopup : Popup
{
    [SerializeField] private RectTransform titleRT;

    [SerializeField] private Button quitButton;
    [SerializeField] private Button replayButton;
    [SerializeField] private Button continueButton;
    
    [SerializeField] private RectTransform quitButtonRT;
    [SerializeField] private RectTransform replayButtonRT;
    [SerializeField] private RectTransform continueButtonRT;

    [SerializeField] private Button muteButton;
    [SerializeField] private Button themeButton;
    [SerializeField] private Button removeAdButton;

    [Space] [Header("TEXT")]
    [SerializeField] private TMP_Text quitText;
    [SerializeField] private TMP_Text replayText;
    [SerializeField] private TMP_Text continueText;

    [Space] [Header("REFERENCE")] [SerializeField]
    private GameManager gameManager;

    private void Start()
    {
        closeButton.onClick.AddListener(ClosePopup);

        quitButton.onClick.AddListener(QuitToHome);
        replayButton.onClick.AddListener(Replay);
        continueButton.onClick.AddListener(ClosePopup);

        muteButton.onClick.AddListener(Mute);

        InitUI();
    }

    private void InitUI()
    {
        SetUIElementSizeToParent(quitButtonRT, container, new Vector2(0.8f, 0.08f));
        SetUIElementSizeToParent(replayButtonRT, container, new Vector2(0.8f, 0.08f));
        SetUIElementSizeToParent(continueButtonRT, container, new Vector2(0.8f, 0.08f));

        closeButtonRT.sizeDelta = 0.1f * new Vector2(container.sizeDelta.x, container.sizeDelta.x);
        closeButtonRT.localPosition = new Vector3(0.45f * (container.sizeDelta.x - closeButtonRT.sizeDelta.x),
            0.45f * (container.sizeDelta.y - closeButtonRT.sizeDelta.x), 0);
        titleRT.localPosition = new Vector3(0, closeButtonRT.localPosition.y, 0);

        quitButtonRT.localPosition = new Vector3(0, 0.1f * container.sizeDelta.y, 0);
        replayButtonRT.localPosition =
            new Vector3(0, 0.1f * container.sizeDelta.y - 1 * 1.1f * quitButtonRT.sizeDelta.y, 0);
        continueButtonRT.localPosition =
            new Vector3(0, 0.1f * container.sizeDelta.y - 2 * 1.1f * quitButtonRT.sizeDelta.y, 0);

        SetTextSize(title, 0.1f);
        SetTextSize(quitText, 0.07f);
        SetTextSize(replayText, 0.07f);
        SetTextSize(continueText, 0.07f);
    }

    private void SetUIElementSizeToParent(RectTransform target, RectTransform parent, Vector2 proportional)
    {
        target.sizeDelta = new Vector2(proportional.x * parent.sizeDelta.x, proportional.y * parent.sizeDelta.y);
    }

    private void SetTextSize(TMP_Text text, float proportion)
    {
        text.fontSize = (int)(proportion * screenSize.x);
    }

    public override void ShowPopup()
    {
        base.ShowPopup();
        gameManager.Pause();
    }

    public override void ClosePopup()
    {
        base.ClosePopup();
        gameManager.UnPause();
    }

    private void QuitToHome()
    {
        Addressables.LoadSceneAsync("Menu");
    }

    private void Replay()
    {
        Addressables.LoadSceneAsync("Gameplay");
    }

    private void Continue()
    {
        gameObject.SetActive(false);
    }

    private void Mute()
    {
        AudioListener.pause = true;
    }
}