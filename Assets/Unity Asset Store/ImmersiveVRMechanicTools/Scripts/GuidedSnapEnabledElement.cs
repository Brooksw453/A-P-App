using System;
using System.Collections.Generic;
using ReliableSolutions.Unity.Common.Debug;
using ReliableSolutions.Unity.Common.Extensions;
using ReliableSolutions.Unity.Common.PropertyDrawer;
using UnityEngine;

public abstract class GuidedSnapEnabledElement : MonoBehaviour
{
    public event EventHandler Unsnapped;
    public event EventHandler<SnappedEventArgs> Snapped;
    public event EventHandler SnappedInFinishPosition;

    public event EventHandler ElementDriverRegistered;
    public event EventHandler ElementDriverUnregistered;

    [Header("Snap Element Options")]
    [SerializeField] protected Transform _snapRaycastOrigin;
    [SerializeField] private float _snapAreaWidth = 0.1f;
    [SerializeField] private XRFrameworkTransformControl _xrFrameworkTransformControl;

    [Header("Debug")]
    [SerializeField] protected bool IsDebug;
    [ShowIf(nameof(IsDebug))] [SerializeField] protected TransformPositionDriver _transformPositionDriver;
    [ShowIf(nameof(IsDebug))] [SerializeField] [ReadOnly] protected GuidedSnapTarget _currentlySnappedToTarget;

    [ShowIf(nameof(IsDebug))] [SerializeField] [ReadOnly] private TryAttachStatus _lastTryAttachStatus;
    [ShowIf(nameof(IsDebug))] [SerializeField] [ReadOnly] private Vector3 _lockedAtPosition = Vector3.zero;
    [ShowIf(nameof(IsDebug))] [SerializeField] [ReadOnly] protected Vector3 _initialLockedAtPosition = Vector3.positiveInfinity;
    [ShowIf(nameof(IsDebug))] [SerializeField] [ReadOnly] private Quaternion _rotationWhenLocked;
    [ShowIf(nameof(IsDebug))] [SerializeField] [ReadOnly] private bool _initialIsKinematicWhenLocked;
    [ShowIf(nameof(IsDebug))] [SerializeField] [ReadOnly] private float _currentAttachedSnapElementProgressAlongDrivePath;

    protected Rigidbody _rigidbody;
    public Vector3 InitialLockedAtPosition => _initialLockedAtPosition;
    private float SnapAreaWidth => _snapAreaWidth * transform.lossyScale.x;
    public Vector3 RaycastDirection => (_snapRaycastOrigin.transform.rotation * Vector3.down).normalized;
    //TODO: removed cached DistanceFromOrigin as that introduced other issues, eg tool not properly aligned, items not locked in correct place on scene start - 'jitter' needs to be sorted differently
    public Vector3 DistanceFromOrigin => SnapRaycastOrigin.transform.position - transform.position; public bool IsLockedAtPosition => LockedAtPosition != Vector3.zero && !float.IsInfinity(LockedAtPosition.x);
    public Transform SnapRaycastOrigin { get => _snapRaycastOrigin; private set => _snapRaycastOrigin = value; }
    public XRFrameworkTransformControl XRFrameworkTransformControl => _xrFrameworkTransformControl;
    public TransformPositionDriver TransformPositionDriver  {  get => _transformPositionDriver;  protected set => _transformPositionDriver = value; }
    public Vector3 LockedAtPosition { get => _lockedAtPosition; protected set => _lockedAtPosition = value;  }
    public Quaternion RotationWhenLocked { get => _rotationWhenLocked; set => _rotationWhenLocked = value; }
    public bool InitialIsKinematicWhenLocked  {  get => _initialIsKinematicWhenLocked; private set => _initialIsKinematicWhenLocked = value; }
    public float CurrentAttachedSnapElementProgressAlongDrivePath  { get => _currentAttachedSnapElementProgressAlongDrivePath;  private set => _currentAttachedSnapElementProgressAlongDrivePath = value; }

    private Vector3 _initialSnapTargetPositionAtMomentOfSettingProperty; 
    public GuidedSnapTarget CurrentlySnappedToTarget
    {
        get => _currentlySnappedToTarget;
        set
        {
            if (_currentlySnappedToTarget == null && value != null)
            {
                _initialSnapTargetPositionAtMomentOfSettingProperty = value.transform.position;
            }
            else if (_currentlySnappedToTarget != null && value == null)
            {
                _initialSnapTargetPositionAtMomentOfSettingProperty = Vector3.zero;
            }

            _currentlySnappedToTarget = value;
        }
    }

    protected Collider _collider;
    public int ElementDriverRegisteredInFrame { get; private set; }

    public void UpdateSnapElementProgressAlongDrivePath(float progress)
    {
        CurrentAttachedSnapElementProgressAlongDrivePath = progress;
    }

    public void SetPositionAdjustedForSnapOrigin()
    {
        var parentPositionDelta = _initialSnapTargetPositionAtMomentOfSettingProperty - (CurrentlySnappedToTarget != null ? CurrentlySnappedToTarget.transform.position : Vector3.zero);

        if (IsLockedAtPosition)
        {
            transform.SetPosition(LockedAtPosition - DistanceFromOrigin - parentPositionDelta);
        }
        else
        {
            transform.SetPosition(transform.position - DistanceFromOrigin - parentPositionDelta);
        }
    }
    
    public virtual void SetTransformValuesForLockedPosition()
    {
        SetPositionAdjustedForSnapOrigin();
        if(_rigidbody != null) _rigidbody.isKinematic = true;
    }

    public void RegisterElementDriver(TransformPositionDriver transformPositionDriver)
    {
        Debug.Log($"Element Driver Registered: object {name}, driver: {transformPositionDriver.name}");
        TransformPositionDriver = transformPositionDriver;
        ElementDriverRegisteredInFrame = Time.frameCount;
        ElementDriverRegistered?.Invoke(this, EventArgs.Empty);
    }

    public virtual void UnregisterElementDriver(TransformPositionDriver transformPositionDriver) => UnregisterElementDriver();
    public virtual void UnregisterElementDriver()
    {
        Debug.Log($"Element Driver UNRegistered: object {name}, driver: {TransformPositionDriver.name}");
        TransformPositionDriver = null;
        ElementDriverUnregistered?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        if (_rigidbody != null)  
            InitialIsKinematicWhenLocked = _rigidbody.isKinematic;
        FindAndSetXRFrameworkTransformControl();
    }

    public void Snap(GuidedSnapTarget snapTarget)
    {
        if (TransformPositionDriver == null)
        {
            Debug.LogWarning($"{nameof(TransformPositionDriver)} not registered - for proper detach support this needs to be set to component that's driving element movement (eg. controller)");
        }

        CurrentlySnappedToTarget = snapTarget;
        XRFrameworkTransformControl.TakeControlFromXrFramework();
        Snapped?.Invoke(this, new SnappedEventArgs(this, CurrentlySnappedToTarget));
    }

    public void Unsnap()
    {
        RotationWhenLocked = Quaternion.identity;
        LockedAtPosition = Vector3.zero;
        _initialLockedAtPosition = Vector3.positiveInfinity;
        XRFrameworkTransformControl.PassControlBackToXrFramework();
        _currentlySnappedToTarget = null;
        if (_rigidbody != null)
        {
            _rigidbody.isKinematic = InitialIsKinematicWhenLocked;
        }
            
        Unsnapped?.Invoke(this, EventArgs.Empty);
    }


    public void LockInPlace(Vector3 lockAtPosition, Quaternion initialRotationWhenLocked)
    {
        LockedAtPosition = lockAtPosition;
        _initialLockedAtPosition = lockAtPosition;
        RotationWhenLocked = initialRotationWhenLocked;

        if (_rigidbody != null)
        {
            InitialIsKinematicWhenLocked = _rigidbody.isKinematic;
            _rigidbody.isKinematic = true;
        }
    }

    public void SetCollisionsWith(Collider other, bool enable)
    {
        _collider.IgnoreCollision(other, !enable);
    }

    public void TriggerSnappedInFinishedPosition()
    {
        SnappedInFinishPosition?.Invoke(this, EventArgs.Empty);
    }

    public abstract bool ShouldPreventDetach(float detachNotAllowedIfRotationProgressMoreThan);
    public abstract bool IsConsideredTool { get; }

    protected virtual void FixedUpdate()
    {
        if (_currentlySnappedToTarget == null && TransformPositionDriver != null)
        {
            TryAttachOnFoundTargets();
        }
    }

    public void TryAttachOnFoundTargets() => TryAttachOnFoundTargets(null);

    public void TryAttachOnFoundTargets(List<TryAttachStatus> skipChecksFor)
    {
        var colliders = Physics.OverlapSphere(_snapRaycastOrigin.position, SnapAreaWidth,~0); //raycast even against IgnoreRaycast layer
        foreach (var collider in colliders)
        {
            var guidedSnapTargetCollider = collider.GetComponent<GuidedSnapTargetCollider>();
            if (guidedSnapTargetCollider != null && guidedSnapTargetCollider.SnapTarget != null)
            {
                var result = guidedSnapTargetCollider.SnapTarget.TryAttach(this, skipChecksFor);
                _lastTryAttachStatus = result.TryAttachStatus;
                if (result.TryAttachStatus == TryAttachStatus.Attached)
                {
                    CurrentlySnappedToTarget = guidedSnapTargetCollider.SnapTarget;
                    break;
                }
            }
        }
    }

    protected virtual void Reset()
    {
        if (SnapRaycastOrigin == null)
        {
            var originGo = new GameObject() { name = "Origin" };
            _snapRaycastOrigin = originGo.transform;
            _snapRaycastOrigin.SetParent(transform, false);
        }

        FindAndSetXRFrameworkTransformControl();
    }


    [ContextMenu(nameof(FindAndSetXRFrameworkTransformControl))]
    public void FindAndSetXRFrameworkTransformControl()
    {
        if(_xrFrameworkTransformControl == null) _xrFrameworkTransformControl = GetComponent<XRFrameworkTransformControl>();
        if(_xrFrameworkTransformControl == null) Debug.LogError($"{nameof(XRFrameworkTransformControl)} is required");
    }

    protected virtual void OnDrawGizmos()
    {
        if (IsDebug)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_snapRaycastOrigin.position, SnapAreaWidth);

            DebugDraw.Line(_snapRaycastOrigin.position, _snapRaycastOrigin.position + RaycastDirection * (SnapAreaWidth * 2f), Color.magenta);

            DebugDraw.LocalDirections(_snapRaycastOrigin);
        }
    }
}

public class SnappedEventArgs: EventArgs
{
    public GuidedSnapEnabledElement GuidedSnapEnabledElement { get; }
    public GuidedSnapTarget GuidedSnapTarget { get; }

    public SnappedEventArgs(GuidedSnapEnabledElement guidedSnapEnabledElement, GuidedSnapTarget guidedSnapTarget)
    {
        GuidedSnapEnabledElement = guidedSnapEnabledElement;
        GuidedSnapTarget = guidedSnapTarget;
    }
}