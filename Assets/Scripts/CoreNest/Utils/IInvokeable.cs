using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInvokeable
{
    public void Invoke();
}

public interface IInvokeable<T>
{
    public void Invoke(T obj);
}
