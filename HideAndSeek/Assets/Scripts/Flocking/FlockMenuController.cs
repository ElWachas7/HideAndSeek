using UnityEngine;

public class FlockMenuController : MonoBehaviour
{
    [SerializeField] private Flockonfiguration normalFlockConfig;
    [SerializeField] private Flockonfiguration alignmentFlockConfig;
    [SerializeField] private Flockonfiguration cohesionFlockConfig;
    [SerializeField] private Flockonfiguration separationFlockConfig;

    private Boid[] activeBoids;

    private void Start()
    {
        activeBoids = FindObjectsByType<Boid>(FindObjectsSortMode.None);
    }
    public void ChangeToNormalFlock()
    {
        ApplyPresetToAllBoids(normalFlockConfig);
    }
    public void ChangeToAlignmentFlock()
    {
        ApplyPresetToAllBoids(alignmentFlockConfig);
    }
    public void ChangeToCohesionFlock()
    {
        ApplyPresetToAllBoids(cohesionFlockConfig);
    }
    public void ChangeToSeparationFlock()
    {
        ApplyPresetToAllBoids(separationFlockConfig);
    }

    private void ApplyPresetToAllBoids(Flockonfiguration selectedPreset)
    {
        if (selectedPreset == null) return;
        for (int i = 0; i < activeBoids.Length; i++)
        {
            if (activeBoids[i] != null)
            {
                activeBoids[i].SetRuntimeConfig(selectedPreset);
            }
        }
    }
}
