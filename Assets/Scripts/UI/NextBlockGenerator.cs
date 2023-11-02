using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NextBlockGenerator : MonoBehaviour
{
    [SerializeField] private Image blockImage;
    [SerializeField] private TMP_Text blockNumberText;

    [SerializeField] private int nextColorIndex;
    [SerializeField] private string nextBlockValue;

    [Header("REFERENCE")][SerializeField] private GameManager gameManager;

    public int NextColorIndex
    {
        get => nextColorIndex;
        set => nextColorIndex = value;
    }

    public string NextBlockValue
    {
        get => nextBlockValue;
        set => nextBlockValue = value;
    }

    public void GenerateNewBlock(int exceptColorIndex = -1)
    {
        RandomNextBlock(exceptColorIndex);

        blockNumberText.text = NextBlockValue;

        TweenColor(Constants.GetColorInTheme(ThemePicker.value)[NextColorIndex], Constants.GetTextColorInTheme(ThemePicker.value)[NextColorIndex]);
    }

    private void RandomNextBlockColorIndex(int exceptColorIndex = -1)
    {
        if (exceptColorIndex != -1)
        {
            List<int> remainingColorIndexes = new List<int>();

            for (int i = 0; i < 5; i++)
            {
                if (i != exceptColorIndex) remainingColorIndexes.Add(i);
            }

            NextColorIndex = remainingColorIndexes[Random.Range(0, 4)];
        }
        else
        {
            NextColorIndex = Random.Range(0, 5);
        }
    }

    private void RandomNextBlock(int exceptColorIndex = -1)
    {
        bool isBonusPick = Random.Range(0, 3) == 0;

        if (isBonusPick)
        {
            gameManager.GetRandomAvailableBlockData(out nextBlockValue, out nextColorIndex);

            if (nextColorIndex == -1)
            {
                RandomNextBlockColorIndex(exceptColorIndex);
                NextBlockValue = "2";
            }
        }
        else
        {
            RandomNextBlockColorIndex(exceptColorIndex);
            NextBlockValue = "2";
        }
    }

    private void TweenColor(Color newColor, Color newTextColor)
    {
        Tween.Custom(blockImage.color, newColor, duration: 0.35f, cycles: 2, cycleMode: CycleMode.Yoyo
                , onValueChange: newVal => blockImage.color = newVal)
            .SetCycles(true);

        Tween.Custom(blockNumberText.color, newTextColor, duration: 0.35f, cycles: 2, cycleMode: CycleMode.Yoyo
                , onValueChange: newVal => blockNumberText.color = newVal)
            .SetCycles(true);

        // Tween.Color(blockImage, 1.1f, duration: 0.2f, cycles: 2, cycleMode: CycleMode.Yoyo)
        //     .SetCycles(false);
    }

    public void ChangeTheme() {
        var theme = ThemePicker.value;

        TweenColor(Constants.GetColorInTheme(theme)[NextColorIndex], Constants.GetTextColorInTheme(theme)[NextColorIndex]);
    }
}