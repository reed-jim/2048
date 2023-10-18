using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using TMPro;
using UnityEngine;

public class Block : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private CustomMaterialProperty _customMaterialProperty;

    private float _number;
    private char? _letter;
    private string _value;
    private int _colorIndex;

    private Vector2 _positionIndex;
    private bool isMoving;

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

    public void SetNumberAndLetter(string value)
    {
        char lastChar = value.Last();

        if ((int)lastChar >= 97)
        {
            Letter = lastChar;
            Number = float.Parse(value.TrimEnd(lastChar));
        }
        else
        {
            Number = float.Parse(value);
        }

        Debug.Log(Letter);
    }

    public void SetValue(TMP_Text valueText)
    {
        if (Number > 1000)
        {
            Number /= 1000f;

            Number = (int)Number;

            if (_letter == null)
            {
                _letter = 'a';
            }
            else
            {
                _letter = (char)((int)_letter + 1);
            }
        }

        Debug.Log("after" + Letter);
        if (_letter == null)
        {
            Value = Number.ToString();
        }
        else
        {
            Value = Number.ToString() + _letter;
        }

        valueText.text = Value;
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