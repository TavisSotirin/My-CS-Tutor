using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Delete me and rename file
public class DSNodes
{

}

public interface INode
{
    public bool isDirty();
}

public abstract class NodeGeneric<T> : INode
{
    public T value;
    protected bool bDirty = true;

    public abstract bool isDirty();
}

public class LinearNode<T> : NodeGeneric<T>
{
    public LinearNode<T> prev;
    public LinearNode<T> next;

    public LinearNode(T _value = default(T), LinearNode<T> _next = null, LinearNode<T> _prev = null)
    {
        this.value = _value;
        this.next = _next;
        this.prev = _prev;
    }

    public override bool isDirty()
    {
        return bDirty;
    }
}

//public class TreeNode<T> : NodeGeneric<T>
//{
    //public TreeNode<T> parent;
    //public TreeNode<T>[] children;
//}