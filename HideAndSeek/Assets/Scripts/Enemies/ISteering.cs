using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISteering
{
    public void Kill();

    Vector3 Velocity { get; }

    Transform transform { get; }
}
