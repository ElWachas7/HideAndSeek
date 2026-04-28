using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingEnemyIdleState : State<EntityStates>
{
    private HidingEnemy _entity;
    private float _timer;
    private float idleTime; 

    private System.Action _OnTargetSpotted; // guardo evento de flee en variable para poder desuscribirse luego 

    public HidingEnemyIdleState(HidingEnemy entity, StateMachine<EntityStates> sm) : base(sm)
    {
        _entity = entity;
    }
    public override void Awake()
    {
        base.Awake();
        _timer = 0f; // reiniciar tiempo cada vez q entra a idle
        idleTime = Random.Range(10f, 20f); // duracion aleatoria del idle, entre 10f y 20f
        _OnTargetSpotted = () => stateMachine.ChangeState(EntityStates.Flee); // que hacer cuando se triggerea el evento
        _entity.OnTargetSpotted += _OnTargetSpotted; // suscribo a evento de flee
    }
    public override void Execute()
    {
        base.Execute();

        _timer += Time.deltaTime;

        float angle = Mathf.Sin(Time.time * 2f) * 180f; // rotacion para que el enemy mire a su alrededor
        _entity.transform.rotation = Quaternion.Euler(0, angle, 0);

        if (_timer >= idleTime && !_entity.HasSeenTarget) // si termino el tiempo y no vio al chasing enemy, pasar a patrol
        {
            stateMachine.ChangeState(EntityStates.Patrol);
        }
    }
    public override void Sleep()
    {
        base.Sleep();
        _entity.OnTargetSpotted -= _OnTargetSpotted; // desuscribo al evento de flee 
    }
}
