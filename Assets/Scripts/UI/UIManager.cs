using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private RectTransform menu;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button continueButton;

    private void Start()
    {
        menuButton.onClick.AddListener(OpenMenu);
        continueButton.onClick.AddListener(CloseMenu);
    }

    private void OpenMenu()
    {
        menu.gameObject.SetActive(true);
    }

    private void CloseMenu()
    {
        menu.gameObject.SetActive(false);
    }
}