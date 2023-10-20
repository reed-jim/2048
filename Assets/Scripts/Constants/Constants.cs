using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public static Color[] AllBlockColors =
    {
        // new Color(255f / 255, 54f / 255, 54f / 255, 1),
        // new Color(252f / 255, 205f / 255, 54f / 255, 1),
        // new Color(137f / 255, 255f / 255, 54f / 255, 1),
        // new Color(54f / 255, 255f / 255, 242f / 255, 1),
        // new Color(215f / 255, 54f / 255, 255f / 255, 1),
        FromRGB(227, 0, 133),
        FromRGB(252, 223, 34),
        FromRGB(244, 109, 27),
        FromRGB(29, 194, 115),
        FromRGB(23, 139, 216)
    };

    private static Color FromRGB(int r, int g, int b)
    {
        return new Color(r / 255f, g / 255f, b / 255f, 1);
    }
}