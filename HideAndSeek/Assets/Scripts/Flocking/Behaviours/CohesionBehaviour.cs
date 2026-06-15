using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flocking/Behavior/Cohesion")]
public class CohesionBehaviour : BoidBehaviour
{
    public override Vector3 CalculateForce(Boid boid, Collider[] boidsInRange, float radius)
    {
        Vector3 avgPosition = Vector3.zero;
        int cont = 0;

        for (int i = 0; i < boidsInRange.Length; i++)
        {
            var currentCollider = boidsInRange[i];
            if (currentCollider == boid.MyCollider) continue;

            avgPosition += currentCollider.transform.position;
            cont++;
        }

        if (cont == 0) return Vector3.zero;

        avgPosition /= cont;
        return boid.Seek(avgPosition);
    }
}
