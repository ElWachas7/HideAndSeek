using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingEnemyFleeState : State<EntityStates>
{
    private HidingEnemy _entity;
    private ObstacleAvoidance obstacleAvoidance;



    public HidingEnemyFleeState(HidingEnemy entity, StateMachine<EntityStates> sm, ObstacleAvoidance obsAvoidance) : base(sm)
    {
        //_entity = entity;
        //obstacleAvoidance = obsAvoidance;


        //newHidingSpot = GameManager.Instance.GetHidingSpot();
        //Debug.Log("set first hiding spot");
        //if (newHidingSpot == null)
        //{
        //    // No hay spots disponibles, volver a Idle
        //    _sm.ChangeState(EntityStates.Idle);
        //}
    }


}
