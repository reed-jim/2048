using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private float _scoreNumber;
    private char? _scoreLetter;
    private float _bestScoreNumber;
    private char? _bestScoreLetter;
    private float _bestBlockNumber;
    private char? _bestBlockLetter;
    private int _bestBlockColorIndex;

    private int _numGem;
    private bool _isAdRemoved;

    public float ScoreNumber
    {
        get => _scoreNumber;
    }

    public char? ScoreLetter
    {
        get => _scoreLetter;
    }

    public float BestScoreNumber
    {
        get => _bestScoreNumber;
        set => _bestScoreNumber = value;
    }

    public char? BestScoreLetter
    {
        get => _bestScoreLetter;
        set => _bestScoreLetter = value;
    }

    public float BestBlockNumber
    {
        get => _bestBlockNumber;
        set => _bestBlockNumber = value;
    }

    public char? BestBlockLetter
    {
        get => _bestBlockLetter;
        set => _bestBlockLetter = value;
    }

    public int BestBlockColorIndex
    {
        get => _bestBlockColorIndex;
        set => _bestBlockColorIndex = value;
    }

    public int NumGem
    {
        get => _numGem;
        set => _numGem = value;
    }

    public bool IsAdRemoved
    {
        get => _isAdRemoved;
        set => _isAdRemoved = value;
    }

    public int[] skillCosts = { 125, 150, 75 };

    private string[] iapValues = { "Remove Ad", "200", "200", "1000", "5000", "20000", "50000", "100000" };
    private string[] iapCosts = { "2.99", "Watch Ad", "0.99", "2.99", "3.99", "5.99", "11.99", "12.99" };

    public string[] IapValues
    {
        get => iapValues;
    }

    public string[] IapCosts
    {
        get => iapCosts;
    }

    #region Management Variable

    private GameplayData _gameplayData;

    public GameplayData GameplayData
    {
        get => _gameplayData;
        set => _gameplayData = value;
    }

    #endregion

#if UNITY_EDITOR
    private string _gameplayDataPath = "Assets/Data/Gameplay/gameplayData.json";
    private string _generalDataPath = "Assets/Data/Gameplay/generalData.json";
    private string _iapDataPath = "Assets/Data/Gameplay/iapData.json";
#elif UNITY_ANDROID
    private string _gameplayDataPath = Path.Combine(Application.persistentDataPath,"gameplayData.json");
    private string _generalDataPath = Path.Combine(Application.persistentDataPath,"generalData.json");
    private string _iapDataPath = Path.Combine(Application.persistentDataPath,"iapData.json");
#endif

    private void Awake()
    {
        GeneralData generalData = LoadGeneralData();

        _scoreNumber = generalData.ScoreNumber;
        _scoreLetter = generalData.ScoreLetter;
        _bestScoreNumber = generalData.BestScoreNumber;
        _bestScoreLetter = generalData.BestScoreLetter;

        IAPData iapData = LoadIAPData();

        _numGem = iapData.NumGem;
        _isAdRemoved = iapData.IsAdRemoved;
        
        GameplayData = LoadGameplayData();
    }

    // public void SaveBestScore()
    // {
    //     PlayerPrefs.SetFloat("best_score_number", bestScoreNumber);
    //
    //     if (bestScoreLetter != null)
    //     {
    //         PlayerPrefs.SetString("best_score_letter", bestScoreLetter.ToString());
    //     }
    // }
    //
    // private void LoadBestScore()
    // {
    //     bestScoreNumber = PlayerPrefs.GetFloat("best_score_number", 0);
    //
    //     string bestScoreLetterInString = PlayerPrefs.GetString("best_score_letter", "");
    //
    //     bestScoreLetter = bestScoreLetterInString == "" ? null : bestScoreLetterInString[0];
    // }

    public void SaveGeneralData(float scoreNumber, char? scoreLetter)
    {
        GeneralData generalData = new GeneralData(scoreNumber, scoreLetter, _bestScoreNumber, _bestScoreLetter,
            _bestBlockNumber, _bestBlockLetter, _bestBlockColorIndex);

        File.WriteAllText(_generalDataPath, JsonConvert.SerializeObject(generalData));
    }

    public GeneralData LoadGeneralData()
    {
        if (File.Exists(_generalDataPath))
        {
            return JsonConvert.DeserializeObject<GeneralData>(
                File.ReadAllText(_generalDataPath));
        }
        else
        {
            return new GeneralData();
        }
    }

    public void SaveIAPData(int numGem, bool isAdRemoved)
    {
        IAPData iapData = new IAPData(numGem, isAdRemoved);

        File.WriteAllText(_iapDataPath, JsonConvert.SerializeObject(iapData));
    }

    public IAPData LoadIAPData()
    {
        if (File.Exists(_iapDataPath))
        {
            return JsonConvert.DeserializeObject<IAPData>(
                File.ReadAllText(_iapDataPath));
        }
        else
        {
            return new IAPData();
        }
    }

    public void SaveGameplayData(int turn, int[,] columnBlockIndexes, float scoreNumber,
        char? scoreLetter, Block[] blockControllers)
    {
        GameplayData data = new GameplayData(turn, columnBlockIndexes, scoreNumber, scoreLetter, blockControllers);

        using (StreamWriter file = File.CreateText(_gameplayDataPath))
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Serialize(file, data);
        }
    }

    private GameplayData LoadGameplayData()
    {
        if (File.Exists(_gameplayDataPath))
        {
            return JsonConvert.DeserializeObject<GameplayData>(
                File.ReadAllText(_gameplayDataPath));
        }
        else
        {
            return new GameplayData();
        }
    }

    public bool IsEnoughGem(SkillType skillType)
    {
        if (_numGem >= skillCosts[(int)skillType]) return true;
        else return false;
    }

    public bool IsNewBestScore(float scoreNumber, char? scoreLetter)
    {
        if (_bestScoreLetter == null)
        {
            if (scoreLetter == null)
            {
                if (scoreNumber > _bestScoreNumber) return true;
            }
            else
            {
                return true;
            }
        }
        else
        {
            if (scoreLetter == null)
            {
            }
            else
            {
                if ((int)scoreLetter > (int)_bestScoreLetter)
                {
                    return true;
                }
                else if ((int)scoreLetter == (int)_bestScoreLetter)
                {
                    if (scoreNumber > _bestScoreNumber)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public bool IsNewBestBlock(float scoreNumber, char? scoreLetter)
    {
        if (_bestBlockLetter == null)
        {
            if (scoreLetter == null)
            {
                if (scoreNumber > _bestBlockNumber) return true;
            }
            else
            {
                return true;
            }
        }
        else
        {
            if (scoreLetter == null)
            {
            }
            else
            {
                if ((int)scoreLetter > (int)_bestBlockLetter)
                {
                    return true;
                }
                else if ((int)scoreLetter == (int)_bestBlockLetter)
                {
                    if (scoreNumber > _bestBlockNumber)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
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

public class GeneralData
{
    private float _scoreNumber;
    private char? _scoreLetter;
    private float _bestScoreNumber;
    private char? _bestScoreLetter;
    private float _bestBlockNumber;
    private char? _bestBlockLetter;
    private int _bestBlockColorIndex;

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

    public float BestScoreNumber
    {
        get => _bestScoreNumber;
        set => _bestScoreNumber = value;
    }

    public char? BestScoreLetter
    {
        get => _bestScoreLetter;
        set => _bestScoreLetter = value;
    }

    public float BestBlockNumber
    {
        get => _bestBlockNumber;
        set => _bestBlockNumber = value;
    }

    public char? BestBlockLetter
    {
        get => _bestBlockLetter;
        set => _bestBlockLetter = value;
    }

    public int BestBlockColorIndex
    {
        get => _bestBlockColorIndex;
        set => _bestBlockColorIndex = value;
    }

    public GeneralData()
    {
    }

    public GeneralData(float scoreNumber, char? scoreLetter, float bestScoreNumber, char? bestScoreLetter,
        float bestBlockNumber, char? bestBlockLetter, int bestBlockColorIndex)
    {
        _scoreNumber = scoreNumber;
        _scoreLetter = scoreLetter;
        _bestScoreNumber = bestScoreNumber;
        _bestScoreLetter = bestScoreLetter;
        _bestBlockNumber = bestBlockNumber;
        _bestBlockLetter = bestBlockLetter;
        _bestBlockColorIndex = bestBlockColorIndex;
    }
}

public class IAPData
{
    private int _numGem;
    private bool _isAdRemoved;

    public int NumGem
    {
        get => _numGem;
        set => _numGem = value;
    }

    public bool IsAdRemoved
    {
        get => _isAdRemoved;
        set => _isAdRemoved = value;
    }

    public IAPData()
    {
    }

    public IAPData(int numGem, bool isAdRemoved)
    {
        _numGem = numGem;
        _isAdRemoved = isAdRemoved;
    }
}