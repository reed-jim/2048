using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardPopup : Popup
{
    [Header("UI")] [SerializeField] private RectTransform innerContainer;
    [SerializeField] private Button dailyRewardButtonPrefab;
    [SerializeField] private Button[] dailyRewardButtons;
    private RectTransform[] _dailyRewardButtonRTs;
    private TMP_Text[] dayTexts;
    private TMP_Text[] valueTexts;
    private Image[] gemImages;
    private Image[] checkMarkImages;

    [SerializeField] private float blockGap;
    private float _blockDistance;

    [Header("REFERENCE")] [SerializeField] private DataManager dataManager;
    [SerializeField] private MenuScreen menuScreen;
    [SerializeField] private RewardClaimPopup rewardClaimPopup;

    protected override void InitUI()
    {
        base.InitUI();

        dailyRewardButtons = new Button[7];
        _dailyRewardButtonRTs = new RectTransform[7];
        dayTexts = new TMP_Text[7];
        valueTexts = new TMP_Text[7];
        gemImages = new Image[7];
        checkMarkImages = new Image[7];

        for (int i = 0; i < dailyRewardButtons.Length; i++)
        {
            dailyRewardButtons[i] = Instantiate(dailyRewardButtonPrefab, innerContainer);
            _dailyRewardButtonRTs[i] = dailyRewardButtons[i].GetComponent<RectTransform>();
            dayTexts[i] = _dailyRewardButtonRTs[i].GetChild(0).GetComponent<TMP_Text>();
            valueTexts[i] = _dailyRewardButtonRTs[i].GetChild(1).GetComponent<TMP_Text>();
            gemImages[i] = _dailyRewardButtonRTs[i].GetChild(2).GetComponent<Image>();
            checkMarkImages[i] = _dailyRewardButtonRTs[i].GetChild(3).GetComponent<Image>();
        }

        blockGap = 0.005f * screenSize.x;

        innerContainer.sizeDelta = new Vector2(0.8f * container.sizeDelta.x, 0.35f * container.sizeDelta.y);

        Vector3 position = Vector3.zero;

        for (int i = 0; i < dailyRewardButtons.Length; i++)
        {
            position.z = 0;

            if (i < 4)
            {
                _dailyRewardButtonRTs[i].sizeDelta =
                    new Vector2(innerContainer.sizeDelta.x / 4, innerContainer.sizeDelta.y / 3);

                _blockDistance = _dailyRewardButtonRTs[i].sizeDelta.x + blockGap;

                position.x = -1.5f * _blockDistance + i * _blockDistance;
                position.y = 0.5f * (innerContainer.sizeDelta.y - _dailyRewardButtonRTs[0].sizeDelta.y);
            }
            else if (i >= 4 && i < 6)
            {
                _dailyRewardButtonRTs[i].sizeDelta =
                    new Vector2(innerContainer.sizeDelta.x / 2 + blockGap,
                        innerContainer.sizeDelta.y / 3);

                _blockDistance = _dailyRewardButtonRTs[i].sizeDelta.x + blockGap;

                position.x = -0.5f * _blockDistance + (i - 4) * _blockDistance;
                position.y = 0.5f * (innerContainer.sizeDelta.y - _dailyRewardButtonRTs[0].sizeDelta.y)
                             - (_dailyRewardButtonRTs[0].sizeDelta.y + blockGap);
            }
            else
            {
                _dailyRewardButtonRTs[i].sizeDelta =
                    new Vector2(innerContainer.sizeDelta.x / 1 + 3 * blockGap,
                        innerContainer.sizeDelta.y / 3);

                position.x = 0;
                position.y = 0.5f * (innerContainer.sizeDelta.y - _dailyRewardButtonRTs[0].sizeDelta.y) -
                             2 * (_dailyRewardButtonRTs[0].sizeDelta.y + blockGap);
            }

            _dailyRewardButtonRTs[i].localPosition = position;

            gemImages[i].rectTransform.sizeDelta = 0.4f * new Vector2(1.1f * _dailyRewardButtonRTs[0].sizeDelta.x,
                _dailyRewardButtonRTs[0].sizeDelta.x);

            SetTextFontSize(dayTexts[i], 0.03f);
            SetTextFontSize(valueTexts[i], 0.05f);

            dayTexts[i].text = "DAY " + (i + 1);
            valueTexts[i].text = dataManager.DailyRewards[i].ToString();
            dayTexts[i].rectTransform.localPosition = new Vector3(0,
                0.4f * (_dailyRewardButtonRTs[i].sizeDelta.y - dayTexts[i].preferredHeight), 0);
            valueTexts[i].rectTransform.localPosition = new Vector3(0,
                -0.7f * (gemImages[i].rectTransform.sizeDelta.y + dayTexts[i].preferredHeight), 0);

            checkMarkImages[i].rectTransform.sizeDelta = 0.7f * new Vector2(_dailyRewardButtonRTs[i].sizeDelta.x,
                _dailyRewardButtonRTs[i].sizeDelta.x);
        }
    }

    private void Start()
    {
        for (int i = 0; i < dailyRewardButtons.Length; i++)
        {
            if (dataManager.DailyRewardData.NumDayGetReward > i)
            {
                ShowDayRewardClaimed(day: i);
            }

            if (dataManager.DailyRewardData.NumDayGetReward == i && dataManager.IsDailyRewardAvailable())
            {
                int index = i;
                dailyRewardButtons[i].onClick
                    .AddListener(() => ClaimDailyReward(index, dataManager.DailyRewards[index]));
            }
            else
            {
                dailyRewardButtons[i].interactable = false;
            }
        }
    }

    private void ClaimDailyReward(int day, int reward)
    {
        if (dataManager.IsDailyRewardAvailable())
        {
            dataManager.NumGem += reward;
            dataManager.SaveIAPData(dataManager.NumGem, dataManager.IsAdRemoved);
            dataManager.SaveDailyRewardData();

            rewardClaimPopup.ShowPopup(reward);

            ShowDayRewardClaimed(day);

            SetGemText();
        }

        rewardClaimPopup.ShowPopup(reward);
    }

    private void ShowDayRewardClaimed(int day)
    {
        checkMarkImages[day].gameObject.SetActive(true);
        gemImages[day].gameObject.SetActive(false);
        valueTexts[day].gameObject.SetActive(false);

        dailyRewardButtons[day].interactable = false;
    }

    private void SetGemText()
    {
        menuScreen.gemText.text = Utils.ToAbbreviatedNumber(dataManager.NumGem);
    }
}