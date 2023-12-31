using System;
using System.IO;
using Newtonsoft.Json;
using PrimeTween;
using UnityEngine;
using UnityEngine.UIElements;

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

    private bool _isUserFirstSelectTheme;

    private DailyRewardData _dailyRewardData;

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

    public bool IsUserFirstSelectTheme {
        get => _isUserFirstSelectTheme;
    }

    public DailyRewardData DailyRewardData
    {
        get => _dailyRewardData;
        set => _dailyRewardData = value;
    }

    public int[] skillCosts = { 125, 150, 75 };

    private string[] productIds = { "remove_ad", "gem200", "gem500", "gem1000", "gem5000", "gem20000", "gem50000", "gem100000" };
    private string[] iapValues = { "Remove Ad", "200", "500", "1000", "5000", "20000", "50000", "100000" };
    private string[] iapCosts = { "2.99", "Watch Ad", "0.99", "1.99", "3.99", "5.99", "9.99", "10.99" };
    private int[] dailyRewards = { 50, 110, 180, 260, 350, 450, 1999, };

    public string[] ProductIds
    {
        get => productIds;
    }

    public string[] IapValues
    {
        get => iapValues;
    }

    public string[] IapCosts
    {
        get => iapCosts;
    }

    public int[] DailyRewards
    {
        get => dailyRewards;
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
    private string _gameplayDataPath = "Assets/Data/Gameplay/gameplay_data.json";
    private string _generalDataPath = "Assets/Data/Gameplay/general_data.json";
    private string _iapDataPath = "Assets/Data/Gameplay/iap_data.json";
    private string _dailyRewardDataPath = "Assets/Data/Gameplay/daily_reward_data.json";
    private string _settingDataPath = "Assets/Data/Gameplay/setting_data.json";
#elif UNITY_ANDROID
    private string _gameplayDataPath = Path.Combine(Application.persistentDataPath,"gameplay_data.json");
    private string _generalDataPath = Path.Combine(Application.persistentDataPath,"general_data.json");
    private string _iapDataPath = Path.Combine(Application.persistentDataPath,"iap_data.json");
    private string _dailyRewardDataPath = Path.Combine(Application.persistentDataPath,"daily_reward_data.json");
    private string _settingDataPath = Path.Combine(Application.persistentDataPath,"setting_data.json");
#endif

    [Header("EVENT")]
    [SerializeField] private ScriptableEventNoParam onGemUpdatedEvent;

    private void Awake()
    {
        GeneralData generalData = LoadGeneralData();

        _scoreNumber = generalData.ScoreNumber;
        _scoreLetter = generalData.ScoreLetter;
        _bestScoreNumber = generalData.BestScoreNumber;
        _bestScoreLetter = generalData.BestScoreLetter;
        _bestBlockNumber = generalData.BestBlockNumber;
        _bestBlockLetter = generalData.BestBlockLetter;
        _bestBlockColorIndex = generalData.BestBlockColorIndex;

        IAPData iapData = LoadIAPData();

        _numGem = iapData.NumGem;
        _isAdRemoved = iapData.IsAdRemoved;

        _dailyRewardData = LoadDailyRewardData();

        GameplayData = LoadGameplayData();

        SettingData settingData = LoadSettingData();

        ThemePicker.value = settingData.Theme;

        _isUserFirstSelectTheme = settingData.IsFirstSelectTheme;
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

    #region SAVE/LOAD
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

    public void SaveIAPData()
    {
        IAPData iapData = new IAPData(_numGem, _isAdRemoved);

        File.WriteAllText(_iapDataPath, JsonConvert.SerializeObject(iapData));
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
            return JsonConvert.DeserializeObject<IAPData>(File.ReadAllText(_iapDataPath));
        }
        else
        {
            return new IAPData();
        }
    }

    public void SaveDailyRewardData()
    {
        _dailyRewardData.NumDayGetReward++;
        _dailyRewardData.LastTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        File.WriteAllText(_dailyRewardDataPath, JsonConvert.SerializeObject(_dailyRewardData));
    }

    public DailyRewardData LoadDailyRewardData()
    {
        if (File.Exists(_dailyRewardDataPath))
        {
            return JsonConvert.DeserializeObject<DailyRewardData>(
                File.ReadAllText(_dailyRewardDataPath));
        }
        else
        {
            return new DailyRewardData();
        }
    }

    public bool IsDailyRewardAvailable()
    {
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        long lastTotalDay = (_dailyRewardData.LastTimestamp - _dailyRewardData.LastTimestamp % 86400) / 86400;
        long curTotalDay = (now - now % 86400) / 86400;

        if (curTotalDay - lastTotalDay >= 1)
        {
            return true;
        }
        else
        {
            return false;
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

    public void SaveSettingData(Constants.Theme theme)
    {
        SettingData data = new SettingData(theme);

        Save(data, _settingDataPath);
    }

    private SettingData LoadSettingData()
    {
        if (File.Exists(_settingDataPath))
        {
            return JsonConvert.DeserializeObject<SettingData>(
                File.ReadAllText(_settingDataPath));
        }
        else
        {
            return new SettingData();
        }
    }
    #endregion

    #region UTIL
    private void Save<T>(T data, string dataPath)
    {
        using (StreamWriter file = File.CreateText(dataPath))
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Serialize(file, data);
        }
    }

    public void ResetGameplayData()
    {
        GameplayData data = new GameplayData();

        File.WriteAllText(_gameplayDataPath, JsonConvert.SerializeObject(data));

        // using (StreamWriter file = File.CreateText(_gameplayDataPath))
        // {
        //     JsonSerializer serializer = new JsonSerializer();
        //     serializer.Serialize(file, data);
        // }
    }

    public bool IsEnoughGem(SkillType skillType)
    {
        if (_numGem >= skillCosts[(int)skillType]) return true;
        else return false;
    }

    public void SpendGem(int numGem)
    {
        _numGem -= numGem;
        onGemUpdatedEvent.Raise();

        SaveIAPData();
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
    #endregion
}

#region DATA CLASS
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
        _isSaved = false;
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

public class DailyRewardData
{
    private int _numDayGetReward;
    private long _lastTimestamp;

    public int NumDayGetReward
    {
        get => _numDayGetReward;
        set => _numDayGetReward = value;
    }

    public long LastTimestamp
    {
        get => _lastTimestamp;
        set => _lastTimestamp = value;
    }

    public DailyRewardData()
    {
    }

    public DailyRewardData(int numDayGetReward, int newTimestamp)
    {
        _numDayGetReward = numDayGetReward;
        _lastTimestamp = newTimestamp;
    }
}

public class SettingData
{
    private Constants.Theme _theme;
    private bool _isFirstSelectTheme;

    public Constants.Theme Theme
    {
        get => _theme;
        set => _theme = value;
    }

    public bool IsFirstSelectTheme {
        get => _isFirstSelectTheme;
        set => _isFirstSelectTheme = value;
    }

    public SettingData()
    {

    }

    public SettingData(Constants.Theme theme)
    {
        _theme = theme;
        _isFirstSelectTheme = true;
    }
}
#endregion