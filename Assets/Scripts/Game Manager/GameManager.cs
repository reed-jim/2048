using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PrimeTween;
using TMPro;
using UnityEngine;
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
    [Header("OBJECTS")] [SerializeField] private GameObject blockCollection;

    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private GameObject[] blocks;
    [SerializeField] private TrailRenderer[] trails;
    private Block[] _blockControllers;
    private Rigidbody[] _blockRigidBodies;

    [Space] [Header("UI")] [SerializeField]
    private TMP_Text blockNumberTextPrefab;

    [SerializeField] private RectTransform blockNumberTextCollection;
    private TMP_Text[] _blockNumberTexts;

    [Space] [Header("MANAGEMENT")] [SerializeField]
    private int turn;

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


    [Space] [Header("REFERENCE")] [SerializeField]
    private BoardGenerator boardGenerator;

    [SerializeField] private NextBlockGenerator nextBlockGenerator;
    [SerializeField] private UIManager uiManager;

    [Space] [Header("CUSTOM")] [SerializeField]
    private int numBlock;

    [SerializeField] private float blockWidth;
    [SerializeField] private float highestBlockPositionY;
    [SerializeField] private Ease moveEasing;
    [SerializeField] private float moveDuration;
    [SerializeField] private float mergeDuration;
    [SerializeField] private bool isDebug;
    [SerializeField] private float delaySwipeTime;

    private int _numBlockPerColumn;
    private float _blockDistance;

    private int _currentPoolBlockIndex;

    private bool _isAnotherBlockMoving;

    private void Start()
    {
        _numBlockPerColumn = numBlock / boardGenerator.NumLane;

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

        for (int i = 0; i < numBlock; i++)
        {
            blocks[i] = Instantiate(blockPrefab, blockCollection.transform);
            trails[i] = blocks[i].transform.GetChild(0).gameObject.GetComponent<TrailRenderer>();
            _blockControllers[i] = blocks[i].GetComponent<Block>();
            _blockRigidBodies[i] = blocks[i].GetComponent<Rigidbody>();

            _blockNumberTexts[i] = Instantiate(blockNumberTextPrefab, blockNumberTextCollection);

            blocks[i].transform.localScale = new Vector3(blockWidth, blockWidth, blocks[i].transform.localScale.z);
            _blockNumberTexts[i].gameObject.SetActive(false);
            blocks[i].SetActive(false);
        }

        _blockDistance = 1.1f * blockWidth;

        nextBlockGenerator.GenerateNewBlock();
    }

    public void OnSwipe()
    {
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

        SpawnThenMoveBlock(rowIndex, columnIndex);

        _isAnotherBlockMoving = true;
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
        _blockControllers[_currentPoolBlockIndex].SetColor(nextBlockGenerator.NextColorIndex);
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
        trails[_currentPoolBlockIndex].startColor =
            Constants.AllBlockColors[nextBlockGenerator.NextColorIndex] - new Color(0, 0, 0, 0.3f);
        trails[_currentPoolBlockIndex].endColor =
            Constants.AllBlockColors[nextBlockGenerator.NextColorIndex] - new Color(0, 0, 0, 1);
    }

    private void MoveBlock(int blockIndex, int rowIndex, int columnIndex, Vector3 end, bool isCheckMergeSingle = true,
        bool isLast = false, bool isFirstTime = false)
    {
        _blockControllers[blockIndex].IsMoving = true;

        StartCoroutine(TextFollowBlock(blockIndex));

        Tween.PositionY(blocks[_currentPoolBlockIndex].transform, end.y, duration: moveDuration, ease: moveEasing)
            .OnComplete(() => OnBlockMoved(blockIndex, rowIndex, columnIndex, isCheckMergeSingle, isLast));
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
            Vector3 blockPosition = Camera.main.WorldToScreenPoint(blocks[blockIndex].transform.position);

            _blockNumberTexts[blockIndex].transform.position = blockPosition;
        }

        while (_blockControllers[blockIndex].IsMoving)
        {
            SetTextPosition();

            yield return new WaitForSeconds(0.002f);
        }
    }

    private void OnBlockMoved(int blockIndex, int rowIndex, int columnIndex, bool isCheckMergeSingle = true,
        bool isLast = false, bool isFirstTime = false)
    {
        _blockControllers[blockIndex].IsMoving = false;

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

            if (isLast)
            {
                Tween.Position(blocks[mergedBlockIndex].transform, destination, duration: mergeDuration)
                    .OnComplete(() => OnBlockMerged(mergedBlockIndex, mergedRowIndex, mergedColumnIndex, isLast));

                Vector3 startScale = blockWidth * Vector3.one;
                startScale.z = blocks[0].transform.localScale.z;
                Vector3 endScale = 1.1f * blockWidth * Vector3.one;
                endScale.z = blocks[0].transform.localScale.z;

                Tween.Scale(blocks[blockIndex].transform, endScale, duration: mergeDuration)
                    .OnComplete(() =>
                        Tween.Scale(blocks[blockIndex].transform, startScale, duration: mergeDuration));
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
                EndTurn();
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

    private void MoveAllBlocksUp()
    {
        List<int> moveBlockIndexes = new List<int>();
        List<Vector2Int> moveBlockNewPositionIndexes = new List<Vector2Int>();
        List<Vector3> moveBlockNewPosition = new List<Vector3>();

        for (int i = 0; i < boardGenerator.NumLane; i++)
        {
            for (int j = 0; j < _numBlockPerColumn; j++)
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
        }

        uiManager.SetScoreText(_scoreNumber, _scoreLetter);
    }

    private void EndTurn()
    {
        _isAnotherBlockMoving = false;
        nextBlockGenerator.GenerateNewBlock();

        turn++;
    }

    private void PushChange(int blockIndex, Vector2Int prevPositionIndex, Vector2Int curPositionIndex)
    {
        _changeStack.Push(new PositionChangeStackItem(turn, blockIndex, prevPositionIndex, curPositionIndex));
    }

    public void RevertMove()
    {
        ChangeStackItem change = _changeStack.Peek();

        if (change.Turn == turn - 1)
        {
            change = _changeStack.Pop();

            if (change is PositionChangeStackItem)
            {
                PositionChangeStackItem positionChange = (PositionChangeStackItem)change;

                if (positionChange.PrevPositionIndex.x == -1)
                {
                    blocks[positionChange.BlockIndex].SetActive(false);
                    _columnBlockIndexes[positionChange.CurPositionIndex.x, positionChange.CurPositionIndex.y] = -1;
                }
            }
        }
    }

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

    #region SwapMode

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
        }

        if (_selectedBlockIndexes.Count == 2)
        {
            Swap();
        }
    }

    public void Swap()
    {
        for (int i = 0; i < _selectedBlockIndexes.Count; i++)
        {
            bool isLast = i == 1;

            MoveBlock(
                _selectedBlockIndexes[i],
                _selectedBlockPositionIndexes[i].x,
                _selectedBlockPositionIndexes[i].y,
                blocks[_selectedBlockIndexes[i == 0 ? 1 : 0]].transform.position,
                isLast: isLast
            );
        }

        _selectedBlockIndexes.Clear();
        _selectedBlockPositionIndexes.Clear();

        _interactMode = InteractMode.Normal;
    }

    #endregion
}