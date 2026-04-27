using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISteering
{
    public void Kill();

    public Vector3 GetDir(Vector3 currentDirection);
}
