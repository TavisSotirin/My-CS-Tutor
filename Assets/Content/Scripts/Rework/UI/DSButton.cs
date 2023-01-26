using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;

public class DSButton : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI text;
    public RectTransform rect;

    public void addListener(UnityEngine.Events.UnityAction action)
    {
        button.onClick.AddListener(action);
    }

    public void setText(string _text)
    {
        text.text = _text;
    }

    public void setup(string _text, UnityEngine.Events.UnityAction _action, Vector2 _size)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(_action);

        rect.sizeDelta = _size;

        text.text = _text;
    }

    public void setup(string _text, UnityEngine.Events.UnityAction _action, Vector2 _size, Vector2 _screenPos)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(_action);

        rect.sizeDelta = _size;

        text.text = _text;

        // TODO: Update position correctly
        var p = rect.parent;
        rect.SetParent(null);
        rect.sizeDelta = _size;
        rect.anchoredPosition = _screenPos;
        rect.SetParent(p);
    }
}
