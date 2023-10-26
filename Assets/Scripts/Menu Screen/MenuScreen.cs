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

    [SerializeField] private Button dailyRewardButton;
    [SerializeField] private Button shopButton;

    [SerializeField] private RectTransform topContainer;
    [SerializeField] private RectTransform gemContainer;
    [SerializeField] private RectTransform bestScoreContainer;
    [SerializeField] private Image gemImage;
    [SerializeField] private Image crownImage;
    [SerializeField] public TMP_Text gemText;
    [SerializeField] private TMP_Text bestScoreText;

    [SerializeField] private Image bestBlockImage;
    [SerializeField] private TMP_Text bestBlockNumberText;

    [Header("POPUP")][SerializeField] private ShopPopup shopPopup;
    [SerializeField] private DailyRewardPopup dailyRewardPopup;
    [SerializeField] private LoadingPopup loadingPopup;
    [SerializeField] private RewardClaimPopup rewardClaimPopup;

    [Header("REFERENCE")][SerializeField] private DataManager dataManager;

    [Header("EVENT")][SerializeField] private ScriptableStringEvent onProductPurchasedEvent;
    [SerializeField] private ScriptableEventNoParam onNumGemUpdatedEvent;

    private Vector2 _screenSize;

    private void Start()
    {
        _screenSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);

        _playButtonRT = playButton.GetComponent<RectTransform>();

        playButton.onClick.AddListener(GoToGameplay);
        dailyRewardButton.onClick.AddListener(ShowDailyRewardPopup);
        shopButton.onClick.AddListener(ShowShopPopup);

        InitUI();

        LoadData();
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
        _playButtonRT.sizeDelta = new Vector2(0.6f * _screenSize.x, 0.1f * _screenSize.y);
        _playButtonRT.localPosition = new Vector3(0, -0.15f * _screenSize.y, 0);

        topContainer.sizeDelta = new Vector2(_screenSize.x, 0.05f * _screenSize.y);
        gemContainer.sizeDelta = new Vector2(0.3f * topContainer.sizeDelta.x, 1f * topContainer.sizeDelta.y);
        bestScoreContainer.sizeDelta = new Vector2(0.3f * topContainer.sizeDelta.x, 1f * topContainer.sizeDelta.y);

        gemContainer.localPosition = new Vector3(-0.4f * (topContainer.sizeDelta.x - gemContainer.sizeDelta.x), 0, 0);
        bestScoreContainer.localPosition =
            new Vector3(0.4f * (topContainer.sizeDelta.x - bestScoreContainer.sizeDelta.x), 0, 0);

        gemImage.rectTransform.sizeDelta = 0.25f * new Vector2(gemContainer.sizeDelta.x, gemContainer.sizeDelta.x);
        crownImage.rectTransform.sizeDelta =
            0.25f * new Vector2(bestScoreContainer.sizeDelta.x, bestScoreContainer.sizeDelta.x);

        gemImage.rectTransform.localPosition =
            new Vector3(-0.4f * (gemContainer.sizeDelta.x - gemImage.rectTransform.sizeDelta.x), 0, 0);

        crownImage.rectTransform.localPosition =
            new Vector3(-0.4f * (bestScoreContainer.sizeDelta.x - crownImage.rectTransform.sizeDelta.x), 0, 0);

        gemText.rectTransform.sizeDelta = new Vector2(0.65f * gemContainer.sizeDelta.x, gemText.preferredHeight);
        gemText.rectTransform.localPosition =
            new Vector3(
                gemImage.rectTransform.localPosition.x +
                0.5f * (gemImage.rectTransform.sizeDelta.x + gemText.rectTransform.sizeDelta.x), 0, 0);

        SetSize(bestScoreText.rectTransform, 0.65f * bestScoreContainer.sizeDelta.x, gemText.preferredHeight);
        SetLocalPositionX(bestScoreText.rectTransform,
            crownImage.rectTransform.localPosition.x +
            0.5f * (crownImage.rectTransform.sizeDelta.x + bestScoreText.rectTransform.sizeDelta.x));

        // bestScoreText.rectTransform.sizeDelta =
        //     new Vector2(0.65f * bestScoreContainer.sizeDelta.x, gemText.preferredHeight);
        // bestScoreText.rectTransform.localPosition =
        //     new Vector3(
        //         crownImage.rectTransform.localPosition.x +
        //         0.5f * (crownImage.rectTransform.sizeDelta.x + bestScoreText.rectTransform.sizeDelta.x), 0, 0);

        AnimatePlayButton();
    }

    private void SetSize(RectTransform target, float width, float height)
    {
        target.sizeDelta = new Vector2(width, height);
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

    private void LoadData()
    {
        gemText.text = Utils.ToAbbreviatedNumber(dataManager.NumGem);
        bestScoreText.text = dataManager.BestScoreNumber.ToString("F2") + dataManager.BestScoreLetter;

        bestBlockImage.color = Constants.AllBlockColors[dataManager.BestBlockColorIndex];
        bestBlockNumberText.text = dataManager.BestBlockNumber.ToString("F0") + dataManager.BestBlockLetter;
    }

    private void GoToGameplay()
    {
        Tween.Delay(1.5f).OnComplete(() => Addressables.LoadSceneAsync("Gameplay"));

        loadingPopup.ShowPopup();
    }

    private void ShowDailyRewardPopup()
    {
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
        Debug.Log("test8 " + gemText.text);
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
    }
}