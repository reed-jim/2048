using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using TMPro;
using UnityEngine;

public class Block : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private CustomMaterialProperty _customMaterialProperty;
    
    private int _number;
    private char _word;
    private string _value;
    private int _colorIndex;

    private Vector2 _positionIndex;
    private bool isMoving;

    public int Number { get; set; }

    public char Word { get; set; }

    public string Value
    {
        get => Number.ToString() + Word;
        set => _value = value;
    }

    public int ColorIndex { get; set; }

    public Vector2 PositionIndex { get; set; }
    
    public bool IsMoving { get; set; }

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _customMaterialProperty = GetComponent<CustomMaterialProperty>();
      
        Number = 2;
        PositionIndex = new Vector2(-1, -1);
    }

    private void OnCollisionEnter(Collision collision)
    {
        _rigidbody.velocity = Vector3.zero;
    }

    public void SetColor(int colorIndex)
    {
        Color color = Constants.AllBlockColors[colorIndex];
        
        _customMaterialProperty.ChangeColor(color);

        ColorIndex = colorIndex;
    }

    public bool IsMatch(Block blockController)
    {
        if (blockController.Value == Value && blockController.ColorIndex == ColorIndex)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}