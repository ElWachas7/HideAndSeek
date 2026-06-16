using UnityEngine;

public class FlockMenuController : MonoBehaviour
{
    [SerializeField] private Flockonfiguration normalFlockConfig;      // se asignan cada una de las config en sus
    [SerializeField] private Flockonfiguration alignmentFlockConfig;   // respectivos Flockonfiguration
    [SerializeField] private Flockonfiguration cohesionFlockConfig;
    [SerializeField] private Flockonfiguration separationFlockConfig;

    private Boid[] activeBoids; // lista de boids en escena

    private void Start()
    {
        activeBoids = FindObjectsByType<Boid>(FindObjectsSortMode.None); // busca la ref de cada boid
    }
    public void ChangeToNormalFlock() // metodos para aplicar en botones UI para cambio de configs
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

    private void ApplyPresetToAllBoids(Flockonfiguration selectedPreset) // cambiar el config actual por el seleccionado
    {
        if (selectedPreset == null) return; // si es null, retorna para evitar errores
        for (int i = 0; i < activeBoids.Length; i++) // recorre la lista de los boids de la escena y cambia el config
        {                                            // y sus behaviours en run time para cada uno
            if (activeBoids[i] != null)
            {
                activeBoids[i].SetRuntimeConfig(selectedPreset);
            }
        }
    }
}
