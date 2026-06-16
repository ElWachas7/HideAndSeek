using UnityEngine;

[CreateAssetMenu(menuName = "Flocking/Behaviour/Separation")]
public class SeparationBehaviour : BoidBehaviour // separacion
{
    public override Vector3 CalculateForce(Boid boid, Collider[] boidsInRange, float radius)
    {
        Vector3 totalForce = Vector3.zero;
        int cont = 0; // contador de vecinos validos detectados en rango

        for (int i = 0; i < boidsInRange.Length; i++) // recorre la lista de boids en radius
        {
            var currentCollider = boidsInRange[i];
            if (currentCollider == boid.MyCollider) continue; // si el collider es de si mismo, ignorar

            var direction = boid.MyPosition - currentCollider.transform.position; // vector de la distancia entre vecino y si mismo
            
            float distance = direction.magnitude;
            if (distance == 0) distance = 0.1f; // evitamos division por cero si estan exactamente en la misma posicion

            var force = direction.normalized / (distance / radius); // la fuerza depende de que tan cerca este el vecino
                                                                    // si esta muy cerca, le da fuerte
            totalForce += force;
            cont++;
        }

        if (cont == 0) return Vector3.zero;

        totalForce /= cont; // saca promedio de fuerza
        return boid.CalculateSteering(totalForce * boid.MaxSpeed); // moverse a esa posicion
    }
}
