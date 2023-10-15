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
        _materialPropertyBlock.SetColor("_BaseColor", newColor);
        _renderer.SetPropertyBlock(_materialPropertyBlock);
    }
}