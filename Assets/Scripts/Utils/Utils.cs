using System.Collections;
using System.Collections.Generic;
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
            > 1000 and < 1000000 => (number / 1000).ToString($"F{decimalPlace}") + "K",
            > 1000000 => (number / 1000).ToString($"F{decimalPlace}") + "M",
            _ => number.ToString()
        };

        return result;
    }
}