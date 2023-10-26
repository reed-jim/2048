using System;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("PREFAB")][SerializeField] private RectTransform gemContainerPrefab;

    [Header("UI")][SerializeField] private Button menuButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Button undoButton;
    [SerializeField] private Button changeColorButton;
    [SerializeField] private Button swapButton;
    [SerializeField] private Button watchAdForGemButton;
    [SerializeField] private Button gemAddButton;

    [SerializeField] private RectTransform undoButtonRT;
    [SerializeField] private RectTransform changeColorButtonRT;
    [SerializeField] private RectTransform swapButtonRT;

    [SerializeField] private RectTransform gameplayTopContainer;
    [SerializeField] private RectTransform gameplayBottomContainer;
    [SerializeField] private RectTransform topOutsideSafeAreaFill;
    [SerializeField] private RectTransform bestScoreRT;
    [SerializeField] private RectTransform bestScoreImage;
    [SerializeField] private RectTransform nextBlockImage;

    [SerializeField] private RectTransform gemContainer;
    [SerializeField] private RectTransform gemImageRT;
    [SerializeField] private RectTransform gemTextRT;
    [SerializeField] private RectTransform gemAddButtonRT;

    [SerializeField] private RectTransform swapGemContainer;
    [SerializeField] private RectTransform changeColorGemContainer;
    [SerializeField] private RectTransform undoGemContainer;

    [SerializeField] private RectTransform watchAdButtonRT;
    private RectTransform _watchAdGemContainer;

    [Header("POPUP")][SerializeField] private ShopPopup shopPopup;
    [SerializeField] private PausePopup pausePopup;
    [SerializeField] public SwapModePopup swapModePopup;
    [SerializeField] public BlockRecordPopup blockRecordPopup;

    [Space]
    [Header("TMP_TEXT")]
    [SerializeField]
    private TMP_Text bestScoreText;

    [SerializeField] private TMP_Text menuText;
    [SerializeField] private TMP_Text nextBlockValueText;
    [SerializeField] private TMP_Text gemText;

    [SerializeField] private TMP_Text swapText;
    [SerializeField] private TMP_Text changeColorText;
    [SerializeField] private TMP_Text undoText;

    [Space]
    [Header("REFERENCE")]
    [SerializeField]
    private AdManager adManager;

    [SerializeField] private DataManager dataManager;
    [SerializeField] private Skill skillManager;

    private Vector2 _screenSize;
    private Vector2 _safeAreaSize;
    private float _safeAreaBottomOffset;
    private float _safeAreaTopSizeY;
    private float _safeAreaSizeYProportionToScreen;

    public float SafeAreaTopSizeY
    {
        get => _safeAreaTopSizeY;
    }

    private void Start()
    {
        _screenSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
        _safeAreaSize = new Vector2(Screen.safeArea.width, Screen.safeArea.height);
        _safeAreaBottomOffset = Screen.safeArea.position.y;
        _safeAreaTopSizeY = _screenSize.y - _safeAreaSize.y - _safeAreaBottomOffset;

        menuButton.onClick.AddListener(OpenMenu);
        continueButton.onClick.AddListener(CloseMenu);
        gemAddButton.onClick.AddListener(OpenShopPopup);
        undoButton.onClick.AddListener(skillManager.UndoSkill);
        changeColorButton.onClick.AddListener(skillManager.ChangeColorSkill);
        swapButton.onClick.AddListener(OnSwapClicked);
        watchAdForGemButton.onClick.AddListener(PlayAdForGem);

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
        RectTransform menuButtonRT = menuButton.GetComponent<RectTransform>();

        gameplayTopContainer.sizeDelta = new Vector2(1f * _screenSize.x, 0.12f * _screenSize.y);
        gameplayBottomContainer.sizeDelta = new Vector2(1f * _screenSize.x, 0.35f * _screenSize.y);

        gameplayTopContainer.localPosition =
            new Vector3(0, 0.5f * (_screenSize.y - gameplayTopContainer.sizeDelta.y) - _safeAreaTopSizeY, 0);
        gameplayBottomContainer.localPosition =
            new Vector3(0, -0.5f * (_screenSize.y - gameplayBottomContainer.sizeDelta.y), 0);

        topOutsideSafeAreaFill.sizeDelta = new Vector2(_screenSize.x, _safeAreaTopSizeY);
        topOutsideSafeAreaFill.localPosition = new Vector3(0,
            gameplayTopContainer.localPosition.y +
            0.5f * (gameplayTopContainer.sizeDelta.y + topOutsideSafeAreaFill.sizeDelta.y), 0);

        // GEM CONTAINER
        SetUIElementSizeToParent(gemContainer, gameplayTopContainer, new Vector2(0.33f, 0.4f));
        gemContainer.localPosition =
            new Vector3(
                -0.5f * (gameplayTopContainer.sizeDelta.x - gemContainer.sizeDelta.x) +
                0.1f * gameplayTopContainer.sizeDelta.y,
                0.3f * (gameplayTopContainer.sizeDelta.y - gemContainer.sizeDelta.y),
                0);

        gemImageRT.sizeDelta = 0.6f * new Vector2(gemContainer.sizeDelta.y, gemContainer.sizeDelta.y);
        gemImageRT.localPosition =
            new Vector3(-0.4f * (gemContainer.sizeDelta.x - gemImageRT.sizeDelta.x), 0, 0);

        gemText.text = Utils.ToAbbreviatedNumber(dataManager.NumGem);
        gemTextRT.localPosition =
            new Vector3(0, 0, 0);
        SetTextSize(gemText, 0.045f);
        gemText.rectTransform.sizeDelta = new Vector2(gemText.preferredWidth, gemText.preferredHeight);

        gemAddButtonRT.sizeDelta = 0.17f * new Vector2(gemContainer.sizeDelta.x, gemContainer.sizeDelta.x);
        gemAddButtonRT.localPosition =
            new Vector3(0.4f * (gemContainer.sizeDelta.x - gemAddButtonRT.sizeDelta.x), 0, 0);
        //

        SetTextSize(swapText, 0.045f);
        SetTextSize(changeColorText, 0.045f);
        SetTextSize(undoText, 0.045f);

        SetUIElementSizeToParent(swapButtonRT, gameplayBottomContainer, new Vector2(0.2f, 0.15f));
        SetUIElementSizeToParent(changeColorButtonRT, gameplayBottomContainer, new Vector2(0.35f, 0.15f));
        SetUIElementSizeToParent(undoButtonRT, gameplayBottomContainer, new Vector2(0.2f, 0.15f));

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

        swapGemContainer = Instantiate(gemContainerPrefab, swapButtonRT);
        changeColorGemContainer = Instantiate(gemContainerPrefab, changeColorButtonRT);
        undoGemContainer = Instantiate(gemContainerPrefab, undoButtonRT);

        swapGemContainer.localScale *= 0.6f;
        changeColorGemContainer.localScale *= 0.6f;
        undoGemContainer.localScale *= 0.6f;

        swapGemContainer.localPosition =
            new Vector3(0, -0.45f * (swapButtonRT.sizeDelta.y + swapGemContainer.sizeDelta.y), 0);
        changeColorGemContainer.localPosition =
            new Vector3(0, -0.45f * (swapButtonRT.sizeDelta.y + swapGemContainer.sizeDelta.y), 0);
        undoGemContainer.localPosition =
            new Vector3(0, -0.45f * (swapButtonRT.sizeDelta.y + swapGemContainer.sizeDelta.y), 0);

        swapGemContainer.GetChild(1).GetComponent<TMP_Text>().text = dataManager.skillCosts[0].ToString();
        changeColorGemContainer.GetChild(1).GetComponent<TMP_Text>().text = dataManager.skillCosts[1].ToString();
        undoGemContainer.GetChild(1).GetComponent<TMP_Text>().text = dataManager.skillCosts[2].ToString();

        menuButtonRT.sizeDelta = 0.7f * nextBlockImage.sizeDelta;
        menuButtonRT.localPosition =
            new Vector3(
                0.35f * (gameplayBottomContainer.sizeDelta.x - menuButtonRT.sizeDelta.x),
                nextBlockImage.localPosition.y, 0);

        watchAdButtonRT.sizeDelta = 0.7f * nextBlockImage.sizeDelta;
        watchAdButtonRT.localPosition =
            new Vector3(-0.35f * (gameplayBottomContainer.sizeDelta.x - watchAdButtonRT.sizeDelta.x),
                nextBlockImage.localPosition.y, 0);

        _watchAdGemContainer = Instantiate(gemContainerPrefab, watchAdButtonRT);
        _watchAdGemContainer.localScale *= 0.75f;
        _watchAdGemContainer.localPosition =
            new Vector3(0, -0.3f * (_watchAdGemContainer.sizeDelta.y + swapGemContainer.sizeDelta.y), 0);

        _watchAdGemContainer.GetChild(1).GetComponent<TMP_Text>().text = "+200";

        SetTextSize(scoreText, 0.08f);
        SetTextSize(bestScoreText, 0.05f);
        SetTextSize(menuText, 0.05f);
        SetTextSize(nextBlockValueText, 0.07f);

        scoreText.rectTransform.localPosition =
            new Vector3(0, -0.5f * (gameplayTopContainer.sizeDelta.y - scoreText.preferredHeight), 0);

        SetBestScore();
    }

    private void OpenMenu()
    {
        pausePopup.ShowPopup();
    }

    private void CloseMenu()
    {
        pausePopup.ClosePopup();
    }

    private void OpenShopPopup()
    {
        shopPopup.ShowPopup();
    }

    public void ShowLosePopup()
    {
    }

    public void SetScoreText(float scoreNumber, char? scoreLetter)
    {
        if (scoreLetter == null)
        {
            scoreText.text = scoreNumber.ToString("F2");
        }
        else
        {
            scoreText.text = scoreNumber.ToString("F2") + scoreLetter;
        }

        Tween.Scale(scoreText.transform, 1.1f, duration: 0.1f)
            .OnComplete(() => Tween.Scale(scoreText.transform, 1f, duration: 0.1f));
    }

    private void SetBestScore()
    {
        bestScoreText.text = dataManager.BestScoreNumber.ToString("F2") + dataManager.ScoreLetter;

        bestScoreRT.sizeDelta = new Vector2(bestScoreText.preferredWidth, bestScoreText.preferredHeight);

        bestScoreRT.localPosition =
            new Vector3(
                0.5f * (gameplayTopContainer.sizeDelta.x - bestScoreText.preferredWidth) -
                0.1f * gameplayTopContainer.sizeDelta.y,
                gemContainer.localPosition.y,
                0);

        bestScoreImage.localPosition =
            new Vector3(-0.5f * (bestScoreRT.sizeDelta.x + 1.5f * bestScoreImage.sizeDelta.x), 0, 0);
    }

    public void SetGemText(int numGem)
    {
        gemText.text = Utils.ToAbbreviatedNumber(numGem);
        gemText.rectTransform.sizeDelta = new Vector2(gemText.preferredWidth, gemText.preferredHeight);
    }

    private void PlayAdForGem()
    {
        adManager.ShowRewardedAd(onRewardedAdCompleted: OnPlayAdForGemCompleted);
    }

    private void OnPlayAdForGemCompleted(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        dataManager.NumGem += 200;
        SetGemText(dataManager.NumGem);

        dataManager.SaveIAPData(dataManager.NumGem, dataManager.IsAdRemoved);
    }

    public void OnSwapClicked()
    {
        skillManager.SwapSkill();
    }

    public void ShowNotEnoughGemEffect(SkillType type)
    {
        RectTransform rectTransform = type switch
        {
            SkillType.Undo => undoButtonRT,
            SkillType.ChangeColor => changeColorButtonRT,
            SkillType.Swap => swapButtonRT
        };

        Image image = rectTransform.GetComponent<Image>();

        Vector2 initialDeltaSize = rectTransform.sizeDelta;
        Color initialColor = image.color;
        Color errorColor = Color.red;

        // Tween.UISizeDelta(rectTransform, 1.1f * initialDeltaSize, duration: 0.2f)
        //     .OnComplete(() => Tween.UISizeDelta(rectTransform, initialDeltaSize, duration: 0.2f));

        Tween.Custom(initialColor, errorColor, duration: 0.15f, cycles: 2, cycleMode: CycleMode.Yoyo, onValueChange: newVal => image.color = newVal)
            .SetCycles(false);

        // Tween.Custom(initialColor, errorColor, duration: 0.15f, onValueChange: newVal => image.color = newVal)
        //     .OnComplete(() =>
        //         Tween.Custom(errorColor, initialColor, duration: 0.15f, onValueChange: newVal => image.color = newVal));
    }
}