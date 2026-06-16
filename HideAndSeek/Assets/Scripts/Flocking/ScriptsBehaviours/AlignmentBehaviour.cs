using UnityEngine;

[CreateAssetMenu(menuName = "Flocking/Behaviour/Alignment")]
public class AlignmentBehaviour : BoidBehaviour // alineacion
{
    public override Vector3 CalculateForce(Boid boid, Collider[] boidsInRange, float radius)
    {
        Vector3 avgVelocity = Vector3.zero;
        int cont = 0; // contador de vecinos validos detectados en rango

        for (int i = 0; i < boidsInRange.Length; i++) // recorre la lista de boids en radius
        {
            var currentBoid = boidsInRange[i].GetComponent<Boid>();
            if (currentBoid == null || currentBoid == boid) continue; // si no hay boid o es el mismo boid, ignorar

            avgVelocity += currentBoid.transform.forward; // lo mueve para adelante
            cont++;
        }

        if (cont == 0) return Vector3.zero;

        avgVelocity /= cont; // saca promedio de direccion

        return boid.CalculateSteering(avgVelocity.normalized * boid.MaxSpeed); // moverse a esa posicion
    }
}
