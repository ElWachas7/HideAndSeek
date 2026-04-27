using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityStates
{
    Idle,
    Patrol,
    Flee
}
public class HidingEnemy : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] Transform target;
    [SerializeField] Transform[] wayPoints;
    [SerializeField] float speed;
    [SerializeField] private LineOfSight viewLos;

    [Header("ObstacleAvoidance")]
    [SerializeField] private float obsRadius;
    [SerializeField] private float obsAngle;
    [SerializeField] private float obsPersonalArea;
    [SerializeField] private LayerMask obsMask;

    public Transform Target => target;
    public Transform[] WayPoints => wayPoints;
    public float Speed => speed;
    public LineOfSight ViewLos => viewLos;


    private StateMachine<EntityStates> _sm;

    void Start()
    {
        _sm = new StateMachine<EntityStates>();
        var obstacleAvoidance = new ObstacleAvoidance(transform, obsRadius, obsAngle, obsPersonalArea, obsMask);

        var idle = new HidingEnemyIdleState(this, _sm);
        var patrol = new HidingEnemyPatrolState(this, _sm, obstacleAvoidance);
        //var flee = new HidingEnemyFleeState(this, _sm);

        _sm.AddState(idle, EntityStates.Idle);
        _sm.AddState(patrol, EntityStates.Patrol);
        //_sm.AddState(flee, EntityStates.Flee);

        _sm.Initialize(patrol);

    }
    void Update()
    {
        if (_sm.CurrentState != null)
        {
            _sm.Update();
        }
    }

    private void OnDrawGizmos()
    {
        Color myColor = Color.magenta;
        myColor.a = 0.5f;
        Gizmos.color = myColor;
        Gizmos.DrawWireSphere(transform.position, obsRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, obsPersonalArea);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, obsAngle / 2, 0) * transform.forward * obsRadius);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -obsAngle / 2, 0) * transform.forward * obsRadius);
    }
}
