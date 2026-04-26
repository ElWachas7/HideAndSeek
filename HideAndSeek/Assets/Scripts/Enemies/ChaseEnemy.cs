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
    Transform target;
    [Header("Stats")]
    public float speed;
    public Transform currentTarget;
    public bool HasSeenEnemy;

    [Header("Patrol")]
    public List<Transform> patrolRoute;
    private bool hasPatrolRoute;
    private float idleTimer = 0f;
    [SerializeField] float idleDuration = 2f;
    private QuestionNode root;


    void Start()
    {

        agent.updatePosition = false;
        agent.updateRotation = false;

        ActionNode idle = new ActionNode(Idle);
        ActionNode getPatrolRoute = new ActionNode(GetPatrolRoute);
        ActionNode moveToTarget = new ActionNode(() => MoveToTarget(currentTarget));
        ActionNode calculatePath = new ActionNode(() => CalculatePath(currentTarget));

        SequenceNode goChase = new SequenceNode(new List<ITreeNode>());
        goChase.Add(calculatePath);
        goChase.Add(moveToTarget);

        SequenceNode goPatrol = new SequenceNode(new List<ITreeNode>());
        goPatrol.Add(calculatePath);
        goPatrol.Add(idle);

        QuestionNode hasPatrolRoute = new QuestionNode(HasAPatrolRoute,goPatrol,getPatrolRoute);
        QuestionNode knownLastPosition = new QuestionNode(KnownLastPosition, moveToTarget, getPatrolRoute);
        QuestionNode isInLos = new QuestionNode(IsInLos, goChase, knownLastPosition);

        root = isInLos;

    }
    void Update()
    {
        root.Execute();
    }

    // ---- QUESTION NODES ----
    private bool IsInLos() => _los.CheckRange(target) && _los.CheckAngle(target) && _los.CheckView(target) || HasSeenEnemy;
    private bool KnownLastPosition()
    {
        return true;
    }
    private bool HasAPatrolRoute() 
    {
        //Get a patrol Route
        return true;
    }

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
        return NodeState.Success;
    }
    private NodeState MoveToTarget(Transform target)
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
    private NodeState CalculatePath(Transform target)
    {
        if (target == null) return NodeState.Failure;

        NavMeshPath path = new NavMeshPath();   

        if (NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path))
        {
            /*
            if (path.corners.Length == 0)
                return NodeState.Failure;*/

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
