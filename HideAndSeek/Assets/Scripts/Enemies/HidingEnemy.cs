using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityStates
{
    Idle,
    Patrol,
    Flee
}
public class HidingEnemy : MonoBehaviour, ISteering
{
    [Header("Movement")]
    [SerializeField] ChaseEnemy target;
    [SerializeField] Transform[] wayPoints;
    [SerializeField] float speed;
    [SerializeField] private LineOfSight viewLos;

    [Header("ObstacleAvoidance")]
    [SerializeField] private float obsRadius;
    [SerializeField] private float obsAngle;
    [SerializeField] private float obsPersonalArea;
    [SerializeField] private LayerMask obsMask;

    [Header("Steering")]
    Vector3 velocity;
    public Vector3 Velocity => velocity;



    public ChaseEnemy Target => target;
    public Transform[] WayPoints => wayPoints;
    public float Speed => speed;
    public LineOfSight ViewLos => viewLos;


    private StateMachine<EntityStates> _sm;

    private bool hasSeenTarget;
    private float losTimer = 0f;

    public bool HasSeenTarget => hasSeenTarget;

    public event System.Action OnTargetSpotted;

    void Start()
    {
        viewLos = GetComponent<LineOfSight>();
        _sm = new StateMachine<EntityStates>();
        var obstacleAvoidance = new ObstacleAvoidance(transform, obsRadius, obsAngle, obsPersonalArea, obsMask);

        var idle = new HidingEnemyIdleState(this, _sm);
        var patrol = new HidingEnemyPatrolState(this, _sm, obstacleAvoidance);
        var flee = new HidingEnemyFleeState(this, _sm, obstacleAvoidance);

        idle.AddTransition(patrol, EntityStates.Patrol);
        patrol.AddTransition(idle, EntityStates.Idle);
        patrol.AddTransition(flee, EntityStates.Flee);
        flee.AddTransition(idle, EntityStates.Idle);

        _sm.AddState(idle, EntityStates.Idle);
        _sm.AddState(patrol, EntityStates.Patrol);
        _sm.AddState(flee, EntityStates.Flee);

        _sm.SetCurrent(patrol);

    }
    void Update()
    {
        if (_sm.CurrentState != null)
        {
            _sm.Update();
        }
        if (IsTargetOnLOS() && !hasSeenTarget) // activa el bool para evitar que se ejecute miles de veces y arranca un contador de 10 segundos
        {
            hasSeenTarget = true;
            losTimer = 0f;
            OnTargetSpotted?.Invoke(); // invoca evento de Flee
        }
        if (hasSeenTarget)
        {
            losTimer += Time.deltaTime;
            if (losTimer > 4f)
            {
                hasSeenTarget = false;
                losTimer = 0f;
            }
        }  
}

    private bool IsTargetOnLOS()
    {
        if (viewLos.CheckRange(target.transform) && viewLos.CheckAngle(target.transform) && viewLos.CheckView(target.transform))
        {
            return true;
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Color myColor = Color.magenta;
        myColor.a = 0.5f;
        Gizmos.color = myColor;
        Gizmos.DrawWireSphere(transform.position, obsRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, obsPersonalArea);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, obsAngle / 2, 0) * transform.forward * obsRadius);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -obsAngle / 2, 0) * transform.forward * obsRadius);
    }

    public void Kill()
    {
        Destroy(gameObject);
    }

}
