using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class Constants
{
    public static Color[] AllBlockColors =
    {
        // FromRGB(227, 0, 133),
        // FromRGB(255, 222, 0),
        // FromRGB(244, 109, 27),
        // FromRGB(29, 194, 115),
        // FromRGB(23, 139, 216),
        FromRGB(243, 159, 90),
        FromRGB(174, 68, 90),
        FromRGB(102, 37, 73),
        FromRGB(69, 25, 82),
        FromRGB(0, 0, 0)
    };

    public static Color[] AllBlockTextColors =
   {
        // Color.white,
        // Color.black,
        // Color.white,
        // Color.black,
        // Color.white
        // FromRGBShift(227, 0, 133, false),
        // FromRGBShift(255, 222, 0, true),
        // FromRGBShift(244, 109, 27, false),
        // FromRGBShift(29, 194, 115, true),
        // FromRGBShift(23, 139, 216, false),
        FromRGBShift(243, 159, 90, true),
        FromRGBShift(174, 68, 90, false),
        FromRGBShift(102, 37, 73, false),
        FromRGBShift(69, 25, 82, false),
        FromRGBShift(0, 0, 0, false)

    };

    private static Color FromRGB(int r, int g, int b)
    {
        return new Color(r / 255f, g / 255f, b / 255f, 1);
    }

    private static Color FromRGBShift(int r, int g, int b, bool isDarker)
    {
        float multiplier = 1 / 3.8f;

        if (isDarker) return new Color(r / 255f * multiplier, g / 255f * multiplier, b / 255f * multiplier, 1);
        else return Color.white;
    }
}