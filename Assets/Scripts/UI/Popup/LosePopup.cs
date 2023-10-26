using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using TMPro;

public class LosePopup : Popup
{
    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text gemText;
    [SerializeField] private Image gemImage;

    [SerializeField] private Button replayButton;

    private RectTransform _replayButtonRT;

    [Header("REFERENCE")]
    [SerializeField] private DataManager dataManager;

    protected override void InitUI()
    {
        base.InitUI();

        _replayButtonRT = replayButton.GetComponent<RectTransform>();

        SetTextFontSize(scoreText, 0.06f);
        SetTextFontSize(gemText, 0.05f);

        SetLocalPositionY(scoreText.rectTransform, 0.1f * container.sizeDelta.y);

        SetSize(_replayButtonRT, 0.35f * container.sizeDelta.x, (0.35f / 2.55f) * container.sizeDelta.x);
        SetLocalPositionY(_replayButtonRT, -0.15f * container.sizeDelta.y);

        closeButton.gameObject.SetActive(false);

        replayButton.onClick.AddListener(Replay);
    }

    public void ShowPopup(float scoreNumber, char? scoreLetter)
    {
        base.ShowPopup();

        scoreText.text = "Your score: " + scoreNumber.ToString("F2") + scoreLetter;
        SetTextPreferredSize(scoreText);

        int numGem;

        if (scoreLetter != null)
        {
            numGem = (int)scoreNumber * ((int)(scoreLetter) - 1);
        }
        else
        {
            numGem = 3 * (int)scoreNumber;
        }

        SetGemText(numGem);
    }

    private void SetGemText(int numGemReward)
    {
        gemText.text = "+" + numGemReward;
        SetTextPreferredSize(gemText);

        SetLocalPosition(gemText.rectTransform, -0.05f * container.sizeDelta.x, 0f * container.sizeDelta.y);
        gemImage.rectTransform.localPosition =
            new Vector3(
                gemText.rectTransform.localPosition.x +
                0.6f * (gemText.preferredWidth + gemImage.rectTransform.sizeDelta.x), gemText.rectTransform.localPosition.y, 0);
    }

    private void Replay()
    {
        dataManager.ResetGameplayData();

        Addressables.LoadSceneAsync("Gameplay");
    }
}
