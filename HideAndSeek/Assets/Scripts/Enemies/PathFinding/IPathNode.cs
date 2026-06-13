using System.Collections.Generic;
using UnityEngine;

public interface IPathNode
{
    Vector3 Position { get; }
    IReadOnlyList<IPathNode> Neighbors { get; }
}
