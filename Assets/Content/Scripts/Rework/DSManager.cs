using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using Unity.VisualScripting;
using UnityEngine;

// Create and store structures
public class DSManager
{
    LinkedList<DSGeneric> structures = new();

    public void addTest()
    {
        var list = new DSLinkedList3<int>();
        structures.AddFirst(list);
    }
}

public interface IDS
{
    public abstract void redraw();
    public abstract void destroy();
}

public abstract class DSGeneric : IDS
{
    protected DSDisplayPanel displayPanel;

    public abstract void Initialize();
    public abstract void redraw();
    public abstract void destroy();
}

public abstract class LinearDataStructure<T> : DSGeneric
{
    public LinearNode<T> head = null;
    public int length = 0;

    public abstract void addFirst(T _newValue);
    public abstract void addLast(T _newValue);
    public abstract void insert(T _newValue, int index);
    public abstract void remove(int index);
    public abstract void remove(ref LinearNode<T> node);
    public abstract void updateValue(T _newValue, int index);
    public abstract void updateValue(T _newValue, ref LinearNode<T> node);
    public abstract LinearNode<T> getIndex(int index);
}

public class DSLinkedList3<T> : LinearDataStructure<T>
{
    public override void Initialize()
    {
        displayPanel = new(); // ASSUMPTION: This creates an empty game object, then adds the panel component to it
        // Unsure if this can be added to a gameobject later or if add comp is needed as below
        //GameObject go = new GameObject();
        //go.AddComponent<DSDisplayPanel>();


    }

    public override LinearNode<T> getIndex(int index)
    {
        if (index >= length || index < 0) return null;

        var cur = head;
        while (cur != null)
        {
            if (index == 0)
                break;

            cur = cur.next;
            index--;
        }

        return cur;
    }

    public override void addFirst(T _newValue)
    {
        if (head == null)
            head = new LinearNode<T>(_newValue);
        else
        {
            head.prev = new LinearNode<T>(_newValue, head);
            head = head.prev;
        }

        length++;
    }

    public override void addLast(T _newValue)
    {
        if (head == null)
            head = new LinearNode<T>(_newValue);
        else
        {
            var cur = head;

            while (cur.next != null)
                cur = cur.next;

            cur.next = new LinearNode<T>(_newValue, null, cur);
        }

        length++;
    }

    public override void insert(T _newValue, int index)
    {
        if (index > length || index < 0)
            return;
        
        if (index == length)
            addLast(_newValue);
        else if (index == 0)
            addFirst(_newValue);
        else
        {
            var cur = getIndex(index);

            if (cur != null)
            {
                var newNode = new LinearNode<T>(_newValue, cur, cur.prev);
                cur.prev.next = cur.prev = newNode;
                length++;
            }
        }
    }

    public override void updateValue(T _newValue, int index)
    {
        var node = getIndex(index);

        if (node != null)
            node.value = _newValue;
    }

    public override void updateValue(T _newValue, ref LinearNode<T> node)
    {
        node.value = _newValue;
    }

    public override void remove(int index)
    {
        var node = getIndex(index);
        if (node != null)
            remove(ref node);
    }

    public override void remove(ref LinearNode<T> node)
    {
        if (node == null) return;

        if (node.prev != null)
            node.prev.next = node.next;
        if (node.next != null)
            node.next.prev = node.prev;
    }
    
    public override void destroy()
    {
        throw new System.NotImplementedException();
    }

    public override void redraw()
    {
        throw new System.NotImplementedException();
    }
}