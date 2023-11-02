using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class Constants
{
    public enum Theme
    {
        Default,
        Classic,
        EasterEgg,
        Christmas,
        Tropical,
        Ocean
    }

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
        FromRGB(35, 35, 35)
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

    public static Color[] AllBlockColorsClassic =
    {
        FromRGB(227, 0, 133),
        FromRGB(255, 222, 0),
        FromRGB(244, 109, 27),
        FromRGB(29, 194, 115),
        FromRGB(23, 139, 216),
    };

    public static Color[] AllBlockTextColorsClassic =
    {
       FromRGBShift(227, 0, 133, false),
        FromRGBShift(255, 222, 0, true),
        FromRGBShift(244, 109, 27, false),
        FromRGBShift(29, 194, 115, true),
        FromRGBShift(23, 139, 216, false),
    };

    public static Color[] AllBlockColorsEasterEgg =
    {
        FromHex("#FDCEDE"),
        FromHex("#9EF8DF"),
        FromHex("#FFFCB8"),
        FromHex("#FCC470"),
        FromHex("#77F8FD"),
    };

    public static Color[] AllBlockTextColorsEasterEgg =
    {
        FromHexShift("#FDCEDE"),
        FromHexShift("#9EF8DF"),
        FromHexShift("#FFFCB8"),
        FromHexShift("#FCC470"),
        FromHexShift("#77F8FD"),
    };

    public static Color[] AllBlockColorsChristmas =
    {
        FromHex("#386641"),
        FromHex("#6a994e"),
        FromHex("#a7c957"),
        FromHex("#f2e8cf"),
        FromHex("#bc4749"),
    };

    public static Color[] AllBlockTextColorsChristmas =
    {
        FromHexShift("#386641", false),
        FromHexShift("#6a994e", false),
        FromHexShift("#a7c957"),
        FromHexShift("#f2e8cf"),
        FromHexShift("#bc4749", false),
    };

    // TROPICAL
    public static Color[] AllBlockColorsTropical =
    {
        FromHex("#264653"),
        FromHex("#2a9d8f"),
        FromHex("#e9c46a"),
        FromHex("#f4a261"),
        FromHex("#e76f51"),
    };

    public static Color[] AllBlockTextColorsTropical =
    {
        FromHexShift("#264653", false),
        FromHexShift("#2a9d8f", false),
        FromHexShift("#e9c46a"),
        FromHexShift("#f4a261"),
        FromHexShift("#e76f51", false),
    };

    // OCEAN
    public static Color[] AllBlockColorsOcean =
    {
        FromHex("#e63946"),
        FromHex("#f1faee"),
        FromHex("#a8dadc"),
        FromHex("#457b9d"),
        FromHex("#1d3557"),
    };

    public static Color[] AllBlockTextColorsOcean =
    {
        FromHexShift("#e63946", false),
        FromHexShift("#f1faee"),
        FromHexShift("#a8dadc", false),
        FromHexShift("#457b9d", false),
        FromHexShift("#1d3557", false),
    };

    public static string[] backgroundAddressableKeys = {
        "default_background", "classic_background", "Easter_Egg_background", "Christmas_background", "tropical_forest_background", "under_sea_background"
    };

    public static Color[] GetColorInTheme(Theme theme)
    {
        if (theme == Theme.Default) return AllBlockColors;
        if (theme == Theme.Classic) return AllBlockColorsClassic;
        if (theme == Theme.EasterEgg) return AllBlockColorsEasterEgg;
        if (theme == Theme.Christmas) return AllBlockColorsChristmas;
        if (theme == Theme.Tropical) return AllBlockColorsTropical;
        if (theme == Theme.Ocean) return AllBlockColorsOcean;
        else return AllBlockColors;
    }

    public static Color[] GetTextColorInTheme(Theme theme)
    {
        if (theme == Theme.Default) return AllBlockTextColors;
        if (theme == Theme.Classic) return AllBlockTextColorsClassic;
        if (theme == Theme.EasterEgg) return AllBlockTextColorsEasterEgg;
        if (theme == Theme.Christmas) return AllBlockTextColorsChristmas;
        if (theme == Theme.Tropical) return AllBlockTextColorsTropical;
        if (theme == Theme.Ocean) return AllBlockTextColorsOcean;
        else return AllBlockTextColors;
    }


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

    private static Color FromHex(string hex)
    {
        Color color;

        UnityEngine.ColorUtility.TryParseHtmlString(hex, out color);

        return color;
    }

    private static Color FromHexShift(string hex, bool isDarker = true)
    {
        float multiplier = 1 / 4f;
        Color color;

        UnityEngine.ColorUtility.TryParseHtmlString(hex, out color);

        if (isDarker) return new Color(color.r * multiplier, color.g * multiplier, color.b * multiplier, 1);
        else return Color.white;
    }
}