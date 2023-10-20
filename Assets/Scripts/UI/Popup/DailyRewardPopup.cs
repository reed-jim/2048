using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardPopup : MonoBehaviour
{
    [Header("UI")] [SerializeField] private RectTransform container;
    [SerializeField] private Button dailyRewardButtonPrefab;
    [SerializeField] private Button[] dailyRewardButtons;
    private RectTransform[] dailyRewardButtonRTs;

    private Vector2 _screenSize;

    private void Start()
    {
        _screenSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);

        Spawn();
    }

    private void Spawn()
    {
        dailyRewardButtons = new Button[7];
        dailyRewardButtonRTs = new RectTransform[7];

        for (int i = 0; i < dailyRewardButtons.Length; i++)
        {
            dailyRewardButtons[i] = Instantiate(dailyRewardButtonPrefab, container);
            dailyRewardButtonRTs[i] = dailyRewardButtons[i].GetComponent<RectTransform>();
        }

        container.sizeDelta = new Vector2(0.8f * _screenSize.x, 0.5f * _screenSize.y);

        Vector3 position = Vector3.zero;

        for (int i = 0; i < dailyRewardButtons.Length; i++)
        {
            position.z = 0;

            if (i < 4)
            {
                dailyRewardButtonRTs[i].sizeDelta = new Vector2(container.sizeDelta.x / 4, container.sizeDelta.y / 3);

                position.x = -0.5f * (container.sizeDelta.x - dailyRewardButtonRTs[i].sizeDelta.x) +
                             i * dailyRewardButtonRTs[i].sizeDelta.x;
                position.y = 0.5f * (container.sizeDelta.y - dailyRewardButtonRTs[0].sizeDelta.y);
            }
            else if (i >= 4 && i < 6)
            {
                dailyRewardButtonRTs[i].sizeDelta = new Vector2(container.sizeDelta.x / 2, container.sizeDelta.y / 3);

                position.x = -0.5f * (container.sizeDelta.x - dailyRewardButtonRTs[i].sizeDelta.x) +
                             (i - 4) * dailyRewardButtonRTs[i].sizeDelta.x;
                position.y = 0.5f * (container.sizeDelta.y - dailyRewardButtonRTs[0].sizeDelta.y) -
                             dailyRewardButtonRTs[0].sizeDelta.y;
            }
            else
            {
                dailyRewardButtonRTs[i].sizeDelta = new Vector2(container.sizeDelta.x / 1, container.sizeDelta.y / 3);

                position.x = 0;
                position.y = 0.5f * (container.sizeDelta.y - dailyRewardButtonRTs[0].sizeDelta.y) -
                             2 * dailyRewardButtonRTs[0].sizeDelta.y;
            }

            dailyRewardButtonRTs[i].localPosition = position;
        }

        for (int i = 0; i < dailyRewardButtons.Length; i++)
        {
            dailyRewardButtonRTs[i].sizeDelta *= 0.9f;
        }
    }
}