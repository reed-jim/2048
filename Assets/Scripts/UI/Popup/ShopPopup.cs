using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopPopup : Popup
{
    [Header("UI")]
    [SerializeField] private Button itemButtonPrefab;
    [SerializeField] private Button[] itemButtons;
    private RectTransform[] _itemButtonRTs;
    private Button[] buyButtons;
    private TMP_Text[] _itemCostText;
    private TMP_Text[] _itemValueText;

    [Space] [Header("REFERENCE")] [SerializeField]
    private DataManager dataManager;

    [SerializeField] private AdManager adManager;

    private void Start()
    {
        Spawn();
    }

    private void Spawn()
    {
        itemButtons = new Button[7];
        _itemButtonRTs = new RectTransform[7];
        buyButtons = new Button[7];
        _itemValueText = new TMP_Text[7];

        for (int i = 0; i < itemButtons.Length; i++)
        {
            itemButtons[i] = Instantiate(itemButtonPrefab, container);
            _itemButtonRTs[i] = itemButtons[i].GetComponent<RectTransform>();
            _itemValueText[i] = _itemButtonRTs[i].GetChild(0).GetComponent<TMP_Text>();
            buyButtons[i] = _itemButtonRTs[i].GetChild(1).GetComponent<Button>();
        }

        for (int i = 0; i < itemButtons.Length; i++)
        {
            _itemButtonRTs[i].sizeDelta = new Vector2(0.9f * container.sizeDelta.x, 0.08f * container.sizeDelta.y);
            _itemButtonRTs[i].localPosition =
                new Vector3(0, 0.3f * container.sizeDelta.y - i * 1.15f * _itemButtonRTs[i].sizeDelta.y, 0);

            _itemValueText[i].text = dataManager.IapValues[i];

            if (i == 1)
            {
                buyButtons[i].onClick.AddListener(TestRewarded);
            }
        }
    }

    private void TestRewarded()
    {
        adManager.ShowRewardedAd(() => dataManager.NumGem += 100);
    }
}