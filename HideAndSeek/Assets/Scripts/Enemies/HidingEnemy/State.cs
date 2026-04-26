using UnityEngine;

public class State<T> : IState
{
    protected StateMachine<T> _sm;

    public State(StateMachine<T> sm)
    {
        _sm = sm;
    }

    public virtual void Awake()
    {
        Debug.Log("Entered " + GetType());
    }

    public virtual void Sleep()
    {
        Debug.Log("Exited " + GetType());
    }

    public virtual void Execute()
    {
    }
}
