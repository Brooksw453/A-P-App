using System;
using System.Collections;
using System.Collections.Generic;
using ReliableSolutions.Unity.Common.Extensions;
using ReliableSolutions.Unity.Common.PropertyDrawer;
using UnityEngine;

public class RotationProgressElement : GuidedSnapEnabledElement
{
#pragma warning disable 649
    [Header("Rotation Progress")]
    [SerializeField] private int RotationAnglesToFinish = 180;
    [SerializeField] [Range(0, 1)] private float RequireCorrectRotationForceFromRotationProgress = 0.9f;
    [SerializeField] private int RequiredToolRotationForce = 30;
    [SerializeField] private RotationProgressLengthDeterminedByMode _rotationProgressLengthDeterminedBy = RotationProgressLengthDeterminedByMode.RotationProgressElement;
    [ShowIf(nameof(ShowRotationProgressEndPointInEditor))] [SerializeField] private Transform _rotationProgressEndPoint;
    [SerializeField] [Range(0, 1)] private float _finalRotationStageDampProgressMultiplier = 0.3f;
    [SerializeField] private float MaxOverRotation = 0.2f;

    [Header("Filter")]
    [SerializeField] [Tag] private List<string> _limitAttachToRotationToolsWithTag;

    [Header("No Tools Rotation")] //TODO: document, experimental - useful if you need to say unscrew by hand. Use at your own risk, no official support
    [SerializeField] private bool _allowAddingRotationProgressWithNoTools = false;
    [SerializeField] [ShowIf(nameof(_allowAddingRotationProgressWithNoTools))] private RotationTool _noToolModeInvisibleTool;

    [Header("Events")]
    public RotationTool.RotationChangeEvent RotationChanged = new RotationTool.RotationChangeEvent();
    public RotationTool.RotationStartedEvent RotationStarted = new RotationTool.RotationStartedEvent();
    public RotationTool.RotationStoppedEvent RotationStopped = new RotationTool.RotationStoppedEvent();

    [Header("Debug")]
    [ShowIf(nameof(IsDebug))] [SerializeField] [ReadOnly] private float _rotationProgress;
#pragma warning restore 649

    public List<string> LimitAttachToRotationToolsWithTag => _limitAttachToRotationToolsWithTag;
    public float FinalRotationStageDampProgressMultiplier => _finalRotationStageDampProgressMultiplier;
    public RotationProgressLengthDeterminedByMode RotationProgressLengthDeterminedBy => _rotationProgressLengthDeterminedBy;
    public Transform RotationProgressEndPoint => _rotationProgressEndPoint;
    public float RotationProgress  {  get => _rotationProgress; private set => _rotationProgress = value; }

    public float RotationProgressLength
    {
        get
        {
            switch (RotationProgressLengthDeterminedBy)
            {
                case RotationProgressLengthDeterminedByMode.RotationProgressElement:
                    return Vector3.Distance(SnapRaycastOrigin.position, RotationProgressEndPoint.position);
                    
                case RotationProgressLengthDeterminedByMode.SnapTarget:
                    if (_currentlySnappedToTarget == null) return 0;
                    return Vector3.Distance(_currentlySnappedToTarget.TargetOrigin.position, _currentlySnappedToTarget.RotationProgressEndPoint.position);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private bool ShowRotationProgressEndPointInEditor => RotationProgressLengthDeterminedBy == RotationProgressLengthDeterminedByMode.RotationProgressElement;

    public bool AllowAddingRotationProgressWithNoTools => _allowAddingRotationProgressWithNoTools;

    private Vector3 _noToolModeInvisibleToolPositionBeforeEnabling;

    public AddRotationProgressStatus AddRotationProgress(float movedAngles, int toolRotationForce)
    {
        var newRotationProgress = RotationProgress + GetRotationProgress(movedAngles);
        var clampedNewRotationProgress = Mathf.Clamp(newRotationProgress, -0.1f, 1f + MaxOverRotation);

        if (RotationProgress >= RequireCorrectRotationForceFromRotationProgress)
        {
            if (toolRotationForce < RequiredToolRotationForce)
            {
                return AddRotationProgressStatus.NotEnoughToolRotationForce; //not enough force, don't apply progress
            }
        }

        if (newRotationProgress > RequireCorrectRotationForceFromRotationProgress && newRotationProgress < 1f)
        {
            RotationProgress += GetRotationProgress(movedAngles) * FinalRotationStageDampProgressMultiplier;
            return AddRotationProgressStatus.FinalRotationStage;
        }

        RotationProgress = clampedNewRotationProgress;
        if (newRotationProgress > 1f + MaxOverRotation) return AddRotationProgressStatus.OverRotatingBreakingPoint;
        if (RotationProgress > 1f) return AddRotationProgressStatus.OverRotating;
        if (RotationProgress < 0f) return AddRotationProgressStatus.UnderRotating;

        return AddRotationProgressStatus.Added;
    }

    public void TriggerRotationChangedEvent(RotationChanged eventArgs)
    {
        RotationChanged?.Invoke(eventArgs);
    }

    public void TriggerRotationStartedEvent()
    {
        RotationStarted?.Invoke();
    }

    public void TriggerRotationStoppedEvent()
    {
        RotationStopped?.Invoke();
    }

    public override void SetTransformValuesForLockedPosition()
    {
        base.SetTransformValuesForLockedPosition();
        transform.SetRotation(RotationWhenLocked);
    }

    public void UpdateLockedAtPositionBasedOnProgress(float? newRotationProgress = null)
    {
        if (newRotationProgress.HasValue)
            RotationProgress = newRotationProgress.Value;

        var moveBy = Mathf.Lerp(0, RotationProgressLength, Mathf.Min(1f, RotationProgress));
        LockedAtPosition = _initialLockedAtPosition + RaycastDirection * moveBy;
    }

    public float GetRotationProgress(float movedAngles)
    {
        return movedAngles / RotationAnglesToFinish;
    }

    public override bool ShouldPreventDetach(float detachNotAllowedIfRotationProgressMoreThan)
    {
        return RotationProgress > detachNotAllowedIfRotationProgressMoreThan;
    }

    public override bool IsConsideredTool { get; } = false;

    protected void Awake()
    {
        if (_allowAddingRotationProgressWithNoTools)
        {
            _noToolModeInvisibleTool.RegisterNoToolElement(this);
        }
    }

    protected virtual void OnEnable()
    {
        //no tools work in a manner where a tool is auto attached on grab
        if (_allowAddingRotationProgressWithNoTools)
        {
            ElementDriverRegistered += HandleOnElementDriverRegistered;
            ElementDriverUnregistered += HandleOnElementDriverUnregistered;
        }
    }

    protected virtual void OnDisable()
    {
        if (_allowAddingRotationProgressWithNoTools)
        {
            ElementDriverRegistered -= HandleOnElementDriverRegistered;
            ElementDriverUnregistered -= HandleOnElementDriverUnregistered;
        }
    }

    private void HandleOnElementDriverUnregistered(object sender, EventArgs e)
    {
        StopUsingNoToolMode();
    }

    private void StopUsingNoToolMode()
    {
        Debug.Log("No tool mode: STOPPED");
        _noToolModeInvisibleTool.UnregisterElementDriver(TransformPositionDriver);
        _noToolModeInvisibleTool.Unsnap();
        _noToolModeInvisibleTool.transform.SetPosition(_noToolModeInvisibleToolPositionBeforeEnabling);
        _noToolModeInvisibleToolPositionBeforeEnabling = Vector3.zero;
    }

    private async void HandleOnElementDriverRegistered(object sender, EventArgs e)
    {
        if (CurrentlySnappedToTarget != null)
        {
            StartUsingNoToolMode();
        }
    }

    private static List<TryAttachStatus> _noToolModeSkipTryAttachChecksFor = new List<TryAttachStatus>()
    {
        TryAttachStatus.UnableToAttachNotCorrectDriverAcceleration,
        TryAttachStatus.UnableToAttachAngleNotWithinRange
    };
    public void StartUsingNoToolMode()
    {
        Debug.Log("No tool mode: STARTED");
        _noToolModeInvisibleToolPositionBeforeEnabling = _noToolModeInvisibleTool.transform.position;
        _noToolModeInvisibleTool.RegisterElementDriver(TransformPositionDriver);
        
        _noToolModeInvisibleTool.Snap(CurrentlySnappedToTarget);
        _noToolModeInvisibleTool.transform.SetPosition(_currentlySnappedToTarget.TargetOrigin.transform.position);
        _noToolModeInvisibleTool.SetPositionAdjustedForSnapOrigin();
        _noToolModeInvisibleTool.TryAttachOnFoundTargets(_noToolModeSkipTryAttachChecksFor);
        
        var toolRotationForProperRotateAroundResolution = Quaternion.LookRotation(Vector3.down, CurrentlySnappedToTarget.SnapDirection);
        _noToolModeInvisibleTool.transform.SetRotation(toolRotationForProperRotateAroundResolution);
        _noToolModeInvisibleTool.LockInPlace(_currentlySnappedToTarget.TargetOrigin.transform.position, toolRotationForProperRotateAroundResolution);
        _noToolModeInvisibleTool.StartRotating(this);
    }

    protected override void Reset()
    {
        base.Reset();

        TrySetRotationProgressEndPoint();
    }

    [ContextMenu(nameof(TrySetRotationProgressEndPoint))]
    private void TrySetRotationProgressEndPoint()
    {
        if (RotationProgressEndPoint == null)
        {
            var go = new GameObject() { name = "RotationProgressEndPoint" };
            _rotationProgressEndPoint = go.transform;
            RotationProgressEndPoint.SetParent(transform, false);
        }
    }
}

public enum AddRotationProgressStatus
{
    None,
    Added,
    NotEnoughToolRotationForce,
    FinalRotationStage,
    OverRotating,
    OverRotatingBreakingPoint,
    UnderRotating
}

public enum RotationProgressLengthDeterminedByMode
{
    RotationProgressElement,
    SnapTarget
}