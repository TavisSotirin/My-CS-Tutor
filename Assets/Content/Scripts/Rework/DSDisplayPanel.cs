using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

[RequireComponent(typeof(RectTransform))]
//[RequireComponent(typeof(IPanel))]
[RequireComponent(typeof(RectMask2D))]
public class DSDisplayPanel : MonoBehaviour
{
    protected class DSDisplayTab
    {
        protected string name;

        public DSDisplayTab(string name)
        {
            this.name = name;
        }
    }
    // tabs for display panel to show off either multi structures, or all vars in a function?
    private LinkedList<DSDisplayTab> tabs;

    public DSDisplayPanel()
    {
        tabs = new LinkedList<DSDisplayTab>();
        tabs.AddFirst(new DSDisplayTab(""));
    }
}