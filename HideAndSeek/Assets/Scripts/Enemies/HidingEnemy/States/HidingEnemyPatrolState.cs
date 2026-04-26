using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingEnemyPatrolState : State<EntityStates>
{
    private HidingEnemy _entity;
    private int currentWP;

    private ObstacleAvoidance obstacleAvoidance;


    public HidingEnemyPatrolState(HidingEnemy entity, StateMachine<EntityStates> sm, ObstacleAvoidance obsAvoidance) : base(sm)
    {
        _entity = entity;
        obstacleAvoidance = obsAvoidance;
    }
    public override void Execute()
    {
        base.Execute();
        Patrol();
    }
    private void Patrol()
    {
        Transform targetWaypoint = _entity.WayPoints[currentWP];
        
        Vector3 dirToWaypoint = (targetWaypoint.position - _entity.transform.position).normalized; // direccion normalizada hacia el waypoint actual

        Vector3 moveDir = obstacleAvoidance.GetDir(dirToWaypoint); // ObstacleAvoidance puede redirigir el movimiento si hay obstaculos

        _entity.transform.position += moveDir * _entity.Speed * Time.deltaTime; // moverse en la direccion (posiblemente corregida)

        if (Vector3.Distance(_entity.transform.position, targetWaypoint.position) <= 0.5f)
        {
            currentWP = (currentWP + 1) % _entity.WayPoints.Length;
            _sm.ChangeState(EntityStates.Idle);
        }
    }

}
