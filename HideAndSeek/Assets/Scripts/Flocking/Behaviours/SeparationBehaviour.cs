using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flocking/Behavior/Separation")]
public class SeparationBehaviour : BoidBehaviour
{
    public override Vector3 CalculateForce(Boid boid, Collider[] boidsInRange, float radius)
    {
        Vector3 totalForce = Vector3.zero;
        int cont = 0;

        for (int i = 0; i < boidsInRange.Length; i++)
        {
            var currentCollider = boidsInRange[i];
            if (currentCollider == boid.MyCollider) continue;

            var direction = boid.MyPosition - currentCollider.transform.position;

            // Evitamos división por cero si están exactamente en la misma posición
            float distance = direction.magnitude;
            if (distance == 0) distance = 0.1f;

            var force = direction.normalized / (distance / radius);

            totalForce += force;
            cont++;
        }

        if (cont == 0) return Vector3.zero;

        totalForce /= cont;
        return boid.CalculateSteering(totalForce * boid.MaxSpeed);
    }
}
