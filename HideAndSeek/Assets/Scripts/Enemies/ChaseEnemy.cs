using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.AI;
using static Nodes;

[RequireComponent(typeof(ChaseLineOfSight))]
public class ChaseEnemy : MonoBehaviour
{
    [Header("AI Sense")]
    private ChaseLineOfSight _clos;
    private int currentCorner = 0;
    private Vector3[] corners;
    public NavMeshAgent agent;
   // Transform target;

    [Header("Stats")]
    public float speed;
    public float rotationSpeed; 
    
    public Transform LastEnemyPosition;
    public bool hasSeenEnemy;

    [Header("Patrol")]
    public List<Transform> patrolRoute;
    private bool hasPatrolRoute;
    public Transform currentPoint;
    private float idleTimer = 0f;
    [SerializeField] float idleDuration = 2f;

    private QuestionNode root;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        _clos = GetComponent<ChaseLineOfSight>();

        ActionNode idle = new ActionNode(Idle);
        ActionNode getPatrolRoute = new ActionNode(GetPatrolRoute);
        ActionNode moveToPoint = new ActionNode(() => MoveToPoint(currentPoint));
        ActionNode calculatePathToPoint = new ActionNode(() => CalculatePathToPoint(currentPoint));
        ActionNode moveToEnemy = new ActionNode(()=> MoveToEnemy(LastEnemyPosition));
        ActionNode calculatePathToEnemy = new ActionNode(()=> CalculatePathToEnemy(LastEnemyPosition));

        SequenceNode goChase = new SequenceNode(new List<ITreeNode>());
        goChase.Add(calculatePathToEnemy);
        goChase.Add(moveToEnemy);

        SequenceNode goPatrol = new SequenceNode(new List<ITreeNode>());
        goPatrol.Add(calculatePathToPoint);
        goPatrol.Add(moveToPoint);
        goPatrol.Add(idle);

        SequenceNode setPatrol = new SequenceNode(new List<ITreeNode>());
        setPatrol.Add(getPatrolRoute);
        setPatrol.Add(goPatrol);

        QuestionNode isInLos = new QuestionNode(IsInLos, goChase, setPatrol);

        root = isInLos;

    }
    void Update()
    {
        root.Execute();
    }

    // ---- QUESTION NODES ----
    private bool IsInLos() 
    {
        if(_clos.HasTarget(out Transform t))
        {
            LastEnemyPosition = t;
            if(LastEnemyPosition != null) 
            {
                hasSeenEnemy = true;
            }
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

        if (dir.magnitude < 2f && Vector3.Distance(targetPos, transform.position) < 2f)
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
            transform.position += dir.normalized * speed * Time.deltaTime;
        }
        return NodeState.Running;
    }
    private NodeState CalculatePathToPoint(Transform Point)
    {
        if (Point == null) return NodeState.Failure;
        if (hasPatrolRoute) 
        {
            NavMeshPath path = new NavMeshPath();
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
    private NodeState MoveToEnemy(Transform target) 
    {

        Vector3 targetPos;
        if (currentCorner < corners.Length && corners.Length != 0)
        {
            targetPos = corners[currentCorner];
        }
        else
        {
            targetPos = target.position;
        }

        Vector3 dir = targetPos - transform.position;
        Quaternion rotacionObjetivo = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, rotationSpeed * Time.deltaTime);

        if (Vector3.Distance(targetPos, transform.position) < 2f)
        {
            
            if (currentCorner < corners.Length && corners.Length != 0)
            {
                Debug.Log("Cambiar de punto");
                currentCorner++;
            }
            else
            {
                Debug.Log("Eliminar al target");
                return NodeState.Success;
            }
        }
        else
        {
            transform.position += dir.normalized * speed * Time.deltaTime;
        }
        return NodeState.Running;
    }
    private NodeState CalculatePathToEnemy(Transform target) 
    {
        if (target == null) return NodeState.Failure;
        NavMeshPath path = new NavMeshPath();

        if (NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path))
        {
            corners = path.corners;
            currentCorner = 0;
            Debug.Log("SI calculo un path");
            return NodeState.Success;
        }
        return NodeState.Failure;
    }












    /*
    private void Seek()
    {
        var desired_velocity = (target.transform.position - transform.position).NoY().normalized * max_speed;
        var steering = desired_velocity - currentSpeed;

        currentSpeed += steering * Time.deltaTime;
    }
    private void Flee()
    {
        var desired_velocity = (transform.position - target.transform.position).NoY().normalized * max_speed;
        var steering = desired_velocity - currentSpeed;

        currentSpeed += steering * Time.deltaTime;
    }

    private void Persuit()
    {
        var future_position = target.transform.position + target.Speed * timePrediction;


        var desired_velocity = (future_position - transform.position).NoY().normalized * max_speed;
        var steering = desired_velocity - currentSpeed;

        currentSpeed += steering * Time.deltaTime;
    }
    */
}
