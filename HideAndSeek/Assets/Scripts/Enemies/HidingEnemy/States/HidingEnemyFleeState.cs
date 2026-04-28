using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HidingEnemyFleeState : State<EntityStates>
{
    private HidingEnemy _entity;
    private ObstacleAvoidance obstacleAvoidance;
    private Vector3 currentSpeed;
    public HidingEnemyFleeState(HidingEnemy entity, StateMachine<EntityStates> sm, ObstacleAvoidance obsAvoidance) : base(sm)
    {
        _entity = entity;
        obstacleAvoidance = obsAvoidance;
    }
    public override void Awake()
    {
        base.Awake();
        currentSpeed = Vector3.zero;
    }
    public override void Execute()
    {
        base.Execute();

        if (!_entity.HasSeenTarget)
        {
            _sm.ChangeState(EntityStates.Patrol);
            return;
        }
        Flee();
    }
    private void Flee()
    {
        Vector3 targetPos = _entity.Target.transform.position;

        Vector3 desiredVelocity = (_entity.transform.position - targetPos).normalized * _entity.Speed; // dir opuesta a chasing enemy (a donde quiero ir)

        Vector3 steering = desiredVelocity - currentSpeed; // cuanto falta corregir para llegar a esa velocidad

        currentSpeed += steering * Time.deltaTime; // aplica la correccion y movimiento suave

        Vector3 moveDir = obstacleAvoidance.GetDir(currentSpeed.normalized);
        moveDir.y = 0;

        if (moveDir != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(moveDir);
            _entity.transform.rotation = Quaternion.Slerp(_entity.transform.rotation, rotation, 5f * Time.deltaTime);
        }

        // calcula a donde iria si se mueve libremente y lo guarda en nextpos
        Vector3 nextPos = _entity.transform.position + moveDir * currentSpeed.magnitude * Time.deltaTime;

        NavMeshHit hit; // se crea para que SamplePosition guarde la pos valida que encontro
        if (NavMesh.SamplePosition(nextPos, out hit, 1f, NavMesh.AllAreas))
        {
            // preservar Y original para que no se hunda en el piso
            // SamplePosition busca el punto valido mas cercano a nextPos, en un radio de 1f
            _entity.transform.position = new Vector3(hit.position.x, _entity.transform.position.y, hit.position.z);
        }
        //_entity.transform.position += moveDir * currentSpeed.magnitude * Time.deltaTime;
    }
    public override void Sleep()
    {
        base.Sleep();

    }
}
