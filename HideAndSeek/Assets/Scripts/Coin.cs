using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private Light light;
    private void Update()
    {
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            light.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
