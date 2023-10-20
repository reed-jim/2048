using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CustomMaterialProperty : MonoBehaviour
{
    [SerializeField] private Color color;

    private MaterialPropertyBlock _materialPropertyBlock;
    private Renderer _renderer;

    private void Awake()
    {
        _materialPropertyBlock = new MaterialPropertyBlock();
        _renderer = GetComponent<Renderer>();

        ChangeColor();
    }

    void OnValidate()
    {
        if (_materialPropertyBlock == null)
        {
            _materialPropertyBlock = new MaterialPropertyBlock();
        }

        if (_renderer == null)
        {
            _renderer = GetComponent<Renderer>();
        }

        ChangeColor();
    }

    private void ChangeColor()
    {
        _materialPropertyBlock.SetColor("_BaseColor", color);
        _renderer.SetPropertyBlock(_materialPropertyBlock);
    }

    public void ChangeColor(Color newColor)
    {
        _materialPropertyBlock.SetColor("_Color_1", newColor);
        // _materialPropertyBlock.SetColor("_Color_2", newColor + new Color(0.3f, 0.3f, 0.3f, 0));
        _materialPropertyBlock.SetColor("_Color_2", new Color(newColor.r * 1.2f, newColor.g * 1.2f, newColor.b * 1.2f, 1));
        _renderer.SetPropertyBlock(_materialPropertyBlock);
    }
}