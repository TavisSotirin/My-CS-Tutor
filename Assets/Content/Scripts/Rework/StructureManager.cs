using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Holds only references to created structure objects and acts as intermediary for all structure commands
public class StructureManager
{
    // Singleton class with static operations
    private static StructureManager _instance;
    public static StructureManager Instance { get { if (_instance == null) _instance = new StructureManager(); return _instance; } }
    private StructureManager() { }

    // All non-destroyed structures (dead and alive) - effectively a memory list
    private LinkedList<DS2> _dataStructures;

    /// Structure commands
    // create new structure
    // delete structure
    // add element
    // remove element
    // modify element
    // modify structure

    // Build new DS using constructors and user provided data
    // Mark dirty and redraw UI
    public static void NewStructure()
    {
        //Instance._dataStructures.addLast();
    }
}

// Responsible for all memory operations
// all operations should either take in relevant structure or get data directly from structure manager
public static class MemoryManager
{
    // defrag
    // free
    // malloc
    // mark dead
}

public class UIManager
{
    // Singleton class with static operations
    private static UIManager _instance;
    public static UIManager Instance { get { if (_instance == null) _instance = new UIManager(); return _instance; } }
    private UIManager() { }

    /// UI commands
    // full redraw
    // partial redraw (redraw dirty)
    // animation
    // rebuild layout (screen size change)
    // user UI edits
    // select object (structure, element, code line, mem button, etc) - should have var for currently selected

    public static void draw(bool partial = true)
    {
        if (partial)
        {
            // ask structure manager for list of stored stuctures
            // redraw all displayable objects (or just ones marked dirty if partial)
            //StructureManager.Instance
        }
    }
}

// Displayable object. Contains drawing functionallity. Structure should provide drawing instructions, but UIManager should do actual drawing
// Create prefab panel for types of disp objects (linear structure, tree structure, graph structure, etc)
public interface IDisp2
{
    public bool isVisible();
    public void setVisible(bool _visible);
}

public interface IMem2
{
    public int memSize();
    public int memAddr();
    // Is memory in use or dead and operating as garbage in mem display 
    public bool memIsDead();
    public void memMarkDead();
}