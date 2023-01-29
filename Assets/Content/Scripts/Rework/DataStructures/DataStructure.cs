using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static ADSStatic;

// ?UPDATE?: Make interface insteaad of abstract class
public abstract class ADataStructure
{
    // Valid options this structure can accomodate
    // ?UPDATE?: Currently stored on individual structures, but some may be redundant. Consider consolidated static storage in DSLIB
    public virtual DSLIB.Structures structureType { get; }
    public virtual DSLIB.DataTypes[] dataTypes { get; }
    public virtual DSLIB.SortAlgorithms[] sortAlgorithms { get; }
    public virtual DSLIB.SearchAlgorithms[] searchAlgorithms { get; }

    // Instantiate null ref, Initialize instantiated ref
    public static DSCreationManager.CreationOption[] Instantiate(ref ADataStructure target) { return null; }
    public abstract void Initialize(ref ADataStructure target);

    //public abstract //DrawInstructions draw(ref ADataStructure target);
    protected ISupportStructure supportStructure;
}

public interface ISupportStructure
{

}

public abstract class ADSLinear : ADataStructure
{
    public int length { get; protected set; }
}

public abstract class ADSStatic : ADSLinear
{
    protected class ADSSupportStructure<T> : ISupportStructure where T : IComparable<T>
    {
        public T[] array;
    }

    protected virtual void Initialize<T>() where T : IComparable<T>
    {
        supportStructure = new ADSSupportStructure<T>();
        ((ADSSupportStructure<T>)supportStructure).array = new T[length];
    }
}

public abstract class ADSDynamic : ADSLinear
{
    
}

public abstract class ADSNonlinear : ADataStructure
{

}

public abstract class ADSTree : ADSNonlinear
{
    public int height { get; protected set; }
}

public class DSArray : ADSStatic
{
    //public override DSLIB.Structures structureType { get { return DSLIB.Structures.ARRAY; } }
    override public DSLIB.Structures structureType { get { return DSLIB.Structures.ARRAY; } }
    override public DSLIB.DataTypes[] dataTypes { get { return DSLIB.typeArray; } }

    private DSArray() {  }

    new public static DSCreationManager.CreationOption[] Instantiate(ref ADataStructure target)
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

    // TODO: Need to get type and length data from user to correctly create base
    override public void Initialize(ref ADataStructure target)
    {
        if (target is DSArray)
        {
            base.Initialize<int>();
        }
        else
            throw new Exception("Invalid structure passed to DSArray Init");
    }

    public static void ForceInit(ref DSArray target, int _length)
    {
        
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

public class DSLinkedList : ADSDynamic
{

}

public class DSBinaryTree : ADSTree
{

}

public class DSGraph : ADSNonlinear
{

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

    public static DSCreationManager.CreationOption[] tryCreate(Structures structure, ref ADataStructure target)
    {
        switch (structure)
        {
            case Structures.ARRAY:
                return DSArray.partialInitialize(ref target);
            default:
                return null;
        }
    }
    public static DSCreationManager.CreationOption[] tryCreate(string structureStr, ref ADataStructure target)
    {
        return tryCreate(GetStructureEnum(structureStr), ref target);
    }
}