using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Boid : SteeringEntity
{
    [SerializeField] private Flockonfiguration flockConfig;
    [SerializeField] private LayerMask boidMask;

    private Transform myTransform;
    private Collider myCollider;

    // Propiedades públicas para que los comportamientos accedan de forma segura
    public Vector3 MyPosition => myTransform.position;
    public Collider MyCollider => myCollider;
    public float MaxSpeed => _maxSpeed; // Asumiendo que _maxSpeed viene de SteeringEntity

    private void Awake()
    {
        myTransform = transform;
        myCollider = GetComponent<Collider>();
    }

    void Start()
    {
        AddForce(new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized * _maxSpeed);
    }

    void Update()
    {
        if (flockConfig != null)
        {
            Flocking();
        }
        Move();
    }

    private void Flocking()
    {
        Vector3 combinedForce = Vector3.zero;

        // Iteramos por cada comportamiento que hayas arrastrado a la lista
        for (int i = 0; i < flockConfig.behaviors.Count; i++)
        {
            var weightedBehavior = flockConfig.behaviors[i];
            if (weightedBehavior.behaviour == null) continue;

            // Optimizamos: Buscamos vecinos usando el radio específico de este comportamiento
            var boidsInRange = Physics.OverlapSphere(MyPosition, weightedBehavior.radius, boidMask);

            // Calculamos la fuerza delegándola al objeto de comportamiento y multiplicamos por su peso
            Vector3 force = weightedBehavior.behaviour.CalculateForce(this, boidsInRange, weightedBehavior.radius);
            combinedForce += force * weightedBehavior.weight;
        }

        AddForce(combinedForce);
    }

    // Métodos puente por si tus comportamientos necesitan llamar funciones heredadas
    public Vector3 Seek(Vector3 target) => base.Seek(target);
    public Vector3 CalculateSteering(Vector3 target) => base.CalculateSteering(target);

    private void OnDrawGizmos()
    {
        if (flockConfig == null) return;

        // Dibuja los radios de los comportamientos activos automáticamente
        Gizmos.color = Color.green;
        foreach (var wb in flockConfig.behaviors)
        {
            if (wb.behaviour != null)
            {
                Gizmos.DrawWireSphere(transform.position, wb.radius);
            }
        }
    }
}
/*public class Boid : SteeringEntity
{
    [SerializeField] private float separationRadius;
    [SerializeField] private float cohesionRadius;

    [SerializeField, Range(0.0f, 3.0f)] private float separationWeight;
    [SerializeField, Range(0.0f, 3.0f)] private float cohesionWeight;
    [SerializeField, Range(0.0f, 3.0f)] private float alignmentWeight;

    [SerializeField] private LayerMask boidMask;

    private Transform myTransform;
    private Vector3 myPosition => myTransform.position;

    private void Awake()
    {
        myTransform = transform;
    }

    void Start()
    {
        AddForce(new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized * _maxSpeed);
    }
    void Update()
    {
        Flocking();
        Move();
    }

    private void Flocking() // se suman las tres fuerzas multiplicadas entre si y se aplican
    {
        AddForce(
        Separation() * separationWeight
        + 
        Cohesion() * cohesionWeight
        + 
        Alignment() * alignmentWeight
            );

    }

    private Vector3 Separation() // evita que se choquen
    {
        var boidsInRange = Physics.OverlapSphere(myPosition, separationRadius, boidMask); // detecta los colliders dentro de separationRadius en la boidMask
        Vector3 totalForce = Vector3.zero;
        int cont = 0;
        for (int i = 0; i < boidsInRange.Length; i++)
        {
            var currentBoid = boidsInRange[i];
            if (currentBoid == this.GetComponent<Collider>()) continue; // saltea su propio collider, para no alejarse de su propio collider

            var direction = myPosition - currentBoid.transform.position; // apunta del vecino hacia el current boid, osea la direccion para alejarse
            var force = direction.normalized / (direction.magnitude / separationRadius); // cuanto mas cerca esta el vecino, se aleja con mas fuerza

            totalForce += force;
            cont++;
        }
        if (cont == 0) return Vector3.zero;

        totalForce /= cont;

        return CalculateSteering(totalForce * _maxSpeed);

    }

    private Vector3 Cohesion() // hace que se queden todos juntos
    {
        var avgPosition = Vector3.zero;
        int cont = 0;
        var boidsInRange = Physics.OverlapSphere(myPosition, cohesionRadius, boidMask);

        for (int i = 0; i < boidsInRange.Length; i++)
        {
            var currentBoid = boidsInRange[i];
            if (currentBoid == this.GetComponent<Collider>()) continue;

            avgPosition += currentBoid.transform.position;
            cont++;
        }
        if (cont == 0) return Vector3.zero;

        avgPosition /= cont;
        return Seek(avgPosition);
    }

    private Vector3 Alignment() // hace que vayan todos en la misma direccion
    {
        var boidsInRange = Physics.OverlapSphere(myPosition, cohesionRadius, boidMask);
        Vector3 avgVelocity = Vector3.zero;
        int cont = 0;
        for (int i = 0; i < boidsInRange.Length; i++)
        {
            var currentBoid = boidsInRange[i].GetComponent<Boid>();
            if (currentBoid == this) continue;


            avgVelocity += currentBoid.transform.forward;
            cont++;
        }
        if (cont == 0) return Vector3.zero;

        return CalculateSteering(avgVelocity.normalized * _maxSpeed);
    }

    private void OnDrawGizmos() // dibuja gizmos para verificar visualmente el tamaño de cada radio
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, separationRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, cohesionRadius);

    }

}
*/

