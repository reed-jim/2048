using System;
using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI")] [SerializeField] private RectTransform menu;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Button undoButton;
    [SerializeField] private Button changeColorButton;
    [SerializeField] private Button swapButton;

    [SerializeField] private RectTransform gameplayTopContainer;
    [SerializeField] private RectTransform gameplayBottomContainer;
    [SerializeField] private RectTransform bestScoreRT;
    [SerializeField] private RectTransform nextBlockImage;

    [SerializeField] private RectTransform gemContainer;
    [SerializeField] private RectTransform gemImageRT;
    [SerializeField] private RectTransform gemTextRT;

    [SerializeField] private RectTransform swapGemContainer;
    [SerializeField] private RectTransform changeColorGemContainer;
    [SerializeField] private RectTransform undoGemContainer;

    [SerializeField] private RectTransform watchAdButtonRT;
    private RectTransform _watchAdGemContainer;

    [Space] [Header("TMP_TEXT")] [SerializeField]
    private TMP_Text bestScoreText;

    [SerializeField] private TMP_Text menuText;
    [SerializeField] private TMP_Text nextBlockValueText;
    [SerializeField] private TMP_Text gemText;

    [Space] [Header("REFERENCE")] [SerializeField]
    private GameManager gameManager;

    [SerializeField] private Skill skillManager;

    private Vector2 _screenSize;

    private void Start()
    {
        _screenSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);

        menuButton.onClick.AddListener(OpenMenu);
        continueButton.onClick.AddListener(CloseMenu);
        undoButton.onClick.AddListener(gameManager.RevertMove);
        changeColorButton.onClick.AddListener(skillManager.ChangeColorSkill);
        swapButton.onClick.AddListener(gameManager.EnterSwapMode);

        InitUI();
    }

    private void SetUIElementSizeToParent(RectTransform target, RectTransform parent, Vector2 proportional)
    {
        target.sizeDelta = new Vector2(proportional.x * parent.sizeDelta.x, proportional.y * parent.sizeDelta.y);
    }

    private void SetTextSize(TMP_Text text, float proportion)
    {
        text.fontSize = (int)(proportion * _screenSize.x);
    }

    private void InitUI()
    {
        RectTransform swapButtonRT = swapButton.GetComponent<RectTransform>();
        RectTransform changeColorButtonRT = changeColorButton.GetComponent<RectTransform>();
        RectTransform undoButtonRT = undoButton.GetComponent<RectTransform>();

        gameplayTopContainer.sizeDelta = new Vector2(0.95f * _screenSize.x, 0.07f * _screenSize.y);
        gameplayBottomContainer.sizeDelta = new Vector2(0.95f * _screenSize.x, 0.3f * _screenSize.y);

        gameplayTopContainer.localPosition =
            new Vector3(0, 0.5f * (_screenSize.y - gameplayTopContainer.sizeDelta.y) - 0.025f * _screenSize.x, 0);
        gameplayBottomContainer.localPosition =
            new Vector3(0, -0.5f * (_screenSize.y - gameplayBottomContainer.sizeDelta.y) + 0.025f * _screenSize.x, 0);

        RectTransform menuButtonRT = menuButton.GetComponent<RectTransform>();

        menuButtonRT.sizeDelta =
            new Vector2(0.15f * gameplayTopContainer.sizeDelta.x, 0.8f * gameplayTopContainer.sizeDelta.y);
        menuButtonRT.localPosition =
            new Vector3(
                -0.5f * (gameplayTopContainer.sizeDelta.x - menuButtonRT.sizeDelta.x) +
                0.1f * gameplayTopContainer.sizeDelta.y, 0, 0);

        bestScoreRT.localPosition =
            new Vector3(
                0.5f * (gameplayTopContainer.sizeDelta.x - menuButtonRT.sizeDelta.x) -
                0.1f * gameplayTopContainer.sizeDelta.y, 0, 0);

        SetUIElementSizeToParent(swapButtonRT, gameplayBottomContainer, new Vector2(0.25f, 0.2f));
        SetUIElementSizeToParent(changeColorButtonRT, gameplayBottomContainer, new Vector2(0.35f, 0.2f));
        SetUIElementSizeToParent(undoButtonRT, gameplayBottomContainer, new Vector2(0.25f, 0.2f));

        swapButtonRT.localPosition =
            new Vector3(-0.55f * (changeColorButtonRT.sizeDelta.x + swapButtonRT.sizeDelta.x),
                changeColorButtonRT.localPosition.y, 0);
        undoButtonRT.localPosition =
            new Vector3(0.55f * (changeColorButtonRT.sizeDelta.x + undoButtonRT.sizeDelta.x),
                changeColorButtonRT.localPosition.y, 0);

        nextBlockImage.sizeDelta =
            0.2f * new Vector2(gameplayBottomContainer.sizeDelta.x, gameplayBottomContainer.sizeDelta.x);
        nextBlockImage.localPosition =
            new Vector3(0, 0.45f * (gameplayBottomContainer.sizeDelta.y - nextBlockImage.sizeDelta.y), 0);

        SetUIElementSizeToParent(gemContainer, gameplayBottomContainer, new Vector2(0.3f, 0.2f));
        gemContainer.localPosition =
            new Vector3(0.45f * (gameplayBottomContainer.sizeDelta.x - gemContainer.sizeDelta.x),
                nextBlockImage.localPosition.y, 0);

        gemImageRT.sizeDelta = 0.65f * new Vector2(gemContainer.sizeDelta.y, gemContainer.sizeDelta.y);
        gemImageRT.localPosition =
            new Vector3(-0.4f * (gemContainer.sizeDelta.x - gemImageRT.sizeDelta.x), 0, 0);

        SetUIElementSizeToParent(gemTextRT, gemContainer, new Vector2(0.6f, 0.9f));
        gemTextRT.localPosition =
            new Vector3(0.4f * (gemContainer.sizeDelta.x - gemTextRT.sizeDelta.x), 0, 0);
        SetTextSize(gemText, 0.05f);

        swapGemContainer = Instantiate(gemContainer, swapButtonRT);
        changeColorGemContainer = Instantiate(gemContainer, changeColorButtonRT);
        undoGemContainer = Instantiate(gemContainer, undoButtonRT);

        swapGemContainer.localScale *= 0.6f;
        changeColorGemContainer.localScale *= 0.6f;
        undoGemContainer.localScale *= 0.6f;

        swapGemContainer.localPosition =
            new Vector3(0, -0.4f * (swapButtonRT.sizeDelta.y + swapGemContainer.sizeDelta.y), 0);
        changeColorGemContainer.localPosition =
            new Vector3(0, -0.4f * (swapButtonRT.sizeDelta.y + swapGemContainer.sizeDelta.y), 0);
        undoGemContainer.localPosition =
            new Vector3(0, -0.4f * (swapButtonRT.sizeDelta.y + swapGemContainer.sizeDelta.y), 0);

        watchAdButtonRT.sizeDelta = 0.7f * nextBlockImage.sizeDelta;
        watchAdButtonRT.localPosition =
            new Vector3(-0.45f * (gameplayBottomContainer.sizeDelta.x - watchAdButtonRT.sizeDelta.x),
                nextBlockImage.localPosition.y, 0);

        _watchAdGemContainer = Instantiate(gemContainer, watchAdButtonRT);
        _watchAdGemContainer.localScale *= 0.6f;
        _watchAdGemContainer.localPosition =
            new Vector3(0, -0.3f * (_watchAdGemContainer.sizeDelta.y + swapGemContainer.sizeDelta.y), 0);

        SetTextSize(scoreText, 0.08f);
        SetTextSize(bestScoreText, 0.05f);
        SetTextSize(menuText, 0.05f);
        SetTextSize(nextBlockValueText, 0.07f);
    }

    private void OpenMenu()
    {
        menu.gameObject.SetActive(true);
    }

    private void CloseMenu()
    {
        menu.gameObject.SetActive(false);
    }

    public void SetScoreText(float scoreNumber, char? scoreLetter)
    {
        if (scoreLetter == null)
        {
            scoreText.text = scoreNumber.ToString();
        }
        else
        {
            scoreText.text = scoreNumber.ToString() + scoreLetter;
        }

        Tween.Scale(scoreText.transform, 1.1f, duration: 0.1f)
            .OnComplete(() => Tween.Scale(scoreText.transform, 1f, duration: 0.1f));
    }

    public void ShowLosePopup()
    {
    }
}