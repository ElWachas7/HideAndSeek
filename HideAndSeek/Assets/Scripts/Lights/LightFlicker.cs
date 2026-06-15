using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    private Light _light;
    [SerializeField] public Renderer renderer;
    private Material _material;

    [Header("Light")]
    [SerializeField] private float minIntensity = 0.2f;
    [SerializeField] private float maxIntensity = 2.5f;

    [Header("Flicker")]
    [SerializeField] private float flickerSpeed = 0.1f;

    [Header("Emission")]
    [SerializeField] private bool flickEmission = true;

    private readonly Color emissionColor = new Color(1f, 0.792f, 0.6f);


    private void Start()
    {
        if (flickEmission)
        {
            _light = GetComponent<Light>();
            if (renderer != null)
            {
                _material = renderer.material;
                _material.EnableKeyword("_EMISSION");
            }
            InvokeRepeating(nameof(Flicker), 0f, flickerSpeed);
        }
    }

    private void Flicker()
    {
        float intensity = Random.Range(minIntensity, maxIntensity);

        if (_light != null)
            _light.intensity = intensity;

        if (_material != null)
        {
            _material.SetColor("_EmissionColor", emissionColor * intensity / 1.3f);
        }
    }
}