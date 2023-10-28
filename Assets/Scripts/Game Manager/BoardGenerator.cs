using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

public class BoardGenerator : MonoBehaviour
{
    [SerializeField] private GameObject lanePrefab;

    [Space]
    [Header("CUSTOM")]
    [SerializeField]
    private int numLane;

    [SerializeField] private float laneWidth;
    [SerializeField] private float laneGap;
    private GameObject[] _lanes;

    private float _topBoundY;
    private float _bottomBoundY;

    public float TopBoundY
    {
        get => _topBoundY;
        set => _topBoundY = value;
    }

    public float BottomBoundY
    {
        get => _bottomBoundY;
        set => _bottomBoundY = value;
    }

    public int NumLane
    {
        get => numLane;
    }

    public float LaneWidth
    {
        get => laneWidth;
    }

    public Vector3[] LanePositions
    {
        get => _lanes.Select(lane => lane.transform.position).ToArray();
    }

    private Vector2 _screenSize;

    void Awake()
    {
        _screenSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);

        SpawnLanes();
        GenerateBoard();
    }

    private void SpawnLanes()
    {
        GameObject laneCollection = GameObject.Find("Lanes");

        _lanes = new GameObject[numLane];

        // laneWidth = 0.0008f * _screenSize.x;
        laneWidth = 0.7f * 8f / 5;

        for (int i = 0; i < numLane; i++)
        {
            _lanes[i] = Instantiate(lanePrefab, laneCollection.transform);
            _lanes[i].transform.localScale = new Vector3(laneWidth, _lanes[i].transform.localScale.y,
                _lanes[i].transform.localScale.z);
        }
    }

    private void GenerateBoard()
    {
        Vector3 position;

        float laneDistance = laneWidth + laneGap;

        for (int i = 0; i < numLane; i++)
        {
            position.x = -((numLane - 1) / 2) * laneDistance + i * laneDistance;
            position.y = 1.5f;
            position.z = _lanes[i].transform.position.z;

            _lanes[i].transform.position = position;
        }

        MeshRenderer laneRenderer = _lanes[0].GetComponent<MeshRenderer>();

        _topBoundY = _lanes[0].transform.position.y + 0.5f * laneRenderer.bounds.size.y;
        _bottomBoundY = _lanes[0].transform.position.y - 0.5f * laneRenderer.bounds.size.y;

        _topBoundY = 0.32f / 0.5f * 8;
        _bottomBoundY = -0.15f / 0.5f * 8;
    }
}