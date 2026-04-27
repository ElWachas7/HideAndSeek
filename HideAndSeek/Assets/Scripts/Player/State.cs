using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Windows;

public class State<T> : IState<T>
{
    protected StateMachine<T> stateMachine;
    private Dictionary<T, IState<T>> transitions;

    public State(StateMachine<T> sm)
    {
        stateMachine = sm;
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
        //Chequea la existencia del input y devuelve un valor, en este caso sería el estado del jugador: Existe move? si, entonces te devuelvo el estado en concreto.
        if (transitions.TryGetValue(input, out IState<T> nextState))
        {
            return nextState;
        }

        return null;
    }

    public void AddTransition(IState<T> state, T input)
    {
        transitions[input] = state;
    }
}
