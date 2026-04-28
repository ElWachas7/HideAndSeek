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


    public HidingEnemyPatrolState(HidingEnemy entity, StateMachine<EntityStates> sm, ObstacleAvoidance obsAvoidance) : base(sm)
    {
        _entity = entity;
        obstacleAvoidance = obsAvoidance;

        path = new NavMeshPath();
        _entity.OnTargetSpotted += () => _sm.ChangeState(EntityStates.Flee);
    }
    public override void Awake()
    {
        //base.Awake();
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