using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HidingEnemyPatrolState : State<EntityStates>
{
    private HidingEnemy _entity;
    private ObstacleAvoidance obstacleAvoidance;
    private Transform newHidingSpot;

    // nav mesh
    private NavMeshPath path;
    private Vector3[] corners;
    private int currentCorner = 0;

    private System.Action _OnTargetSpotted;

    public HidingEnemyPatrolState(HidingEnemy entity, StateMachine<EntityStates> sm, ObstacleAvoidance obsAvoidance) : base(sm)
    {
        _entity = entity;
        obstacleAvoidance = obsAvoidance;

        path = new NavMeshPath();
    }
    public override void Awake()
    {
        base.Awake();
        SetNewHidingSpot();
        _OnTargetSpotted = () => stateMachine.ChangeState(EntityStates.Flee); // guardo evento de flee en variable 
                                                                              // para poder desuscribirse luego 
        _entity.OnTargetSpotted += _OnTargetSpotted; // suscribo a evento de flee
    }
    public override void Execute()
    {
        base.Execute();
        if (newHidingSpot != null)
            Patrol();
    }
    public override void Sleep()
    {
        base.Sleep();
        _entity.OnTargetSpotted -= _OnTargetSpotted; // desuscribo a evento de flee
    }
    private void SetNewHidingSpot()
    {
        newHidingSpot = GameManager.Instance.GetHidingSpot().Transform; // le pide una nueva hiding spot al GameManager

        if (NavMesh.CalculatePath(_entity.transform.position, newHidingSpot.position, NavMesh.AllAreas, path))
        {   // calcula el camino mas corto navegable entre la pos actual del hiding enemy y el hiding spot
            // utilizando cualquier superficie del nav mesh, y el resultado se guarda en path
            corners = path.corners; // los corners son los puntos del camino que hay que recorrer en orden
            currentCorner = 0; // empezar desde el primer punto del camino
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
                stateMachine.ChangeState(EntityStates.Idle);
            }
        }
    }
}