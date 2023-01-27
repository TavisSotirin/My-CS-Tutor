using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DataStructure : MonoBehaviour
{
    // Options for user input UI creation manager, unique per structure, to some degree
    public abstract DSCreationManager.CreationOption[] getCreationOptions();
    
    // ADD: Structure 'Active options'

    // Valid options this structure can accomodate
    public DSLIB.Structures structureType { get; private set; }
    public DSLIB.DataTypes[] validType { get; private set; }
    public DSLIB.SortAlgorithms[] validSort { get; private set; }
    public DSLIB.SearchAlgorithms[] validSearch { get; private set; }
}

public class DSArray : DataStructure
{
    new DSLIB.Structures structureType = DSLIB.Structures.ARRAY;

    public override DSCreationManager.CreationOption[] getCreationOptions()
    {
        DSCreationManager.CreationOption[] cOptions = 
            { 
            new DSCreationManager.CreationOption("Size",DSCreationManager.OptionType.USER_INPUT_INT, cSize), 
            new DSCreationManager.CreationOption("Dynamic?",DSCreationManager.OptionType.BOOL, cDynamic),
            new DSCreationManager.CreationOption("Clear",DSCreationManager.OptionType.BUTTON, cClear)
            };

        return cOptions;
    }

    private void cSize()
    {
        print("New array size");
    }

    private void cDynamic()
    {
        print("New array location");
    }

    private void cClear()
    {
        print("Clear creation array");
    }
}

public static class DSLIB
{
    public enum Structures
    {
        ARRAY,
        LINKED_LIST,
        TREE,
        GRAPH
    }

    public enum DataTypes
    {
        BOOL,
        CHAR,
        INT,
        FLOAT,
        STRING
    }
    
    public enum SortAlgorithms
    {
        QUICK,
        HEAP,
        BUBBLE,
        MERGE,
        SELECTION,
        INSERTION
    }

    public enum SearchAlgorithms
    {
        ITERATIVE,
        BINARY,
        DEPTH_FIRST,
        BREADTH_FIRST
    }
}