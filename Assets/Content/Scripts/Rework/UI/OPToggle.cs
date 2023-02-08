using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OPToggle : MonoBehaviour
{
    [SerializeField]
    private Toggle toggle;
    [SerializeField]
    private Text text;
    public RectTransform rect;

    public void addListener(UnityAction<bool> action)
    {
        toggle.onValueChanged.AddListener(action);
    }

    public void setText(string _text)
    {
        text.text = _text;
    }
}
