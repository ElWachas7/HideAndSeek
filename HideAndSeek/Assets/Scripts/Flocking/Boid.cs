using System.Collections.Generic;
using UnityEngine;

public class Boid : SteeringEntity
{
    private Flockonfiguration flockConfig;// la config actual
    [SerializeField] private LayerMask boidMask;

    private Transform myTransform;
    private Collider myCollider;

    private List<WeightedBehaviour> runtimeBehaviours = new List<WeightedBehaviour>();

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
        flockConfig = null; // limpia el flockConfig asi no hay behaviours activos
        runtimeBehaviours.Clear(); // limpia la lista de behaviours
    }

    void Update()
    {
        if (runtimeBehaviours != null && runtimeBehaviours.Count > 0) // solo se activa el flocking si hay algun behaviour activo
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

    public void SetRuntimeConfig(Flockonfiguration newConfig)
    {
        if (newConfig != null)
        {
            flockConfig = newConfig; // sobreescribe el config actual con el nuevo

            // Critical: Update the actual list that runs in Update()
            runtimeBehaviours = new List<WeightedBehaviour>(flockConfig.behaviours);

            Vector3 flatCenter = new Vector3(0f, myTransform.position.y, 0f); // mantiene su pos en eje Y en 0
            // los envia al centro para que si se alejaron mucho entre si puedan reencontrarse y volver a formarse
            Vector3 centerDirection = (flatCenter - myTransform.position).normalized;
            AddForce(centerDirection * _maxSpeed);
        }
    }

    private void OnDrawGizmos() // gizmos para visualizar el tamaño de los radius
    {
        if (flockConfig == null || flockConfig.behaviours == null) return;

        Gizmos.color = Color.green;
        foreach (var wb in flockConfig.behaviours)
        {
            if (wb.behaviour != null) Gizmos.DrawWireSphere(transform.position, wb.radius);
        }
    }
}

