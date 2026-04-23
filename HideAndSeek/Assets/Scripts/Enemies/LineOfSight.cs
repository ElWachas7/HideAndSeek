using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    public float range;
    public float angle;
    public LayerMask obsMask;
    public bool CheckRange(Transform target)
    {
        float distanceToTarget = (target.position - transform.position).sqrMagnitude; //Vector3.Distance(target.position, Origin);
        return distanceToTarget <= range * range;
    }
    public bool CheckAngle(Transform target)
    {
        //B-A
        Vector3 dirToTarget = target.position - transform.position;
        float angleToTarget = Vector3.Angle(dirToTarget, transform.forward);
        return angleToTarget <= angle / 2;
    }
    public bool CheckView(Transform target)
    {
        Vector3 dirToTarget = target.position - transform.position;
        return !Physics.Raycast(transform.position, dirToTarget.normalized, dirToTarget.magnitude, obsMask);
    }
    private void OnDrawGizmos()
    {
        Color myColor = Color.blue;
        myColor.a = 0.5f;
        Gizmos.color = myColor;
        Gizmos.DrawWireSphere(Origin, range);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(Origin, Quaternion.Euler(0, angle / 2, 0) * Forward * range);
        Gizmos.DrawRay(Origin, Quaternion.Euler(0, -angle / 2, 0) * Forward * range);
    }
}
