using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAvoidance : MonoBehaviour
{
    Transform _entity; // transform de la entidad que usa este obstacle avoidance
    float _radius; // radio del cono de deteccion de obstaculos
    float _angle; // angulo del cono de deteccion
    float _personalArea; // zona de su area personal, detecta obstaculos en cualquier direccion
    LayerMask _obsMask; // layer de los obstaculos
    Collider[] _colls; // array para guardar los colliders detectados
    public ObstacleAvoidance(Transform entity, float radius, float angle, float personalArea, LayerMask obsMask, int countMaxObs = 5)
    {
        _entity = entity;
        _radius = radius;
        _angle = angle;
        _obsMask = obsMask;
        _colls = new Collider[countMaxObs]; // espacio de memoria reservado para un maximo de obstaculos a detecctar dentro del radio
        _personalArea = personalArea;
    }

    public Vector3 GetDir(Vector3 currentSpeed)
    {
        int count = Physics.OverlapSphereNonAlloc(_entity.position, _radius, _colls, _obsMask); // guarda la cantidad de cosas en la obsMask dentro del radio del enemigo
        Collider nearColl = null;
        float nearCollDistance = 0;
        Vector3 nearClosestPoint = Vector3.zero;

        for (int i = 0; i < count; i++)
        {
            var currColl = _colls[i];
            Vector3 closestPoint = currColl.ClosestPoint(_entity.position); // punto del collider mas cerca a enemy
            closestPoint.y = _entity.position.y;
            Vector3 dirToColl = closestPoint - _entity.position; // direccion hacia el obstaculo
            float distance = dirToColl.magnitude; // distancia al obstaculo
            float currAngle = Vector3.Angle(dirToColl, currentSpeed); // angulo entre el obstaculo y la direccion actual

            // detecta obstaculos muy cercanos aunque esten fuera del cono
            bool inCone = currAngle <= _angle / 2f; // el obstaculo esta dentro del cono
            bool tooClose = distance < _personalArea * 1.5f; // el obstaculo esta dentro del espacio personal aunque este fuera del cono

            if (!inCone && !tooClose) continue; // ignorar obstaculos que no cumplan ninguna condicion

            if (nearColl == null || distance < nearCollDistance) // quedarse con el obstaculo mas dcercano
            {
                nearColl = currColl;
                nearCollDistance = distance;
                nearClosestPoint = closestPoint;
            }
        }

        if (nearColl == null) // no hay obstaculos, seguir en la misma direccion
            return currentSpeed;

        Vector3 relativePos = _entity.InverseTransformPoint(nearClosestPoint);
        Vector3 dirToClosestPoint = (nearClosestPoint - _entity.position).normalized;

        Vector3 avoidDir;
        if (relativePos.x < 0)
            avoidDir = Vector3.Cross(_entity.up, dirToClosestPoint); // obstaculo a la izquierda, esquivar a la derecha
        else
            avoidDir = -Vector3.Cross(_entity.up, dirToClosestPoint); // obstaculo a la derecha, esquivar a la izquierda
        
        float weight = (_radius - Mathf.Clamp(nearCollDistance - _personalArea, 0, _radius)) / _radius; // weight aumenta cuanto mas cerca esta el obstaculo

        // normalizado: evita que el agente se frene al lerp entre vectores
        return Vector3.Lerp(currentSpeed, avoidDir, weight).normalized;
    }
}
    

