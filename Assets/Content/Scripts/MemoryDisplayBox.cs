using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class MemoryDisplayBoxButton : Button
{
    public struct DisplayData
    {
        public string dataType;
        public string data;
        public int memSize;

        public DisplayData(string _dataType = "", string _data = "", int _memSize = 0)
        {
            dataType = _dataType;
            data = _data;
            memSize = _memSize;
        }
    }
    // Once set to true can't be set to false again
    private bool _initialized = false;
    public bool initialized { get { return _initialized; } protected set { _initialized = _initialized ? true : value; } }

    protected bool _isSelected = false;
    public bool isSelected { get { return _isSelected; } set { _isSelected = value; } }
    public RectTransform rect { get; protected set; }
    protected new void Awake()
    {
        rect = GetComponent<RectTransform>();
        this.onClick.AddListener(onPress);
    }

    public abstract void onPress();
}

public class MemoryDisplayBox : MemoryDisplayBoxButton
{
    public RectTransform TextBox;
    public RectTransform AddressBox;
    public RectTransform DataTypeBox;

    public int margin = 2;
    public float textBoxWRatio = 0.6f;
    public float sideBoxHRatio = 0.3f;

    public int height { get; private set; } = 100;
    public int expandedHeightOffset { get; private set; } = 50;
    public int maxHeight { get { return height + expandedHeightOffset; } }

    public float yPos { get { return rect.anchoredPosition.y; } set { rect.anchoredPosition = new Vector2(0, value); } }

    public int memAddress { get; private set; }
    public DisplayData storedData;

    public void Initialize(DisplayData _data, int _memAddr, Vector2 position)
    {
        storedData = _data;
        memAddress = _memAddr;

        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = new Vector2(0, height);
        rect.pivot = new Vector2(0.5f, 1);
        rect.anchoredPosition = position;

        autoLayout();
        initialized = true;
        updateText();
    }

    public void autoLayout()
    {
        float marginPct = margin / 100f;

        TextBox.anchorMin = new Vector2(1 - textBoxWRatio, marginPct);
        TextBox.anchorMax = new Vector2(1 - marginPct, 1 - marginPct);
        TextBox.sizeDelta = Vector2.zero;
        TextBox.anchoredPosition = Vector2.zero;

        AddressBox.anchorMin = new Vector2(marginPct, 1 - marginPct - sideBoxHRatio);
        AddressBox.anchorMax = new Vector2(TextBox.anchorMin.x - marginPct * 2, 1 - marginPct);
        AddressBox.sizeDelta = Vector2.zero;
        AddressBox.anchoredPosition = Vector2.zero;

        DataTypeBox.anchorMin = new Vector2(marginPct, marginPct);
        DataTypeBox.anchorMax = new Vector2(TextBox.anchorMin.x - marginPct * 2, marginPct + sideBoxHRatio);
        DataTypeBox.sizeDelta = Vector2.zero;
        DataTypeBox.anchoredPosition = Vector2.zero;
    }

    private void updateText()
    {
        if (!initialized) return;

        TextBox.gameObject.GetComponent<TextMeshProUGUI>().text = storedData.data;
        AddressBox.gameObject.GetComponent<TextMeshProUGUI>().text = "0x" + memAddress.ToString();
        DataTypeBox.gameObject.GetComponent<TextMeshProUGUI>().text = storedData.dataType;
    }

    public override void onPress()
    {
        isSelected = !isSelected;
        rect.sizeDelta = new Vector2(0,isSelected ? (height + expandedHeightOffset) : height);

        var self = this.gameObject.GetComponent<MemoryDisplayBox>();
        GetComponentInParent<MemoryPanel>().childCallback_SizeChange(ref self);
        autoLayout();
    }
}
