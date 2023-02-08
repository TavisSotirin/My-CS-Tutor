using System;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Events;

// ?UPDATE?: Make interface instead of abstract class
public abstract class ADataStructure
{
    // Valid options this structure can accomodate
    // ?UPDATE?: Currently stored on individual structures, but some may be redundant. Consider consolidated static storage in DSLIB
    public virtual DSLIB.Structures structureType { get; set; }
    public virtual DSLIB.DataTypes dataType { get; set; }
    public virtual DSLIB.DataTypes[] dataTypes { get; }
    public virtual DSLIB.SortAlgorithms[] sortAlgorithms { get; }
    public virtual DSLIB.SearchAlgorithms[] searchAlgorithms { get; }

    // Instantiate null ref, Initialize instantiated obj
    public static DSCreationManager.CreationOption[] Instantiate(ref ADataStructure target) { return null; }
    public abstract void Initialize();
    public abstract void FinalizeCreation();

    //public abstract //DrawInstructions draw(ref ADataStructure target);
    protected ISupportStructure supportStructure;
}

public interface ISupportStructure
{

}

public interface IDisplayableStructure
{
    public GameObject getDisplayNodePrefab();
}

public abstract class ADSLinear : ADataStructure, IDisplayableStructure
{
    public int length { get; protected set; }

    public GameObject getDisplayNodePrefab()
    {
        return DSPrefabs.GetPrefab(DSPrefabs.PrefabEnums.VIEWNODE_LINEAR);
    }
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

    new public static DSCreationManager.CreationOptionSet Instantiate(ref ADataStructure target)
    {
        target = new DSArray();

        DSCreationManager.CreationOptionSet set = new();

        set.options = new DSCreationManager.CreationOption[]
            {
            DSCreationManager.NewOption("Size",DSCreationManager.OptionType.USER_INPUT_INT, new UnityAction<int>(((DSArray)target).cSize)),
            DSCreationManager.NewOption("Is Dynamic?",DSCreationManager.OptionType.BOOL, new UnityAction<bool>(((DSArray)target).cDynamic)),
            DSCreationManager.NewOption("Clear",DSCreationManager.OptionType.BUTTON, ((DSArray)target).cClear)
            };

        set.finalize = DSCreationManager.NewOption("Finalize Array", DSCreationManager.OptionType.BUTTON, ((DSArray)target).FinalizeCreation);

        return set;
    }

    // TODO: Need to pre-set type and length data from user to correctly create base
    override public void Initialize()
    {
        switch (dataType)
        {
            case DSLIB.DataTypes.BOOL:
                base.Initialize<bool>();
                break;
            case DSLIB.DataTypes.STRING:
                base.Initialize<string>();
                break;
            case DSLIB.DataTypes.CHAR:
                base.Initialize<char>();
                break;
            case DSLIB.DataTypes.FLOAT:
                base.Initialize<float>();
                break;
            case DSLIB.DataTypes.INT:
                base.Initialize<int>();
                break;
            default:
                throw new Exception("Attempted to initialize DSArray with invalid DataType");
        }
    }

    override public void FinalizeCreation()
    {
        Debug.Log("Finalize array called");
    }

    public static void ForceInit(ref DSArray target, int _length, DSLIB.DataTypes _type)
    {
        target = new DSArray();
        target.length = _length;
        target.dataType = _type;
        target.Initialize();
    }

    private void cSize(int _size)
    {
        length = _size;

        // Run update code for size change in display
    }

    private void cDynamic(bool isDynamic)
    {
        // TODO: set mem location
        Debug.Log($"isDynamic in array creation: {isDynamic}");
    }

    private void cClear()
    {
        // needed?
        Debug.Log("Clear in array creation");
    }
}

/*
public class DSLinkedList : ADSDynamic
{

}

public class DSBinaryTree : ADSTree
{

}

public class DSGraph : ADSNonlinear
{

}
*/

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

    public static DataTypes GetDataTypeEnum(string dataTypeString)
    {
        DataTypes outStruct;
        Enum.TryParse<DataTypes>(dataTypeString, out outStruct);
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

    public static DSCreationManager.CreationOptionSet? Instantiate(Structures _structureType, DataTypes _dataType, ref ADataStructure target)
    {
        switch (_structureType)
        {
            case Structures.ARRAY:
                var set = DSArray.Instantiate(ref target);
                target.dataType = _dataType;
                target.structureType = _structureType;
                return set;
            default:
                return null;
        }
    }
    public static DSCreationManager.CreationOptionSet? Instantiate(string _structureType, string _dataTypeStr, ref ADataStructure target)
    {
        return Instantiate(GetStructureEnum(_structureType), GetDataTypeEnum(_dataTypeStr), ref target);
    }
    public static DSCreationManager.CreationOptionSet? Instantiate(string _structureTypeStr, DataTypes _dataType, ref ADataStructure target)
    {
        return Instantiate(GetStructureEnum(_structureTypeStr), _dataType, ref target);
    }
    public static DSCreationManager.CreationOptionSet? Instantiate(Structures _structureType, string _dataTypeStr, ref ADataStructure target)
    {
        return Instantiate(_structureType, GetDataTypeEnum(_dataTypeStr), ref target);
    }
}

[System.Serializable]
public class UnityEventParam<T> : UnityEvent
{
    public T paramValue;
}