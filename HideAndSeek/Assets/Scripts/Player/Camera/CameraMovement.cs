using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform camTarget;
    public float posSpeed = .02f;
    public float rotSpeed = .05f;

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position,camTarget.position, posSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation,camTarget.rotation, rotSpeed);
    }
}
