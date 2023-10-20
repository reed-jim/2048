using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor.AddressableAssets.Build.Layout;
using UnityEngine;
using Newtonsoft.Json;

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

    public int[] skillCosts = { 125, 150, 75 };

    private string[] iapValues = { "Ad Free", "200", "1000", "5000", "20000", "50000", "100000" };

    public string[] IapValues
    {
        get => iapValues;
    }


    #region Management Variable

    private GameplayData _gameplayData;

    public GameplayData GameplayData
    {
        get => _gameplayData;
        set => _gameplayData = value;
    }

    #endregion

    private void Awake()
    {
        LoadBestScore();
        GameplayData = LoadGameplayData();
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

    public void SaveGameplayData(int turn, int[,] columnBlockIndexes, float scoreNumber,
        char? scoreLetter, Block[] blockControllers)
    {
        GameplayData data = new GameplayData(turn, columnBlockIndexes, scoreNumber, scoreLetter, blockControllers);

        using (StreamWriter file = File.CreateText(@"Assets/Data/Gameplay/gameplayData.json"))
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Serialize(file, data);
        }
        //
        // File.WriteAllText(@"Assets/Data/Gameplay/gameplayData.json", JsonConvert.SerializeObject(data));
    }

    private GameplayData LoadGameplayData()
    {
        if (File.Exists(@"Assets/Data/Gameplay/gameplayData.json"))
        {
            return JsonConvert.DeserializeObject<GameplayData>(
                File.ReadAllText(@"Assets/Data/Gameplay/gameplayData.json"));
        }
        else
        {
            return new GameplayData();
        }
    }

    public bool IsEnoughGem(SkillType skillType)
    {
        if (numGem >= skillCosts[(int)skillType]) return true;
        else return false;
    }
}

public class GameplayData
{
    private bool _isSaved;
    private int _turn;
    private int[,] _columnBlockIndexes;
    private float _scoreNumber;
    private char? _scoreLetter;

    private SavedBlockData[] _blockData;

    public bool IsSaved
    {
        get => _isSaved;
        set => _isSaved = value;
    }

    public int Turn
    {
        get => _turn;
        set => _turn = value;
    }

    public int[,] ColumnBlockIndexes
    {
        get => _columnBlockIndexes;
        set => _columnBlockIndexes = value;
    }

    public float ScoreNumber
    {
        get => _scoreNumber;
        set => _scoreNumber = value;
    }

    public char? ScoreLetter
    {
        get => _scoreLetter;
        set => _scoreLetter = value;
    }

    public SavedBlockData[] BlockData
    {
        get => _blockData;
        set => _blockData = value;
    }

    public GameplayData()
    {
    }

    public GameplayData(int turn, int[,] columnBlockIndexes, float scoreNumber,
        char? scoreLetter, Block[] blockControllers)
    {
        _isSaved = true;
        _turn = turn;
        _columnBlockIndexes = columnBlockIndexes;
        _scoreNumber = scoreNumber;
        _scoreLetter = scoreLetter;

        _blockData = new SavedBlockData[blockControllers.Length];

        for (int i = 0; i < blockControllers.Length; i++)
        {
            _blockData[i] = new SavedBlockData();

            _blockData[i].Number = blockControllers[i].Number;
            _blockData[i].Letter = blockControllers[i].Letter;
            _blockData[i].Value = blockControllers[i].Value;
            _blockData[i].ColorIndex = blockControllers[i].ColorIndex;
        }
    }
}

public class SavedBlockData
{
    private float _number;
    private char? _letter;
    private string _value;
    private int _colorIndex;

    public float Number
    {
        get => _number;
        set => _number = value;
    }

    public char? Letter
    {
        get => _letter;
        set => _letter = value;
    }

    public string Value
    {
        get => _value;
        set => _value = value;
    }

    public int ColorIndex { get; set; }
}