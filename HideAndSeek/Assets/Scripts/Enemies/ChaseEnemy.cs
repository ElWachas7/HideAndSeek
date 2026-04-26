using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static Nodes;

[RequireComponent(typeof(LineOfSight))]
public class ChaseEnemy : MonoBehaviour
{
    [Header("AI Sense")]
    private LineOfSight _los;
    private int currentCorner = 0;
    private Vector3[] corners;
    public NavMeshAgent agent;
   // Transform target;

    [Header("Stats")]
    public float speed;
    public Transform currentPoint;
    public Transform LastEnemyPosition;
    public bool HasSeenEnemy;
    private float repathTimer = 0f;
    [SerializeField] float repathRate = 0.5f;

    [Header("Patrol")]
    public List<Transform> patrolRoute;
    private bool hasPatrolRoute;
    private float idleTimer = 0f;
    [SerializeField] float idleDuration = 2f;

    [Header("TEST")]
    [SerializeField] Transform point;
    [SerializeField] Transform point2;
    [SerializeField] Transform point3;
    [SerializeField] Transform point4;

    private QuestionNode root;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        //agent.updatePosition = false;
        //agent.updateRotation = false;

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
    private bool IsInLos() => /*_los.CheckRange(target) && _los.CheckAngle(target) && _los.CheckView(target) ||*/ HasSeenEnemy;

    // ---- ACTION NODES ----
    private NodeState Idle()
    {
        idleTimer += Time.deltaTime;

        float angle = Mathf.Sin(Time.time * 2f) * 30f;
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
        //LISTA DE PUNTOS = GameManager.instance.GetPatrolRoute();
        if (patrolRoute.Count == 0) 
        {
            patrolRoute.Add(point);
            patrolRoute.Add(point2);
            patrolRoute.Add(point3);
            patrolRoute.Add(point4);
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
                Debug.Log("SI calculo un path");
                return NodeState.Success;
            }
            Debug.Log("no calculo ningun path");
            return NodeState.Failure;
        }
        return NodeState.Success;
    }




    private NodeState MoveToEnemy(Transform target) 
    {
        if (corners == null || corners.Length == 0)
            return NodeState.Failure;

        Vector3 targetCorner = corners[currentCorner];
        Vector3 dir = targetCorner - transform.position;

        if (dir.magnitude < 0.2f)
        {
            currentCorner++;
            if (currentCorner >= corners.Length)
            {
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

        repathTimer += Time.deltaTime;
        if (repathTimer < repathRate)
            return NodeState.Success;

        repathTimer = 0f;

        NavMeshPath path = new NavMeshPath();

        if (NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path))
        {
            corners = path.corners;
            currentCorner = 0;
            Debug.Log("SI calculo un path");
            return NodeState.Success;
        }
        Debug.Log("no calculo ningun path");
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
