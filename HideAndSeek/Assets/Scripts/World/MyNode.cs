using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class MyNode : MonoBehaviour, IPathNode
{
    #region Variables
    private List<IPathNode> _neighborsList;
    private readonly HashSet<IPathNode> _neighborsSet = new();
    [SerializeField] private string _nodeName;
    [SerializeField] private float _radius = 15;
    [SerializeField] private int _maxNeighbors = 8;
    private Collider[] _overlapBuffer;
    [SerializeField] private LayerMask _wallLayer;
    [SerializeField] private LayerMask _nodeLayer;
    public float chance;
    #endregion

    #region Gets
    [Tooltip("start with 10")]
    private Vector3 _cachedPosition;
    public string NodeName => _nodeName;
    public Vector3 Position => _cachedPosition;
    public IReadOnlyList<IPathNode> Neighbors => _neighborsList;
    #endregion

    #region MagicMethods
    private void Awake()
    {
        _cachedPosition = transform.position;
        _overlapBuffer = new Collider[_maxNeighbors];
    }
    private void Start()
    {
        GenerateNeighbors();
        _neighborsList = new List<IPathNode>(_neighborsSet);
        _neighborsSet.Clear();
        _neighborsSet.TrimExcess();
        _overlapBuffer = null;
    }
    #endregion

    private void GenerateNeighbors()
    {
        int count = Physics.OverlapSphereNonAlloc(_cachedPosition, _radius, _overlapBuffer, _nodeLayer, QueryTriggerInteraction.Collide);

        for (int i = 0; i < count; i++)
        {
            if (!_overlapBuffer[i].TryGetComponent(out IPathNode node))
            {
                continue;
            }
            if (node == (IPathNode)this)
            {
                continue;
            }

            Vector3 direction = node.Position - _cachedPosition;
            float distance = direction.magnitude;

            if (Physics.Raycast(_cachedPosition, direction / distance, distance, _wallLayer))
            {
                continue;
            }
            _neighborsSet.Add(node);
        }
    }

    #region Gizmos
    public void OnDrawGizmos()
    {
        if (_neighborsList == null) return;
        Color color = Color.green;
        color.a = 0.5f;
        Gizmos.color = color;
        foreach (IPathNode node in _neighborsList)
        {
            Gizmos.DrawLine(_cachedPosition, node.Position);
        }
    }
    public void OnDrawGizmosSelected()
    {
        Color color = Color.red;
        color.a = 0.5f;
        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
    #endregion
}