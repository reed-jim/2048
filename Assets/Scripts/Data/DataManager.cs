using System;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    [SerializeField] private float bestScoreNumber;
    [SerializeField] private char? bestScoreLetter;

    [SerializeField] private int numGem;

    public float BestScoreNumber { get; set; }
    public char? BestScoreLetter { get; set; }

    public int NumGem
    {
        get => numGem;
        set => numGem = value;
    }

    private string[] iapValues = { "Ad Free", "200", "1000", "5000", "20000", "50000", "100000" };

    public string[] IapValues
    {
        get => iapValues;
    }

    private void Awake()
    {
        LoadBestScore();
    }

    public void SaveBestScore()
    {
        PlayerPrefs.SetFloat("best_score_number", bestScoreNumber);

        if (bestScoreLetter != null)
        {
            PlayerPrefs.SetString("best_score_letter", bestScoreLetter.ToString());
        }
    }

    private void LoadBestScore()
    {
        bestScoreNumber = PlayerPrefs.GetFloat("best_score_number", 0);

        string bestScoreLetterInString = PlayerPrefs.GetString("best_score_letter", "");

        bestScoreLetter = bestScoreLetterInString == "" ? null : bestScoreLetterInString[0];
    }
}