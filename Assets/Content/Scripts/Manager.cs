using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// TODO?: MOVE ALL MEM CONTENT TO MEM CLASS AND MAKE SINGLETON CLASS LIKE MANAGER
public class Manager
{
    // Singleton class with static operations
    private static Manager _instance;
    public static Manager Instance { get { if (_instance == null) _instance = new Manager(); return _instance; } }
    private Manager() { }
    public static void Initialize()
    {
        Instance.memory.Clear();
    }

    public static void testStaticFunc()
    {
        Manager.Initialize();

        if (_instance != null)
            Debug.Log("Manager starting test function called");
        else
            return;

        newStructure(DSStructType.ARRAY, DSDataType.FLOAT, 5);
        newStructure(DSStructType.ARRAY, DSDataType.INT, 10);
    }

    /////////////////////
    private LinkedList<MemoryBlock> memory = new();
    public DynamicCode dynamicCode { get; private set; } = new();

    public static void newStructure(DSStructType _sType, DSDataType _dType, int count = 0)
    {
        //int newAddress = Instance.memory.Count == 0 ? 0 : Instance.memory.Last.Value.startAddr + Instance.memory.Last.Value.data.getSize();

        DataStructure newStructure;
        switch(_sType)
        {
            case DSStructType.ARRAY:
                newStructure = new DSArray(_dType, count);
                break;
            case DSStructType.LINKED_LIST:
                newStructure = new DSLinkedList();
                break;
            default:
                newStructure = null;
                break;
        }

        // Add structure to memory list - should this be done for individual nodes?
        // malloc written to place chunks of requested memory but should this be done only in manager or called elsewhere as needed? probably as needed, but this means the structures malloc themselves
        Instance.memory.AddLast(new MemoryBlock(newStructure));
    }

    // Request to get added to memory list. Either add to end or in middle of other nodes if there is room. Initially list is empty and acts like a standard stack
    // Very prone to fragmentation but ignoring for the time being for PoC
    // UPDATE: CURRENTLY ADDS ABSTRACT STRUCTURE AND ROOT NODE TO MEM LIST
    // MEM MANAGER SHOULD CREATE LL NODES FOR ANYTHING IN MEMORY
    // STRUCTURE CONTAINERS AND OTHER ABSTRACT CLASSES SHOULD BE MARKED AS ABSTRACT TO IGNORE BUT STILL BE VISIBLE IN MEM LIST

    // TODO: WANT TO RETURN REF TO IN-HEAP MemoryBlock. Currently have issues with being 'local' despite 'new' keyword
    public static MemoryBlock malloc(int _size)
    {
        MemoryBlock blankBlock = new MemoryBlock(0, null);
        //ref MemoryBlock blankBlock = ref blankBlock;
        //var x = ref blankBlock;

        if (Instance.memory.Count == 0)
        {
            Instance.memory.AddFirst(blankBlock);
            return blankBlock;
        }

        var curMemNode = Instance.memory.First;
        int lastEndAddr = curMemNode.Value.endAddr;
        curMemNode = curMemNode.Next;

        while (curMemNode != null)
        {
            // Requested size fits in this space
            if (curMemNode.Value.startAddr - lastEndAddr >= _size)
                break;

            lastEndAddr = curMemNode.Value.endAddr;
            curMemNode = curMemNode.Next;
        }

        blankBlock.startAddr = lastEndAddr;

        if (curMemNode != null)
            Instance.memory.AddBefore(curMemNode, blankBlock);
        else
            Instance.memory.AddLast(blankBlock);

        return blankBlock;
    }

    public static void memset(int _address, ref IMemory _memObject)
    {
        if (getAtAddress(_address) is MemoryBlock memBlock)
        {
            memBlock.data = _memObject;
        }
    }

    public static MemoryBlock? getAtAddress(int _address)
    {
        var cur = Instance.memory.First;

        while (cur != null)
        {
            if (!cur.Value.data.isAbstract())
                if (cur.Value.startAddr == _address)
                    return cur.Value;
            cur = cur.Next;
        }

        return null;
    }

    // Mark memory blocks as no longer in use, but do not delete. Allow memory to be overwritten as user adds/manipulates structures
    public static void free(int _address)
    {
        var cur = Instance.memory.First;

        while (cur != null)
        {
            if (cur.Value.startAddr == _address)
            {
                cur.Value.MarkUnused();
                return;
            }
            cur = cur.Next;
        }
    }

    // Implement later
    public static void defrag()
    {

    }
}

public class Memory // static MemOps?
{
    // Singleton class with static operations - change to static if instance not actually needed
    private static Memory _instance;
    public static Memory Instance { get { if (_instance == null) _instance = new Memory(); return _instance; } }
    private Memory() { }

    public static int sizeOf(DSDataType type)
    {
        int size = 0;
        switch(type)
        {
            case DSDataType.INT:
            case DSDataType.FLOAT:
                size = 4;
                break;
            case DSDataType.BOOL:
            case DSDataType.CHAR:
                size = 1;
                break;
        }

        return size;
    }
}

public abstract class DataStructure :  IDSComponent, IDynamicCode, IMemory // dyn code interface should generate code needed to create associated structure
{
    protected IDSComponent root;
    protected DSInfo info { get; set; }
    protected int memAddr;
    protected DataStructure() { }
    public int getMemAddress() { return memAddr; }
    public DSDataType nodeType { get { return info.dataType; } }
    public int getSize() { return info.memSize; }
    public abstract bool isAbstract();
}

public class DSArray : DataStructure
{
    // TODO: IMPLEMENT OTHER DATA TYPES
    public DSArray(DSDataType _type, int _length)
    {
        if (_length <= 0) throw new System.ArgumentOutOfRangeException(); // REMOVE AND REPLACE WITH USER WARNING WHEN USER CAN CREATE OWN STRUCTURES

        info = new(_length, _length * Memory.sizeOf(_type), _type, DSStructType.ARRAY);
        int memInc = Memory.sizeOf(_type);

        // Request contiguous memory allocation from manager for array
        // UPDATE: REMOVE _address PARAM AND RUN THROUGH TO MAKE STRUCTURE CALL MALLOC AND NOT MANAGER
        int _address = Manager.malloc(memInc * _length).startAddr;

        // UPDATE: CONSIDER CHANGING THIS AND FOR LOOP CONSTRUCTOR TO ACCOUNT FOR STACKED STRUCTURES?
        // Set DS structure object as [root -> prev] for any type of structure so nodes can backtrack to parent structure as needed without storing excess data
        switch (_type)
        {
            case DSDataType.INT:
                root = new DSNode<int>(this, null, 0, _address, memInc);
                break;
            case DSDataType.FLOAT:
                root = new DSNode<float>(this, null, 0, _address, memInc);
                break;
        }

        IDSNode curNode = root as IDSNode;
        IDSNode nextNode = null;
        int indexAddress = _address;
        for (int i = 1; i < _length; i++)
        {
            indexAddress += memInc;

            switch (_type)
            {
                case DSDataType.INT:
                    nextNode = new DSNode<int>(((DSNode<int>)curNode), null, 0, indexAddress, memInc);
                    break;
                case DSDataType.FLOAT:
                    nextNode = new DSNode<float>(((DSNode<float>)curNode), null, 0, indexAddress, memInc);
                    break;
                default:
                    nextNode = null;
                    break;
            }

            // UPDATE: NOT THE BEST WAY TO DO THIS - POSSIBLE STACKED STRUCTURE ISSUES? MAYBE NOT
            if (curNode is DataStructure)
            {
                curNode = nextNode;
                continue;
            }

            curNode.setNext(nextNode);
            curNode = curNode.getNext(); // CHECK FIRST IF ISSUES - MIGHT NOT BE CALLING CORRECT FUNCTION
        }
    }

    public IDSComponent this[int index]
    {
        get => getIndex(index);
        //set => setIndex(key, value);
    }

    public IDSComponent getIndex(int index)
    {
        if (index >= this.getSize()) throw new System.IndexOutOfRangeException(); // Update to not throw error but relate info to user?

        var curNode = root as IDSNode;
        int curIndex = 0;

        while (curNode != null && curIndex != index)
        {
            curNode = curNode.getNext();
            curIndex++;
        }

        return (IDSComponent)curNode;
    }

    public void setIndex<T>(int index, T value)
    {
        var node = getIndex(index);
        if (node == null) return;

        if(((DSNode<T>)node).valueIsValid<T>())
            ((DSNode<T>)node).setValue(value);
    }

    public override bool isAbstract() { return false; }
}

public class DSLinkedList : DataStructure
{
    public override bool isAbstract() { return true; }
}

public class DSNode<T> : IMemory, IDSNode
{

    public IDSComponent next; // UPDATE: USED FOR BINODES
    public IDSComponent prev;
    protected T value;
    public int memAddr { get; protected set; }
    protected int memSize;

    private DSNode() { }

    public DSNode(IDSComponent _prev, IDSComponent _next, T _value, int _addr, int _size)
    {
        this.next = _next;
        this.prev = _prev;
        this.value = _value;
        this.memAddr = _addr;
        this.memSize = _size;
    }

    public IDSNode getNext() { return ((next is IDSNode) ? (IDSNode)next : null); }
    public IDSNode getPrev() { return ((prev is IDSNode) ? (IDSNode)prev : null); }
    public void setNext(IDSNode _next) { next = (DSNode<T>)_next; }
    public void setPrev(IDSNode _prev) { prev = (DSNode<T>)_prev; }
    public int getSize() { return memSize; }
    public bool isAbstract() { return false; }
    public int getMemAddress() { return getStructureMemAddr(); }

    public void setValue(T _value)
    {
        this.value = _value;
    }

    public bool valueIsValid<K>()
    {
        return typeof(T).Equals(typeof(K));
    }

    protected int getStructureMemAddr()
    {
        int addr = 0;

        IDSComponent cur = this;
        while (cur != null)
        {
            if (cur is DataStructure)
                { addr = cur.getMemAddress(); break; }
            else if (cur is IDSNode)
                cur = (IDSComponent)((IDSNode)cur).getNext();
            else break;
        }

        return addr;
    }

    protected DSDataType getStructureType()
    {
        IDSComponent cur = this;
        while (cur != null)
        {
            if (cur is DataStructure)
                return ((DataStructure)cur).nodeType;
            else if (cur is IDSNode)
                cur = (IDSComponent)((IDSNode)cur).getNext();
            else break;
        }
        return DSDataType.INT;
    }
}

public struct DSInfo
{
    public int nodeCount;
    public int memSize;
    public DSDataType dataType;
    public DSStructType structType;

    public DSInfo(int count = 0, int size = 0, DSDataType _dType = DSDataType.INT, DSStructType _sType = DSStructType.ARRAY)
    {
        nodeCount = count;
        memSize = size;
        dataType = _dType;
        structType = _sType;
    }
}

public enum DSStructType
{
    ARRAY,
    LINKED_LIST
    // TREE_BINARY
    // GRAPH
}

public enum DSDataType
{
    CHAR,
    INT,
    FLOAT,
    BOOL,
    STRING
}

// Malloc could return these instead of int addr, then calling func can set data as needed
public struct MemoryBlock
{
    public int startAddr;
    // CURRENT NOTE: IF DATA VALUE GETS SET, 'inUse' gets auto set to true
    private IMemory _data;
    public IMemory data { get { return _data; } set { inUse = true; _data = value; } }
    public bool inUse;
    public int endAddr { get { return (data == null) ? startAddr : startAddr + data.getSize(); } }
    
    public MemoryBlock(int _addr, IMemory _dat)
    {
        startAddr = _addr;
        _data = _dat;
        inUse = true;
    }

    public MemoryBlock(DataStructure _struct)
    {
        startAddr = _struct.getMemAddress();
        _data = (IMemory)_struct;
        inUse = true;
    }

    public MemoryBlock(int _addr = 0)
    {
        startAddr = _addr;
        _data = null;
        inUse = false;
    }

    public void MarkUnused() { inUse = false; }
}

public class DynamicCode
{
    //public IDynamicCode codedObj { get; private set; }
    public string code { get { return codeLines.ToString(); } }
    private List<string> codeLines = new();
}

public interface IMemory
{
    public int getSize();
    public bool isAbstract();

}

public interface IDSNode : IDSComponent
{
    public IDSNode getNext();
    public IDSNode getPrev();
    public void setNext(IDSNode _next);
    public void setPrev(IDSNode _prev);
}

public interface IDynamicCode
{

}

public interface IDSComponent
{
    public int getMemAddress();
}