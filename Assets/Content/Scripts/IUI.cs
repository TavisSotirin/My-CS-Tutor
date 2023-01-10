using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUI // static class instead?
{
    public static Vector2 GetTrueScreenSize(ref RectTransform _rect)
    {
        var parent = _rect.parent;
        _rect.SetParent(null);
        var screenSize = _rect.sizeDelta;
        _rect.SetParent(parent);
        return screenSize;
    }

    public static Vector2 GetTrueScreenLocation(ref RectTransform _rect)
    {
        var parent = _rect.parent;
        _rect.SetParent(null);
        var screenLoc = _rect.anchoredPosition;
        _rect.SetParent(parent);
        return screenLoc;
    }

    public static void GetTrueScreenTransform(ref RectTransform _rect, ref RectTransform trueTransform)
    {
        var parent = _rect.parent;
        _rect.SetParent(null);
        trueTransform = _rect;
        _rect.SetParent(parent);
    }
}