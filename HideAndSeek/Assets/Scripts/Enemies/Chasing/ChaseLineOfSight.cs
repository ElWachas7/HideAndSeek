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

    private Collider[] _hits = new Collider[6];
    public ISteering GetVisibleTarget()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position,range,_hits,targetMask);
        ISteering bestTarget = null;

        bool _distance = false;
        bool _angle = false;
        bool _view = false;

        for (int i = 0; i < count; i++)
        {
            var hit = _hits[i];

            ISteering target = hit.GetComponent(typeof(ISteering)) as ISteering;
            if (target == null)
                continue;


            float distanceToTarget = (target.transform.position - transform.position).sqrMagnitude; // Chequeo de distancia
            _distance = distanceToTarget <= range * range;

            Vector3 dirToTarget = target.transform.position - transform.position;
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

    public bool HasTarget(out ISteering target)
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
