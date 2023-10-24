using System;
using System.Collections.Generic;

public class SingletonStack<T>
{
    private LinkedList<T> linkedList = new LinkedList<T>();
    private HashSet<T> hashSet = new HashSet<T>();

    public void Push(T value)
    {
        if (hashSet.Contains(value))
        {
            linkedList.Remove(value);
        }
        else
        {
            hashSet.Add(value);
        }

        linkedList.AddFirst(value);
    }

    public T Pop()
    {
        if (linkedList.Count == 0) throw new InvalidOperationException("The stack is empty");

        T value = linkedList.First.Value;
        linkedList.RemoveFirst();
        hashSet.Remove(value);

        return value;
    }

    public bool TryPop(out T value)
    {
        if (linkedList.Count == 0)
        {
            value = default;
            return false;
        }

        value = linkedList.First.Value;
        linkedList.RemoveFirst();
        hashSet.Remove(value);

        return true;
    }

    public T Peek()
    {
        if (linkedList.Count == 0) throw new InvalidOperationException("The stack is empty");
        return linkedList.First.Value;
    }

    public bool TryPeek(out T value)
    {
        if (linkedList.Count == 0)
        {
            value = default;
            return false;
        }

        value = linkedList.First.Value;
        return true;
    }

    public bool Contains(T value)
    {
        return hashSet.Contains(value);
    }

    public void Clear()
    {
        linkedList.Clear();
        hashSet.Clear();
    }

    public bool IsEmpty()
    {
        return linkedList.Count == 0;
    }

    public List<T> ToList()
    {
        return new List<T>(linkedList);
    }

    public int Count
    {
        get { return linkedList.Count; }
    }
}
