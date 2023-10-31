using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using PrimeTween;
using TMPro;
using UnityEngine;

public class Block : MonoBehaviour
{
    private CustomMaterialProperty _customMaterialProperty;

    private float _number;
    private char? _letter;
    private string _value;
    private int _colorIndex;

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

    public delegate void OnTweenColorCompleted();

    void Awake()
    {
        _customMaterialProperty = GetComponent<CustomMaterialProperty>();

        Number = 2;
        PositionIndex = new Vector2(-1, -1);
    }

    public void SetColor(TMP_Text blockNumber)
    {
        // Color color = Constants.AllBlockColors[ColorIndex];
        Color color = Constants.GetColorInTheme(ThemePicker.value)[ColorIndex];
        _customMaterialProperty.ChangeColor(color);
        // blockNumber.color = Constants.AllBlockTextColors[ColorIndex];
        blockNumber.color = Constants.GetTextColorInTheme(ThemePicker.value)[ColorIndex];
    }

    public void SetColor(int colorIndex, TMP_Text blockNumber)
    {
        // Color color = Constants.AllBlockColors[colorIndex];
        Color color = Constants.GetColorInTheme(ThemePicker.value)[ColorIndex];

        _customMaterialProperty.ChangeColor(color);
        // blockNumber.color = Constants.AllBlockTextColors[colorIndex];
        blockNumber.color = Constants.GetTextColorInTheme(ThemePicker.value)[ColorIndex];

        ColorIndex = colorIndex;
    }

    public void TweenColor(Color newColor, float duration, OnTweenColorCompleted onTweenColorCompleted)
    {
        Color currentColor = Constants.AllBlockColors[ColorIndex];

        Tween.Custom(currentColor, newColor, duration: duration,
                onValueChange: newVal => _customMaterialProperty.ChangeColor(newVal))
            .OnComplete(() => onTweenColorCompleted());
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

        if (_letter == null)
        {
            Value = Number.ToString();
        }
        else
        {
            Value = Number.ToString() + _letter;
        }

        valueText.text = Value.ToUpper();
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