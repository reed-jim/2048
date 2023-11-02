using System;
using PrimeTween;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region PROPERTY
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

    [SerializeField] private RectTransform nextBlockImage;

    [SerializeField] private RectTransform gameplayTopContainer;
    [SerializeField] private RectTransform gameplayBottomContainer;
    [SerializeField] private RectTransform topOutsideSafeAreaFill;

    [SerializeField] private RectTransform bestScoreContainer;
    [SerializeField] private RectTransform bestScoreRT;
    [SerializeField] private Image bestScoreImage;


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
    [SerializeField] private RewardClaimPopup rewardClaimPopup;

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

    [Header("EVENT")]
    [SerializeField] private ScriptableStringEvent onProductPurchasedEvent;
    [SerializeField] private ScriptableEventNoParam onNumGemUpdatedEvent;

    public float SafeAreaTopSizeY
    {
        get => _safeAreaTopSizeY;
    }
    #endregion

    private void OnEnable()
    {
        onProductPurchasedEvent.Register(HandlePurchaseIAPCompleted);
        onNumGemUpdatedEvent.Register(SetGemText);
    }

    private void OnDisable()
    {
        onProductPurchasedEvent.Unregister(HandlePurchaseIAPCompleted);
        onNumGemUpdatedEvent.Unregister(SetGemText);
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

    private void InitUI()
    {
        RectTransform swapButtonRT = swapButton.GetComponent<RectTransform>();
        RectTransform changeColorButtonRT = changeColorButton.GetComponent<RectTransform>();
        RectTransform undoButtonRT = undoButton.GetComponent<RectTransform>();
        RectTransform menuButtonRT = menuButton.GetComponent<RectTransform>();

        UtilsUI.SetSize(gameplayTopContainer, 1f * _screenSize.x, 0.12f * _screenSize.y);
        UtilsUI.SetSize(gameplayBottomContainer, 1f * _screenSize.x, 0.35f * _screenSize.y);

        UtilsUI.SetLocalPositionY(gameplayTopContainer, 0.5f * (_screenSize.y - gameplayTopContainer.sizeDelta.y) - _safeAreaTopSizeY);
        UtilsUI.SetLocalPositionY(gameplayBottomContainer, -0.5f * (_screenSize.y - gameplayBottomContainer.sizeDelta.y));

        topOutsideSafeAreaFill.sizeDelta = new Vector2(_screenSize.x, _safeAreaTopSizeY);
        topOutsideSafeAreaFill.localPosition = new Vector3(0,
            gameplayTopContainer.localPosition.y +
            0.5f * (gameplayTopContainer.sizeDelta.y + topOutsideSafeAreaFill.sizeDelta.y), 0);

        // GEM CONTAINER
        SetUIElementSizeToParent(gemContainer, gameplayTopContainer, new Vector2(0.38f, 0.4f));
        gemContainer.localPosition =
            new Vector3(
                -0.5f * (gameplayTopContainer.sizeDelta.x - gemContainer.sizeDelta.x) +
                0.1f * gameplayTopContainer.sizeDelta.y,
                0.3f * (gameplayTopContainer.sizeDelta.y - gemContainer.sizeDelta.y),
                0);

        SetSizeKeepRatioX(gemImageRT.GetComponent<Image>(), 0.6f * gemContainer.sizeDelta.y);
        UtilsUI.SetLocalPositionX(gemImageRT, -0.4f * (gemContainer.sizeDelta.x - gemImageRT.sizeDelta.x));

        gemText.text = Utils.ToAbbreviatedNumber(dataManager.NumGem);
        UtilsUI.SetTextFontSize(gemText, 0.035f);
        UtilsUI.SetTextPreferredSize(gemText);

        SetSizeKeepRatioX(gemAddButtonRT.GetComponent<Image>(), 0.6f * gemContainer.sizeDelta.y);
        UtilsUI.SetLocalPositionX(gemAddButtonRT, 0.4f * (gemContainer.sizeDelta.x - gemAddButtonRT.sizeDelta.x));
        //

        // BEST SCORE CONTAINER
        SetUIElementSizeToParent(bestScoreContainer, gameplayTopContainer, new Vector2(0.33f, 0.4f));
        UtilsUI.SetLocalPosition(bestScoreContainer,
            0.5f * (gameplayTopContainer.sizeDelta.x - bestScoreContainer.sizeDelta.x) -
            0.1f * gameplayTopContainer.sizeDelta.y,
            gemContainer.localPosition.y
        );
        SetSizeKeepRatioX(bestScoreImage, 0.6f * bestScoreContainer.sizeDelta.y);
        UtilsUI.SetLocalPositionX(bestScoreImage.rectTransform, -0.4f * (bestScoreContainer.sizeDelta.x - bestScoreImage.rectTransform.sizeDelta.x));

        UtilsUI.SetTextFontSize(bestScoreText, 0.035f);
        UtilsUI.SetSize(bestScoreText.rectTransform, 0.65f * bestScoreContainer.sizeDelta.x, gemText.preferredHeight);
        UtilsUI.SetLocalPositionX(bestScoreText.rectTransform,
            bestScoreImage.rectTransform.localPosition.x +
            0.5f * (bestScoreImage.rectTransform.sizeDelta.x + bestScoreText.rectTransform.sizeDelta.x));
        //

        // BOTTOM
        SetTextSize(swapText, 0.045f);
        SetTextSize(changeColorText, 0.045f);
        SetTextSize(undoText, 0.045f);

        SetUIElementSizeToParent(swapButtonRT, gameplayBottomContainer, new Vector2(0.2f, 0.15f));
        SetUIElementSizeToParent(changeColorButtonRT, gameplayBottomContainer, new Vector2(0.35f, 0.15f));
        SetUIElementSizeToParent(undoButtonRT, gameplayBottomContainer, new Vector2(0.2f, 0.15f));

        UtilsUI.SetLocalPosition(swapButtonRT, -0.55f * (changeColorButtonRT.sizeDelta.x + swapButtonRT.sizeDelta.x), changeColorButtonRT.localPosition.y);
        UtilsUI.SetLocalPosition(undoButtonRT, 0.55f * (changeColorButtonRT.sizeDelta.x + undoButtonRT.sizeDelta.x), changeColorButtonRT.localPosition.y);

        UtilsUI.SetSizeEqual(nextBlockImage, 0.2f * gameplayBottomContainer.sizeDelta.x);
        UtilsUI.SetLocalPositionY(nextBlockImage, 0.45f * (gameplayBottomContainer.sizeDelta.y - nextBlockImage.sizeDelta.y));

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
        SetTextSize(menuText, 0.05f);
        SetTextSize(nextBlockValueText, 0.07f);

        UtilsUI.SetLocalPositionY(scoreText.rectTransform, -0.5f * (gameplayTopContainer.sizeDelta.y - scoreText.preferredHeight));
    }

    #region OPEN/CLOSE POPUP
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
    #endregion

    #region SET TEXT
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

    public void SetBestScore(float scoreNumber, char? scoreLetter)
    {
        if (scoreLetter == null)
        {
            bestScoreText.text = scoreNumber.ToString("F2");
        }
        else
        {
            bestScoreText.text = scoreNumber.ToString("F2") + scoreLetter;
        }

        Tween.Scale(bestScoreText.transform, 1.1f, duration: 0.1f)
            .OnComplete(() => Tween.Scale(bestScoreText.transform, 1f, duration: 0.1f));
    }

    public void SetGemText()
    {
        gemText.text = Utils.ToAbbreviatedNumber(dataManager.NumGem);
        gemText.rectTransform.sizeDelta = new Vector2(gemText.preferredWidth, gemText.preferredHeight);

        Tween.Scale(gemText.rectTransform, 1.1f, duration: 0.2f, cycles: 2, cycleMode: CycleMode.Yoyo).SetCycles(false);
    }

    public void SetGemText(int numGem)
    {
        gemText.text = Utils.ToAbbreviatedNumber(numGem);
        gemText.rectTransform.sizeDelta = new Vector2(gemText.preferredWidth, gemText.preferredHeight);
    }
    #endregion

    #region UTIL
    private void SetUIElementSizeToParent(RectTransform target, RectTransform parent, Vector2 proportional)
    {
        target.sizeDelta = new Vector2(proportional.x * parent.sizeDelta.x, proportional.y * parent.sizeDelta.y);
    }

    private void SetTextSize(TMP_Text text, float proportion)
    {
        text.fontSize = (int)(proportion * _screenSize.x);
    }

    private void SetSizeKeepRatioX(Image target, float height)
    {
        float ratio = target.sprite.rect.size.x / target.sprite.rect.size.y;
        target.rectTransform.sizeDelta = new Vector2(height * ratio, height);
    }
    #endregion

    #region SKILL
    public void OnSwapClicked()
    {
        skillManager.SwapSkill();
    }

    public void PlaySkillButtonPressEffect(SkillType type)
    {
        RectTransform rectTransform = type switch
        {
            SkillType.Undo => undoButtonRT,
            SkillType.ChangeColor => changeColorButtonRT,
            SkillType.Swap => swapButtonRT
        };

        Tween.Scale(rectTransform, 1.1f, duration: 0.2f, cycles: 2, cycleMode: CycleMode.Yoyo).SetCycles(false);
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
        TMP_Text text = rectTransform.GetChild(0).GetComponent<TMP_Text>();

        Vector2 initialDeltaSize = rectTransform.sizeDelta;
        Color initialColor = image.color;
        // Color errorColor = initialColor + new Color(0.7f, 0, 0, 0.2f);
        Color errorColor = Color.red;
        errorColor.a = image.color.a;

        Tween.Scale(rectTransform, 1.05f, duration: 0.3f, cycles: 2, cycleMode: CycleMode.Yoyo).SetCycles(false);

        Tween.Custom(initialColor, errorColor, duration: 0.4f, cycles: 2, cycleMode: CycleMode.Yoyo, onValueChange: newVal => image.color = newVal)
            .SetCycles(false);

        Tween.Custom(text.color, new Color(1, 195f / 255, 180f / 255, 1), duration: 0.5f, cycles: 2, cycleMode: CycleMode.Yoyo, onValueChange: newVal => text.color = newVal)
            .SetCycles(false);
    }
    #endregion

    #region ADVERTISEMENT
    private void PlayAdForGem()
    {
        if (dataManager.IsAdRemoved)
        {
            GiveAdGemReward();
            watchAdForGemButton.gameObject.SetActive(false);
        }
        else
        {
            adManager.ShowRewardedAd(onRewardedAdCompleted: OnPlayAdForGemCompleted);
        }
    }

    private void OnPlayAdForGemCompleted(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        GiveAdGemReward();
    }

    private void GiveAdGemReward()
    {
        dataManager.NumGem += 200;
        SetGemText(dataManager.NumGem);

        rewardClaimPopup.ShowPopup(200);

        dataManager.SaveIAPData();
    }
    #endregion

    #region IAP
    private void HandlePurchaseIAPCompleted(string productId)
    {
        if (productId.Contains("gem"))
        {
            int numGemPurchased = int.Parse(productId.Substring(3));

            rewardClaimPopup.ShowPopup(numGemPurchased);

            dataManager.NumGem += numGemPurchased;

            SetGemText();
        }
        else if (productId == "remove_ad")
        {
            dataManager.IsAdRemoved = true;

            shopPopup.HandleAdRemoved();

            adManager.DestroyBannerAd();
        }

        dataManager.SaveIAPData();
    }
    #endregion
}