using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

[RequireComponent(typeof(RectTransform))]
public class PanningPanel : MonoBehaviour, IScrollHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private GameObject viewport;
    [SerializeField]
    private RectTransform viewportLLCorner;
    private RectTransform viewRect;
    private bool inputDragging = false;
    private PointerEventData lastEventData;

    public float minScale = 0.5f;
    public float maxScale = 10f;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        viewRect = viewport.GetComponent<RectTransform>();
        //viewRect.anchorMin = Vector2.zero;
        //viewRect.anchorMax = Vector2.one;
        //viewRect.pivot = Vector2.zero;

        viewRect.anchorMin = viewRect.anchorMax = viewRect.pivot = new Vector2(.5f, .5f);
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

    // Calculate panels normalized pivot location based on a provided screen location
    // Note: Only meant to be used by panels with centered pivots and anchors and who have parents with the same
    //     : sizeDelta and anchoredPosition are not the same when this is not the case
    // UPDATE: Does not work with variable scale and position on viewRect - need to fix calc 
    private Vector2 calcPivotFromScreenPos(Vector2 screenPos)
    {
        var rect = GetComponent<RectTransform>();
        viewportLLCorner.SetParent(null);
        var worldCornerLoc = viewportLLCorner.anchoredPosition;
        viewportLLCorner.SetParent(viewRect);
        viewportLLCorner.anchoredPosition = Vector2.zero;

        Vector2 pivot;

        if (lastMousePos != null)
        {
            pivot = ((Vector2)lastMousePos - worldCornerLoc);
            pivot = new Vector2(Mathf.Abs(pivot.x), Mathf.Abs(pivot.y));
            pivot /= (viewRect.localScale * rect.sizeDelta);
        }
        else
        {
            pivot = (screenPos - worldCornerLoc);
            pivot = new Vector2(Mathf.Abs(pivot.x), Mathf.Abs(pivot.y));
            pivot /= (viewRect.localScale * rect.sizeDelta);
        }

        print(viewport.transform.position);
        print($"{screenPos} - {worldCornerLoc} / ({viewRect.localScale} * {rect.sizeDelta}) = {pivot}");

        // Calc panel mask center screen pos
        //Vector2 pivot = new Vector2(rect.anchoredPosition.x + (Screen.width) * 0.5f, rect.anchoredPosition.y + (Screen.height) * 0.5f);
        // Calc normalized screenPos loc within panel mask
        //pivot = (screenPos - pivot) / rect.sizeDelta + new Vector2(0.5f, 0.5f);
        // Offset pivot by normalized viewRect anchor offset, using viewRect current scale
        //pivot += viewRect.anchoredPosition / (rect.sizeDelta * viewRect.localScale);
        
        return pivot;
    }

    private Vector2? lastPivot = null;
    private Vector2? lastMousePos = null;
    private float scrollDeadZone = 10;
    private float scrollScale = .1f;
    public void OnScrollOld2(PointerEventData eventData)
    {
        if (false)
        {
            var pivot = (eventData.position - (Vector2)viewRect.position) / (viewRect.localScale * GetComponent<RectTransform>().sizeDelta);
            pivot += new Vector2(0.5f, 0.5f);

            print(pivot);

            

            viewRect.pivot = pivot;
            viewRect.localScale += new Vector3(eventData.scrollDelta.y * scrollScale, eventData.scrollDelta.y * scrollScale, 0);


            pivot = (eventData.position - (Vector2)viewRect.position) / (viewRect.localScale * GetComponent<RectTransform>().sizeDelta);
            pivot += new Vector2(0.5f, 0.5f);
            var offset = (pivot - new Vector2(0.5f, 0.5f)) * GetComponent<RectTransform>().sizeDelta;
            print(offset);

            viewRect.anchoredPosition += offset;

            
            viewRect.pivot = new Vector2(0.5f, 0.5f);
        }
    }

    private Vector2 getMousePivot(ref RectTransform panelRect)
    {
        var mousePos = Input.mousePosition;
        var pivot = (mousePos - panelRect.position) / (panelRect.localScale * panelRect.sizeDelta);
        pivot += new Vector2(0.5f, 0.5f);

        // SCREEN COORD: bottom left corner {0,0} // top right corner {Screen.width,Screen.height}
        // PIVOT COORD: bottom left corner {0,0} // top right corner {1,1}
        // GLOBAL COORD: world space coord {0,0,0}
        print($"\n-----\n" +
            $"Mouse position: {mousePos}\n" + /*SCREEN. current mouse position*/
            $"P position: {panelRect.position}\n" + /*SCREEN. pivot position*/
            $"P local position: {panelRect.localPosition}\n" + /**/
            $"P local scale: {panelRect.localScale}\n" + /**/
            $"P size delta: {panelRect.sizeDelta}\n" + /**/
            $"P anchored pos: {panelRect.anchoredPosition}\n" + /**/
            $"P anchor max/min: {panelRect.anchorMax} / {panelRect.anchorMin}\n" + /**/
            $"P pivot: {panelRect.pivot}\n" + /**/
            $"PParent position: {panelRect.parent?.transform.position}\n" + /**/
            $"Calc Pivot: {pivot}"); /**/

        return pivot;
    }

    private Vector2 mousePivot { 
        get 
        {
            return ((Input.mousePosition - viewRect.position) / (viewRect.localScale * viewRect.sizeDelta)) + new Vector2(0.5f, 0.5f);
        }
    }
    private bool viewScrolling = false;
    private float scrollOffsetTarget = 0;
    private float endTime;
    private Coroutine scaleCor = null;
    private float scrollTime = 0.2f;
    private float epsilon = .01f;
    private bool scalePaused = false;

    public void OnScroll(PointerEventData eventData)
    {
        scalePaused = true;
        if (scaleCor != null)
        {
            print("Scroll scale force stop");
            StopCoroutine(scaleCor);
            scaleCor = null;
        }

        endTime = Time.time + scrollTime;
        if (eventData.scrollDelta.y < 0)
            scrollOffsetTarget = eventData.scrollDelta.y * scrollScale * viewRect.localScale.x;
        else
            scrollOffsetTarget += eventData.scrollDelta.y * scrollScale * viewRect.localScale.x;

        scalePaused = false;

        if (scrollOffsetTarget + viewRect.localScale.x > maxScale || scrollOffsetTarget + viewRect.localScale.x < minScale)
            return;

        testScroll();//scaleCor = StartCoroutine("ScaleCoroutine");
    }

    private void testScroll()
    {
        var start = viewRect.localScale.x;
        var target = start + scrollOffsetTarget;

        viewRect.localScale = new Vector2(target, target);

        var x = ((mousePivot - new Vector2(.5f,.5f)) * viewRect.sizeDelta) / viewRect.localScale;

        viewRect.localPosition += (Vector3)x;
        //mousePivot - new Vector2(.5f,.5f)

        //viewRect.anchoredPosition = new Vector2(start, start);
    }

    private IEnumerator ScaleCoroutine()
    {
        print("Scale cor started");
        var start = viewRect.localScale.x;
        var target = start + scrollOffsetTarget;

        float startTime = Time.time;
        float lerp;

        float scaleMin = 0.5f;
        float scaleMax = 10;

        viewScrolling = true;
        while (viewScrolling)
        {
            lerp = (endTime - Time.time) / (endTime - startTime);
            print(lerp);
            lerp = Mathf.Lerp(start, target, lerp);

            lerp = lerp > scaleMin ? (lerp < scaleMax ? lerp : scaleMax) : scaleMin;

            //var offset = viewRect.sizeDelta * (mousePivot - viewRect.pivot);

            viewRect.pivot = mousePivot;


            //viewRect.localScale = new Vector2(lerp, lerp);

            if (Mathf.Abs(lerp - target) < epsilon)
            {
                print("Scroll scale natural stop");
                break;
            }
            else if (lerp == maxScale || lerp == minScale)
            {
                print("-Scroll scale boundary stop");
                break;
            }

            yield return null;
        }
        viewScrolling = false;

        viewRect.localScale = new Vector2(target, target);
        print("Scale cor ending");
        print(scalePaused);

        yield break;
    }    


    // Scale viewport with scroll input, bounded to min/max scale vars
    //private Vector2 lastMousePos = new Vector2(-1,-1);
    public void OnScrollold(PointerEventData eventData)
    {
        if (!inputDragging)
        {
            var origPivot = viewRect.pivot;
            var scrollScale = 0.1f;
            var screenPos = eventData.position;

            var rect = GetComponent<RectTransform>();
            viewportLLCorner.SetParent(null);
            var worldCornerLoc = viewportLLCorner.anchoredPosition;
            viewportLLCorner.SetParent(viewRect);
            viewportLLCorner.anchoredPosition = Vector2.zero;

            var eventPosToLLCornerRatioPre = (screenPos - worldCornerLoc) / (viewRect.localScale * rect.sizeDelta);

            // Set pivot to normalized pointer pos within viewport, then scale, then reset pivot
            //viewRect.pivot = calcPivotFromScreenPos(eventData.position);// * viewRect.localScale;
            viewRect.localScale += new Vector3(eventData.scrollDelta.y * scrollScale, eventData.scrollDelta.y * scrollScale, 0);
            viewRect.pivot = origPivot;

            print(eventData.position - lastMousePos);


            viewportLLCorner.SetParent(null);
            worldCornerLoc = viewportLLCorner.anchoredPosition;
            viewportLLCorner.SetParent(viewRect);
            viewportLLCorner.anchoredPosition = Vector2.zero;
            var newPos = worldCornerLoc + eventPosToLLCornerRatioPre * viewRect.localScale * rect.sizeDelta;
            
            var offset = newPos - screenPos;

            var scaledBounds = (viewRect.localScale - Vector3.one) * (0.5f * rect.sizeDelta);

            var scaledExtent = viewRect.localScale * rect.sizeDelta / 2;

            // Prevent dragging out of mask bounds?
            if (viewRect.anchoredPosition.x > scaledBounds.x)
            {
                offset = new Vector2(scaledBounds.x, offset.y);
            }
            else if (viewRect.anchoredPosition.x < -scaledBounds.x)
            {
                offset = new Vector2(-scaledBounds.x, offset.y);
            }
            if (viewRect.anchoredPosition.y > scaledBounds.y)
            {
                offset = new Vector2(offset.x, scaledBounds.y);
            }
            else if (viewRect.anchoredPosition.y < -scaledBounds.y)
            {
                offset = new Vector2(offset.x, -scaledBounds.y);
            }

            viewRect.anchoredPosition -= offset;
            //print(offset);

            // Scale bounds
            if (viewRect.localScale.y > maxScale || viewRect.localScale.x > maxScale)
            {
                viewRect.localScale = new Vector3(maxScale, maxScale, 1);
                //viewRect.anchoredPosition = Vector2.zero;
            }
            else if (viewRect.localScale.y < minScale || viewRect.localScale.x < minScale)
            { 
                viewRect.localScale = new Vector3(minScale, minScale, 1);
                viewRect.anchoredPosition = Vector2.zero;
            }
        }

        lastMousePos = eventData.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //print($"Pan panel drag start - {viewRect.position}");
    }

    public void OnDrag(PointerEventData eventData)
    {
        viewRect.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //print($"Pan panel drag end - {viewRect.position}");
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
