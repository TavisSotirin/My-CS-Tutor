using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class GridLayoutPanel : MonoBehaviour
{
    public GridPanelRow[] LayoutMatrix;
    public Margins margins = Vector4.zero;
    public float padding = 0;
    public RectTransform rect;
    
    private void Start()
    {
        updateLayout();
    }

    private void updateLayout()
    {
        var panelSize = IUIDisplayPanel.GetElementSize(ref rect);

        //var gridSize = panelSize / uiDisplayElements.Length;
    }
}

[Serializable]
public struct GridPanelRow
{
    [Serializable]
    internal struct DisplayPanelStruct
    {
        public IUIDisplayPanel panel;
        public int priority;
    }

    [SerializeField]
    DisplayPanelStruct[] uiDisplayPanels;
}

[Serializable]
public struct Margins
{
    public float top;
    public float bottom;
    public float left;
    public float right;

    public Margins(float _top = 0, float _bottom = 0, float _left = 0, float _right = 0)
    {
        top = _top;
        bottom = _bottom;
        left = _left;
        right = _right;
    }

    public static implicit operator Margins(Vector4 inVec) => new Margins(inVec.x, inVec.y, inVec.z, inVec.w);
    public static implicit operator Vector4(Margins inMargins) => new Vector4(inMargins.top, inMargins.bottom, inMargins.left, inMargins.right);
}

public interface IUIDisplayPanel
{
    public static Vector2 GetElementSize(ref RectTransform rect)
    {
        // Check for stretch anchors
        var anchorDiff = rect.anchorMax - rect.anchorMin;
        if (anchorDiff.x == 0 && anchorDiff.y != 0 || anchorDiff.y == 0 && anchorDiff.x != 0 || (Mathf.Abs(anchorDiff.x) == 1f && Mathf.Abs(anchorDiff.y) == 1f))
        {
            var parent = rect.parent;
            rect.SetParent(null);
            var size = rect.sizeDelta;
            rect.SetParent(parent);

            return size;
        }
        else
            return rect.sizeDelta;
    }
}