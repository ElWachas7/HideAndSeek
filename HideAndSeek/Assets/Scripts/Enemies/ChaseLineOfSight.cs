using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChaseLineOfSight : MonoBehaviour
{
    [Header("Vision")]
    public float range;
    public float angle;

    [Header("Layers")]
    public LayerMask targetMask; 
    public LayerMask obsMask;

    private Collider[] _hits = new Collider[20];
    public Transform GetVisibleTarget()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position,range,_hits,targetMask);
        Transform bestTarget = null;
        bool _distance = false;
        bool _angle = false;
        bool _view = false;

        for (int i = 0; i < count; i++)
        {
            Transform target = _hits[i].transform;

            float distanceToTarget = (target.position - transform.position).sqrMagnitude;
            _distance = distanceToTarget <= range * range;

            Vector3 dirToTarget = target.position - transform.position;
            float angleToTarget = Vector3.Angle(dirToTarget, transform.forward);
            _angle = angleToTarget <= angle / 2;

            _view = !Physics.Raycast(transform.position, dirToTarget.normalized, dirToTarget.magnitude, obsMask);

            if (_distance && _angle && _view)
            {
                bestTarget = target;
            }
        }
        return bestTarget;
    }

    public bool HasTarget(out Transform target)
    {
        target = GetVisibleTarget();
        return target != null;
    }

    private void OnDrawGizmos()
    {
        Color myColor = Color.blue;
        myColor.a = 0.5f;
        Gizmos.color = myColor;
        Gizmos.DrawWireSphere(transform.position, range);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, angle / 2, 0) * transform.forward * range);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -angle / 2, 0) * transform.forward * range);
    }
}
