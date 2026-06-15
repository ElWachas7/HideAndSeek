using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HidingEnemyPatrolState : State<EntityStates>
{
    private HidingEnemy _entity;
    private ObstacleAvoidance obstacleAvoidance;
    private IPathNode _goal;
    private List<IPathNode> _path;
    private System.Action _OnTargetSpotted;

    public HidingEnemyPatrolState(HidingEnemy entity, StateMachine<EntityStates> sm, ObstacleAvoidance obsAvoidance) : base(sm)
    {
        _entity = entity;
        obstacleAvoidance = obsAvoidance;
    }
    public override void Awake()
    {
        base.Awake();
        SetNewHidingSpot();
        _OnTargetSpotted = () => stateMachine.ChangeState(EntityStates.Flee);
        _entity.OnTargetSpotted += _OnTargetSpotted;
    }
    public override void Execute()
    {
        base.Execute();
        if (_path != null && _path.Count > 0)
            Patrol();
    }
    public override void Sleep()
    {
        base.Sleep();
        _entity.OnTargetSpotted -= _OnTargetSpotted;
    }
    private void SetNewHidingSpot()
    {
        _goal = GameManager.Instance.GetHidingSpot();
        IPathNode start = GetClosestNode(_entity.transform.position);
        _path = GenericPathfinding.ThetaStar<IPathNode>(start, node => node == _goal, node => node.Neighbors.ToList(), (a, b) => Vector3.Distance(a.Position, b.Position), node => Vector3.Distance(node.Position, _goal.Position), (a, b) => a.HasLineOfSight(b, _entity.WallLayer));
    }
    private void Patrol()
    {
        IPathNode current = _path[0];
        Vector3 targetPos = current.Position;

        Vector3 dir = (targetPos - _entity.transform.position).normalized;
        Vector3 moveDir = obstacleAvoidance.GetDir(dir);

        if (moveDir != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(moveDir);
            _entity.transform.rotation = Quaternion.Slerp(_entity.transform.rotation, rotation, 5f * Time.deltaTime);
        }

        _entity.transform.position += moveDir * _entity.Speed * Time.deltaTime;

        if (Vector3.Distance(_entity.transform.position, targetPos) <= 2f)
        {
            _path.RemoveAt(0);

            if (_path.Count == 0)
                stateMachine.ChangeState(EntityStates.Idle);
        }
    }
    private IPathNode GetClosestNode(Vector3 position)
    {
        Collider[] cols = Physics.OverlapSphere(position, 5f, _entity.NodeLayer, QueryTriggerInteraction.Collide);

        IPathNode closest = null;
        float minDist = Mathf.Infinity;

        foreach (Collider col in cols)
        {
            if (!col.TryGetComponent(out IPathNode node)) continue;

            Vector3 direction = node.Position - position;
            float distance = direction.magnitude;

            if (Physics.Raycast(position, direction / distance, distance, _entity.WallLayer)) continue;

            if (distance < minDist)
            {
                minDist = distance;
                closest = node;
            }
        }
        return closest;
    }
}