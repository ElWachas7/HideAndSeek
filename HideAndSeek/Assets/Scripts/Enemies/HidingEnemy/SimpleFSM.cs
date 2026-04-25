using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityStates
{
    Idle,
    Patrol,
    Flee
}
public class SimpleFSM : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] Transform target;
    [SerializeField] Transform[] wayPoints;
    [SerializeField] float speed;
    [SerializeField] private LineOfSight viewLos;

    public Transform Target => target;
    public Transform[] WayPoints => wayPoints;
    public float Speed => speed;
    public LineOfSight ViewLos => viewLos;


    private StateMachine<EntityStates> _sm;

    void Start()
    {
        _sm = new StateMachine<EntityStates>();

        var idle = new HidingEnemyIdleState(this, _sm);
        var patrol = new HidingEnemyPatrolState(this, _sm);
        //var flee = new HidingEnemyFleeState(this, _sm);

        _sm.AddState(idle, EntityStates.Idle);
        _sm.AddState(patrol, EntityStates.Patrol);
        //_sm.AddState(flee, EntityStates.Flee);

        _sm.SetCurrent(patrol);

    }
        void Update()
        {
            if (_sm.CurrentState != null)
            {
                _sm.Update();
            }
        }
    }

