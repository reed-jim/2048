using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public static class UtilsUI
{
    private static Vector2 _screenSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);

    public static Vector2 GetSizeByScale(RectTransform target)
    {
        return new Vector2(target.sizeDelta.x * target.localScale.x, target.sizeDelta.y * target.localScale.y);
    }

    public static void SetTextFontSize(TMP_Text text, float proportion)
    {
        text.fontSize = proportion * _screenSize.x;
    }

    public static void SetTextPreferredSize(TMP_Text text)
    {
        text.rectTransform.sizeDelta = new Vector2(text.preferredWidth, text.preferredHeight);
    }

    public static void SetSize(RectTransform target, float width, float height)
    {
        target.sizeDelta = new Vector2(width, height);
    }

    public static void SetSizeEqual(RectTransform target, float width)
    {
        target.sizeDelta = new Vector2(width, width);
    }

    public static void SetSizeKeepRatioX(Image target, float height)
    {
        float ratio = target.sprite.rect.size.x / target.sprite.rect.size.y;
        target.rectTransform.sizeDelta = new Vector2(height * ratio, height);
    }

    public static void SetSizeKeepRatioY(Image target, float width)
    {
        float ratio = target.sprite.rect.size.y / target.sprite.rect.size.x;
        target.rectTransform.sizeDelta = new Vector2(width, width * ratio);
    }

    public static void SetLocalScale(RectTransform target, float scaleX, float scaleY)
    {
        target.localScale = new Vector3(scaleX, scaleY, 1);
    }

    public static void SetLocalScaleEqual(RectTransform target, float scaleX)
    {
        target.localScale = new Vector3(scaleX, scaleX, 1);
    }

    public static void SetLocalPosition(RectTransform target, float x, float y)
    {
        target.localPosition = new Vector3(x, y, 0);
    }

    public static void SetLocalPositionX(RectTransform target, float x)
    {
        SetLocalPosition(target, x, 0);
    }

    public static void SetLocalPositionY(RectTransform target, float y)
    {
        SetLocalPosition(target, 0, y);
    }
}
