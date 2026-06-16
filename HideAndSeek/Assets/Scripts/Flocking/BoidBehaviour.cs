using UnityEngine;

public abstract class BoidBehaviour : ScriptableObject 
{
    public abstract Vector3 CalculateForce(Boid boid, Collider[] boidsInRange, float radius);
}
