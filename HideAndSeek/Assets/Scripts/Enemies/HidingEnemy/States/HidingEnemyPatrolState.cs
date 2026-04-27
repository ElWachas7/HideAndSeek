using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.GridLayoutGroup;

public class HidingEnemyPatrolState : State<EntityStates>
{
    private HidingEnemy _entity;
    private ObstacleAvoidance obstacleAvoidance;
    private Transform newHidingSpot;

    // nav mesh
    private NavMeshPath path;
    private Vector3[] corners;
    private int currentCorner = 0;

    private float timer = 0f;

    public HidingEnemyPatrolState(HidingEnemy entity, StateMachine<EntityStates> sm, ObstacleAvoidance obsAvoidance) : base(sm)
    {
        _entity = entity;
        obstacleAvoidance = obsAvoidance;

        path = new NavMeshPath();
    }
    public override void Awake()
    {
        //base.Awake();
        Debug.Log("ejecuta awake de patrol");
        SetNewHidingSpot();
    }
    public override void Execute()
    {
        //base.Execute();
        if (newHidingSpot != null)
            Patrol();
    }
    private void SetNewHidingSpot()
    {
        newHidingSpot = null;
        newHidingSpot = GameManager.Instance.GetHidingSpot().Transform;
        Debug.Log("set new hiding spot");
        if (newHidingSpot == null)
        {
            // no hay spots disponibles, volver a idle
            _sm.ChangeState(EntityStates.Idle);
            return;
        }
        // se calcula una ruta desde nuestra pos hasta el hiding spot
        if (NavMesh.CalculatePath(_entity.transform.position, newHidingSpot.position, NavMesh.AllAreas, path))
        {
            corners = path.corners; // extraer vertices del path
            currentCorner = 0;
        }
        else
        {
            Debug.LogWarning("no se pudo calcular un path al hiding spot");
            _sm.ChangeState(EntityStates.Idle);
        }

    }
    private void Patrol()
    {
        if (corners == null || corners.Length == 0 || currentCorner >= corners.Length) return;

        Vector3 targetWaypoint; // apunta a la corner actual del nav mesh

        if (currentCorner < corners.Length)
        {
            targetWaypoint = corners[currentCorner];
        }
        else
        {
            targetWaypoint = newHidingSpot.position;
        }

        Vector3 dirToWaypoint = (targetWaypoint - _entity.transform.position).normalized; // direccion normalizada hacia el waypoint actual

        Vector3 moveDir = obstacleAvoidance.GetDir(dirToWaypoint); // ObstacleAvoidance puede redirigir el movimiento si hay obstaculos
        //Vector3 moveDir = dirToWaypoint;


        if (moveDir != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(moveDir);
            _entity.transform.rotation = Quaternion.Slerp(_entity.transform.rotation, rotation, 5f * Time.deltaTime);
        }

        _entity.transform.position += moveDir * _entity.Speed * Time.deltaTime; // moverse en la direccion (posiblemente corregida)

        if (Vector3.Distance(_entity.transform.position, targetWaypoint) <= 2f)
        {
            currentCorner++;
            if (currentCorner >= corners.Length)
            {
                _sm.ChangeState(EntityStates.Idle);
            }
        }
    }
}

//private void Patrol()
//{
//    if (corners == null || corners.Length == 0 || currentCorner >= corners.Length) return;

//    Vector3 targetWaypoint = newHidingSpot.Transform.position;

//    Vector3 dirToWaypoint = (targetWaypoint - _entity.transform.position).normalized; // direccion normalizada hacia el waypoint actual

//    Vector3 moveDir = obstacleAvoidance.GetDir(dirToWaypoint); // ObstacleAvoidance puede redirigir el movimiento si hay obstaculos

//    Quaternion rotation = Quaternion.LookRotation(moveDir);
//    _entity.transform.rotation = Quaternion.Slerp(_entity.transform.rotation, rotation, 5f * Time.deltaTime);

//    //_entity.transform.position += _entity.transform.forward * _entity.Speed * Time.deltaTime;

//    _entity.transform.position += moveDir * _entity.Speed * Time.deltaTime; // moverse en la direccion (posiblemente corregida)

//    if (Vector3.Distance(_entity.transform.position, targetWaypoint) <= 0.5f)
//    {
//        newHidingSpot = GameManager.Instance.GetHidingSpot();
//        //currentWP = (currentWP + 1) % _entity.WayPoints.Length;
//        _sm.ChangeState(EntityStates.Idle);
//    }
//}