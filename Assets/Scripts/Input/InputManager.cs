using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    [SerializeField] private UnityEvent swipeEvent;
    [SerializeField] private GameManager gameManager;

    private void Start()
    {
        // swipeEvent.AddListener(gameManager.OnSwipe);
        StartCoroutine(Check());
    }

    IEnumerator Check()
    {
        while (true)
        {
            var mouseY = Input.GetAxis("Mouse Y");

            if (gameManager.InteractMode == InteractMode.Normal)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    gameManager.OnSwipe();
                }
            }
            else if (gameManager.InteractMode == InteractMode.Swap)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    gameManager.OnSwapSelect();
                }
            }

            // if (mouseY > 0)
            // {
            //     swipeEvent.Invoke();
            // }

            yield return new WaitForSeconds(0.002f);
        }
    }
}