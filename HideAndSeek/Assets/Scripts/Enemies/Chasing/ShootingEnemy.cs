using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Nodes;

[DefaultExecutionOrder(-500)]
[RequireComponent(typeof(ChaseLineOfSight))]
public class ShootingEnemy : MonoBehaviour, ISteering
{
    #region Properties
    [Header("AI Sense")]
    private ChaseLineOfSight _clos;
    [SerializeField] private LayerMask _nodeLayer;
    [SerializeField] private LayerMask _wallLayer;

    [Header("ObstacleAvoidance")]
    [SerializeField] private float _radius;
    [SerializeField] private float _angle;
    [SerializeField] private float _personalArea;
    [SerializeField] LayerMask _obsMask;
    private ObstacleAvoidance _obstacleAvoidance;

    [Header("Movement")]
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _maxForce;
    [SerializeField] private float _predictionTime;
    private Vector3 _velocity;
    public Vector3 Velocity => _velocity;

    [Header("Patrol")]
    [SerializeField] private float patrolSpeed;
    private bool resetPatrol;
    private IPathNode _start = null;
    private IPathNode _goal = null;
    private Vector3 _destination;
    private List<IPathNode> _path = new List<IPathNode>();

    [Header("Idle")]
    [SerializeField] private float _idleDuration;
    [SerializeField] private float _idleTimer;

    [Header("Chasing")]
    public ISteering enemyReference;
    private Vector3 _direction;
    private bool _seeingEnemyRightNow;

    [Header("Shoot")]
    public Bullet _bulletPrefab;
    public float _bulletSpeed;
    public float _bulletLifeTime;
    public float _cooldown;
    private float _shootTimer;
    #endregion

    private QuestionNode root;
    void Start()
    {
        _clos = GetComponent<ChaseLineOfSight>();
        _obstacleAvoidance = new ObstacleAvoidance(transform, _radius, _angle, _personalArea, _obsMask);

        ActionNode moveToPoint = new ActionNode(() => MoveToPoint());
        ActionNode idle = new ActionNode(Idle);

        SequenceNode goPatrol = new SequenceNode(new List<ITreeNode>());
        goPatrol.Add(moveToPoint);
        goPatrol.Add(idle);

        ActionNode getPatrolRoute = new ActionNode(GetPatrolRoute);

        SequenceNode setPatrol = new SequenceNode(new List<ITreeNode>());
        setPatrol.Add(getPatrolRoute);
        setPatrol.Add(goPatrol);

        ActionNode chase = new ActionNode(() => Attack());
        QuestionNode isInLos = new QuestionNode(IsInLos, chase, setPatrol);

        root = isInLos;
    }
    void Update()
    {
        root.Execute();
    }
    private bool IsInLos()
    {
        if (_clos.HasTarget(out ISteering t))
        {
            _seeingEnemyRightNow = true;
            resetPatrol = true;
            enemyReference = t;
            return true;
        }
        return false;
    }
    private NodeState Idle()
    {
        _velocity = Vector3.zero;
        _idleTimer += Time.deltaTime;
        float angle = Mathf.Sin(Time.time * 2f) * 180f;
        transform.rotation = Quaternion.Euler(0, angle, 0);

        if (_idleTimer >= _idleDuration)
        {
            _idleTimer = 0f;
            return NodeState.Success;
        }
        return NodeState.Running;
    }
    private NodeState GetPatrolRoute()
    {
        if (_path == null || _path.Count == 0 || resetPatrol)
        {
            _goal = null;
            _start = null;
            _path = new List<IPathNode>();
            _velocity = Vector3.zero;

            _destination = GameManager.Instance.GetPoint();
            _start = GetClosestNode(transform.position);
            _goal = GetClosestNode(_destination);

            _path = GenericPathfinding.ThetaStar<IPathNode>(
                _start,
                node => node == _goal,
                node => node.Neighbors.ToList(),
                (a, b) => Vector3.Distance(a.Position, b.Position),
                node => Vector3.Distance(node.Position, _goal.Position),
                (a, b) => a.HasLineOfSight(b, _wallLayer)
            );
            resetPatrol = false;
        }
        return NodeState.Success;
    }
    private IPathNode GetClosestNode(Vector3 position)
    {
        Collider[] nodes = Physics.OverlapSphere(position, 5f, _nodeLayer, QueryTriggerInteraction.Collide);

        IPathNode closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (var col in nodes)
        {
            if (!col.TryGetComponent<IPathNode>(out var node)) continue;

            Vector3 direction = node.Position - position;
            float distance = direction.magnitude;

            if (Physics.Raycast(position, direction / (distance + 0.01f), distance, _wallLayer)) continue;

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = node;
            }
        }
        return closest;
    }
    private NodeState MoveToPoint()
    {
        if (_path != null && _path.Count > 0)
        {
            IPathNode currentNode = _path[0];
            Vector3 targetPos = currentNode.Position;
            targetPos.y = transform.position.y;

            Vector3 dir = (targetPos - transform.position).normalized;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), _rotationSpeed * Time.deltaTime);
            transform.position += dir * patrolSpeed * Time.deltaTime;

            if (Vector3.Distance(transform.position, targetPos) < 2f)
                _path.RemoveAt(0);

            return NodeState.Running;
        }

        return MoveToDestination();
    }
    private NodeState MoveToDestination()
    {
        Vector3 targetPos = _destination;
        targetPos.y = transform.position.y;

        Vector3 dir = (targetPos - transform.position).normalized;
        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, _rotationSpeed * Time.deltaTime);
        transform.position += dir * patrolSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetPos) < 2f)
            return NodeState.Success;

        return NodeState.Running;
    }
    private NodeState Attack()
    {
        if (_seeingEnemyRightNow && enemyReference != null)
        {
            resetPatrol = true;
            _velocity = Vector3.zero;

            Vector3 targetPosition = enemyReference.transform.position;
            targetPosition.y = transform.position.y;
            _direction = (targetPosition - transform.position).normalized;

            if (_direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(_direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
            }

            _shootTimer += Time.deltaTime;
            if (_shootTimer >= _cooldown)
            {
                Shoot(enemyReference.transform.position);
                _shootTimer = 0f;
            }

            return NodeState.Running;
        }
        enemyReference = null;
        return NodeState.Failure;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, _angle / 2, 0) * transform.forward * _radius);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -_angle / 2, 0) * transform.forward * _radius);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, _destination);
    }
    public void Kill() { }

    private void Shoot(Vector3 direction) 
    {
        if (_bulletPrefab != null)
        {
            Bullet newBullet = Instantiate(_bulletPrefab, transform.position + direction , Quaternion.LookRotation(direction));
            newBullet.Initialize(direction, _bulletSpeed, _bulletLifeTime);
            Debug.Log("Shooting");
        }
    }
}
