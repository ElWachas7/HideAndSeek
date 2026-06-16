using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private Light light;
    private void Start()
    {
        GameManager.Instance.RegisterCoin();
    }
    private void Update()
    {
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.UnregisterCoin();
            light.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
