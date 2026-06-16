using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WeightedBehaviour
{
    public BoidBehaviour behaviour;
    [Range(0f, 3f)] public float weight; // es la fuerza que tiene el comportamiento
    public float radius; // cada comportamiento puede tener su propio radio 
}

[CreateAssetMenu(menuName = "Flocking/Flock Configuration")]
public class Flockonfiguration : ScriptableObject
{
    public List<WeightedBehaviour> behaviours; 
}