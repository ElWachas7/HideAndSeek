
using System.Collections.Generic;
using UnityEngine;
using static Nodes;

[RequireComponent(typeof(LineOfSight))]
public class ChaseEnemy : MonoBehaviour
{
    private LineOfSight _los;
    [Header("Stats")]
    public float stamina;
    public float speed;
    public Transform target;
    [Header("Patrol")]
    public List<Transform> patrolRoute;
    private bool hasPatrolRoute;
    [SerializeField] private float _cooldown;
    private QuestionNode root;
    void Start()
    {
        
        ActionNode idle = new ActionNode(Idle);
        ActionNode patrol = new ActionNode(Patrol);
        ActionNode chase = new ActionNode(Chase);
        ActionNode moveToPosition = new ActionNode(MoveToLastPosition);
        ActionNode getPatrolRoute = new ActionNode(GetPatrolRoute);

        var goPatrol = new SequenceNode(new List<ITreeNode>());
        goPatrol.Add(patrol);
        goPatrol.Add(idle);

        QuestionNode hasPatrolRoute = new QuestionNode(HasAPatrolRoute,goPatrol,getPatrolRoute);
        QuestionNode knownLastPosition = new QuestionNode(KnownLastPosition, moveToPosition, getPatrolRoute);
        QuestionNode isInLos = new QuestionNode(IsInLos, chase, knownLastPosition);

        root = isInLos;

    }
    void Update()
    {
        root.Execute();
    }

    // ---- QUESTION NODES ----
    private bool IsInLos() => _los.CheckRange(target) && _los.CheckAngle(target) && _los.CheckView(target);
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
        float currentTime = _cooldown;
        while (currentTime <= 0) 
        {
            currentTime -= Time.deltaTime;
            Lerp(transform.rotation.x, )

        }
        return NodeState.Success;
    }
    private NodeState Chase() 
    {
        var dir = target.position - transform.position;
        transform.position += dir.normalized * speed * Time.deltaTime;
        stamina -= Time.deltaTime;
        return NodeState.Success;
        //Aplicar Seering Behaviour

    }
    private NodeState GetPatrolRoute() 
    {
        //LISTA DE PUNTOS = GameManager.instance.GetPatrolRoute();
        return NodeState.Success;
    }
    
    private NodeState MoveToLastPosition()
    {
        return NodeState.Success;
    }
    private NodeState Patrol()
    {
        if (hasPatrolRoute)
            return NodeState.Failure;

        Transform currentTarget = patrolRoute[0];
        Vector3 dir = currentTarget.position - transform.position;
        float dist = dir.magnitude;

        if (dist <= 0.5f)
        {
            patrolRoute.RemoveAt(0);
            return NodeState.Success;
        }
        else
        {
            transform.position += dir.normalized * speed * Time.deltaTime;
        }

        return NodeState.Running;
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
