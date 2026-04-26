using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAvoidance : MonoBehaviour
{
    Transform _entity;
    float _radius;
    float _angle;
    float _personalArea;
    LayerMask _obsMask;
    Collider[] _colls;
    public ObstacleAvoidance(Transform entity, float radius, float angle, float personalArea, LayerMask obsMask, int countMaxObs = 5)
    {
        _entity = entity;
        _radius = radius;
        //  Eliminado el Mathf.Min que rompía el radio
        _angle = angle;
        _obsMask = obsMask;
        _colls = new Collider[countMaxObs];
        _personalArea = personalArea;
    }

    public Vector3 GetDir(Vector3 currentSpeed)
    {
        int count = Physics.OverlapSphereNonAlloc(_entity.position, _radius, _colls, _obsMask);
        Collider nearColl = null;
        float nearCollDistance = 0;
        Vector3 nearClosestPoint = Vector3.zero;

        for (int i = 0; i < count; i++)
        {
            var currColl = _colls[i];
            Vector3 closestPoint = currColl.ClosestPoint(_entity.position);
            closestPoint.y = _entity.position.y;
            Vector3 dirToColl = closestPoint - _entity.position;
            float distance = dirToColl.magnitude;
            float currAngle = Vector3.Angle(dirToColl, currentSpeed);

            // detecta obstaculos muy cercanos aunque esten fuera del cono
            bool inCone = currAngle <= _angle / 2f;
            bool tooClose = distance < _personalArea * 1.5f;

            if (!inCone && !tooClose) continue;

            if (nearColl == null || distance < nearCollDistance)
            {
                nearColl = currColl;
                nearCollDistance = distance;
                nearClosestPoint = closestPoint;
            }
        }

        if (nearColl == null)
            return currentSpeed;

        Vector3 relativePos = _entity.InverseTransformPoint(nearClosestPoint);
        Vector3 dirToClosestPoint = (nearClosestPoint - _entity.position).normalized;

        Vector3 avoidDir;
        if (relativePos.x < 0)
            avoidDir = Vector3.Cross(_entity.up, dirToClosestPoint);
        else
            avoidDir = -Vector3.Cross(_entity.up, dirToClosestPoint);

        float weight = (_radius - Mathf.Clamp(nearCollDistance - _personalArea, 0, _radius)) / _radius;

        // normalizado: evita que el agente se frene al lerp entre vectores
        return Vector3.Lerp(currentSpeed, avoidDir, weight).normalized;
    }
}
    

