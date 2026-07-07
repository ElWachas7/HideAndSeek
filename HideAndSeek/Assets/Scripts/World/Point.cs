using UnityEngine;
public class Point : MonoBehaviour
{
    [SerializeField] private float _chance;
    private Vector3 _position;
    public Vector3 Position => _position;
    public void Awake()
    {
        _position = transform.position;
        GameManager.Instance.AddPoint(_position, _chance);
        //Destroy(gameObject);
    }
}
