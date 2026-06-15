using UnityEngine;

[CreateAssetMenu(menuName = "Flocking/Behaviour/Alignment")]
public class AlignmentBehaviour : BoidBehaviour
{
    public override Vector3 CalculateForce(Boid boid, Collider[] boidsInRange, float radius)
    {
        Vector3 avgVelocity = Vector3.zero;
        int cont = 0;

        for (int i = 0; i < boidsInRange.Length; i++)
        {
            var currentBoid = boidsInRange[i].GetComponent<Boid>();
            if (currentBoid == null || currentBoid == boid) continue;

            avgVelocity += currentBoid.transform.forward;
            cont++;
        }

        if (cont == 0) return Vector3.zero;

        return boid.CalculateSteering(avgVelocity.normalized * boid.MaxSpeed);
    }
}
