using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using TMPro;
using Unity.VisualScripting;
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

    [Header("REFERENCE")]
    [SerializeField] private DataManager dataManager;

    [Header("POPUP")]
    [SerializeField] private PausePopup pausePopup;

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
            SetText(_themeNames[i], ((Constants.Theme)i).ToString());
            _themeNames[i].color = Constants.GetTextColorInTheme((Constants.Theme)i)[0];

            int index = i;
            _chooseButtons[i].onClick.AddListener(() => ChooseTheme(index));
        }
    }

    private void ChooseTheme(int index)
    {
        Constants.Theme theme = (Constants.Theme)index;

        Vector3 initialScale = _chooseButtonRTs[index].localScale;

        Tween.Scale(_chooseButtonRTs[index], new Vector3(1.05f * initialScale.x, initialScale.y, 1), duration: 0.3f, cycles: 2, cycleMode: CycleMode.Yoyo)
            .OnComplete(() => OnAnimationCompleted())
            .SetCycles(false);

        Tween.Delay(0.1f).OnComplete(() => SetText(_themeNames[index], "Selected"));

        ThemePicker.value = theme;

        dataManager.SaveSettingData(theme);

        if(pausePopup != null) pausePopup.ClosePopup();

        if (gameManager != null) gameManager.ChangeTheme();

        void OnAnimationCompleted()
        {
            Tween.Delay(0.1f).OnComplete(() => SetText(_themeNames[index], ((Constants.Theme)index).ToString()));

            ClosePopup();
        }
    }
}
