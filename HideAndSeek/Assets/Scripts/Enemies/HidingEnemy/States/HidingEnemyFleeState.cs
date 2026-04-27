using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingEnemyFleeState : State<EntityStates>
{
    private HidingEnemy _entity;
    private ObstacleAvoidance obstacleAvoidance;

    public HidingEnemyFleeState(HidingEnemy entity, StateMachine<EntityStates> sm, ObstacleAvoidance obsAvoidance) : base(sm)
    {
        
    }


}
