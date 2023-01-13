using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;



public abstract class DS2 : MonoBehaviour
{
    public enum DSStructureType
    {
        ARRAY,
        LINKED_LIST
    }

    public enum DSDataType
    {
        CHAR,
        INT
    }

    protected DSStructureType structureType;
    protected DSDataType dataType;
    protected DSDisplayPanel_old displayPanel;

    protected static int globalCount = 0;

    protected string varName;
    protected IDSNode2 head;


    public DS2 Initialize(DSStructureType _sType, DSDataType _dType, string _name = "")
    {
        structureType = _sType;
        dataType = _dType;

        InitializeInternal(_name);

        return this;
    }

    protected abstract void InitializeInternal(string _name);

    protected abstract void Destroy();
}

public interface IDSNode2
{
    
}

public class DSLinkedList2 : DS2
{
    protected class Node<T> : IDSNode2
    {
        Node<T> next;
        Node<T> prev;
        T value;
    }

    

    public IDSNode2 NewElement()
    {
        switch (dataType)
        {
            case DSDataType.CHAR:
                return new Node<char>();
            case DSDataType.INT:
                return new Node<int>();
            default:
                return null;
        }
    }

    override protected void InitializeInternal(string _name)
    {
        varName = (_name.Equals("") ? "Linked_List_Var_" + globalCount++ : _name);

        head = NewElement();





        print("Init running for linked list object - " + varName);
    }

    // Should remove game object, UI/visual elements, and memory data
    protected override void Destroy()
    {
        throw new NotImplementedException();
    }
}

// TODO: Mesh.CombineMeshes
// Once drawn, combine meshes and remove excess child objects since certain structures won't be altered once drawn
// Change this to be a static class that draws UI elements and returns them
public class DrawPanel : MonoBehaviour
{
    public enum UIShapeENUM
    {
        CIRCLE,
        RECT
    }

    // Set bulk vars to avoid needing to pass them for every call to draw functions - mostly for function call convenience
    //public static void SetWorkingData() { }

    public UIShapeENUM shape;

    private static int nodeCount = 0;

    [Min(0.1f)]
    public float radius = 1f;
    public (float, float) center = (0, 0);
    [Min(0.1f)]
    public float width = 0.15f;
    [Min(3)]
    public int vertCount = 10;
    public bool pauseRedraw = true;
    public bool doubleScale = false;
    public bool filled = false;

    public GameObject drawCircle(float radius, (float,float)? center = null, float lineWidth = .15f, int vertCount = 10, bool filled = false)
    {
        var child = new GameObject("UIShape_Circ_" + DrawPanel.nodeCount++);
        var rect = child.AddComponent<RectTransform>();
        var lr = child.AddComponent<LineRenderer>();

        // Mesh display components
        var mf = child.AddComponent<MeshFilter>();
        var mr = child.AddComponent<MeshRenderer>();
        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        mr.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        mr.allowOcclusionWhenDynamic = false;

        rect.SetParent(this.transform);
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;

        center = center != null ? center : (0f, 0f);

        lr.positionCount = vertCount;
        lr.loop = true;
        lr.widthMultiplier = lineWidth;

        float angleRads = 2 * Mathf.PI / vertCount;

        for (int i = 0; i < vertCount; i++)
        {
            float t = (angleRads * i);
            
            float x = radius * Mathf.Cos(t) + (float)center?.Item1;
            float y = radius * Mathf.Sin(t) + (float)center?.Item2;
                
            lr.SetPosition(i, new Vector3(x,y,0));
        }

        // Bake line mesh and remove line renderer
        // UPDATE: Store this mesh - no need to recaculate/redraw multiple since all nodes for a given panel will be the same
        Mesh mesh = new Mesh();
        lr.BakeMesh(mesh);
        mf.sharedMesh = mesh;
        GameObject.Destroy(lr);

        rect.sizeDelta = mesh.bounds.extents * 2;

        return child;
    }

    private GameObject drawRect(float width, float height, (float, float)? center = null, float lineWidth = .15f, bool filled = false)
    {
        var child = new GameObject("UIShape_Rect_" + DrawPanel.nodeCount++);
        var rect = child.AddComponent<RectTransform>();
        var lr = child.AddComponent<LineRenderer>();

        // Mesh display components
        var mf = child.AddComponent<MeshFilter>();
        var mr = child.AddComponent<MeshRenderer>();
        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        mr.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        mr.allowOcclusionWhenDynamic = false;

        rect.SetParent(this.transform);
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;

        center = center != null ? center : (0f, 0f);

        float halfHeight = 0.5f * height;
        float halfWidth = 0.5f * width;

        if (!filled)
        {
            lr.widthMultiplier = lineWidth;
            lr.positionCount = 4;
            lr.loop = true;

            // Set 4 corners
            lr.SetPosition(0, new Vector3(halfWidth , halfHeight, 0));
            lr.SetPosition(1, new Vector3(-halfWidth, halfHeight, 0));
            lr.SetPosition(2, new Vector3(-halfWidth, -halfHeight, 0));
            lr.SetPosition(3, new Vector3(halfWidth, -halfHeight, 0));
        }
        else
        {
            lr.useWorldSpace = true;
            lr.widthMultiplier = width;
            lr.positionCount = 2;
            lr.loop = false;

            // set 2 corners
            lr.SetPosition(0, new Vector3(0, -halfHeight, 0));
            lr.SetPosition(1, new Vector3(0, halfHeight, 0));
        }

        // Bake line mesh and remove line renderer
        // UPDATE: Store this mesh - no need to recaculate/redraw multiple since all nodes for a given panel will be the same
        Mesh mesh = new Mesh();
        lr.BakeMesh(mesh);
        mf.sharedMesh = mesh;
        GameObject.Destroy(lr);

        rect.sizeDelta = mesh.bounds.extents * 2;
        rect.anchoredPosition = new Vector2((float)center?.Item1, (float)center?.Item2);

        return child;
    }

    private void redraw()
    {
        if (pauseRedraw) return;

        switch(shape)
        {
            case UIShapeENUM.CIRCLE:
                drawCircle(this.radius, this.center, this.width, this.vertCount, this.filled);
                break;
            case UIShapeENUM.RECT:
                drawRect(this.radius, this.radius, this.center, this.width, this.filled);
                break;
            default:
                return;
        }
        
    }

    void OnValidate()
    {
        redraw();

        if (doubleScale)
        {
            doubleScale = false;
            
        }    
    }

    private void Start()
    {
        //var obj1 = drawRect(3, 2, (0, 0));
        //var obj2 = drawRect(3, 2, (4.5f, 0));

        //drawConnectingLine(obj1, obj2);
        var box1 = UIShape.DrawRect(30, 20, this.transform, 2f, false);
        //var box2 = GameObject.Instantiate(box1, this.transform);

        box1.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        //box2.GetComponent<RectTransform>().anchoredPosition = new Vector2(4.5f, 0);

        //var line = UIShape.DrawStraightLine(1.5f,.1f,this.transform, true, true);
        //line.GetComponent<RectTransform>().anchoredPosition = new Vector2(4.5f / 2f, 0);
    }

    private void drawConnectingLine(GameObject obj1, GameObject obj2, Transform parent = null, float lineWidth = 0.05f)
    {
        var child = new GameObject(obj1.name + "----" + obj2.name);
        var rect = child.AddComponent<RectTransform>();
        var lr = child.AddComponent<LineRenderer>();

        if (parent == null)
            parent = this.transform;
        rect.SetParent(parent);
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;

        lr.loop = false;

        Vector2 leftPoint;
        Vector2 rightPoint;
        float arrowScale = 4f;

        // Find right side and left side points of leftmost and rightmost object, respectively
        {
            var rect1 = obj1.gameObject.GetComponent<RectTransform>();
            var rect2 = obj2.gameObject.GetComponent<RectTransform>();

            if (rect1.anchoredPosition.x > rect2.anchoredPosition.x)
            {
                var tmp = rect1;
                rect1 = rect2;
                rect2 = tmp;
            }

            leftPoint = new Vector2(rect1.anchoredPosition.x + rect1.sizeDelta.x * 0.5f, rect1.anchoredPosition.y);
            rightPoint = new Vector2(rect2.anchoredPosition.x - rect2.sizeDelta.x * 0.5f, rect2.anchoredPosition.y);
        }

        // Draw line
        lr.positionCount = 2;
        lr.widthMultiplier = lineWidth;

        var lineLength = rightPoint.x - leftPoint.x - lineWidth * arrowScale;

        lr.SetPosition(0, Vector2.zero);
        lr.SetPosition(1, new Vector2(lineLength, 0));
        //lr.SetPosition(0, leftPoint + new Vector2(lineWidth, 0));
        //lr.SetPosition(1, rightPoint - new Vector2(lineWidth, 0));

        // Mesh display components
        var mf = child.AddComponent<MeshFilter>();
        var mr = child.AddComponent<MeshRenderer>();
        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        mr.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        mr.allowOcclusionWhenDynamic = false;

        // Bake line mesh and remove line renderer
        // UPDATE: Store this mesh - no need to recaculate/redraw multiple since all nodes for a given panel will be the same
        Mesh mesh = new Mesh();
        lr.BakeMesh(mesh);
        mf.sharedMesh = mesh;
        GameObject.Destroy(lr);

        rect.sizeDelta = mesh.bounds.extents * 2;
        rect.pivot = new Vector2(0,0.5f);
        //rect.anchoredPosition = new Vector2(leftPoint.x + lineWidth, leftPoint.y);
        rect.anchoredPosition = leftPoint + new Vector2(lineWidth * arrowScale * 0.5f, 0);

        var arrow = drawArrow();
        arrow.transform.SetParent(parent);
        //var arrow2 = GameObject.Instantiate(arrow);
        //arrow2.transform.SetParent(parent);

        arrow.transform.localScale = Vector3.one * lineWidth * arrowScale;
        //arrow2.transform.localScale = Vector3.one * lineWidth;

        var arrow2 = GameObject.Instantiate(arrow, parent);
        arrow.transform.Rotate(new Vector3(0, 0, 90));
        arrow2.transform.Rotate(new Vector3(0, 0, -90));

        arrow.GetComponent<RectTransform>().anchoredPosition = leftPoint;
        arrow2.GetComponent<RectTransform>().anchoredPosition = rightPoint;
    }

    private GameObject drawArrow()
    {
        var child = new GameObject("Arrow" + DrawPanel.nodeCount);
        var rect = child.AddComponent<RectTransform>();
        var mf = child.AddComponent<MeshFilter>();
        var mr = child.AddComponent<MeshRenderer>();

        mr.sharedMaterial = new Material(Shader.Find("Standard"));

        // Create mesh components for object
        Mesh mesh = new Mesh();

        Vector3[] verts = new Vector3[4];
        verts[0] = Vector3.zero;
        verts[1] = new Vector3(0.5f, -1f, 0);
        verts[2] = new Vector3(0, -0.75f, 0);
        verts[3] = new Vector3(-0.5f, -1f, 0);
        mesh.vertices = verts;

        // Calculate TRI positions (Clockwise in vertex order from above)
        int[] tris = { 0,1,2,0,2,3};
        mesh.triangles = tris;

        // Normal per vertex (auto normals can be done as well using mesh.RecalculateNormals();
        Vector3[] normals = { new Vector3(0, 0, 1), new Vector3(0, 0, 1), new Vector3(0, 0, 1), new Vector3(0, 0, 1) };
        mesh.normals = normals;

        // TODO?: UV layout

        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        mr.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        mr.allowOcclusionWhenDynamic = false;
        mf.mesh = mesh;
        rect.sizeDelta = mesh.bounds.extents * 2;
        rect.pivot = new Vector2(.5f, 1);

        return child;
    }
}

public static class UIShape
{
    public enum Shapes
    {
        RECT,
        CIRCLE,
        LINE,
        ARROW
    }

    private static int nodeCount = 0;

    private static GameObject lastRect = null;
    private static GameObject lastCircle = null;
    private static GameObject lastArrow = null;
    private static GameObject lastLine = null;

    // NOTE: Not thread safe!
    public static GameObject GetLastShape(Shapes shape)
    {
        switch (shape)
        {
            case Shapes.RECT:
                return lastRect;
            case Shapes.CIRCLE:
                return lastCircle;
            case Shapes.ARROW:
                return lastArrow;
            case Shapes.LINE:
                return lastLine;
            default:
                return null;
        }
    }

    public static void ClearStoredData()
    {
        lastRect = null;
        lastCircle = null;
        lastArrow = null;
        lastLine = null;
    }

    private static GameObject initLineMesh(Transform parent, ref RectTransform rect, ref LineRenderer lr, ref MeshFilter mf, ref MeshRenderer mr)
    {
        var child = new GameObject("UIShape_Comp_" + UIShape.nodeCount++);

        rect = child.AddComponent<RectTransform>();
        lr = child.AddComponent<LineRenderer>();
        mf = child.AddComponent<MeshFilter>();
        mr = child.AddComponent<MeshRenderer>();

        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        mr.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        mr.allowOcclusionWhenDynamic = false;

        rect.SetParent(parent);
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;

        return child;
    }

    public static GameObject DrawRect(float width, float height, Transform parent, float lineWidth, bool filled)
    {
        RectTransform rect = null;
        LineRenderer lr = null;
        MeshFilter mf = null;
        MeshRenderer mr = null;
        var child = initLineMesh(parent, ref rect, ref lr, ref mf, ref mr);

        float halfHeight = 0.5f * height;
        float halfWidth = 0.5f * width;

        if (!filled)
        {
            lr.widthMultiplier = lineWidth;
            lr.positionCount = 4;
            lr.loop = true;

            // Set 4 corners
            lr.SetPosition(0, new Vector3(halfWidth, halfHeight, 0));
            lr.SetPosition(1, new Vector3(-halfWidth, halfHeight, 0));
            lr.SetPosition(2, new Vector3(-halfWidth, -halfHeight, 0));
            lr.SetPosition(3, new Vector3(halfWidth, -halfHeight, 0));
        }
        else
        {
            lr.useWorldSpace = true;
            lr.widthMultiplier = width;
            lr.positionCount = 2;
            lr.loop = false;

            // set 2 corners
            lr.SetPosition(0, new Vector3(0, -halfHeight, 0));
            lr.SetPosition(1, new Vector3(0, halfHeight, 0));
        }

        // Bake line mesh and remove line renderer
        Mesh mesh = new Mesh();
        lr.BakeMesh(mesh);
        mf.sharedMesh = mesh;
        mf.sharedMesh.name = "SimpleRect";
        GameObject.Destroy(lr);

        rect.sizeDelta = mesh.bounds.extents * 2;

        lastRect = child;
        AddTextComp(ref child, new Rect(new Vector2(0,0), mesh.bounds.size));
        return child;
    }

    public static GameObject DrawCircle(float radius, Transform parent, float lineWidth, int vertCount)
    {
        RectTransform rect = null;
        LineRenderer lr = null;
        MeshFilter mf = null;
        MeshRenderer mr = null;
        var child = initLineMesh(parent, ref rect, ref lr, ref mf, ref mr);

        lr.positionCount = vertCount;
        lr.loop = true;
        lr.widthMultiplier = lineWidth;

        float angleRads = 2 * Mathf.PI / vertCount;

        for (int i = 0; i < vertCount; i++)
        {
            float t = (angleRads * i);

            float x = radius * Mathf.Cos(t);
            float y = radius * Mathf.Sin(t);

            lr.SetPosition(i, new Vector3(x, y, 0));
        }

        // Bake line mesh and remove line renderer
        Mesh mesh = new Mesh();
        lr.BakeMesh(mesh);
        mf.sharedMesh = mesh;
        GameObject.Destroy(lr);

        rect.sizeDelta = mesh.bounds.extents * 2;

        lastCircle = child;
        return child;
    }

    public static GameObject DrawArrow()
    {
        if (lastArrow != null) return lastArrow;

        var child = new GameObject("Arrow_" + UIShape.nodeCount++);
        var rect = child.AddComponent<RectTransform>();
        var mf = child.AddComponent<MeshFilter>();
        var mr = child.AddComponent<MeshRenderer>();

        mr.sharedMaterial = new Material(Shader.Find("Standard"));

        // Create mesh components for object
        Mesh mesh = new Mesh();

        Vector3[] verts = new Vector3[4];
        verts[0] = Vector3.zero;
        verts[1] = new Vector3(0.5f, -1f, 0);
        verts[2] = new Vector3(0, -0.75f, 0);
        verts[3] = new Vector3(-0.5f, -1f, 0);
        mesh.vertices = verts;

        // Calculate TRI positions (Clockwise in vertex order from above)
        int[] tris = { 0, 1, 2, 0, 2, 3 };
        mesh.triangles = tris;

        // Normal per vertex (auto normals can be done as well using mesh.RecalculateNormals();
        Vector3[] normals = { new Vector3(0, 0, 1), new Vector3(0, 0, 1), new Vector3(0, 0, 1), new Vector3(0, 0, 1) };
        mesh.normals = normals;

        // TODO?: UV layout

        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        mr.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        mr.allowOcclusionWhenDynamic = false;
        mf.mesh = mesh;
        rect.sizeDelta = mesh.bounds.extents * 2;
        rect.pivot = new Vector2(.5f, 1);

        lastArrow = child;
        return child;
    }

    public static GameObject DrawStraightLine(float length, float lineWidth, Transform parent, bool leftArrow = false, bool rightArrow = false)
    {
        RectTransform rect = null;
        LineRenderer lr = null;
        MeshFilter mf = null;
        MeshRenderer mr = null;
        var child = initLineMesh(parent, ref rect, ref lr, ref mf, ref mr);

        float arrowScale = 4;
        float lengthOffset = 0;

        if ((leftArrow || rightArrow) && lastArrow == null)
        {
            DrawArrow();

            if (leftArrow && rightArrow)
                lengthOffset = lineWidth * arrowScale * 0.5f;
            else
                lengthOffset = lineWidth * arrowScale * 0.25f;
        }

        lr.positionCount = 2;
        lr.loop = false;
        lr.widthMultiplier = lineWidth;

        var halfLength = length * 0.5f;
        lr.SetPosition(0, new Vector2(-halfLength + lengthOffset, 0));
        lr.SetPosition(1, new Vector2(halfLength - lengthOffset, 0));

        Mesh mesh = new Mesh();
        lr.BakeMesh(mesh);
        mf.sharedMesh = mesh;
        GameObject.Destroy(lr);

        if (leftArrow)
        {
            var lArrow = GameObject.Instantiate(lastArrow, child.transform);
            lArrow.transform.Rotate(new Vector3(0,0,90));
            lArrow.GetComponent<RectTransform>().anchoredPosition = new Vector2(-halfLength, 0);
            lArrow.transform.localScale = Vector2.one * (lineWidth * arrowScale);
        }
        if (rightArrow)
        {
            var rArrow = GameObject.Instantiate(lastArrow, child.transform);
            rArrow.transform.Rotate(new Vector3(0, 0, -90));
            rArrow.GetComponent<RectTransform>().anchoredPosition = new Vector2(halfLength, 0);
            rArrow.transform.localScale = Vector2.one * (lineWidth * arrowScale);
        }

        rect.sizeDelta = mesh.bounds.extents * 2;

        lastLine = child;
        return child;
    }

    private static void AddTextComp(ref GameObject shape, Rect posRect)
    {
        //var mf = shape.GetComponent<MeshFilter>();
        //if (mf == null) return;

        var txt = new GameObject(shape.name + "_inText").AddComponent<TextMeshProUGUI>();
        var tRect = txt.GetComponent<RectTransform>();
        tRect.SetParent(shape.transform);

        // TEMP 
        txt.text = UIShape.nodeCount.ToString();
        txt.fontSize = 12;
        txt.alignment = TextAlignmentOptions.Center;

        tRect.anchorMin = Vector2.zero;
        tRect.anchorMax = Vector2.one;
        tRect.anchoredPosition = Vector2.zero;
        tRect.sizeDelta = new Vector2(posRect.size.x, posRect.size.y);

        txt.enableAutoSizing = true;
        txt.fontSizeMin = 6;
    }
}