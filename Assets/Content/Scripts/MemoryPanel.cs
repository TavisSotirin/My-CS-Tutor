using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MemoryPanel : UIPanelLib
{
    public GameObject MemoryDisplayBoxPrefab;
    public float defaultBoxHeight = 100;
    public int boxMargin = 10;
    public const int startingMemAddr = 1000;

    // tmp until ds class set up
    public string[] strings2 = { "ArrayBox0", "ArrayBox1", "ArrayBox2", "ArrayBox3", "ArrayBox4", "ArrayBox5" };
    
    private LinkedList<MemoryDisplayBox> _displayBoxes = new LinkedList<MemoryDisplayBox>();

    public Vector2 margins = new Vector2(5, 2);
    public float listYPos { get; private set; } = 0;
    private Vector2 minMaxYDeadZone = new Vector2(Screen.height, 0);

    public override void Initialize()
    {
        buildListFull();
        print("Init mem panel");
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            scrollList(1);
        }
    }

    public void autoLayout()
    {

    }

    private void buildListFull()
    {
        var viewRect = (new GameObject()).AddComponent<RectTransform>();
        viewRect.gameObject.name = "MaskingContainer";
        viewRect.gameObject.AddComponent<RectMask2D>();
        viewRect.transform.SetParent(this.transform);

        viewRect.anchorMin = new Vector2(margins.x / 100, margins.y / 100);
        viewRect.anchorMax = new Vector2((100 - margins.x) / 100, (100 - margins.y) / 100);
        viewRect.sizeDelta = Vector2.zero;
        viewRect.anchoredPosition = Vector2.zero;

        //float viewablePanelHeight = IUI.GetTrueScreenSize(ref viewRect).y;

        //int hiddenBoxCount = (defaultBoxHeight + boxMargin);
        int boxSpawnCount = (int)(IUI.GetTrueScreenSize(ref viewRect).y / (defaultBoxHeight + boxMargin)) + 1 + 2;

        int yOffset = (int)(defaultBoxHeight + boxMargin);
        int memAddr = startingMemAddr;
        for (int i = 0; i < boxSpawnCount; i++)
        {

            //for (int i = 0; i < strings.Length; i++)
            {
                var box = Instantiate(MemoryDisplayBoxPrefab, viewRect).GetComponent<MemoryDisplayBox>();
                box.gameObject.name = "MemBox_" + i.ToString();

                MemoryDisplayBox.DisplayData displayData = new();
                if (i < strings2.Length)
                {
                    displayData.data = strings2[i];
                    displayData.dataType = strings2.GetType().ToString(); //"String";
                    displayData.memSize = strings2[i].Length;
                }

                box.Initialize(displayData, memAddr, new Vector2(0, yOffset));

                yOffset -= box.height + boxMargin;

                memAddr += displayData.memSize;
                _displayBoxes.AddLast(box);
            }
        }
    }

    public void buildList()
    {
        var viewRect = (new GameObject()).AddComponent<RectTransform>();
        viewRect.gameObject.name = "MaskingContainer";
        viewRect.gameObject.AddComponent<RectMask2D>();
        viewRect.transform.parent = this.transform;

        viewRect.anchorMin = new Vector2(margins.x/100, margins.y/100);
        viewRect.anchorMax = new Vector2((100-margins.x)/100, (100-margins.y)/100);
        viewRect.sizeDelta = Vector2.zero;
        viewRect.anchoredPosition = Vector2.zero;

        tmp_BuildStructureMemory(viewRect);
    }

    private void tmp_BuildStructureMemory(RectTransform viewRect)
    {
        int yOffset = 0;
        int memAddr = 1000;

        for (int i = 0; i < strings2.Length; i++)
        {
            var box = Instantiate(MemoryDisplayBoxPrefab, viewRect).GetComponent<MemoryDisplayBox>();

            MemoryDisplayBox.DisplayData displayData = new();
            displayData.data = strings2[i];
            displayData.dataType = "String";
            displayData.memSize = strings2[i].Length;

            box.Initialize(displayData, memAddr, new Vector2(0, -yOffset));

            yOffset += box.height + boxMargin;

            memAddr += displayData.memSize;
            _displayBoxes.AddLast(box);
        }
    }

    public void childCallback_SizeChange(ref MemoryDisplayBox box)
    {
        var boxNode = _displayBoxes.Find(box);

        if (boxNode != null)
        {
            print("Child call back in mem panel. Found child in list");
            float yOffset = Mathf.Abs(boxNode.Value.yPos) + boxNode.Value.height + (boxNode.Value.isSelected ? boxNode.Value.expandedHeightOffset : 0) + boxMargin;
            boxNode = boxNode.Next;
            
            while (boxNode != null)
            {
                boxNode.Value.yPos = -yOffset;
                yOffset += boxNode.Value.height + (boxNode.Value.isSelected ? boxNode.Value.expandedHeightOffset : 0) + boxMargin;
                boxNode = boxNode.Next;
            }
        }
    }

    private void scrollList(float yOffset)
    {

    }

    public void scrollListold(float yOffset)
    {
        listYPos += yOffset;

        foreach (var box in _displayBoxes)
        {
            box.yPos += yOffset;

            var curRect = box.rect;

            //if (Mathf.Abs(box.yPos) > listYDeadZone || Mathf.Abs(box.yPos) > )
            //if(IUI.GetTrueScreenLocation(ref curRect).y > listYDeadZone)
            // UPDATE: Add || < -deadzone
            // UPDATE: Change to teleport box to lower section if able to avoid wasteful create/destory calls
            // UPDATE: Add if else for boxes that need to scroll in from off screen
            if (IUI.GetTrueScreenLocation(ref curRect).y > Screen.height + box.maxHeight)
            {
                _displayBoxes.Remove(box);
                Destroy(box.gameObject);


                // Below screen - move to head of list and use at top of screen
                if (box.yPos <= minMaxYDeadZone.x)
                {
                    _displayBoxes.Remove(box);
                    _displayBoxes.AddFirst(box);
                }
                // Above screen - move to tail of list and use at bottom of screen
                else if (box.yPos >= minMaxYDeadZone.y)
                {

                }
            }
        }
    }
}
