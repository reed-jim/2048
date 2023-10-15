using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

public class BoardGenerator : MonoBehaviour
{
    [SerializeField] private GameObject lanePrefab;

    [Space] [Header("CUSTOM")] [SerializeField]
    private int numLane;

    [SerializeField] private float laneWidth;
    [SerializeField] private float laneGap;
    private GameObject[] _lanes;

    public int NumLane
    {
        get => numLane;
    }

    public Vector3[] LanePositions
    {
        get => _lanes.Select(lane => lane.transform.position).ToArray();
    }

    void Start()
    {
        SpawnLanes();
        GenerateBoard();
    }

    private void SpawnLanes()
    {
        GameObject laneCollection = GameObject.Find("Lanes");

        _lanes = new GameObject[numLane];

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
    }
}