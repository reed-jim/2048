using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

public static class Utils
{
    public static Vector3 GetMousePosition()
    {
        Vector3 worldPosition = Vector3.zero;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            worldPosition = hit.point;
        }

        return worldPosition;
    }

    public static string ToAbbreviatedNumber(int number, int decimalPlace = 2)
    {
        // char initialLetter = 'a';
        // string result = score.ToString();
        //
        // for (int i = 3; i < 16; i++)
        // {
        //     if (score > Mathf.Pow(10, i) && score < Mathf.Pow(10, i + 1))
        //     {
        //         result = (score / Mathf.Pow(10, i)).ToString("F2") + ((char)((int)initialLetter + (i - 3)));
        //     }
        // }

        string result = number switch
        {
            > 1000 and < 1000000 => (number / 1000f).ToString($"F{decimalPlace}") + "K",
            > 1000000 => (number / 1000f).ToString($"F{decimalPlace}") + "M",
            _ => number.ToString()
        };

        return result;
    }

    public static void ScaleUpDownUI(RectTransform target, float duration)
    {
        Vector3 initialScale = target.localScale;
        Vector3 newScale = 1.1f * initialScale;

        Tween.Scale(target, newScale, duration: duration, cycles: -1, cycleMode: CycleMode.Yoyo);
    }

    public static void MoveUpDownUI(RectTransform target)
    {
        Vector3 initialPosition = target.localPosition;
        Vector3 newPosition = initialPosition - new Vector3(0, 0.15f * target.sizeDelta.y, 0);

        Tween.LocalPosition(target, newPosition, duration: 0.5f, cycles: 50, cycleMode: CycleMode.Yoyo);
    }
}