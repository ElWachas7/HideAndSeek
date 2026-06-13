using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringEntity : MonoBehaviour
{
    [SerializeField] protected float _maxSpeed;
    [SerializeField] protected float _maxForce;
    [SerializeField] protected Vector3 _velocity;

    public Vector3 Velocity { get { return _velocity; } }

    protected Vector3 Seek(Vector3 target)
    {
        Vector3 desired = (target - transform.position).normalized;
        return CalculateSteering(desired);
    }

    protected Vector3 CalculateSteering(Vector3 desired) // calcula hacia dónde quiere ir
    {
        Vector3 steering = desired - _velocity;
        return Vector3.ClampMagnitude(steering, _maxForce * Time.deltaTime);
    } 

    protected void AddForce(Vector3 force) // suma fuerza a la velocidad actual
    {
        _velocity = Vector3.ClampMagnitude(_velocity + force, _maxSpeed);
    }

    protected void Move() // lo mueve utilizando la velocidad
    {
        if (_velocity == Vector3.zero) return;
        transform.forward = _velocity;
        transform.position += _velocity * Time.deltaTime;
    }
}

