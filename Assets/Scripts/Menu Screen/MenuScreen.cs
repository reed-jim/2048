using System;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class MenuScreen : MonoBehaviour
{
    [Header("UI")] [SerializeField] private Button playButton;
    private RectTransform _playButtonRT;

    [SerializeField] private Button dailyRewardButton;
    [SerializeField] private Button shopButton;

    [SerializeField] private GameObject dailyRewardContainer;
    [SerializeField] private GameObject shopContainer;

    [SerializeField] private RectTransform topContainer;
    [SerializeField] private RectTransform gemContainer;
    [SerializeField] private RectTransform bestScoreContainer;
    [SerializeField] private TMP_Text bestScoreText;
    [SerializeField] private TMP_Text gemText;

    private Vector2 _screenSize;

    private void Start()
    {
        _screenSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);

        _playButtonRT = playButton.GetComponent<RectTransform>();

        playButton.onClick.AddListener(GoToGameplay);
        dailyRewardButton.onClick.AddListener(ShowDailyRewardPopup);
        shopButton.onClick.AddListener(ShowShopPopup);

        InitUI();
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
    }

    private void GoToGameplay()
    {
        Addressables.LoadSceneAsync("Gameplay");
    }

    private void ShowDailyRewardPopup()
    {
        dailyRewardContainer.SetActive(true);
    }

    private void ShowShopPopup()
    {
        shopContainer.SetActive(true);
    }
}