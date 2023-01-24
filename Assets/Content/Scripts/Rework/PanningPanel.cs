using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(RectTransform))]
public class PanningPanel : MonoBehaviour, IScrollHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private GameObject viewport;
    [SerializeField]
    private RectTransform viewportLLCorner;
    private RectTransform viewRect;

    public float minScale = 0.5f;
    public float maxScale = 10f;
    private float scrollScale = .1f;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        viewRect = viewport.GetComponent<RectTransform>();

        viewRect.anchorMin = viewRect.anchorMax = viewRect.pivot = Vector2.zero;
        viewRect.sizeDelta = GetComponent<RectTransform>().sizeDelta;
        viewRect.anchoredPosition = Vector2.zero;

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

    public void OnScroll(PointerEventData eventData)
    {
        var scrollOffset = 0f;
        if (eventData.scrollDelta.y < 0)
            scrollOffset = eventData.scrollDelta.y * scrollScale * viewRect.localScale.x;
        else
            scrollOffset += eventData.scrollDelta.y * scrollScale * viewRect.localScale.x;

        if (scrollOffset + viewRect.localScale.x > maxScale || scrollOffset + viewRect.localScale.x < minScale)
            return;

        var relNormMousePivot = (Input.mousePosition - viewRect.position) / (viewRect.sizeDelta * viewRect.localScale);
        viewRect.localScale = new Vector2(viewRect.localScale.x + scrollOffset, viewRect.localScale.y + scrollOffset);
        var newPos = Input.mousePosition - (Vector3)(relNormMousePivot * viewRect.sizeDelta * viewRect.localScale);
        positionVPanel(newPos);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //print($"Pan panel drag start - {viewRect.position}");
    }

    public void OnDrag(PointerEventData eventData)
    {
        var epsilon = 0.01f;
        if (viewRect.localScale.x <= minScale + epsilon)
        {
            viewRect.anchoredPosition = Vector2.zero;
            return;
        }

        //viewRect.position += (Vector3)offsetVPanel(eventData.delta);
        offsetVPanel(eventData.delta);
    }

    private void offsetVPanel(Vector2 offset)
    {
        var parent = viewRect.parent.GetComponent<RectTransform>();
        var upCorner = (Vector2)parent.position + parent.sizeDelta * 0.5f; // These are constant values if the parent panel doesnt change size. Update to be member var that sets itself if needed (dirty panel?)
        var lowCorner = (Vector2)parent.position - parent.sizeDelta * 0.5f;
        var bound1 = (Vector2)viewRect.position - lowCorner; // should be 0 or negative
        var bound2 = ((Vector2)viewRect.position + viewRect.sizeDelta * viewRect.localScale) - upCorner; // should be positive or 0

        if (bound1.x + offset.x > 0)
            offset.x = 0;
        if (bound1.y + offset.y > 0)
            offset.y = 0;

        if (bound2.x + offset.x < 0)
            offset.x = 0;
        if (bound2.y + offset.y < 0)
            offset.y = 0;

        viewRect.position += (Vector3)offset;
    }

    // UPDATE: Positioning on scale doesn't work correctly
    private void positionVPanel(Vector2 newPos)
    {
        var parent = viewRect.parent.GetComponent<RectTransform>();
        var upCorner = (Vector2)parent.position + parent.sizeDelta * 0.5f; // These are constant values if the parent panel doesnt change size. Update to be member var that sets itself if needed (dirty panel?)
        var lowCorner = (Vector2)parent.position - parent.sizeDelta * 0.5f;

        var upNewPos = newPos + viewRect.sizeDelta * viewRect.localScale;

        if (upCorner.x > upNewPos.x)
            newPos.x = upCorner.x;
        if (upCorner.y > upNewPos.y)
            newPos.y = upCorner.y;
        if (lowCorner.x < newPos.x)
            newPos.x = lowCorner.x;
        if (lowCorner.y >= newPos.y)
            newPos.y = lowCorner.y;

        viewRect.position = newPos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //print($"Pan panel drag end - {viewRect.position}");
    }

    private void Update()
    {
        
    }
}
