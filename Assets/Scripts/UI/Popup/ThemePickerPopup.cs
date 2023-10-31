using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThemePickerPopup : Popup
{
    [SerializeField] private Image themeContainerPrefab;
    [SerializeField] private RectTransform scrollView;
    [SerializeField] private RectTransform themeCollection;
    [SerializeField] private Image[] themeContainers;
    private TMP_Text[] _themeNames;
    private Button[] _chooseButtons;
    private RectTransform[] _chooseButtonRTs;

    protected override void InitUI()
    {
        base.InitUI();

        int numTheme = System.Enum.GetValues(typeof(Constants.Theme)).Length;

        themeContainers = new Image[numTheme];
        _themeNames = new TMP_Text[numTheme];
        _chooseButtons = new Button[numTheme];
        _chooseButtonRTs = new RectTransform[numTheme];

        for (int i = 0; i < numTheme; i++)
        {
            themeContainers[i] = Instantiate(themeContainerPrefab, themeCollection);
            _themeNames[i] = themeContainers[i].rectTransform.GetChild(0).GetComponent<TMP_Text>();
            _chooseButtons[i] = themeContainers[i].GetComponent<Button>();
            _chooseButtonRTs[i] = _chooseButtons[i].GetComponent<RectTransform>();
        }

        SetSize(scrollView, container.sizeDelta.x, 0.8f * container.sizeDelta.y);
        SetLocalPositionY(scrollView, title.rectTransform.localPosition.y - 0.6f * (title.preferredHeight + scrollView.sizeDelta.y));
        SetSize(themeCollection, container.sizeDelta.x, 0.8f * container.sizeDelta.y);
        // SetLocalPositionY(themeCollection, title.rectTransform.localPosition.y - 0.6f * (title.preferredHeight + themeCollection.sizeDelta.y));

        for (int i = 0; i < numTheme; i++)
        {
            SetSize(themeContainers[i].rectTransform, 0.9f * container.sizeDelta.x, 0.09f * container.sizeDelta.y);
            SetLocalPositionY(themeContainers[i].rectTransform,
                0.5f * (themeCollection.sizeDelta.y - themeContainers[i].rectTransform.sizeDelta.y) - 1.1f * i * themeContainers[i].rectTransform.sizeDelta.y);
            themeContainers[i].color = Constants.GetColorInTheme((Constants.Theme)i)[0];

            SetTextFontSize(_themeNames[i], 0.06f);
            _themeNames[i].text = ((Constants.Theme)i).ToString();
            _themeNames[i].color = Constants.GetTextColorInTheme((Constants.Theme)i)[0];
            SetTextPreferredSize(_themeNames[i]);
            // SetLocalPositionX(_themeNames[i].rectTransform, -0.4f * (themeContainers[i].rectTransform.sizeDelta.x - _themeNames[i].preferredWidth));

            // SetSize(_chooseButtonRTs[i], 1f * themeContainers[i].rectTransform.sizeDelta.y, 0.5f * themeContainers[i].rectTransform.sizeDelta.y);
            // SetLocalPositionX(_chooseButtonRTs[i], 0.4f * (themeContainers[i].rectTransform.sizeDelta.x - _chooseButtonRTs[i].sizeDelta.x));

            int index = i;
            _chooseButtons[i].onClick.AddListener(() => ChooseTheme((Constants.Theme)index));
        }
    }

    private void ChooseTheme(Constants.Theme theme)
    {
        ThemePicker.value = theme;
        Debug.Log("test00 " + theme);
        Debug.Log("test01 " + ThemePicker.value);
        gameManager.ChangeTheme();
    }
}
