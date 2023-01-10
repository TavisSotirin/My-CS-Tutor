using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

// EXAMPLES OF VARIOUS INSPECTOR EDIT OPTIONS WITHIN UNITY

[RequireComponent(typeof(RectTransform))]
public class UnityInspectorExamples : MonoBehaviour
{
    // Serialized, private struct can be edited in the inspector but not by other classes, when the member variable is created with [SerializeField]
    // Access modifiers within structs work as expected within inspector
    // [System.Serializable] is needed above struct/class declaration to allow edits of actual struct variables in inspector as well
    [System.Serializable]
    private struct TStruct1
    {
        public int x;
        public int y;
        public int z;
        public string name;
    }

    [System.Serializable]
    public struct TStruct2
    {
        public int[] ints;
        private bool check;
    }

    // Bold header
    [Header("Panel Views")]
    // Allows private fields to be set in inspector
    [SerializeField]
    private RectTransform MemoryPanel;
    [SerializeField]
    private RectTransform ToolPanel;
    [SerializeField]
    private RectTransform WorkPanel;
    [SerializeField]
    private RectTransform MenuPanel;
    [SerializeField]
    private RectTransform InfoPanel;
    [SerializeField]
    private RectTransform CodePanel;

    // Gap
    [Space(10)]
    [Header("New header")]
    public int x = 10;
    public bool check;
    // Prevents values lower than this in inspector
    [MinAttribute(-10)]
    public float y = 0;
    // Create inspector range slider for this var
    [RangeAttribute(-10, 10)]
    public int l = 0;

    // Create large, newline-able text box in inspector for this var
    [UnityEngine.TextAreaAttribute]
    public string text;

    // Inspector tooltip when hovering over var
    [TooltipAttribute("This is the tool top dog")]
    public int tooltipint = 10;

    public TestClass testClass;
    [SerializeField]
    private TStruct1 s1;
    public TStruct2 s2;
}

[System.Serializable]
public class TestClass
{
    public int tClass_int = 11;
}