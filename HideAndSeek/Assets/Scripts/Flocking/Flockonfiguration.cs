using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WeightedBehaviour
{
    public BoidBehaviour behaviour;
    [Range(0f, 3f)] public float weight;
    public float radius; // Cada comportamiento puede tener su propio radio ahora
}

[CreateAssetMenu(menuName = "Flocking/Flock Configuration")]
public class Flockonfiguration : ScriptableObject
{
    public List<WeightedBehaviour> behaviours;
}