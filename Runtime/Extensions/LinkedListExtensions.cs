using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LinkedListExtensions
{
    public static void RemoveSafeForEach<T>(this LinkedList<T> list, System.Action<T> action)
    {
        LinkedListNode<T> current = list.First;
        LinkedListNode<T> next;
        while (current != null)
        {
            next = current.Next;

            action(current.Value);

            current = next;
        }
    }

    public static void RemoveSafeForEachNode<T>(this LinkedList<T> list, System.Action<LinkedListNode<T>> action)
    {
        LinkedListNode<T> current = list.First;
        LinkedListNode<T> next;
        while (current != null)
        {
            next = current.Next;

            action(current);

            current = next;
        }
    }

    public static void RemoveSafeForEach<T>(this LinkedList<T> list, IInvokeable<T> invokeable)
    {
        LinkedListNode<T> current = list.First;
        LinkedListNode<T> next;
        while (current != null)
        {
            next = current.Next;

            invokeable.Invoke(current.Value);

            current = next;
        }
    }

    public static void RemoveSafeForEachNode<T>(this LinkedList<T> list, IInvokeable<LinkedListNode<T>> invokeable)
    {
        LinkedListNode<T> current = list.First;
        LinkedListNode<T> next;
        while (current != null)
        {
            next = current.Next;

            invokeable.Invoke(current);

            current = next;
        }
    }

    // this will clear other list
    public static void AppendLinkedList<T>(this LinkedList<T> list, LinkedList<T> other)
    {
        if (other == null)
        {
            return;
        }

        LinkedListNode<T> node;
        while (other.First != null)
        {
            node = other.First;
            other.RemoveFirst();
            list.AddLast(node);
        }
    }

    public static void Add<T>(this LinkedList<T> list, T value)
    {
        list.AddLast(value);
    }
}
