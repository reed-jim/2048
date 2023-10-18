using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeStackItem
{
    private int _turn;
    private int blockIndex;
    
    public int Turn { get; set; }
    public int BlockIndex { get; set; }
}

public class PositionChangeStackItem : ChangeStackItem
{
    private Vector2 _prevPositionIndex;
    private Vector2 _curPositionIndex;
    
    public Vector2Int PrevPositionIndex { get; set; }
    public Vector2Int CurPositionIndex { get; set; }

    public PositionChangeStackItem(int turn, int blockIndex, Vector2Int prevPositionIndex, Vector2Int curPositionIndex)
    {
        Turn = turn;
        BlockIndex = blockIndex;
        PrevPositionIndex = prevPositionIndex;
        CurPositionIndex = curPositionIndex;
    }
}