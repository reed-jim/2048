using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public enum MergeDirection
{
    Horizontal,
    Vertical
}

public class GameManager : MonoBehaviour
{
    [Space] [Header("OBJECTS")] [SerializeField]
    private GameObject blockCollection;

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
    private int[,] _columnBlockIndexes;

    private bool[,] _isBlockChecks;

    [Space] [Header("REFERENCE")] [SerializeField]
    private BoardGenerator boardGenerator;

    [SerializeField] private NextBlockGenerator nextBlockGenerator;

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

    private bool isAnotherBlockMoving;

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

        _currentPoolBlockIndex = 0;
        _blockDistance = 1.1f * blockWidth;

        nextBlockGenerator.GenerateNewBlock();
    }

    public void OnSwipe()
    {
        if (isAnotherBlockMoving) return;

        Vector3 mousePosition = GetMousePosition();

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

        int rowIndex = minIndex;
        int columnIndex = 0;

        for (int i = 0; i < _numBlockPerColumn; i++)
        {
            if (_columnBlockIndexes[rowIndex, i] == -1)
            {
                columnIndex = i;
            }
            else
            {
                _columnBlockIndexes[rowIndex, columnIndex] = _currentPoolBlockIndex;
                
                break;
            }

            if (i == _numBlockPerColumn - 1)
            {
                _columnBlockIndexes[rowIndex, columnIndex] = _currentPoolBlockIndex;
            }
        }

        _blockControllers[_currentPoolBlockIndex].PositionIndex = new Vector2(rowIndex, columnIndex);

        Vector3 start = new Vector3(
            boardGenerator.LanePositions[minIndex].x,
            -7f,
            blocks[0].transform.position.z
        );

        Vector3 end = new Vector3(
            boardGenerator.LanePositions[minIndex].x,
            highestBlockPositionY - (_numBlockPerColumn - 1 - columnIndex) * _blockDistance,
            blocks[0].transform.position.z
        );

        blocks[_currentPoolBlockIndex].transform.position = start;

        _blockControllers[_currentPoolBlockIndex].SetColor(nextBlockGenerator.NextColorIndex);
        trails[_currentPoolBlockIndex].startColor =
            Constants.AllBlockColors[nextBlockGenerator.NextColorIndex] - new Color(0, 0, 0, 0.3f);
        trails[_currentPoolBlockIndex].endColor =
            Constants.AllBlockColors[nextBlockGenerator.NextColorIndex] - new Color(0, 0, 0, 1);
        blocks[_currentPoolBlockIndex].SetActive(true);

        _blockNumberTexts[_currentPoolBlockIndex].text = _blockControllers[_currentPoolBlockIndex].Number.ToString();
        _blockNumberTexts[_currentPoolBlockIndex].gameObject.SetActive(true);

        _blockControllers[_currentPoolBlockIndex].IsMoving = true;

        int blockIndex = _currentPoolBlockIndex;

        StartCoroutine(TextFollowBlock(blockIndex));

        Tween.PositionY(blocks[_currentPoolBlockIndex].transform, end.y, duration: moveDuration, ease: moveEasing)
            .OnComplete(() => OnBlockMoved(blockIndex, rowIndex, columnIndex));

        // Tween.Delay(delaySwipeTime)
        //     .OnComplete(() => isAnotherBlockMoving = false);

        _currentPoolBlockIndex++;

        isAnotherBlockMoving = true;
    }

    private Vector3 GetMousePosition()
    {
        Vector3 worldPosition = Vector3.zero;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            worldPosition = hit.point;
        }

        return worldPosition;
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

    private void OnBlockMoved(int blockIndex, int rowIndex, int columnIndex)
    {
        _blockControllers[blockIndex].IsMoving = false;

        Tween.Delay(0.1f).OnComplete(() => trails[blockIndex].gameObject.SetActive(false));

        CheckMerge(blockIndex, rowIndex, columnIndex);
    }

    private void CheckMerge(int blockIndex, int rowIndex, int columnIndex)
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

        Vector3 destination = blocks[blockIndex].transform.position;
        int finalBlockIndex = blockIndex;
        Vector2Int finalPositionIndex = new Vector2Int(rowIndex, columnIndex);

        void Merge(Vector3 destination, int mergedBlockIndex, int mergedRowIndex, int mergedColumnIndex,
            MergeDirection mergeDirection, bool isLast = false)
        {
            if (isLast)
            {
                Tween.Position(blocks[mergedBlockIndex].transform, destination, duration: mergeDuration)
                    .OnComplete(() => OnBlockMerged(mergedBlockIndex, mergedRowIndex, mergedColumnIndex, isLast));

                Vector3 startScale = blockWidth * Vector3.one;
                startScale.z = blocks[0].transform.localScale.z;
                Vector3 endScale = 1.1f * blockWidth * Vector3.one;
                endScale.z = blocks[0].transform.localScale.z;

                Tween.Scale(blocks[finalBlockIndex].transform, endScale, duration: mergeDuration)
                    .OnComplete(() =>
                        Tween.Scale(blocks[finalBlockIndex].transform, startScale, duration: mergeDuration));
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
            _blockControllers[finalBlockIndex].Number *= 2;
            _blockNumberTexts[finalBlockIndex].text = _blockControllers[finalBlockIndex].Value;

            _blockNumberTexts[mergedBlockIndex].gameObject.SetActive(false);
            blocks[mergedBlockIndex].SetActive(false);

            _blockControllers[mergedBlockIndex].IsMoving = false;
            _columnBlockIndexes[mergedRowIndex, mergedColumnIndex] = -1;

            if (isLast)
            {
                _columnBlockIndexes[finalPositionIndex.x, finalPositionIndex.y] = finalBlockIndex;

                _isBlockChecks[finalPositionIndex.x, finalPositionIndex.y] = false;

                ContinueCheckMerge();
            }
        }

        int checkBlockIndex;
        List<int> mergeBlockIndexes = new List<int>();
        List<Vector2Int> mergeBlockPositionIndexes = new List<Vector2Int>();
        List<MergeDirection> mergeBlockDirections = new List<MergeDirection>();

        if (rowIndex + 1 < boardGenerator.NumLane)
        {
            checkBlockIndex = _columnBlockIndexes[rowIndex + 1, columnIndex];

            AddMergeBlockToList(rowIndex + 1, columnIndex, MergeDirection.Horizontal);
        }

        if (rowIndex - 1 >= 0)
        {
            checkBlockIndex = _columnBlockIndexes[rowIndex - 1, columnIndex];

            AddMergeBlockToList(rowIndex - 1, columnIndex, MergeDirection.Horizontal);
        }

        if (columnIndex + 1 < _numBlockPerColumn)
        {
            checkBlockIndex = _columnBlockIndexes[rowIndex, columnIndex + 1];

            AddMergeBlockToList(rowIndex, columnIndex + 1, MergeDirection.Vertical);
        }

        // for (int i = 0; i < mergeBlockIndexes.Count; i++)
        // {
        //     Debug.Log(rowIndex + "/" + columnIndex + ": " + mergeBlockIndexes[i] + "/" + destination + "/" +
        //               finalBlockIndex);
        // }

        if (mergeBlockIndexes.Count == 0)
        {
            ContinueCheckMerge();

            // if (isBlockChecks != null)
            // {
            //     ContinueCheckMerge(_isBlockChecks);
            // }
            // else
            // {
            //     isAnotherBlockMoving = false;
            // }
        }

        for (int i = 0; i < mergeBlockIndexes.Count; i++)
        {
            bool isLast = i == mergeBlockIndexes.Count - 1;

            if (mergeBlockDirections[i] == MergeDirection.Horizontal)
            {
                Merge(destination, mergeBlockIndexes[i], mergeBlockPositionIndexes[i].x, mergeBlockPositionIndexes[i].y,
                    mergeBlockDirections[i], isLast);
            }
            else
            {
                Merge(destination, blockIndex, rowIndex, columnIndex,
                    MergeDirection.Vertical, isLast);
            }
        }

        void AddMergeBlockToList(int mergeRowIndex, int mergeColumnIndex, MergeDirection mergeDirection)
        {
            if (checkBlockIndex != -1)
            {
                if (_blockControllers[blockIndex].IsMatch(_blockControllers[checkBlockIndex]) &&
                    !_blockControllers[checkBlockIndex].IsMoving)
                {
                    mergeBlockIndexes.Add(checkBlockIndex);
                    mergeBlockPositionIndexes.Add(new Vector2Int(mergeRowIndex, mergeColumnIndex));
                    mergeBlockDirections.Add(mergeDirection);

                    if (mergeDirection == MergeDirection.Vertical)
                    {
                        destination = blocks[checkBlockIndex].transform.position;
                        finalBlockIndex = checkBlockIndex;
                        finalPositionIndex = new Vector2Int(rowIndex, columnIndex + 1);
                    }
                }
            }
        }
    }

    private void ContinueCheckMerge()
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

        isAnotherBlockMoving = false;
        nextBlockGenerator.GenerateNewBlock();
    }
}