using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class DataStructure : MonoBehaviour
{
    // Options for user input UI creation manager, unique per structure, to some degree
    public abstract DSCreationManager.CreationOption[] getCreationOptions();
    
    // ADD: Structure 'Active options'

    // Valid options this structure can accomodate
    public static DSLIB.Structures structureType { get; }
    public static DSLIB.DataTypes[] dataTypes { get; }
    public virtual DSLIB.SortAlgorithms[] validSorts { get; }
    public virtual DSLIB.SearchAlgorithms[] validSearches { get; }
}

public class DSArray : DataStructure
{
    //public override DSLIB.Structures structureType { get { return DSLIB.Structures.ARRAY; } }
    new public static DSLIB.Structures structureType { get { return DSLIB.Structures.ARRAY; } }
    //new public static DSLIB.DataTypes[] dataTypes { get { return DSLIB.typeArray; } }
    new public static DSLIB.DataTypes[] dataTypes { get { return new DSLIB.DataTypes[] { DSLIB.DataTypes.FLOAT}; } }

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
        BOOL = 0,
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

    public static DataTypes[] typeArray { get { return (DSLIB.DataTypes[])Enum.GetValues(typeof(DSLIB.DataTypes)); } }

    public static Structures GetStructureEnum(string structureString)
    {
        Structures outStruct;
        Enum.TryParse<Structures>(structureString, out outStruct);
        return outStruct;
    }

    // Update switch cases when restricted types are needed
    public static DataTypes[] GetValidDataTypes(Structures structure)
    {
        switch (structure)
        {
            default:
                return typeArray;
        }
    }

    public static DataTypes[] GetValidDataTypes(string structureString)
    {
        return GetValidDataTypes(GetStructureEnum(structureString));
    }
}