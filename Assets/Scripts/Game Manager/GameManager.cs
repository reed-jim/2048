using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PrimeTween;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Random = UnityEngine.Random;

public enum MergeDirection
{
    Horizontal,
    Vertical
}

public enum InteractMode
{
    Normal,
    Swap,
    Destroy
}

public class GameManager : MonoBehaviour
{
    [Header("OBJECTS")][SerializeField] private GameObject blockCollection;
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private GameObject[] blocks;
    [SerializeField] private TrailRenderer[] trails;
    [SerializeField] private SpriteRenderer backgroundRenderer;
    private Block[] _blockControllers;
    private Rigidbody[] _blockRigidBodies;
    private Camera _mainCamera;

    [Header("UI")]
    [SerializeField]
    private TMP_Text blockNumberTextPrefab;

    [SerializeField] private RectTransform blockNumberTextCollection;
    private TMP_Text[] _blockNumberTexts;


    [Header("MANAGEMENT")]
    [SerializeField]
    private int turn;

    private bool _isPaused;
    private int newBestBlockIndex = -1;

    private InteractMode _interactMode = InteractMode.Normal;

    public InteractMode InteractMode
    {
        get => _interactMode;
        set => _interactMode = value;
    }

    private int[,] _columnBlockIndexes;
    private bool[,] _isBlockChecks;
    private float _scoreNumber;
    private char? _scoreLetter;
    private Stack<ChangeStackItem> _changeStack = new Stack<ChangeStackItem>();

    [Header("SWAP MODE")] private List<int> _selectedBlockIndexes = new List<int>();
    private List<Vector2Int> _selectedBlockPositionIndexes = new List<Vector2Int>();

    [Header("REFERENCE")]
    [SerializeField]
    private BoardGenerator boardGenerator;

    [SerializeField] private NextBlockGenerator nextBlockGenerator;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private DataManager dataManager;
    [SerializeField] private AdManager adManager;

    [Header("POPUP")]
    [SerializeField] private LosePopup losePopup;

    [Header("CUSTOM")]
    [SerializeField]
    private int numBlock;

    [SerializeField] private float blockWidth;
    [SerializeField] private float highestBlockPositionY;
    [SerializeField] private Ease moveEasing;
    [SerializeField] private float moveDuration;
    [SerializeField] private float mergeDuration;
    [SerializeField] private bool isDebug;
    [SerializeField] private float delaySwipeTime;

    [Header("THEME")]


    private int _numBlockPerColumn;
    private float _blockDistance;

    private int _currentPoolBlockIndex;

    private bool _isAnotherBlockMoving;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        Application.targetFrameRate = 60;

        _numBlockPerColumn = numBlock / boardGenerator.NumLane;

        _mainCamera = Camera.main;

        blocks = new GameObject[numBlock];
        trails = new TrailRenderer[numBlock];
        _blockControllers = new Block[numBlock];
        _blockRigidBodies = new Rigidbody[numBlock];
        _columnBlockIndexes = new int[boardGenerator.NumLane, _numBlockPerColumn];
        _isBlockChecks = new bool[boardGenerator.NumLane, _numBlockPerColumn];

        _blockNumberTexts = new TMP_Text[numBlock];

        for (int i = 0; i < boardGenerator.NumLane; i++)
        {
            for (int j = 0; j < _numBlockPerColumn; j++)
            {
                _columnBlockIndexes[i, j] = -1;
            }
        }

        blockWidth = 0.5f * boardGenerator.LaneWidth;

        for (int i = 0; i < numBlock; i++)
        {
            blocks[i] = Instantiate(blockPrefab, blockCollection.transform);
            trails[i] = blocks[i].transform.GetChild(0).gameObject.GetComponent<TrailRenderer>();
            _blockControllers[i] = blocks[i].GetComponent<Block>();
            _blockRigidBodies[i] = blocks[i].GetComponent<Rigidbody>();

            blocks[i].transform.localScale = blockWidth * Vector3.one;
            blocks[i].SetActive(false);

            _blockNumberTexts[i] = Instantiate(blockNumberTextPrefab, blockNumberTextCollection);
            _blockNumberTexts[i].fontSize = 0.04f * Screen.currentResolution.width;
            _blockNumberTexts[i].gameObject.SetActive(false);
        }

        _blockDistance = 2.2f * blockWidth;
        highestBlockPositionY =
            (1 - 0.12f * 2 - uiManager.SafeAreaTopSizeY / Screen.currentResolution.height) *
            _mainCamera.orthographicSize - _blockDistance;

        nextBlockGenerator.GenerateNewBlock();

        LoadData();

        ApplySettingData();

        AudioManager.Instance.PlayBackgroundSound();
    }

    #region SAVE/LOAD
    private void LoadData()
    {
        LoadGeneralData();

        GameplayData data = dataManager.GameplayData;

        if (data.IsSaved)
        {
            turn = data.Turn;
            _columnBlockIndexes = data.ColumnBlockIndexes;
            _scoreNumber = data.ScoreNumber;
            _scoreLetter = data.ScoreLetter;

            for (int i = 0; i < data.BlockData.Length; i++)
            {
                _blockControllers[i].Number = data.BlockData[i].Number;
                _blockControllers[i].Letter = data.BlockData[i].Letter;
                _blockControllers[i].Value = data.BlockData[i].Value;
                _blockControllers[i].ColorIndex = data.BlockData[i].ColorIndex;
            }

            for (int i = 0; i < boardGenerator.NumLane; i++)
            {
                for (int j = 0; j < _numBlockPerColumn; j++)
                {
                    if (_columnBlockIndexes[i, j] != -1)
                    {
                        int blockIndex = _columnBlockIndexes[i, j];

                        _blockControllers[blockIndex].transform.position = new Vector3(
                            boardGenerator.LanePositions[i].x,
                            highestBlockPositionY - (_numBlockPerColumn - 1 - j) * _blockDistance,
                            _blockControllers[blockIndex].transform.position.z);
                        _blockControllers[blockIndex].SetColor(blockNumber: _blockNumberTexts[blockIndex]);
                        blocks[blockIndex].SetActive(true);

                        Vector3 blockPosition = _mainCamera.WorldToScreenPoint(blocks[blockIndex].transform.position);
                        _blockNumberTexts[blockIndex].transform.position = blockPosition;
                        _blockControllers[blockIndex].SetValue(_blockNumberTexts[blockIndex]);
                        // _blockNumberTexts[blockIndex].text = _blockControllers[blockIndex].Value;
                        _blockNumberTexts[blockIndex].gameObject.SetActive(true);
                    }
                }
            }

            uiManager.SetScoreText(_scoreNumber, _scoreLetter);
        }
    }

    private void LoadGeneralData()
    {
        uiManager.SetBestScore(dataManager.BestScoreNumber, dataManager.BestScoreLetter);
    }

    private void ApplySettingData()
    {
        Addressables.LoadAssetAsync<Sprite>(Constants.backgroundAddressableKeys[(int)ThemePicker.value]).Completed += OnSpriteLoaded;
    }
    #endregion

    #region SHOOT BLOCK
    public void OnSwipe()
    {
        if (_isPaused) return;

        if (_isAnotherBlockMoving) return;

        if (turn > 0) _currentPoolBlockIndex = FindAvailableBlockIndex();
        if (_currentPoolBlockIndex == -1)
        {
            uiManager.ShowLosePopup();

            return;
        }

        int rowIndex;
        int columnIndex;

        GetDestinationPositionIndex(out rowIndex, out columnIndex);

        if (columnIndex == -1) return;

        newBestBlockIndex = -1;

        SpawnThenMoveBlock(rowIndex, columnIndex);

        _isAnotherBlockMoving = true;

        AudioManager.Instance.PlayShootBlockSound();
    }

    private void SpawnThenMoveBlock(int rowIndex, int columnIndex)
    {
        _blockControllers[_currentPoolBlockIndex].PositionIndex = new Vector2(rowIndex, columnIndex);

        Vector3 start = new Vector3(
            boardGenerator.LanePositions[rowIndex].x,
            -7f,
            blocks[0].transform.position.z
        );

        Vector3 end = new Vector3(
            boardGenerator.LanePositions[rowIndex].x,
            highestBlockPositionY - (_numBlockPerColumn - 1 - columnIndex) * _blockDistance,
            blocks[0].transform.position.z
        );

        blocks[_currentPoolBlockIndex].transform.position = start;
        _blockControllers[_currentPoolBlockIndex].SetColor(nextBlockGenerator.NextColorIndex, _blockNumberTexts[_currentPoolBlockIndex]);
        SetTrail();
        blocks[_currentPoolBlockIndex].SetActive(true);

        _blockControllers[_currentPoolBlockIndex].Value = nextBlockGenerator.NextBlockValue;
        _blockControllers[_currentPoolBlockIndex].SetNumberAndLetter(_blockControllers[_currentPoolBlockIndex].Value);
        _blockNumberTexts[_currentPoolBlockIndex].text = _blockControllers[_currentPoolBlockIndex].Value;
        _blockNumberTexts[_currentPoolBlockIndex].gameObject.SetActive(true);

        MoveBlock(_currentPoolBlockIndex, rowIndex, columnIndex, end, isFirstTime: true);
    }

    private void SetTrail()
    {
        var theme = ThemePicker.value;

        trails[_currentPoolBlockIndex].startColor =
            Constants.GetColorInTheme(theme)[nextBlockGenerator.NextColorIndex] - new Color(0, 0, 0, 0.1f);
        trails[_currentPoolBlockIndex].endColor =
            Constants.GetColorInTheme(theme)[nextBlockGenerator.NextColorIndex] - new Color(0, 0, 0, 1);
        trails[_currentPoolBlockIndex].gameObject.SetActive(true);
    }
    #endregion

    #region MOVE BLOCK
    private void MoveBlock(int blockIndex, int rowIndex, int columnIndex, Vector3 end, bool isCheckMergeSingle = true,
        bool isLast = false, bool isFirstTime = false)
    {
        _blockControllers[blockIndex].IsMoving = true;

        StartCoroutine(TextFollowBlock(blockIndex));

        Tween.Position(blocks[blockIndex].transform, end,
                duration: isFirstTime ? moveDuration : moveDuration / 2f, ease: moveEasing)
            .OnComplete(() => OnBlockMoved(blockIndex, rowIndex, columnIndex, isCheckMergeSingle, isLast, isFirstTime));
    }

    private void GetDestinationPositionIndex(out int rowIndex, out int columnIndex)
    {
        Vector3 mousePosition = Utils.GetMousePosition();

        if (mousePosition.y > boardGenerator.TopBoundY || mousePosition.y < boardGenerator.BottomBoundY)
        {
            rowIndex = -1;
            columnIndex = -1;

            return;
        }

        float minDistance = float.MaxValue;
        int minIndex = 0;

        for (int i = 0; i < boardGenerator.NumLane; i++)
        {
            float distance = Mathf.Abs(mousePosition.x - boardGenerator.LanePositions[i].x);

            if (distance < minDistance)
            {
                minIndex = i;
                minDistance = distance;
            }
        }

        rowIndex = minIndex;
        columnIndex = -1;

        for (int i = 0; i < _numBlockPerColumn; i++)
        {
            if (_columnBlockIndexes[rowIndex, i] == -1)
            {
                columnIndex = i;
            }
            else
            {
                if (columnIndex != -1)
                {
                    _columnBlockIndexes[rowIndex, columnIndex] = _currentPoolBlockIndex;
                }

                break;
            }

            if (i == _numBlockPerColumn - 1)
            {
                _columnBlockIndexes[rowIndex, columnIndex] = _currentPoolBlockIndex;
            }
        }
    }

    IEnumerator TextFollowBlock(int blockIndex)
    {
        void SetTextPosition()
        {
            Vector3 blockPosition = _mainCamera.WorldToScreenPoint(blocks[blockIndex].transform.position);

            _blockNumberTexts[blockIndex].transform.position = blockPosition;
        }

        while (_blockControllers[blockIndex].IsMoving)
        {
            SetTextPosition();

            yield return new WaitForSeconds(0.002f);
        }

        yield return new WaitForSeconds(0.005f);

        SetTextPosition();
    }

    private void OnBlockMoved(int blockIndex, int rowIndex, int columnIndex, bool isCheckMergeSingle = true,
        bool isLast = false, bool isFirstTime = false)
    {
        _blockControllers[blockIndex].IsMoving = false;
        _columnBlockIndexes[rowIndex, columnIndex] = blockIndex;

        Tween.Delay(0.1f).OnComplete(() => trails[blockIndex].gameObject.SetActive(false));

        PushChange(blockIndex, -1 * Vector2Int.one, new Vector2Int(rowIndex, columnIndex));

        if (isCheckMergeSingle) CheckMerge(blockIndex, rowIndex, columnIndex, isFirstTime: isFirstTime);
        else
        {
            if (isLast)
            {
                ContinueCheckMergeAll();
            }
        }
    }
    #endregion

    #region MERGE
    private void CheckMerge(int blockIndex, int rowIndex, int columnIndex, bool isFirstTime = false)
    {
        if (isDebug)
        {
            for (int i = 0; i < boardGenerator.NumLane; i++)
            {
                for (int j = 0; j < _numBlockPerColumn; j++)
                {
                    if (_columnBlockIndexes[i, j] != -1)
                    {
                        Debug.Log("column " + i + "/" + j + ": " + _columnBlockIndexes[i, j]);
                    }
                }
            }
        }

        void Merge(int mergedBlockIndex, int mergedRowIndex, int mergedColumnIndex,
            MergeDirection mergeDirection, bool isLast = false)
        {
            Vector3 destination = blocks[blockIndex].transform.position;

            trails[mergedBlockIndex].gameObject.SetActive(false);
            _blockNumberTexts[mergedBlockIndex].gameObject.SetActive(false);

            if (isLast)
            {
                ChangeMergedBlocksColor(blockIndex, mergedBlockIndex);

                Tween.Position(blocks[mergedBlockIndex].transform, destination, duration: mergeDuration)
                    .OnComplete(() => OnBlockMerged(mergedBlockIndex, mergedRowIndex, mergedColumnIndex, isLast));

                Vector3 startScale = blockWidth * Vector3.one;
                startScale.z = blocks[0].transform.localScale.z;
                Vector3 endScale = 1.05f * blockWidth * Vector3.one;
                endScale.z = blocks[0].transform.localScale.z;

                Tween.StopAll(blocks[blockIndex].transform);
                Tween.Scale(blocks[mergedBlockIndex].transform, endScale, duration: mergeDuration,
                   cycles: 2, cycleMode: CycleMode.Yoyo).SetCycles(false);

                Tween.Scale(blocks[blockIndex].transform, endScale, duration: mergeDuration,
                    cycles: 2, cycleMode: CycleMode.Yoyo).SetCycles(false);

                // Tween.Scale(blocks[blockIndex].transform, endScale, duration: mergeDuration)
                //     .OnComplete(() =>
                //         Tween.Scale(blocks[blockIndex].transform, startScale, duration: mergeDuration));
            }
            else
            {
                if (mergeDirection == MergeDirection.Vertical)
                {
                    Tween.Position(blocks[mergedBlockIndex].transform, destination, duration: mergeDuration);
                }
                else
                {
                    Tween.Position(blocks[mergedBlockIndex].transform,
                            new Vector3(destination.x, blocks[mergedBlockIndex].transform.position.y,
                                blocks[mergedBlockIndex].transform.position.z),
                            duration: mergeDuration / 2)
                        .OnComplete(() =>
                        {
                            Tween.Position(blocks[mergedBlockIndex].transform,
                                    new Vector3(blocks[mergedBlockIndex].transform.position.x, destination.y,
                                        blocks[mergedBlockIndex].transform.position.z),
                                    duration: mergeDuration / 2)
                                .OnComplete(() => OnBlockMerged(mergedBlockIndex, mergedRowIndex, mergedColumnIndex));
                        });
                }
            }

            _blockControllers[mergedBlockIndex].IsMoving = true;

            StartCoroutine(TextFollowBlock(mergedBlockIndex));
        }

        void OnBlockMerged(int mergedBlockIndex, int mergedRowIndex, int mergedColumnIndex, bool isLast = false)
        {
            _blockControllers[blockIndex].Number *= 2;
            _blockControllers[blockIndex].SetValue(_blockNumberTexts[blockIndex]);

            if (dataManager.IsNewBestBlock(_blockControllers[blockIndex].Number,
                    _blockControllers[blockIndex].Letter))
            {
                dataManager.BestBlockNumber = _blockControllers[blockIndex].Number;
                dataManager.BestBlockLetter = _blockControllers[blockIndex].Letter;
                dataManager.BestBlockColorIndex = _blockControllers[blockIndex].ColorIndex;

                newBestBlockIndex = blockIndex;
            }

            EarnScore(_blockControllers[blockIndex].Number, _blockControllers[blockIndex].Letter);

            _blockNumberTexts[mergedBlockIndex].gameObject.SetActive(false);
            blocks[mergedBlockIndex].SetActive(false);

            _blockControllers[mergedBlockIndex].IsMoving = false;
            _columnBlockIndexes[mergedRowIndex, mergedColumnIndex] = -1;

            if (isLast)
            {
                // _columnBlockIndexes[rowIndex, columnIndex] = finalBlockIndex;

                _isBlockChecks[rowIndex, columnIndex] = false;

                MoveAllBlocksUp();
            }
        }

        int checkBlockIndex;
        List<int> mergeBlockIndexes = new List<int>();
        List<Vector2Int> mergeBlockPositionIndexes = new List<Vector2Int>();
        List<MergeDirection> mergeBlockDirections = new List<MergeDirection>();

        FindAllMergeBlocks();

        if (isDebug)
        {
            for (int i = 0; i < mergeBlockIndexes.Count; i++)
            {
                Debug.Log(rowIndex + "/" + columnIndex + ": " + mergeBlockIndexes[i] + "/" +
                          blockIndex);
            }
        }

        if (mergeBlockIndexes.Count == 0)
        {
            if (isFirstTime)
            {
                EndTurn(false);
            }
            else
            {
                ContinueCheckMergeAll();
            }
        }

        for (int i = 0; i < mergeBlockIndexes.Count; i++)
        {
            bool isLast = i == mergeBlockIndexes.Count - 1;

            Merge(mergeBlockIndexes[i], mergeBlockPositionIndexes[i].x, mergeBlockPositionIndexes[i].y,
                mergeBlockDirections[i], isLast);

            if (isLast)
            {
                AudioManager.Instance.PlayMergeBlockSound();
            }
        }

        void FindAllMergeBlocks()
        {
            if (rowIndex + 1 < boardGenerator.NumLane)
            {
                TryAddMergeBlockToList(rowIndex + 1, columnIndex, MergeDirection.Horizontal);
            }

            if (rowIndex - 1 >= 0)
            {
                TryAddMergeBlockToList(rowIndex - 1, columnIndex, MergeDirection.Horizontal);
            }

            if (columnIndex + 1 < _numBlockPerColumn)
            {
                TryAddMergeBlockToList(rowIndex, columnIndex + 1, MergeDirection.Vertical);
            }
        }

        void TryAddMergeBlockToList(int mergeRowIndex, int mergeColumnIndex, MergeDirection mergeDirection)
        {
            checkBlockIndex = _columnBlockIndexes[mergeRowIndex, mergeColumnIndex];

            if (checkBlockIndex != -1)
            {
                if (_blockControllers[blockIndex].IsMatch(_blockControllers[checkBlockIndex]) &&
                    !_blockControllers[checkBlockIndex].IsMoving)
                {
                    mergeBlockIndexes.Add(checkBlockIndex);
                    mergeBlockPositionIndexes.Add(new Vector2Int(mergeRowIndex, mergeColumnIndex));
                    mergeBlockDirections.Add(mergeDirection);
                }
            }
        }
    }

    private void ChangeMergedBlocksColor(int blockIndex, int mergedBlockIndex)
    {
        List<int> remainingColorIndexes = new List<int>();

        for (int i = 0; i < 5; i++)
        {
            if (i != _blockControllers[blockIndex].ColorIndex) remainingColorIndexes.Add(i);
        }

        // int newColorIndex = remainingColorIndexes[Random.Range(0, 4)];
        // Color newColor = Constants.AllBlockColors[newColorIndex];

        _blockControllers[mergedBlockIndex].TweenColor(
            Constants.GetColorInTheme(ThemePicker.value)[_blockControllers[blockIndex].ColorIndex], 0.9f * mergeDuration, () =>
            {
                // _blockControllers[mergedBlockIndex].TweenColor(
                //     newColor, 0.6f * mergeDuration, () => { }
                // );
            }
        );
    }

    private void ChangeShotBlockColor()
    {
        int blockIndex = _currentPoolBlockIndex;
        Vector2Int positionIndex = new Vector2Int(-1, -1);

        for (int i = 0; i < boardGenerator.NumLane; i++)
        {
            if (positionIndex.x != -1) break;

            for (int j = 0; j < _numBlockPerColumn; j++)
            {
                if (_columnBlockIndexes[i, j] == blockIndex)
                {
                    positionIndex = new Vector2Int(i, j);
                    break;
                }
            }
        }

        if (positionIndex.x == -1) return;

        List<int> remainingColorIndexes = new List<int>();

        for (int i = 0; i < 5; i++)
        {
            if (i != _blockControllers[blockIndex].ColorIndex) remainingColorIndexes.Add(i);
        }

        if (positionIndex.y + 1 < _numBlockPerColumn)
        {
            int upBlockIndex = _columnBlockIndexes[positionIndex.x, positionIndex.y + 1];

            if (upBlockIndex != -1)
            {
                if (_blockControllers[upBlockIndex].Value == _blockControllers[blockIndex].Value)
                {
                    remainingColorIndexes.Remove(_blockControllers[upBlockIndex].ColorIndex);
                }
            }
        }

        if (positionIndex.x + 1 < boardGenerator.NumLane)
        {
            int rightBlockIndex = _columnBlockIndexes[positionIndex.x + 1, positionIndex.y];

            if (rightBlockIndex != -1)
            {
                if (_blockControllers[rightBlockIndex].Value == _blockControllers[blockIndex].Value)
                {
                    remainingColorIndexes.Remove(_blockControllers[rightBlockIndex].ColorIndex);
                }
            }
        }

        if (positionIndex.x - 1 >= 0)
        {
            int leftBlockIndex = _columnBlockIndexes[positionIndex.x - 1, positionIndex.y];

            if (leftBlockIndex != -1)
            {
                if (_blockControllers[leftBlockIndex].Value == _blockControllers[blockIndex].Value)
                {
                    remainingColorIndexes.Remove(_blockControllers[leftBlockIndex].ColorIndex);
                }
            }
        }

        var theme = ThemePicker.value;

        int newColorIndex = remainingColorIndexes[Random.Range(0, remainingColorIndexes.Count)];
        Color newColor = Constants.GetColorInTheme(theme)[newColorIndex];

        if (newBestBlockIndex == blockIndex)
        {
            dataManager.BestBlockColorIndex = newColorIndex;
        }

        _blockControllers[blockIndex].TweenColor(
           newColor, 0.9f * mergeDuration, () => { _blockControllers[blockIndex].ColorIndex = newColorIndex; }
        );

        Tween.Custom(_blockNumberTexts[blockIndex].color, Constants.GetTextColorInTheme(theme)[newColorIndex], duration: 0.9f * mergeDuration,
            onValueChange: newVal => _blockNumberTexts[blockIndex].color = newVal
        );
    }

    private void MoveAllBlocksUp()
    {
        List<int> moveBlockIndexes = new List<int>();
        List<Vector2Int> moveBlockNewPositionIndexes = new List<Vector2Int>();
        List<Vector3> moveBlockNewPosition = new List<Vector3>();

        for (int i = 0; i < boardGenerator.NumLane; i++)
        {
            for (int j = _numBlockPerColumn - 2; j >= 0; j--)
            {
                if (_columnBlockIndexes[i, j] != -1)
                {
                    for (int k = _numBlockPerColumn - 1; k > j; k--)
                    {
                        if (_columnBlockIndexes[i, k] == -1)
                        {
                            int blockIndex = _columnBlockIndexes[i, j];

                            Vector3 end = new Vector3(
                                blocks[blockIndex].transform.position.x,
                                highestBlockPositionY - (_numBlockPerColumn - 1 - k) * _blockDistance,
                                blocks[0].transform.position.z
                            );

                            moveBlockIndexes.Add(blockIndex);
                            moveBlockNewPositionIndexes.Add(new Vector2Int(i, k));
                            moveBlockNewPosition.Add(end);

                            _columnBlockIndexes[i, j] = -1;
                            _columnBlockIndexes[i, k] = blockIndex;

                            _isBlockChecks[i, k] = false;
                        }
                    }
                }
            }
        }

        if (moveBlockIndexes.Count == 0)
        {
            ContinueCheckMergeAll();
        }

        for (int i = 0; i < moveBlockIndexes.Count; i++)
        {
            bool isLast = i == moveBlockIndexes.Count - 1;

            MoveBlock(moveBlockIndexes[i], moveBlockNewPositionIndexes[i].x, moveBlockNewPositionIndexes[i].y,
                moveBlockNewPosition[i],
                false, isLast);
        }
    }

    private void ContinueCheckMergeAll()
    {
        for (int i = 0; i < boardGenerator.NumLane; i++)
        {
            for (int j = 0; j < _numBlockPerColumn; j++)
            {
                if (_columnBlockIndexes[i, j] != -1 && _isBlockChecks[i, j] == false)
                {
                    _isBlockChecks[i, j] = true;

                    CheckMerge(_columnBlockIndexes[i, j], i, j);

                    return;
                }
            }
        }

        EndTurn();
    }
    #endregion

    #region END TURN
    private void EarnScore(float newScoreNumber, char? newScoreLetter)
    {
        if (newScoreLetter == null)
        {
            if (_scoreLetter == null)
            {
                _scoreNumber += newScoreNumber;
            }
            else
            {
                if (_scoreLetter == 'a' && _scoreNumber < 10)
                {
                    _scoreNumber = _scoreNumber * 1000 + newScoreNumber;
                }
            }

            if (_scoreNumber > 1000)
            {
                _scoreNumber /= 1000;
                _scoreLetter = 'a';
            }
        }
        else
        {
            if (_scoreLetter != null)
            {
                int powerDistance = (int)_scoreLetter - (int)newScoreLetter;

                if (powerDistance < 2)
                {
                    _scoreNumber += newScoreNumber / Mathf.Pow(1000, powerDistance);
                }

                if (_scoreNumber > 1000)
                {
                    _scoreNumber /= 1000;
                    _scoreLetter = (char)((int)_scoreLetter + 1);
                }
            }
            else
            {
                _scoreNumber += newScoreNumber * Mathf.Pow(1000, (int)newScoreLetter - 97);

                if (_scoreNumber > 1000)
                {
                    _scoreNumber /= 1000;
                    _scoreLetter = (char)((int)newScoreLetter + 1);
                }
                else
                {
                    _scoreLetter = newScoreLetter;
                }
            }
        }

        uiManager.SetScoreText(_scoreNumber, _scoreLetter);
    }

    private void EndTurn(bool isAnyMerge = true)
    {
        if (blocks[blocks.Length - 1].activeSelf)
        {
            losePopup.ShowPopup(_scoreNumber, _scoreLetter);

            return;
        }

        if (isAnyMerge)
        {
            ChangeShotBlockColor();

            Tween.Delay(mergeDuration).OnComplete(() => DoEndTurn());
        }
        else
        {
            DoEndTurn();
        }
    }

    private void DoEndTurn()
    {
        _isAnotherBlockMoving = false;
        nextBlockGenerator.GenerateNewBlock();

        turn++;

        dataManager.SaveGameplayData(turn, _columnBlockIndexes, _scoreNumber, _scoreLetter, _blockControllers);

        if (dataManager.IsNewBestScore(_scoreNumber, _scoreLetter))
        {
            dataManager.BestScoreNumber = _scoreNumber;
            dataManager.BestScoreLetter = _scoreLetter;

            uiManager.SetBestScore(dataManager.BestScoreNumber, dataManager.BestScoreLetter);
        }

        dataManager.SaveGeneralData(_scoreNumber, _scoreLetter);

        if (newBestBlockIndex != -1)
        {
            uiManager.blockRecordPopup.ShowPopup(dataManager.BestBlockNumber, dataManager.BestBlockLetter,
                dataManager.BestBlockColorIndex);
        }
        else
        {
            if (turn % 25 == 0 && !dataManager.IsAdRemoved)
            {
                adManager.ShowInterstitialAd();
            }
        }
    }
    #endregion

    #region SKILL
    public void X2BlockValue()
    {
        _blockControllers[newBestBlockIndex].Number *= 2;
        _blockControllers[newBestBlockIndex].SetValue(_blockNumberTexts[newBestBlockIndex]);

        dataManager.BestBlockNumber = _blockControllers[newBestBlockIndex].Number;
        dataManager.BestBlockLetter = _blockControllers[newBestBlockIndex].Letter;

        uiManager.blockRecordPopup.UpdateBestBlock(_blockControllers[newBestBlockIndex].Number,
            _blockControllers[newBestBlockIndex].Letter);
    }

    public void X2BlockValue(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        X2BlockValue();
    }

    private void PushChange(int blockIndex, Vector2Int prevPositionIndex, Vector2Int curPositionIndex)
    {
        _changeStack.Push(new PositionChangeStackItem(turn, blockIndex, prevPositionIndex, curPositionIndex));
    }

    public void RevertMove()
    {
        if (_changeStack.Count == 0) return;

        ChangeStackItem change = _changeStack.Peek();

        if (change.Turn == turn - 1)
        {
            change = _changeStack.Pop();

            if (change is PositionChangeStackItem)
            {
                PositionChangeStackItem positionChange = (PositionChangeStackItem)change;

                if (positionChange.PrevPositionIndex.x == -1)
                {
                    _blockNumberTexts[positionChange.BlockIndex].gameObject.SetActive(false);

                    Tween.Scale(blocks[positionChange.BlockIndex].transform, Vector3.zero, duration: 0.15f)
                        .OnComplete(() =>
                        {
                            blocks[positionChange.BlockIndex].transform.localScale = blockWidth * Vector3.one;
                            blocks[positionChange.BlockIndex].SetActive(false);
                            _columnBlockIndexes[positionChange.CurPositionIndex.x, positionChange.CurPositionIndex.y] =
                                -1;

                            dataManager.SpendGem(75);
                        });
                }
            }
        }
    }
    #endregion

    #region UTIL
    private int FindAvailableBlockIndex()
    {
        int index = -1;

        for (int i = 0; i < blocks.Length; i++)
        {
            if (!blocks[i].activeSelf)
            {
                index = i;

                break;
            }
        }

        return index;
    }

    public void GetRandomAvailableBlockData(out string blockValue, out int colorIndex)
    {
        blockValue = "";
        colorIndex = -1;

        for (int i = 0; i < boardGenerator.NumLane; i++)
        {
            for (int j = 0; j < _numBlockPerColumn; j++)
            {
                if (_columnBlockIndexes[i, j] != -1)
                {
                    bool isPick = Random.Range(0, 3) == 0;

                    if (isPick)
                    {
                        int blockIndex = _columnBlockIndexes[i, j];
                        blockValue = _blockControllers[blockIndex].Value;
                        colorIndex = _blockControllers[blockIndex].ColorIndex;

                        return;
                    }
                }
            }
        }
    }

    public void ChangeTheme()
    {
        for (int i = 0; i < boardGenerator.NumLane; i++)
        {
            for (int j = 0; j < _numBlockPerColumn; j++)
            {
                if (_columnBlockIndexes[i, j] != -1)
                {
                    int blockIndex = _columnBlockIndexes[i, j];

                    _blockControllers[blockIndex].SetColor(_blockNumberTexts[blockIndex]);
                }
            }
        }

        nextBlockGenerator.ChangeTheme();

        Addressables.LoadAssetAsync<Sprite>(Constants.backgroundAddressableKeys[(int)ThemePicker.value]).Completed += OnSpriteLoaded;
    }

    private void OnSpriteLoaded(AsyncOperationHandle<Sprite> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            backgroundRenderer.sprite = handle.Result;
        }
    }
    #endregion

    #region SWAP MODE

    public void EnterSwapMode()
    {
        _interactMode = InteractMode.Swap;
    }

    public void OnSwapSelect()
    {
        // Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //
        // RaycastHit hit;
        //
        // Physics.Raycast(ray.origin, ray.direction, out hit);
        //
        // if (hit.collider != null)
        // {
        //     Debug.Log("hit:" + hit.collider.name);
        // }

        Vector3 mousePosition = Utils.GetMousePosition();

        if (mousePosition.y > boardGenerator.TopBoundY || mousePosition.y < boardGenerator.BottomBoundY)
        {
            return;
        }

        float minDistance = float.MaxValue;
        int cachedBlockIndex = -1;
        Vector2Int cachedPositionIndex = Vector2Int.zero;

        for (int i = 0; i < boardGenerator.NumLane; i++)
        {
            for (int j = 0; j < _numBlockPerColumn; j++)
            {
                if (_columnBlockIndexes[i, j] != -1)
                {
                    int blockIndex = _columnBlockIndexes[i, j];
                    Vector3 position = blocks[blockIndex].transform.position;
                    float distance = Mathf.Abs(mousePosition.x - position.x) + Mathf.Abs(mousePosition.y - position.y);

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        cachedBlockIndex = blockIndex;
                        cachedPositionIndex = new Vector2Int(i, j);
                    }
                }
            }
        }

        if (cachedBlockIndex != -1)
        {
            _selectedBlockIndexes.Add(cachedBlockIndex);
            _selectedBlockPositionIndexes.Add(cachedPositionIndex);

            Tween.Scale(blocks[cachedBlockIndex].transform, 1.1f * blockWidth * Vector3.one, duration: 0.15f);
        }

        if (_selectedBlockIndexes.Count == 2)
        {
            Swap();
        }
    }

    public void Swap()
    {
        _isBlockChecks = new bool[boardGenerator.NumLane, _numBlockPerColumn];

        for (int i = 0; i < _selectedBlockIndexes.Count; i++)
        {
            bool isLast = i == 1;
            int otherIndexInSwapList = i == 0 ? 1 : 0;
            int otherRowIndex = _selectedBlockPositionIndexes[otherIndexInSwapList].x;
            int otherColumnIndex = _selectedBlockPositionIndexes[otherIndexInSwapList].y;

            Tween.Scale(blocks[_selectedBlockIndexes[i]].transform, blockWidth * Vector3.one, duration: 0.15f);

            trails[_selectedBlockIndexes[i]].gameObject.SetActive(false);

            MoveBlock(
                _selectedBlockIndexes[i],
                otherRowIndex,
                otherColumnIndex,
                blocks[_selectedBlockIndexes[otherIndexInSwapList]].transform.position,
                isCheckMergeSingle: false,
                isLast: isLast
            );
        }

        _selectedBlockIndexes.Clear();
        _selectedBlockPositionIndexes.Clear();

        _interactMode = InteractMode.Normal;

        uiManager.swapModePopup.ClosePopup();

        dataManager.SpendGem(125);
    }

    #endregion

    #region PAUSE
    public void Pause()
    {
        _isPaused = true;
    }

    public void UnPause()
    {
        _isPaused = false;
    }
    #endregion
}