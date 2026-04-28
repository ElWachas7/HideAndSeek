using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static Nodes;

[RequireComponent(typeof(ChaseLineOfSight))]
public class ChaseEnemy : MonoBehaviour, ISteering
{
    [Header("AI Sense")]
    private ChaseLineOfSight _clos;
    private int currentCorner = 0;
    private Vector3[] corners;
    public NavMeshAgent agent;
    private NavMeshPath path;
    // Transform target;

    [Header("Stats")]
    Vector3 velocity;
    public Vector3 Velocity => velocity;
    [SerializeField] float maxSpeed = 5f;
    [SerializeField] float maxForce = 10f;
    [SerializeField] float predictionTime = 1f;


    [Header("Patrol")]
    public List<Transform> patrolRoute;
    private bool hasPatrolRoute;
    public Transform currentPoint;
    private float idleTimer;
    [SerializeField] float idleDuration;
    [SerializeField] private float patrolSpeed;
    [SerializeField] private float rotationSpeed;

    [Header("Chasing")]
    public ISteering enemyReference;
    [SerializeField] private float chaseSpeed;
    private Vector3 lastKnownPosition;
    private bool hasLastKnownPosition;
    private bool seeingEnemyRightNow;

    private QuestionNode root;

    public void Kill(){ }
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        _clos = GetComponent<ChaseLineOfSight>();

        ActionNode idle = new ActionNode(Idle);
        ActionNode getPatrolRoute = new ActionNode(GetPatrolRoute);
        ActionNode moveToPoint = new ActionNode(() => MoveToPoint(currentPoint));
        ActionNode calculatePathToPoint = new ActionNode(() => CalculatePathToPoint(currentPoint));
        ActionNode chase = new ActionNode(()=> Chase());

        SequenceNode goPatrol = new SequenceNode(new List<ITreeNode>());
        goPatrol.Add(calculatePathToPoint);
        goPatrol.Add(moveToPoint);
        goPatrol.Add(idle);

        SequenceNode setPatrol = new SequenceNode(new List<ITreeNode>());
        setPatrol.Add(getPatrolRoute);
        setPatrol.Add(goPatrol);

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
        if(_clos.HasTarget(out ISteering t))
        {
            hasLastKnownPosition = true;
            seeingEnemyRightNow = true;
            enemyReference = t;
            lastKnownPosition = t.transform.position;
            return true;
        }
        else if (hasLastKnownPosition) 
        {
            seeingEnemyRightNow = false;
            return true;
        }
        return false;
    } 

    // ---- ACTION NODES ----
    private NodeState Idle()
    {
        idleTimer += Time.deltaTime;

        float angle = Mathf.Sin(Time.time * 2f) * 180f ;
        transform.rotation = Quaternion.Euler(0, angle, 0);

        if (idleTimer >= idleDuration)
        {
            idleTimer = 0f;
            return NodeState.Success;
        }
        return NodeState.Running;
    }
    private NodeState GetPatrolRoute() 
    {
        if (patrolRoute.Count == 0) 
        {
            patrolRoute = GameManager.Instance.GetPath().pathway;
            currentPoint = patrolRoute[0];
            hasPatrolRoute = true;
        }
        else { currentPoint = patrolRoute[0]; }
            return NodeState.Success;
    }
    private NodeState MoveToPoint(Transform point)
    {
        if (corners == null || corners.Length == 0)
            return NodeState.Failure;

        Vector3 targetPos;
        if (currentCorner < corners.Length)
        {
            targetPos = corners[currentCorner];
        }
        else
        {
            targetPos = point.position;
        }

        Vector3 dir = targetPos - transform.position;
        Quaternion rotacionObjetivo = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, rotationSpeed * Time.deltaTime);

        if (dir.magnitude < 2f)
        {
            if (currentCorner < corners.Length)
            {
                currentCorner++;
            }
            else
            {
                patrolRoute.RemoveAt(0);
                return NodeState.Success;
            }
        }
        else
        {
            transform.position += dir.normalized * patrolSpeed * Time.deltaTime;
        }
        return NodeState.Running;
    }
    private NodeState CalculatePathToPoint(Transform Point)
    {
        if (Point == null) return NodeState.Failure;
        if (hasPatrolRoute) 
        {
            path = new NavMeshPath();
            if (NavMesh.CalculatePath(transform.position, Point.position, NavMesh.AllAreas, path))
            {
                corners = path.corners;
                currentCorner = 0;
                return NodeState.Success;
            }
            return NodeState.Failure;
        }
        return NodeState.Success;
    }
    private NodeState Chase() 
    {
        Vector3 steering;

        if (seeingEnemyRightNow)
        {
            float dist = Vector3.Distance(transform.position, enemyReference.transform.position);

            if (dist < 3f)
            {
                steering = Seek(enemyReference.transform.position);
            }
            else
            {
                steering = Pursuit(enemyReference);
            }
        }
        else if (hasLastKnownPosition)
        {
            enemyReference = null;
            steering = Seek(lastKnownPosition);
        }
        else
        {
            return NodeState.Failure;
        }

        velocity += steering * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        transform.position += velocity * Time.deltaTime;

        Quaternion rot = Quaternion.LookRotation(velocity);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotationSpeed * Time.deltaTime);

        Vector3 targetPos = seeingEnemyRightNow ? enemyReference.transform.position : lastKnownPosition;

        if (Vector3.Distance(transform.position, targetPos) < 1.5f)
        {
            if (seeingEnemyRightNow && enemyReference != null)
            {
                enemyReference.Kill();
            }

            hasLastKnownPosition = false;
            return NodeState.Success;
        }
        return NodeState.Running;
    }
    
    Vector3 Seek(Vector3 targetPos)
    {
        Vector3 desired = (targetPos - transform.position).normalized * maxSpeed;
        Vector3 steering = desired - velocity;

        return Vector3.ClampMagnitude(steering, maxForce);
    }

    Vector3 Pursuit(ISteering target)
    {
        Vector3 futurePos = target.transform.position + target.Velocity * predictionTime;

        Vector3 desired = (futurePos - transform.position).normalized * maxSpeed;
        Vector3 steering = desired - velocity;

        return Vector3.ClampMagnitude(steering, maxForce);
    }
    
}
