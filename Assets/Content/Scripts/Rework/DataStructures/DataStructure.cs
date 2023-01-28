using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class DataStructure
{
    // Options for user input UI creation manager, unique per structure, to some degree
    public DSCreationManager.CreationOption[] creationOptions { get; }

    // ADD: Structure 'Active options'

    // Valid options this structure can accomodate
    public virtual DSLIB.Structures structureType { get; }
    public static DSLIB.DataTypes[] dataTypes { get; }
    public virtual DSLIB.SortAlgorithms[] validSorts { get; }
    public virtual DSLIB.SearchAlgorithms[] validSearches { get; }


    public virtual DSCreationManager.CreationOption[] tryCreate(ref DataStructure target)
    {
        throw new AmbiguousImplementationException("Cannot call creation options from base DataStructure class");
    }
}

public class DSArray : DataStructure
{
    //public override DSLIB.Structures structureType { get { return DSLIB.Structures.ARRAY; } }
    override public DSLIB.Structures structureType { get { return DSLIB.Structures.ARRAY; } }
    new public static DSLIB.DataTypes[] dataTypes { get { return DSLIB.typeArray; } }

    private DSArray() {  }

    new public static DSCreationManager.CreationOption[] tryCreate(ref DataStructure target)
    {
        target = new DSArray();
        DSCreationManager.CreationOption[] cOptions = 
            { 
            new DSCreationManager.CreationOption("Size",DSCreationManager.OptionType.USER_INPUT_INT, ((DSArray)target).cSize),
            new DSCreationManager.CreationOption("Dynamic?",DSCreationManager.OptionType.BOOL, ((DSArray)target).cDynamic),
            new DSCreationManager.CreationOption("Clear",DSCreationManager.OptionType.BUTTON, ((DSArray)target).cClear)
            };

        return cOptions;
    }

    private void cSize()
    {
        //print("New array size");
    }

    private void cDynamic()
    {
        //print("New array location");
    }

    private void cClear()
    {
        //print("Clear creation array");
        throw new Exception("IT WORKED BUTTON ERROR ON CLEAR FROM OPTIONS!");
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
    public static DataTypes[] GetValidDataTypes(string structureStr)
    {
        return GetValidDataTypes(GetStructureEnum(structureStr));
    }

    public static DSCreationManager.CreationOption[] tryCreate(Structures structure, ref DataStructure target)
    {
        switch (structure)
        {
            case Structures.ARRAY:
                return DSArray.tryCreate(ref target);
            default:
                return null;
        }
    }
    public static DSCreationManager.CreationOption[] tryCreate(string structureStr, ref DataStructure target)
    {
        return tryCreate(GetStructureEnum(structureStr), ref target);
    }
}