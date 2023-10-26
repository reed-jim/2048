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

    [Header("REFERENCE")] [SerializeField] private GameManager gameManager;

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
        if (exceptColorIndex != -1)
        {
            NextColorIndex = exceptColorIndex;
            NextBlockValue = "2";

            while (NextColorIndex == exceptColorIndex)
            {
                NextColorIndex = Random.Range(0, 5);
            }
        }
        else
        {
            bool isBonusPick = Random.Range(0, 0) == 0;

            if (isBonusPick)
            {
                gameManager.GetRandomAvailableBlockData(out nextBlockValue, out nextColorIndex);

                if (nextColorIndex == -1)
                {
                    NextColorIndex = Random.Range(0, 5);
                    NextBlockValue = "2a";
                }
            }
            else
            {
                NextColorIndex = Random.Range(0, 5);
                NextBlockValue = "2";
            }
        }

        // blockImage.color = Constants.AllBlockColors[NextColorIndex];
        blockNumberText.text = NextBlockValue;

        PlayEffect(Constants.AllBlockColors[NextColorIndex], Constants.AllBlockTextColors[NextColorIndex]);
    }

    private void PlayEffect(Color newColor, Color newTextColor)
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
}