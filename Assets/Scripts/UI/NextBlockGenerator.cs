using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NextBlockGenerator : MonoBehaviour
{
    [SerializeField] private Image blockImage;
    [SerializeField] private TMP_Text blockNumberText;

    [SerializeField] private int nextColorIndex;
    [SerializeField] private string nextBlockValue;

    public int NextColorIndex { get; set; }
    public string NextBlockValue { get; set; }

    public void GenerateNewBlock()
    {
        NextColorIndex = Random.Range(0, 5);

        NextBlockValue = "2";

        blockImage.color = Constants.AllBlockColors[NextColorIndex];
        blockNumberText.text = NextBlockValue;
    }
}