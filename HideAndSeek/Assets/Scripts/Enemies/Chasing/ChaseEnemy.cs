using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static Nodes;

[DefaultExecutionOrder(-500)]
[RequireComponent(typeof(ChaseLineOfSight))]
public class ChaseEnemy : MonoBehaviour, ISteering
{
    #region Properties
    [Header("AI Sense")]
    private ChaseLineOfSight _clos;
    [SerializeField] private LayerMask _nodeLayer;
    [SerializeField] private LayerMask _wallLayer; // same as ObstacleAvoidance

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
    private Rigidbody _rb;
    private Vector3 _velocity;
    public Vector3 Velocity => _velocity;

    [Header("Patrol")]
    [SerializeField] private float patrolSpeed;
    private bool resetPatrol;
    private IPathNode _start = null;
    private IPathNode _goal = null;
    private List<IPathNode> _path = new List<IPathNode>();

    [Header("Idle")]
    [SerializeField] private float _idleDuration;
    [SerializeField] private float _idleTimer;

    [Header("Chasing")]
    public ISteering enemyReference;
    private Vector3 _lastKnownPosition;
    private bool _hasLastKnownPosition;
    private bool _seeingEnemyRightNow;
    #endregion

    private QuestionNode root;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _clos = GetComponent<ChaseLineOfSight>();
        _obstacleAvoidance = new ObstacleAvoidance(transform, _radius, _angle, _personalArea, _obsMask);

        // ---- Follow a patrol Route ----
        ActionNode moveToPoint = new ActionNode(() => MoveToPoint());
        ActionNode idle = new ActionNode(Idle);

        SequenceNode goPatrol = new SequenceNode(new List<ITreeNode>());
        goPatrol.Add(moveToPoint);
        goPatrol.Add(idle);

        // ---- Search a patrol Route ----
        ActionNode getPatrolRoute = new ActionNode(GetPatrolRoute);

        SequenceNode setPatrol = new SequenceNode(new List<ITreeNode>());
        setPatrol.Add(getPatrolRoute);
        setPatrol.Add(goPatrol);

        // ---- Chase or Patrol
        ActionNode chase = new ActionNode(() => Chase());
        QuestionNode isInLos = new QuestionNode(IsInLos, chase, setPatrol);

        root = isInLos;
    }
    void Update()
    {
        root.Execute();
    }

    // ---- QUESTION NODES ----
    private bool IsInLos()
    {
        if (_clos.HasTarget(out ISteering t))
        {
            _hasLastKnownPosition = true;
            _seeingEnemyRightNow = true;
            resetPatrol = true;
            enemyReference = t;
            _lastKnownPosition = t.transform.position;
            return true;
        }
        else if (_hasLastKnownPosition)
        {
            _seeingEnemyRightNow = false;
            return true;
        }
        //Debug.Log("LOS = False");
        return false;
    }

    // ---- ACTION NODES ----
    private NodeState Idle()
    {
        _idleTimer += Time.deltaTime;
        _rb.velocity = Vector3.zero;
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
            _rb.velocity = Vector3.zero;
            _goal = GameManager.Instance.GetSearchingSpot();
            _start = GetClosestNode(transform.position);
            _path = GenericPathfinding.ThetaStar<IPathNode>(_start, node => node == _goal, node => node.Neighbors.ToList(), (a, b) => Vector3.Distance(a.Position, b.Position), node => Vector3.Distance(node.Position, _goal.Position), (a, b) => a.HasLineOfSight(b, _wallLayer));
            resetPatrol = false;
            Debug.Log($"Path count: {_path.Count} | Start: {_start?.NodeName} | Goal: {_goal?.NodeName}");
        }
        return NodeState.Success;
    }
    private IPathNode GetClosestNode(Vector3 position)
    {
        Collider[] nodes = Physics.OverlapSphere(transform.position, 5f, _nodeLayer, QueryTriggerInteraction.Collide);

        IPathNode closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (var col in nodes)
        {
            if (!col.TryGetComponent<IPathNode>(out var node))
                continue;

            Vector3 direction = node.Position - position;
            float distance = direction.magnitude;

            if (Physics.Raycast(position, direction / (distance + 0.01f), distance, _wallLayer))
            {
                continue;
            }

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
       // Debug.Log($"MoveToPoint - path es null: {_path == null} | count: {_path?.Count}");
        if (_path == null || _path.Count == 0)
            return NodeState.Failure;

        IPathNode _currentNode = _path[0];
        Vector3 targetPos = _currentNode.Position;
        targetPos.y = transform.position.y;
        Vector3 dir = (targetPos - transform.position).normalized;
        Vector3 dirobs = _obstacleAvoidance.GetDir(dir);

        Quaternion rotacionObjetivo = Quaternion.LookRotation(dirobs);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, _rotationSpeed * Time.deltaTime);

        //Debug.Log($"dir: {dir} | dirobs: {dirobs} | patrolSpeed: {patrolSpeed} | rb null: {_rb == null}");

        _rb.velocity = dirobs * patrolSpeed;

        if (Vector3.Distance(transform.position, targetPos) < 2f)
        {
            _path.RemoveAt(0);
            if (_path.Count == 0)
                return NodeState.Success;
        }
        return NodeState.Running;
    }
    private NodeState Chase()
    {
        resetPatrol = true;
        Vector3 steering;
        if (_seeingEnemyRightNow)
        {
            if (Vector3.Distance(transform.position, enemyReference.transform.position) < 3f)
            {
                steering = Seek(enemyReference.transform.position);
            }
            else
            {
                steering = Pursuit(enemyReference);
            }
        }
        else if (_hasLastKnownPosition)
        {
            enemyReference = null;
            steering = Seek(_lastKnownPosition);
        }
        else
        {
            return NodeState.Failure;
        }

        //Convertir el steering en una direccion y luego aplicar el avoidance
        _velocity += steering * Time.deltaTime;
        //velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        Vector3 dir = _velocity.normalized;

        Vector3 finalDir = _obstacleAvoidance.GetDir(dir);

        _velocity = finalDir * _velocity.magnitude;

        transform.position += _velocity * Time.deltaTime;

        Quaternion rot = Quaternion.LookRotation(_velocity);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, _rotationSpeed * Time.deltaTime);

        Vector3 targetPos = _seeingEnemyRightNow ? enemyReference.transform.position : _lastKnownPosition;

        if (Vector3.Distance(transform.position, targetPos) < 1.5f)
        {
            if (_seeingEnemyRightNow && enemyReference != null)
            {
                enemyReference.Kill();
            }
            _hasLastKnownPosition = false;
            return NodeState.Success;
        }
        return NodeState.Running;
    }
    // ---- Steering Behaviour ----
    Vector3 Seek(Vector3 targetPos) // posicion guardada
    {
        targetPos.y = transform.position.y;
        Vector3 desired = (targetPos - transform.position).normalized * _maxSpeed;
        Vector3 steering = desired - _velocity;

        return Vector3.ClampMagnitude(steering, _maxForce);
    }
    Vector3 Pursuit(ISteering target) // posicion actualizada
    {
        Vector3 futurePos = target.transform.position + target.Velocity * _predictionTime;
        futurePos.y = transform.position.y;
        Vector3 desired = (futurePos - transform.position).normalized * _maxSpeed;
        Vector3 steering = desired - _velocity;

        return Vector3.ClampMagnitude(steering, _maxForce);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _radius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, _angle / 2, 0) * transform.forward * _radius);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -_angle / 2, 0) * transform.forward * _radius);
    }
    public void Kill() { }
}
