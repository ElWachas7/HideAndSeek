using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingEnemyIdleState : State<EntityStates>
{
    private HidingEnemy _entity;
    private float _timer;
    private float idleTime;

    public HidingEnemyIdleState(HidingEnemy entity, StateMachine<EntityStates> sm) : base(sm)
    {
        _entity = entity;
    }
    public override void Awake()
    {
        base.Awake();
        _timer = 0f; // reiniciar tiempo cada vez q entra a idle
        idleTime = Random.Range(10f, 20f);
    }
    public override void Execute()
    {
        base.Execute();

        //if (_entity.ViewLos.CheckRange(_entity.Target) &&
        //    _entity.ViewLos.CheckAngle(_entity.Target) &&
        //    _entity.ViewLos.CheckView(_entity.Target))
        //{
        //    _sm.ChangeState(EntityStates.Flee);
        //    return;
        //}
        _timer += Time.deltaTime;

        float angle = Mathf.Sin(Time.time * 2f) * 180f;
        _entity.transform.rotation = Quaternion.Euler(0, angle, 0);

        if (_timer >= idleTime)
        {
            stateMachine.ChangeState(EntityStates.Patrol);
            //Debug.Log("cambia a patrol");
        }
    }
}
