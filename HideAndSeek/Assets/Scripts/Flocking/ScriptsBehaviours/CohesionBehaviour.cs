using UnityEngine;

[CreateAssetMenu(menuName = "Flocking/Behaviour/Cohesion")]
public class CohesionBehaviour : BoidBehaviour // cohesion
{
    public override Vector3 CalculateForce(Boid boid, Collider[] boidsInRange, float radius)
    {
        Vector3 avgPosition = Vector3.zero;
        int cont = 0; // contador de vecinos validos detectados en rango

        for (int i = 0; i < boidsInRange.Length; i++) // recorre la lista de boids en radius
        {
            var currentCollider = boidsInRange[i];
            if (currentCollider == boid.MyCollider) continue; // si el collider es el de si mismo, ignorar

            avgPosition += currentCollider.transform.position;
            cont++;
        }

        if (cont == 0) return Vector3.zero;

        avgPosition /= cont; // saca promedio de posicion

        return boid.Seek(avgPosition); // moverse a la posicion deseada
    }
}
