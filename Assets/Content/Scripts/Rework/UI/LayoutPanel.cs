using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

// TODO?: Update inspector UI
[RequireComponent(typeof(RectTransform)),RequireComponent(typeof(CanvasRenderer))]
[ExecuteInEditMode]
public class LayoutPanel : MonoBehaviour
{
    private int[] _panelColumnsPerRowCount = new int[] { 1 };
    public int[] panelColumnsPerRowCount { get { return _panelColumnsPerRowCount; } set { _panelColumnsPerRowCount = value; rebuildLayout(); } }
    public RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        rebuildLayout();
    }

    private void DestroyChildren()
    {
        foreach (var child in GetComponentsInChildren<RectTransform>())
            if (child.gameObject != this.gameObject)
                GameObject.Destroy(child.gameObject);
    }

    // TODO: Add switch case for layout style, for now only evenly distributed
    // TODO: Remove color panel, add panel interface for all panel types to be spawned instead
    protected void rebuildLayout()
    {
        DestroyChildren();
        GameObject panelPrefab = DSPrefabs.GetPrefab(DSPrefabs.PrefabEnums.DEBUG_COLOR_PANEL);

        float pHeight = rect.sizeDelta.y / (float)_panelColumnsPerRowCount.Length;
        for (int row = 0; row < _panelColumnsPerRowCount.Length; row++)
        {
            float colCount = _panelColumnsPerRowCount[row];
            float pWidth = rect.sizeDelta.x / colCount;

            for (int col = 0; col < colCount; col++)
            {
                var panel = Instantiate(panelPrefab).GetComponent<ColorPanel>();
                panel.gameObject.name = $"LPanel_{row}-{col}";
                panel.rect.SetParent(transform);

                panel.rect.sizeDelta = new Vector2(pWidth, pHeight);

                var xOffset = (col + .5f) * pWidth - .5f * rect.sizeDelta.x;
                var yOffset = pHeight * (.5f - row + _panelColumnsPerRowCount.Length - 1) - .5f * rect.sizeDelta.y;

                panel.rect.anchoredPosition = new Vector2(xOffset, yOffset);

                panel.SetRandomColor();
            }
        }
    }

    public GameObject findChild(int row, int col)
    {
        string cName = $"LPanel_{row}-{col}";
        foreach (var child in GetComponentsInChildren<RectTransform>())
            if (child.gameObject.name.Equals(cName))
                return child.gameObject;

        return null;
    }

    public bool rebuild = false;
    public int[] tmp;
    private void Update()
    {
        if (rebuild)
        {
            rebuild = false;
            if (tmp.Length > 0)
                panelColumnsPerRowCount = tmp;
        }
    }
}
