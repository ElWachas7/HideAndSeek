using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T>
{

    private IState<T> currentState;
    private Dictionary<T, IState<T>> states = new();

    public IState<T> CurrentState => currentState;


    public void SetCurrent(IState<T> state)
    {
        currentState = state;
        currentState.Awake();
    }

    public void AddState(IState<T> state, T stateValue)
    {
        states.Add(stateValue, state);
    }

    public void Update() => currentState.Execute();

    public void ChangeState(T input)
    {
        //Chequea si el estado actual es un dato tipo STATE
        if(currentState is State<T> stateWithTransitions)
        {
            //En caso de serlo, se crea una variable que toma la transición del input con su siguiente estado. Ej; idle -> move / move -> idle
            IState<T> nextState = stateWithTransitions.GetTransition(input);
            //luego chequea si es nulo
            if (nextState != null)
            {
                //En caso de no serlo, duerme al actual (lo 'apaga') lo actualiza (idle -> move) y lo hace funcionar
                currentState.Sleep();
                currentState = nextState;
                currentState.Awake();
            }
        }
       
    }
}
