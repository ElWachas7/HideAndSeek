using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Windows;

public class State<T> : IState<T>
{
    protected StateMachine<T> _sm;
    private Dictionary<T, IState<T>> transitions;

    public State(StateMachine<T> sm)
    {
        _sm = sm;
        transitions = new Dictionary<T, IState<T>>();
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

    public IState<T> GetTransition(T input)
    {
        if (transitions.ContainsKey(input))
        {
            return transitions[input];
        }
        return null;
    }

    public void AddTransition(IState<T> state, T input)
    {
        transitions[input] = state;
    }
}
