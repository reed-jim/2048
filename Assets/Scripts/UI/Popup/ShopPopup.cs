using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class ShopPopup : Popup
{
    [Header("UI")][SerializeField] private Button itemButtonPrefab;
    [SerializeField] private Button[] itemButtons;
    private Button[] _buyButtons;
    private CodelessIAPButton[] _iapButtons;
    private RectTransform[] _itemButtonRTs;
    private RectTransform[] _buyButtonRTs;
    private Image[] _gemImages;
    private TMP_Text[] _itemCostText;
    private TMP_Text[] _itemValueText;

    [Header("SPRITE")][SerializeField] private Sprite removeAdSprite;

    [Space]
    [Header("REFERENCE")]
    [SerializeField]
    private DataManager dataManager;

    [SerializeField] private AdManager adManager;
    [SerializeField] private IAPManager iapManager;

    [Header("POPUP")]
    [SerializeField] private RewardClaimPopup rewardClaimPopup;

    [Header("CUSTOM")][SerializeField] private int numItem;

    [Header("EVENT")]
    [SerializeField] private ScriptableEventNoParam onNumGemUpdatedEvent;

    private void Start()
    {
        // Spawn();
    }

    private void OnDisable()
    {
        if (!dataManager.IsAdRemoved)
        {
            adManager.ShowBannerAd();
        }
    }

    protected override void InitUI()
    {
        base.InitUI();

        itemButtons = new Button[numItem];
        _buyButtons = new Button[numItem];
        _itemButtonRTs = new RectTransform[numItem];
        _buyButtonRTs = new RectTransform[numItem];
        _iapButtons = new CodelessIAPButton[numItem];
        _gemImages = new Image[numItem];
        _itemValueText = new TMP_Text[numItem];
        _itemCostText = new TMP_Text[numItem];

        for (int i = 0; i < itemButtons.Length; i++)
        {
            itemButtons[i] = Instantiate(itemButtonPrefab, container);
            _itemButtonRTs[i] = itemButtons[i].GetComponent<RectTransform>();
            _buyButtons[i] = _itemButtonRTs[i].GetChild(1).GetComponent<Button>();
            _iapButtons[i] = _buyButtons[i].GetComponent<CodelessIAPButton>();
            _buyButtonRTs[i] = _buyButtons[i].GetComponent<RectTransform>();
            _gemImages[i] = _itemButtonRTs[i].GetChild(2).GetComponent<Image>();
            _itemValueText[i] = _itemButtonRTs[i].GetChild(0).GetComponent<TMP_Text>();
            _itemCostText[i] = _buyButtonRTs[i].GetChild(0).GetComponent<TMP_Text>();
        }

        for (int i = 0; i < itemButtons.Length; i++)
        {
            _itemButtonRTs[i].sizeDelta = new Vector2((1 - 2.5f * paddingPercent) * container.sizeDelta.x, 0.085f * container.sizeDelta.y);
            _itemButtonRTs[i].localPosition =
                new Vector3(0, 0.3f * container.sizeDelta.y - i * 1.15f * _itemButtonRTs[i].sizeDelta.y, 0);

            SetTextFontSizeDirectly(_itemCostText[i], 0.2f * _itemButtonRTs[i].sizeDelta.y);
            SetTextFontSizeDirectly(_itemValueText[i], 0.35f * _itemButtonRTs[i].sizeDelta.y);

            _itemCostText[i].text = i == 1 ? dataManager.IapCosts[i] : "$" + dataManager.IapCosts[i];
            _itemValueText[i].text = dataManager.IapValues[i];
            _itemValueText[i].rectTransform.sizeDelta =
                new Vector2(_itemValueText[i].preferredWidth, _itemValueText[i].preferredHeight);

            if (i == 0)
            {
                _gemImages[i].sprite = removeAdSprite;
            }

            SetSizeKeepRatioX(_gemImages[i], 0.4f * _itemButtonRTs[i].sizeDelta.y);
            _gemImages[i].rectTransform.localPosition =
                new Vector3(-0.45f * (_itemButtonRTs[i].sizeDelta.x - _gemImages[i].rectTransform.sizeDelta.x), 0, 0);

            SetLocalPositionX(_itemValueText[i].rectTransform,
                _gemImages[i].rectTransform.localPosition.x +
                    0.5f * (2f * _gemImages[i].rectTransform.sizeDelta.x + _itemValueText[i].preferredWidth)
            );

            _buyButtonRTs[i].sizeDelta =
                new Vector2(0.25f * _itemButtonRTs[i].sizeDelta.x, 2f * _itemCostText[i].preferredHeight);

            _buyButtonRTs[i].localPosition =
                new Vector3(0.45f * (_itemButtonRTs[i].sizeDelta.x - _buyButtonRTs[i].sizeDelta.x), 0, 0);

            if (i == 1)
            {
                _buyButtons[i].onClick.AddListener(OnWatchAdForGemButtonPressed);
            }
            else
            {
                string productId = dataManager.ProductIds[i];
                _buyButtons[i].onClick.AddListener(() => OnBuyButtonPressed(productId));
            }
        }
    }

    public override void ShowPopup()
    {
        base.ShowPopup();

        if (dataManager.IsAdRemoved)
        {
            _buyButtons[0].interactable = false;
            _itemCostText[0].text = "Owned";
        }

        adManager.HideBannerAd();
    }

    private void OnBuyButtonPressed(string productId)
    {
        AudioManager.Instance.PlayPopupSound();

        iapManager.BuyProducts(productId);
    }

    private void OnWatchAdForGemButtonPressed()
    {
        AudioManager.Instance.PlayPopupSound();

        if (dataManager.IsAdRemoved)
        {
            dataManager.NumGem += 200;
            dataManager.SaveIAPData();
            onNumGemUpdatedEvent.Raise();

            _buyButtons[1].interactable = false;

            rewardClaimPopup.ShowPopup(200);
        }
        else
        {
            adManager.ShowRewardedAd(onRewardedAdCompleted: HandleOnRewardedAdCompleted);
        }
    }

    private void HandleOnRewardedAdCompleted(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        dataManager.NumGem += 200;
        dataManager.SaveIAPData();
        onNumGemUpdatedEvent.Raise();

        rewardClaimPopup.ShowPopup(200);
    }

    public void HandleAdRemoved() {
        _buyButtons[0].interactable = false;
        _itemCostText[0].text = "Owned";
    }
}