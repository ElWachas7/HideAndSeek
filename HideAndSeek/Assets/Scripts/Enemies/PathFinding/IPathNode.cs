using System.Collections.Generic;
using UnityEngine;

public interface IPathNode
{
    string NodeName { get; }
    Vector3 Position { get; }
    IReadOnlyList<IPathNode> Neighbors { get; }
    bool HasLineOfSight(IPathNode target, LayerMask wallLayer);
}
