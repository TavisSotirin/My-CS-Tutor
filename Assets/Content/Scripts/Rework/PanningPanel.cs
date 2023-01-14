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
public class PanningPanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler, IPointerExitHandler, IScrollHandler
{
    [SerializeField]
    private GameObject viewport;
    private RectTransform viewRect;
    private bool inputDragging = false;
    private PointerEventData lastEventData;

    public float minScale = 1f;
    public float maxScale = 10f;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        viewRect = viewport.GetComponent<RectTransform>();
        viewRect.anchoredPosition = viewRect.sizeDelta = Vector2.zero;
        viewRect.anchorMin = Vector2.zero;
        viewRect.pivot = Vector2.zero;

        DEBUG_BuildStruct();
    }

    public GameObject NodePrefab;

    private void DEBUG_BuildStruct()
    {
        int[] arr = {10,20,30,40,50,100};
        int nodeOffset = 0;

        for (int i = 0; i < arr.Length; i++)
        {
            var newNode = GameObject.Instantiate(NodePrefab, viewRect).GetComponent<DSViewNode>();

            newNode.setText(arr[i].ToString(), i.ToString());
            newNode.transform.position = new Vector3(i * newNode.bounds.size.x + nodeOffset,0,0);
        }
    }

    // Set dragging bool and event data to start drag
    public void OnPointerDown(PointerEventData eventData)
    {
        inputDragging = true;
        lastEventData = eventData;
    }

    // Set dragging bool to stop drag
    public void OnPointerUp(PointerEventData eventData)
    {
        inputDragging = false;
        lastEventData = null;
    }

    // Needed?
    public void OnPointerMove(PointerEventData eventData)
    {
    }
    // Needed?
    public void OnPointerExit(PointerEventData eventData)
    {
    }

    // Scale viewport with scroll input, bounded to min/max scale vars
    public void OnScroll(PointerEventData eventData)
    {
        if (!inputDragging)
        {
            viewport.GetComponent<RectTransform>().localScale += new Vector3(eventData.scrollDelta.y, eventData.scrollDelta.y, 0);

            // Scale bounds
            if (viewRect.localScale.y > maxScale || viewRect.localScale.x > maxScale)
                viewRect.localScale = new Vector3(maxScale, maxScale, 1);
            else if (viewRect.localScale.y < minScale || viewRect.localScale.x < minScale)
                viewRect.localScale = new Vector3(minScale, minScale, 1);
        }
    }

    private void Update()
    {
        if(inputDragging) dragUpdate();
    }

    private void dragUpdate()
    {
        if (lastEventData != null)
            viewRect.anchoredPosition += lastEventData.delta;
    }
}
