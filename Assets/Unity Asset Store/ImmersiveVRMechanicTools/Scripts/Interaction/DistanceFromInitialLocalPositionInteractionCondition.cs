using ReliableSolutions.Unity.Common.PropertyDrawer;
using UnityEngine;

public class DistanceFromInitialLocalPositionInteractionCondition: InteractionCondition
{
    [SerializeField] private float DistanceToConsiderSatisfied = 0.2f;

    [SerializeField] private bool IsDebug;

    [SerializeField] [ShowIf(nameof(IsDebug))] private Transform _transformDistanceReference;
    private Transform TransformDistanceReference
    {
        get
        {
            InitializeTransformDistanceReference();
            return _transformDistanceReference;
        }
    }

    void Start()
    {
        InitializeTransformDistanceReference();
    }

    public override bool IsSatisfied()
    {
        return Mathf.Abs(Vector3.Distance(transform.position, TransformDistanceReference.position)) > DistanceToConsiderSatisfied;
    }

    void OnDrawGizmos()
    {
        if (IsDebug)
        {
            Gizmos.DrawSphere(_transformDistanceReference != null ? _transformDistanceReference.position : transform.position, DistanceToConsiderSatisfied);
        }
    }

    private void InitializeTransformDistanceReference()
    {
        if (_transformDistanceReference == null)
        {
            _transformDistanceReference = new GameObject(name + "-DistanceReference").transform;
            if (transform.parent != null) _transformDistanceReference.parent = transform.parent;
            _transformDistanceReference.position = transform.position;
        }
    }
}