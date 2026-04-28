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
        _entity.OnTargetSpotted += () => _sm.ChangeState(EntityStates.Flee);
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

        _timer += Time.deltaTime;

        float angle = Mathf.Sin(Time.time * 2f) * 180f;
        _entity.transform.rotation = Quaternion.Euler(0, angle, 0);

        if (_timer >= idleTime)
        {
            _sm.ChangeState(EntityStates.Patrol);
        }
    }
}
