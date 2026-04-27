using System.Collections;
using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    public float range;
    public float angle;
    public LayerMask obsMask;
    private Collider[] _hits = new Collider[6];
    public bool CheckRange(Transform target) //distancia
    {
        float distanceToTarget = (target.position - transform.position).sqrMagnitude;
        return distanceToTarget <= range * range;
    }
    public bool CheckAngle(Transform target) // angulo
    {
        Vector3 dirToTarget = target.position - transform.position;
        float angleToTarget = Vector3.Angle(dirToTarget, transform.forward);
        return angleToTarget <= angle / 2;
    }
    public bool CheckView(Transform target) // vision
    {
        Vector3 dirToTarget = target.position - transform.position;
        return !Physics.Raycast(transform.position, dirToTarget.normalized, dirToTarget.magnitude, obsMask);
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
