using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public abstract class UIPanelLib : MonoBehaviour
{
    public RectTransform rect { get; private set; }
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }
    public abstract void Initialize();
    
}
