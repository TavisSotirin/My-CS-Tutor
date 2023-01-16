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
public class PanningPanel : MonoBehaviour, IScrollHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
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
        viewRect.anchorMin = Vector2.zero;
        viewRect.anchorMax = Vector2.one;
        viewRect.pivot = Vector2.zero;
        viewRect.anchoredPosition = viewRect.sizeDelta = Vector2.zero;

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
            //newNode.transform.position = new Vector3(i * newNode.bounds.size.x + nodeOffset,0,0);
            newNode.GetComponent<RectTransform>().anchoredPosition = new Vector2(i * newNode.bounds.size.x + nodeOffset, 0);
        }
    }

    // Calculate panels normalized pivot location based on a provided screen location
    // Note: Only meant to be used by panels with centered pivots and anchors and who have parents with the same
    //     : sizeDelta and anchoredPosition are not the same when this is not the case
    // UPDATE: Does not work with variable scale and position on viewRect - need to fix calc 
    private Vector2 calcPivotFromScreenPos(Vector2 screenPos)
    {
        var rect = GetComponent<RectTransform>();

        // Calc panel mask center screen pos
        Vector2 pivot = new Vector2(rect.anchoredPosition.x + (Screen.width) * 0.5f, rect.anchoredPosition.y + (Screen.height) * 0.5f);
        // Calc normalized screenPos loc within panel mask
        pivot = (screenPos - pivot) / rect.sizeDelta + new Vector2(0.5f, 0.5f);
        // Offset pivot by normalized viewRect anchor offset, using viewRect current scale
        pivot += viewRect.anchoredPosition / (rect.sizeDelta * viewRect.localScale);

        return pivot;
    }

    // Scale viewport with scroll input, bounded to min/max scale vars
    public void OnScroll(PointerEventData eventData)
    {
        if (!inputDragging)
        {
            var origPivot = viewRect.pivot;
            var scrollScale = 0.1f;

            // Set pivot to normalized pointer pos within viewport, then scale, then reset pivot
            viewRect.pivot = calcPivotFromScreenPos(eventData.position) * viewRect.localScale;
            viewRect.localScale += new Vector3(eventData.scrollDelta.y * scrollScale, eventData.scrollDelta.y * scrollScale, 0);

            // Scale bounds
            if (viewRect.localScale.y > maxScale || viewRect.localScale.x > maxScale)
                viewRect.localScale = new Vector3(maxScale, maxScale, 1);
            else if (viewRect.localScale.y < minScale || viewRect.localScale.x < minScale)
                viewRect.localScale = new Vector3(minScale, minScale, 1);

            //viewRect.pivot = origPivot;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        print("Pan panel drag start");
    }

    public void OnDrag(PointerEventData eventData)
    {
        viewRect.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        print("Pan panel drag end");
    }

    private void Update()
    {
        //if(inputDragging) dragUpdate();
    }

    private void dragUpdate()
    {
        if (lastEventData != null)
            viewRect.anchoredPosition += lastEventData.delta;
    }
}
