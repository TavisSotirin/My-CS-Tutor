using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

[RequireComponent(typeof(RectTransform))]
public class PanningPanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler, IPointerExitHandler
{
    public GameObject viewport;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        
        /*
        GameObject viewPanel = new("PanningView", typeof(RectTransform), typeof(EventTrigger));
        viewPanel.transform.SetParent(this.transform);
        */

        var rect = viewport.GetComponent<RectTransform>();
        rect.anchoredPosition = rect.sizeDelta = Vector2.zero;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.zero;
        rect.pivot = Vector2.zero;

        int size = 10;

        for (int i = 0; i < 4; i++)
        {
            var shape = UIShape.DrawRect(size, size, viewport.transform, .15f, false);
            var sRect = shape.GetComponent<RectTransform>();
            //sRect.SetParent(viewport.transform);
            sRect.anchoredPosition = new Vector2(size * i, 0);
        }
    }

    //Vector2 lastMousePos = Vector2.zero;
    bool drag = false;
    PointerEventData lastEvent;

    public void OnPointerDown(PointerEventData eventData)
    {
        print("Begin drag called in pan panel - " + this.name);
        eventData.dragging = true;
        //eventData.pressPosition = viewport.GetComponent<RectTransform>().anchoredPosition;
        //lastMousePos = eventData.pressPosition;
        eventData.useDragThreshold = false;

        drag = true;
        lastEvent = eventData;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        print("End drag called in pan panel");
        //print(eventData.pressPosition);
        //print(eventData.position);
        eventData.dragging = false;

        drag = false;
        lastEvent = null;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (eventData.dragging)
        {
            viewport.GetComponent<RectTransform>().anchoredPosition += eventData.delta;
            //lastMousePos = eventData.position;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        eventData.dragging = true;
    }

    private void Update()
    {
        if(drag)
        {
            dragUpdate();
        }
    }

    private void dragUpdate()
    {
        if (lastEvent != null)
        {
            print(lastEvent.delta);
        }
    }
}
