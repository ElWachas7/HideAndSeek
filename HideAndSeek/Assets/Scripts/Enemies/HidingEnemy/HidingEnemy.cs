using UnityEngine;

public enum EntityStates
{
    Idle,
    Patrol,
    Flee
}

[DefaultExecutionOrder(-500)]
public class HidingEnemy : MonoBehaviour, ISteering
{
    [Header("Movement")]
    [SerializeField] ChaseEnemy target;
    [SerializeField] float speed;
    [SerializeField] private LineOfSight viewLos;

    [Header("ObstacleAvoidance")]
    [SerializeField] private float obsRadius;
    [SerializeField] private float obsAngle;
    [SerializeField] private float obsPersonalArea;
    [SerializeField] private LayerMask obsMask;

    [Header("Theta")]
    [SerializeField] private LayerMask _nodeLayer;
    [SerializeField] private LayerMask _wallLayer;
    public LayerMask NodeLayer => _nodeLayer;
    public LayerMask WallLayer => _wallLayer;

    [Header("Steering")]
    Vector3 velocity;
    public Vector3 Velocity => velocity;
    public ChaseEnemy Target => target;
    public float Speed => speed;


    private StateMachine<EntityStates> _sm;
    private bool hasSeenTarget;
    private float losTimer = 0f;
    public bool HasSeenTarget => hasSeenTarget;

    public event System.Action OnTargetSpotted;

    void Start()
    {
        GameManager.Instance.RegisterEnemy();
        viewLos = GetComponent<LineOfSight>();
        _sm = new StateMachine<EntityStates>();
        var obstacleAvoidance = new ObstacleAvoidance(transform, obsRadius, obsAngle, obsPersonalArea, obsMask);

        var idle = new HidingEnemyIdleState(this, _sm);
        var patrol = new HidingEnemyPatrolState(this, _sm, obstacleAvoidance);
        var flee = new HidingEnemyFleeState(this, _sm, obstacleAvoidance);

        idle.AddTransition(patrol, EntityStates.Patrol); // agregar todas las transiciones posibles (y deseadas) al diccionario de transiciones
        idle.AddTransition(flee, EntityStates.Flee);
        patrol.AddTransition(idle, EntityStates.Idle);
        patrol.AddTransition(flee, EntityStates.Flee);
        flee.AddTransition(idle, EntityStates.Idle);

        _sm.AddState(idle, EntityStates.Idle); // agregar los estados a utilizar al diccionario de estados
        _sm.AddState(patrol, EntityStates.Patrol);
        _sm.AddState(flee, EntityStates.Flee);

        _sm.SetCurrent(patrol); // settear el estado con el que comenzara el juego el hiding enemy
    }
    void Update()
    {
        if (_sm.CurrentState != null)
        {
            _sm.Update();
        }
        if (IsTargetOnLOS() && !hasSeenTarget)
        {
            hasSeenTarget = true; // activa el bool para evitar que se ejecute miles de veces 
            losTimer = 0f;
            OnTargetSpotted?.Invoke(); // invoca evento de Flee
        }
        if (hasSeenTarget) // una vez que vio al Chasing enemy, arranca el timer de duracion del flee
        {
            losTimer += Time.deltaTime;
            if (losTimer > 4f)
            {
                hasSeenTarget = false;
                losTimer = 0f;
            }
        }
    }

    private bool IsTargetOnLOS() // chequea si en el Line of sight del enemigo (en sus 3 variables) logra detectar al Chasing enemy
    {
        if(target == null) 
        {
            Debug.Log($"{gameObject.name} no tiene referencia al targer");
        }
        if (viewLos.CheckRange(target.transform) && viewLos.CheckAngle(target.transform) && viewLos.CheckView(target.transform))
        {
            return true;
        }
        return false;
    }

    private void OnDrawGizmos() // Gizmos para poder visualizar en el editor los tama�os de las variables de obstacle avoidance
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, obsRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, obsPersonalArea);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, obsAngle / 2, 0) * transform.forward * obsRadius);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -obsAngle / 2, 0) * transform.forward * obsRadius);
    }

    public void Kill() // cuando el enemigo colisiona con el, es destruido
    {
        GameManager.Instance.UnregisterEnemy();
        Destroy(gameObject);
    }

}
