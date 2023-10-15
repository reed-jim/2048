using System;
using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [Space] [Header("OBJECTS")] [SerializeField]
    private GameObject blockCollection;

    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private GameObject[] blocks;
    private Block[] _blockControllers;
    private Rigidbody[] _blockRigidBodies;

    [Space] [Header("UI")] [SerializeField]
    private TMP_Text blockNumberTextPrefab;

    [SerializeField] private RectTransform blockNumberTextCollection;
    private TMP_Text[] _blockNumberTexts;

    [Space] [Header("MANAGEMENT")] [SerializeField]
    private int[,] _columnBlockIndexes;

    [Space] [Header("REFERENCE")] [SerializeField]
    private BoardGenerator boardGenerator;

    [Space] [Header("CUSTOM")] [SerializeField]
    private int numBlock;

    [SerializeField] private float blockWidth;

    private int _numBlockPerColumn;
    private float _blockDistance;

    private int _currentPoolBlockIndex;

    private void Start()
    {
        _numBlockPerColumn = numBlock / boardGenerator.NumLane;

        blocks = new GameObject[numBlock];
        _blockControllers = new Block[numBlock];
        _blockRigidBodies = new Rigidbody[numBlock];
        _columnBlockIndexes = new int[boardGenerator.NumLane, _numBlockPerColumn];

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
            _blockControllers[i] = blocks[i].GetComponent<Block>();
            _blockRigidBodies[i] = blocks[i].GetComponent<Rigidbody>();

            _blockNumberTexts[i] = Instantiate(blockNumberTextPrefab, blockNumberTextCollection);

            blocks[i].transform.localScale = new Vector3(blockWidth, blockWidth, blocks[i].transform.localScale.z);
            _blockNumberTexts[i].gameObject.SetActive(false);
            blocks[i].SetActive(false);
        }

        _currentPoolBlockIndex = 0;
        _blockDistance = 1.1f * blockWidth;
    }

    public void OnSwipe()
    {
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

        for (int i = _numBlockPerColumn - 1; i >= 0; i--)
        {
            if (_columnBlockIndexes[rowIndex, i] == -1)
            {
                columnIndex = i;
                _columnBlockIndexes[rowIndex, i] = _currentPoolBlockIndex;

                break;
            }
        }

        _blockControllers[_currentPoolBlockIndex].PositionIndex = new Vector2(rowIndex, columnIndex);

        Vector3 start = new Vector3(
            boardGenerator.LanePositions[minIndex].x,
            -5.5f,
            0
        );

        Vector3 end = new Vector3(
            boardGenerator.LanePositions[minIndex].x,
            6 - (_numBlockPerColumn - 1 - columnIndex) * _blockDistance,
            0
        );

        blocks[_currentPoolBlockIndex].transform.position = start;
        _blockControllers[_currentPoolBlockIndex].SetColor(Random.Range(0, 0));
        blocks[_currentPoolBlockIndex].SetActive(true);

        _blockNumberTexts[_currentPoolBlockIndex].text = _blockControllers[_currentPoolBlockIndex].Number.ToString();
        _blockNumberTexts[_currentPoolBlockIndex].gameObject.SetActive(true);


        _blockControllers[_currentPoolBlockIndex].IsMoving = true;
        StartCoroutine(TextFollowBlock(_currentPoolBlockIndex));

        Tween.PositionY(blocks[_currentPoolBlockIndex].transform, end.y, duration: 2f)
            .OnComplete(() => OnBlockMoved(_currentPoolBlockIndex - 1, rowIndex, columnIndex));


        _currentPoolBlockIndex++;
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
        while (_blockControllers[blockIndex].IsMoving)
        {
            Vector3 blockPosition = Camera.main.WorldToScreenPoint(blocks[blockIndex].transform.position);

            _blockNumberTexts[blockIndex].transform.position = blockPosition;

            yield return new WaitForSeconds(0.01f);
        }
    }

    private void OnBlockMoved(int blockIndex, int rowIndex, int columnIndex)
    {
        _blockControllers[_currentPoolBlockIndex].IsMoving = false;
        CheckMerge(blockIndex, rowIndex, columnIndex);
    }

    private void CheckMerge(int blockIndex, int rowIndex, int columnIndex)
    {
        void Merge()
        {
            _blockControllers[blockIndex].Number *= 2;

            _blockNumberTexts[blockIndex].text = _blockControllers[blockIndex].Value;
        }

        int checkBlockIndex;

        if (columnIndex + 1 < _numBlockPerColumn)
        {
            checkBlockIndex = _columnBlockIndexes[rowIndex, columnIndex + 1];

            if (checkBlockIndex != -1)
            {
                if (_blockControllers[blockIndex].IsMatch(_blockControllers[checkBlockIndex]))
                {
                    Merge();
                }
            }
        }
    }
}