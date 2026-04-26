using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingEnemyPatrolState : State<EntityStates>
{
    private HidingEnemy _entity;
    private int currentWP;

    public HidingEnemyPatrolState(HidingEnemy entity, StateMachine<EntityStates> sm) : base(sm)
    {
        _entity = entity;
    }
    public override void Execute()
    {
        base.Execute();
        Patrol();
    }
    private void Patrol()
    {
        Transform targetWaypoint = _entity.WayPoints[currentWP];

        _entity.transform.position = Vector3.MoveTowards(_entity.transform.position, targetWaypoint.position, _entity.Speed * Time.deltaTime);

        if (Vector3.Distance(_entity.transform.position, targetWaypoint.position) <= 0.5f)
        {
            currentWP = (currentWP + 1) % _entity.WayPoints.Length;
        }
    }

}
