using UnityEngine;
using System.Collections.Generic;
public interface IState<T>
{
    public void Awake();
    public void Execute();
    public void Sleep();
}
