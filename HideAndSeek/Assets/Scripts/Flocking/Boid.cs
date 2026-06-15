using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : SteeringEntity
{
    [SerializeField] private Flockonfiguration flockConfig;
    [SerializeField] private LayerMask boidMask;

    private Transform myTransform;
    private Collider myCollider;

    private List<WeightedBehaviour> runtimeBehaviours;

    public Vector3 MyPosition => myTransform.position;
    public Collider MyCollider => myCollider;
    public float MaxSpeed => _maxSpeed;

    private void Awake()
    {
        myTransform = transform;
        myCollider = GetComponent<Collider>();
    }

    void Start()
    {
        // FIX 1: We clear flockConfig from inspector so it starts with NO active behaviors.
        flockConfig = null;
        runtimeBehaviours.Clear();

        // Give them an initial random push so they are moving from the start
        //AddForce(new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized * _maxSpeed);
    }

    void Update()
    {
        if (runtimeBehaviours != null && runtimeBehaviours.Count > 0)
        {
            Flocking();
        }
        Move();
    }

    private void Flocking()
    {
        Vector3 combinedForce = Vector3.zero;

        for (int i = 0; i < runtimeBehaviours.Count; i++)
        {
            var weightedBehavior = runtimeBehaviours[i];
            if (weightedBehavior.behaviour == null) continue;

            var boidsInRange = Physics.OverlapSphere(MyPosition, weightedBehavior.radius, boidMask);

            Vector3 force = weightedBehavior.behaviour.CalculateForce(this, boidsInRange, weightedBehavior.radius);
            combinedForce += force * weightedBehavior.weight;
        }

        AddForce(combinedForce);
    }

    // ====================================================================
    // FIX 2: Correctly overwrite the runtime list and teleport/reset if needed
    // ====================================================================
    public void SetRuntimeConfig(Flockonfiguration newConfig)
    {
        if (newConfig != null)
        {
            flockConfig = newConfig;

            // Critical: Update the actual list that runs in Update()
            runtimeBehaviours = new List<WeightedBehaviour>(flockConfig.behaviours);

            // Optional/Highly Recommended: If they are too far apart to see each other,
            // we redirect them towards the center of the world (0,0,0) so they group up again.
            Vector3 flatCenter = new Vector3(0f, myTransform.position.y, 0f);
            Vector3 centerDirection = (flatCenter - myTransform.position).normalized;

            AddForce(centerDirection * _maxSpeed);
        }
    }

    public Vector3 Seek(Vector3 target) => base.Seek(target);
    public Vector3 CalculateSteering(Vector3 target) => base.CalculateSteering(target);

    private void OnDrawGizmos()
    {
        if (flockConfig == null || flockConfig.behaviours == null) return;

        Gizmos.color = Color.green;
        foreach (var wb in flockConfig.behaviours)
        {
            if (wb.behaviour != null) Gizmos.DrawWireSphere(transform.position, wb.radius);
        }
    }
}


//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//public class Boid : SteeringEntity
//{
//    [SerializeField] private Flockonfiguration flockConfig;
//    [SerializeField] private LayerMask boidMask;

//    private Transform myTransform;
//    private Collider myCollider;

//    private List<WeightedBehaviour> runtimeBehaviours = new List<WeightedBehaviour>();
//    // Propiedades públicas para que los comportamientos accedan de forma segura
//    public Vector3 MyPosition => myTransform.position;
//    public Collider MyCollider => myCollider;
//    public float MaxSpeed => _maxSpeed; // Asumiendo que _maxSpeed viene de SteeringEntity

//    private void Awake()
//    {
//        myTransform = transform;
//        myCollider = GetComponent<Collider>();
//    }

//    void Start()
//    {
//        if (flockConfig != null)
//        {
//            runtimeBehaviours = new List<WeightedBehaviour>(flockConfig.behaviours);
//        }
//        AddForce(new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized * _maxSpeed);
//    }

//    void Update()
//    {
//        if (runtimeBehaviours != null && runtimeBehaviours.Count > 0)
//        {
//            Flocking();
//        }
//        Move();
//    }

//    private void Flocking()
//    {
//        Vector3 combinedForce = Vector3.zero;

//        // Iteramos por cada comportamiento que hayas arrastrado a la lista
//        for (int i = 0; i < runtimeBehaviours.Count; i++)
//        {
//            var weightedBehavior = runtimeBehaviours[i];
//            if (weightedBehavior.behaviour == null) continue;

//            // Optimizamos: Buscamos vecinos usando el radio específico de este comportamiento
//            var boidsInRange = Physics.OverlapSphere(MyPosition, weightedBehavior.radius, boidMask);

//            // Calculamos la fuerza delegándola al objeto de comportamiento y multiplicamos por su peso
//            Vector3 force = weightedBehavior.behaviour.CalculateForce(this, boidsInRange, weightedBehavior.radius);
//            combinedForce += force * weightedBehavior.weight;
//        }

//        AddForce(combinedForce);
//    }
//    public void SetRuntimeConfig(Flockonfiguration newConfig)
//    {
//        if (newConfig != null)
//        {
//            flockConfig = newConfig;
//        }
//    }

//    // Métodos puente por si tus comportamientos necesitan llamar funciones heredadas
//    public Vector3 Seek(Vector3 target) => base.Seek(target);
//    public Vector3 CalculateSteering(Vector3 target) => base.CalculateSteering(target);

//    private void OnDrawGizmos()
//    {
//        // Dibuja los radios de los comportamientos activos automáticamente
//        Gizmos.color = Color.green;
//        foreach (var wb in flockConfig.behaviours)
//        {
//            if (wb.behaviour != null) Gizmos.DrawWireSphere(transform.position, wb.radius);
//        }
//    }
//}

