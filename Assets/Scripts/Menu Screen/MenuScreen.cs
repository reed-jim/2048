using System;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class MenuScreen : MonoBehaviour
{
    [Header("UI")][SerializeField] private Button playButton;
    private RectTransform _playButtonRT;
    [SerializeField] TMP_Text playButtonText;

    [SerializeField] private Button dailyRewardButton;
    [SerializeField] private Button shopButton;
    [SerializeField] private Button removeAdButton;

    private RectTransform _dailyRewardButtonRT;
    private RectTransform _shopButtonRT;
    private RectTransform _removeAdButtonRT;

    [SerializeField] private RectTransform topContainer;
    [SerializeField] private RectTransform gemContainer;
    [SerializeField] private RectTransform bestScoreContainer;
    [SerializeField] private Image gemImage;
    [SerializeField] private Image crownImage;
    [SerializeField] public TMP_Text gemText;
    [SerializeField] private TMP_Text bestScoreText;

    [SerializeField] private RectTransform bestBlockContainer;
    [SerializeField] private Image bestBlockImage;
    [SerializeField] private Image bestBlockCrownImage;
    [SerializeField] private TMP_Text bestBlockNumberText;
    [SerializeField] private TMP_Text bestBlockDescription;

    [Header("POPUP")][SerializeField] private ShopPopup shopPopup;
    [SerializeField] private DailyRewardPopup dailyRewardPopup;
    [SerializeField] private LoadingPopup loadingPopup;
    [SerializeField] private RewardClaimPopup rewardClaimPopup;

    [Header("REFERENCE")][SerializeField] private DataManager dataManager;
    [SerializeField] private IAPManager iapManager;
    [SerializeField] private AdManager adManager;

    [Header("EVENT")][SerializeField] private ScriptableStringEvent onProductPurchasedEvent;
    [SerializeField] private ScriptableEventNoParam onNumGemUpdatedEvent;

    private Vector2 _screenSize;

    private void Start()
    {
        _screenSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);

        _playButtonRT = playButton.GetComponent<RectTransform>();
        _dailyRewardButtonRT = dailyRewardButton.GetComponent<RectTransform>();
        _shopButtonRT = shopButton.GetComponent<RectTransform>();
        _removeAdButtonRT = removeAdButton.GetComponent<RectTransform>();

        playButton.onClick.AddListener(GoToGameplay);
        dailyRewardButton.onClick.AddListener(ShowDailyRewardPopup);
        shopButton.onClick.AddListener(ShowShopPopup);
        removeAdButton.onClick.AddListener(OnRemoveAdButtonPressed);

        InitUI();

        LoadData();

        AudioManager.Instance.PlayBackgroundSound();
    }

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

    private void InitUI()
    {
        SetSize(_playButtonRT, 0.6f * _screenSize.x, 0.1f * _screenSize.y);
        SetLocalPositionY(_playButtonRT, -0.15f * _screenSize.y);
        SetTextFontSize(playButtonText, 0.1f);

        SetSize(topContainer, _screenSize.x, 0.05f * _screenSize.y);
        SetSize(gemContainer, 0.3f * topContainer.sizeDelta.x, 1f * topContainer.sizeDelta.y);
        SetSize(bestScoreContainer, 0.3f * topContainer.sizeDelta.x, 1f * topContainer.sizeDelta.y);
        SetLocalPositionY(topContainer, 0.45f * (_screenSize.y - topContainer.sizeDelta.y));
        SetLocalPositionX(gemContainer, -0.4f * (topContainer.sizeDelta.x - gemContainer.sizeDelta.x));
        SetLocalPositionX(bestScoreContainer, 0.4f * (topContainer.sizeDelta.x - bestScoreContainer.sizeDelta.x));

        SetLocalScaleEqual(_shopButtonRT, 0.0007f * _screenSize.x);
        SetLocalScaleEqual(_dailyRewardButtonRT, 0.0007f * _screenSize.x);
        SetLocalScaleEqual(_removeAdButtonRT, 0.0006f * _screenSize.x);

        SetLocalPosition(_shopButtonRT, gemContainer.localPosition.x - 0.5f * (gemContainer.sizeDelta.x - GetSizeByScale(_shopButtonRT).x),
            topContainer.localPosition.y - 1f * (gemContainer.sizeDelta.y + GetSizeByScale(_shopButtonRT).y)
        );
        SetLocalPosition(_dailyRewardButtonRT, bestScoreContainer.localPosition.x + 0.5f * (bestScoreContainer.sizeDelta.x - GetSizeByScale(_dailyRewardButtonRT).x),
            _shopButtonRT.localPosition.y
        );
        SetLocalPosition(_removeAdButtonRT, _dailyRewardButtonRT.localPosition.x,
            _dailyRewardButtonRT.localPosition.y - 0.6f * (GetSizeByScale(_dailyRewardButtonRT).y + GetSizeByScale(_removeAdButtonRT).x)
        );

        SetSizeKeepRatioX(gemImage, 0.7f * gemContainer.sizeDelta.y);
        SetSizeKeepRatioX(crownImage, 0.7f * bestScoreContainer.sizeDelta.y);

        SetLocalPositionX(gemImage.rectTransform, -0.4f * (gemContainer.sizeDelta.x - gemImage.rectTransform.sizeDelta.x));
        SetLocalPositionX(crownImage.rectTransform, -0.4f * (bestScoreContainer.sizeDelta.x - crownImage.rectTransform.sizeDelta.x));

        SetTextFontSize(gemText, 0.04f);
        SetSize(gemText.rectTransform, 0.65f * gemContainer.sizeDelta.x, gemText.preferredHeight);
        SetLocalPositionX(gemText.rectTransform, gemImage.rectTransform.localPosition.x +
                0.5f * (gemImage.rectTransform.sizeDelta.x + gemText.rectTransform.sizeDelta.x));

        SetTextFontSize(bestScoreText, 0.04f);
        SetSize(bestScoreText.rectTransform, 0.65f * bestScoreContainer.sizeDelta.x, gemText.preferredHeight);
        SetLocalPositionX(bestScoreText.rectTransform,
            crownImage.rectTransform.localPosition.x +
            0.5f * (crownImage.rectTransform.sizeDelta.x + bestScoreText.rectTransform.sizeDelta.x));

        SetLocalPositionY(bestBlockContainer, -0.05f * _screenSize.y);
        SetSizeEqual(bestBlockImage.rectTransform, 0.25f * _screenSize.x);
        SetSizeKeepRatioY(bestBlockCrownImage, 0.3f * bestBlockImage.rectTransform.sizeDelta.x);
        SetLocalPositionY(bestBlockCrownImage.rectTransform, 0.5f * (bestBlockImage.rectTransform.sizeDelta.y + bestBlockCrownImage.rectTransform.sizeDelta.y));

        SetTextFontSize(bestBlockNumberText, 0.06f);

        SetTextFontSize(bestBlockDescription, 0.05f);
        SetLocalPositionY(bestBlockDescription.rectTransform, -0.55f * (bestBlockImage.rectTransform.sizeDelta.y + bestBlockDescription.preferredHeight));

        SetLocalPositionY(_playButtonRT, bestBlockContainer.localPosition.y - 2 * bestBlockImage.rectTransform.sizeDelta.y);

        if (dataManager.IsAdRemoved)
        {
            removeAdButton.gameObject.SetActive(false);
        }

        if (dataManager.IsDailyRewardAvailable())
        {
            Utils.ScaleUpDownUI(_dailyRewardButtonRT, 0.4f);
        }

        AnimatePlayButton();
    }

    #region UTIL
    private Vector2 GetSizeByScale(RectTransform target)
    {
        return new Vector2(target.sizeDelta.x * target.localScale.x, target.sizeDelta.y * target.localScale.y);
    }

    private void SetTextFontSize(TMP_Text text, float proportion)
    {
        text.fontSize = proportion * _screenSize.x;
    }

    private void SetTextPreferredSize(TMP_Text text)
    {
        text.rectTransform.sizeDelta = new Vector2(text.preferredWidth, text.preferredHeight);
    }

    private void SetSize(RectTransform target, float width, float height)
    {
        target.sizeDelta = new Vector2(width, height);
    }

    private void SetSizeEqual(RectTransform target, float width)
    {
        target.sizeDelta = new Vector2(width, width);
    }

    private void SetSizeKeepRatioX(Image target, float height)
    {
        float ratio = target.sprite.rect.size.x / target.sprite.rect.size.y;
        target.rectTransform.sizeDelta = new Vector2(height * ratio, height);
    }

    private void SetSizeKeepRatioY(Image target, float width)
    {
        float ratio = target.sprite.rect.size.y / target.sprite.rect.size.x;
        target.rectTransform.sizeDelta = new Vector2(width, width * ratio);
    }

    private void SetLocalScale(RectTransform target, float scaleX, float scaleY)
    {
        target.localScale = new Vector3(scaleX, scaleY, 1);
    }

    private void SetLocalScaleEqual(RectTransform target, float scaleX)
    {
        target.localScale = new Vector3(scaleX, scaleX, 1);
    }

    private void SetLocalPosition(RectTransform target, float x, float y)
    {
        target.localPosition = new Vector3(x, y, 0);
    }

    private void SetLocalPositionX(RectTransform target, float x)
    {
        SetLocalPosition(target, x, 0);
    }

    private void SetLocalPositionY(RectTransform target, float y)
    {
        SetLocalPosition(target, 0, y);
    }
    #endregion

    private void LoadData()
    {
        gemText.text = Utils.ToAbbreviatedNumber(dataManager.NumGem);
        bestScoreText.text = dataManager.BestScoreNumber.ToString("F2") + dataManager.BestScoreLetter;

        bestBlockImage.color = Constants.GetColorInTheme(ThemePicker.value)[dataManager.BestBlockColorIndex];
        bestBlockNumberText.text = dataManager.BestBlockNumber.ToString("F0") + dataManager.BestBlockLetter;
        bestBlockNumberText.text = bestBlockNumberText.text.ToUpper();
        SetTextPreferredSize(bestBlockNumberText);
    }

    private void GoToGameplay()
    {
        AudioManager.Instance.PlayPopupSound();

        Tween.Delay(1.5f).OnComplete(() => Addressables.LoadSceneAsync("Gameplay"));

        loadingPopup.ShowPopup();
    }

    private void ShowDailyRewardPopup()
    {
        Tween.StopAll(_dailyRewardButtonRT);
        dailyRewardPopup.ShowPopup();
    }

    private void ShowShopPopup()
    {
        shopPopup.ShowPopup();
    }

    private void AnimatePlayButton()
    {
        Tween.Scale(_playButtonRT, 1.05f, duration: 0.5f, cycles: -1, cycleMode: CycleMode.Yoyo);
    }

    private void SetGemText()
    {
        gemText.text = Utils.ToAbbreviatedNumber(dataManager.NumGem);
    }

    private void OnRemoveAdButtonPressed()
    {
        AudioManager.Instance.PlayPopupSound();

        iapManager.BuyProducts(dataManager.ProductIds[0]);
    }

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

            adManager.DestroyBannerAd();
        }

        dataManager.SaveIAPData();
    }
}