using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ReliableSolutions.Unity.Common;
using ReliableSolutions.Unity.Common.Extensions;
using ReliableSolutions.Unity.Common.PropertyDrawer;
using ReliableSolutions.Unity.Common.Utilities;
using TMPro;
using UnityEngine;

public class RotationTool : GuidedSnapEnabledElement
{
    [Serializable]
    public class RotationChangeEvent : UnityEngine.Events.UnityEvent<RotationChanged>
    {
    }

    [Serializable]
    public class RotationStartedEvent : UnityEngine.Events.UnityEvent
    {
    }

    [Serializable]
    public class RotationStoppedEvent : UnityEngine.Events.UnityEvent
    {
    }

#pragma warning disable 649
    [Header("Tool Options")]
    [SerializeField] private RotationProgressAccumulationDirection _rotationProgressAccumulationDirection = RotationProgressAccumulationDirection.Right;
    
    [SerializeField]
    private RotationProgressAccumulationDriverMethod _rotationProgressAccumulationDriverMethod = RotationProgressAccumulationDriverMethod.AngleDueToPositionChange;

    [SerializeField] [ShowIf(nameof(ShowRotationProgressAccumulationViaTransformRotationOptions))]
    private Vector3Axis _rotationProgressAccumulationDriverMethodAxis = Vector3Axis.Z;

    [SerializeField] [Range(1, 100)] private int RotationToolForce = 30;
    [SerializeField] private bool IsRotationForceAdjustable = true;
    [ShowIf(nameof(IsRotationForceAdjustable))] [SerializeField] [Range(1, 100)] private int MaxRotationToolForce = 70;
    [ShowIf(nameof(IsRotationForceAdjustable))] [SerializeField] [Range(1, 100)] private int MinRotationToolForce = 10;

    [Header("Rotation Result Multipliers")]
    [SerializeField] [Range(0, 200)] private float AddedMaxMoveAnglePerSecond = 90f;
    [SerializeField] [Range(0, 200)] private float NotEnoughForceMaxMoveAnglePerSecond = 0f;
    [SerializeField] [Range(0, 200)] private float OverRotatingMaxMoveAnglePerSecond = 30f;
    [SerializeField] [Range(0, 200)] private float OverRotatingBreakingPointMaxMoveAnglePerSecond = 0f;
    [SerializeField] [Range(0, 200)] private float FinalRotationStageMaxMoveAnglePerSecond = 50f;

    [Header("Tool Display")]
    [SerializeField] private TextMeshProUGUI ForceText;
    [SerializeField] private TextMeshProUGUI DirectionText;
    [SerializeField] private List<XRFrameworkToolInput> XRFrameworkToolInputs;

    [Header("Tool Direction Auto Recognition")] [SerializeField]
    private bool AutomaticallyRecognizeToolDirection;

    //[Header("Debug")] //adding debug is causing issues when rendering in editor, probably due to IsDebug being declared in base class (conditional field will likely not handle that)
    [ShowIf(nameof(IsDebug))] [SerializeField] private AddRotationProgressStatus _previousAddRotationProgressStatus = AddRotationProgressStatus.None;
    [ShowIf(nameof(IsDebug))] [SerializeField] [ReadOnly] private RotationProgressElement _rotatingElement;
#pragma warning restore 649

    [Header("Events")]
    public RotationChangeEvent RotationChanged = new RotationChangeEvent();
    public RotationStartedEvent RotationStarted = new RotationStartedEvent();
    public RotationStoppedEvent RotationStopped = new RotationStoppedEvent();

    public RotationProgressAccumulationDirection RotationProgressAccumulationDirection => _rotationProgressAccumulationDirection;
    private bool CanAcumulationDirectionBeChanged => RotationProgressAccumulationDirection == RotationProgressAccumulationDirection.Left || RotationProgressAccumulationDirection == RotationProgressAccumulationDirection.Right;
    private bool ShowRotationProgressAccumulationViaTransformRotationOptions => _rotationProgressAccumulationDriverMethod == RotationProgressAccumulationDriverMethod.AngleDueToRotationChange;

    public bool IsRotating => _rotatingElement != null;
    private RotationProgressElement _registeredNoToolRotationElement;
    private bool IsActingAsNoToolRotationHandler => _registeredNoToolRotationElement != null;

    private static readonly Dictionary<AddRotationProgressStatus, Func<RotationTool, float>> AddRotationProgressStatusToMaxAnglesMovePerSecond = new Dictionary<AddRotationProgressStatus, Func<RotationTool, float>>()
    {
        [AddRotationProgressStatus.None] = t => float.MaxValue,
        [AddRotationProgressStatus.Added] = t => t.AddedMaxMoveAnglePerSecond,
        [AddRotationProgressStatus.NotEnoughToolRotationForce] = t => t.NotEnoughForceMaxMoveAnglePerSecond,
        [AddRotationProgressStatus.FinalRotationStage] = t => t.FinalRotationStageMaxMoveAnglePerSecond,
        [AddRotationProgressStatus.OverRotating] = t => t.OverRotatingMaxMoveAnglePerSecond,
        [AddRotationProgressStatus.OverRotatingBreakingPoint] = t => t.OverRotatingBreakingPointMaxMoveAnglePerSecond,
        [AddRotationProgressStatus.UnderRotating] = t => float.MaxValue,
    };

    private static readonly List<AddRotationProgressStatus> AddRotationProgressStatusThatMovesRotationProgressElement = new List<AddRotationProgressStatus>()
    {
        AddRotationProgressStatus.Added,
        AddRotationProgressStatus.FinalRotationStage,
        AddRotationProgressStatus.OverRotating
    };

    private Quaternion _lastToolRotation;
    private float _lastTransformPositionRotationAngleOnProgressAccumulatingAxis;

    protected override void Start()
    {
        base.Start();
        UpdateToolUI();

        StartCoroutine(AttachInputEventHandlers());

        this.Unsnapped += (sender, e) =>
        {
            if (_rotatingElement != null)
            {
                ResetPreviousRotationProgressStatus();
                RotationStopped?.Invoke();
                _rotatingElement.TriggerRotationStoppedEvent();
                _rotatingElement = null;
            }
        };
    }

    public void StartRotating(RotationProgressElement rotationProgressElement)
    {
        _rotatingElement = rotationProgressElement;
        if (AutomaticallyRecognizeToolDirection)
        {
            const float rotationTolerance = 0.03f;
            if (rotationProgressElement.RotationProgress >= 1f - rotationTolerance && RotationProgressAccumulationDirection == RotationProgressAccumulationDirection.Right)
            {
                ChangeDirection(RotationProgressAccumulationDirection.Left);
            }
            else if (rotationProgressElement.RotationProgress <= 0 + rotationTolerance && RotationProgressAccumulationDirection == RotationProgressAccumulationDirection.Left)
            {
                ChangeDirection(RotationProgressAccumulationDirection.Right);
            }
        }

        RotationStarted?.Invoke();
        rotationProgressElement.TriggerRotationStartedEvent();
    }

    public void IncreaseRotationForce()
    {
        if(!IsRotationForceAdjustable) return;
        if(RotationToolForce + 1 > MaxRotationToolForce) return;

        RotationToolForce += 1;

        ResetPreviousRotationProgressStatus();
        UpdateToolUI();
    }

    public void DecreaseRotationForce()
    {
        if (!IsRotationForceAdjustable) return;
        if (RotationToolForce - 1 < MinRotationToolForce) return;

        RotationToolForce -= 1;

        ResetPreviousRotationProgressStatus();
        UpdateToolUI();
    }

    //HACK: tracking previous rotation progress status is not correct, it'll always allow to do at least 1 frame of movement, resolving current move possible should be done without it
    private void ResetPreviousRotationProgressStatus()
    {
        _previousAddRotationProgressStatus = AddRotationProgressStatus.None;
    }

    public void ChangeDirection(RotationProgressAccumulationDirection direction)
    {
        if (!CanAcumulationDirectionBeChanged) return;

        _rotationProgressAccumulationDirection = direction;

        ResetPreviousRotationProgressStatus();
        UpdateToolUI();
    }
    public void ToggleDirection()
    {
        ChangeDirection(RotationProgressAccumulationDirection == RotationProgressAccumulationDirection.Left 
            ? RotationProgressAccumulationDirection.Right
            : RotationProgressAccumulationDirection.Left
        );
    }

    public void ProcessRotationProgress(Vector3 rotationPivot, RotationProgressElement rotationProgressElement)
    {
        if(TransformPositionDriver == null) return;
        
        var driverMovedAngle = GetMovedAngle(rotationPivot);
        var absoluteDriverMovedAngle = Mathf.Abs(driverMovedAngle);
        if (absoluteDriverMovedAngle > 0.01f)
        {
            if (_previousAddRotationProgressStatus == AddRotationProgressStatus.OverRotatingBreakingPoint
                && RotationProgressAccumulationDirection == RotationProgressAccumulationDirection.Both
                && driverMovedAngle > 0)
            {
                //When rotation is allowed in both directions, at the OverRotatingBreakingPoint movement will stop, this causes
                //issues when user likes to undo progress but initially moves forward even by a marginal angle (which is very easy with XR controllers)
                //if that's the case change previous status to OverRotating as to allow it to move
                _previousAddRotationProgressStatus = AddRotationProgressStatus.OverRotating;
            }

            var maxAnglesPerSecond = AddRotationProgressStatusToMaxAnglesMovePerSecond[_previousAddRotationProgressStatus](this);
            var maxAngleMoveTimeInterpolated = Mathf.Lerp(
                0f,
                maxAnglesPerSecond,
                Time.deltaTime
            );
            var movedAnglesAfterAdjustment = Mathf.Min(absoluteDriverMovedAngle, maxAngleMoveTimeInterpolated);
            movedAnglesAfterAdjustment = driverMovedAngle < 0 ? movedAnglesAfterAdjustment * -1 : movedAnglesAfterAdjustment;

            if (Mathf.Abs(movedAnglesAfterAdjustment) > 0)
            {
                var addRotationProgressStatus = AddRotationProgressStatus.None;
                if (IsRotatingInAccumulatingDirection(driverMovedAngle))
                {
                    addRotationProgressStatus = rotationProgressElement.AddRotationProgress(-movedAnglesAfterAdjustment, RotationToolForce);
                    rotationProgressElement.UpdateLockedAtPositionBasedOnProgress();
                }
                if (IsDebug) Debug.Log($"Last Rotation Result: {addRotationProgressStatus}, moved: {movedAnglesAfterAdjustment}");
                _previousAddRotationProgressStatus = addRotationProgressStatus;

                var rotationChangedArgs = new RotationChanged(
                    rotationProgressElement,
                    rotationProgressElement.GetRotationProgress(-movedAnglesAfterAdjustment),
                    addRotationProgressStatus,
                    movedAnglesAfterAdjustment,
                    TransformPositionDriver,
                    this
                );

                transform.RotateAroundTracked(rotationPivot, RaycastDirection, movedAnglesAfterAdjustment);
                _lastToolRotation = transform.rotation;

                if (AddRotationProgressStatusThatMovesRotationProgressElement.Contains(addRotationProgressStatus))
                {
                    rotationProgressElement.transform.RotateAroundTracked(rotationPivot, RaycastDirection, movedAnglesAfterAdjustment);
                    rotationProgressElement.RotationWhenLocked = rotationProgressElement.transform.rotation;
                }

                AdjustToolPositionToAccountForRotationProgressMade(rotationProgressElement);

                RotationChanged?.Invoke(rotationChangedArgs);
                rotationProgressElement.TriggerRotationChangedEvent(rotationChangedArgs);
            }
        }
    }

    public void RegisterNoToolElement(RotationProgressElement rotationProgressElement)
    {
        if(_registeredNoToolRotationElement) 
            throw new Exception($"Tool already used in NoToolMode for with {_registeredNoToolRotationElement.name}");
        
        _registeredNoToolRotationElement = rotationProgressElement;
    }

    private float GetMovedAngle(Vector3 rotationPivot)
    {
        switch (_rotationProgressAccumulationDriverMethod)
        {
            case RotationProgressAccumulationDriverMethod.AngleDueToPositionChange:
                var rotationAxis = RaycastDirection;
                var driverMovedAngle = AngleBetweenPoints.GetThreePointAngleSigned180(transform.position,
                    TransformPositionDriver.transform.position,
                    rotationPivot,
                    rotationAxis
                );

                return driverMovedAngle;

            case RotationProgressAccumulationDriverMethod.AngleDueToRotationChange:
                var currentAngleOnProgressAccumulatingAxis = TransformPositionDriver.transform.rotation.eulerAngles[(int)_rotationProgressAccumulationDriverMethodAxis];
                var rotationChangeAngle = Mathf.DeltaAngle(currentAngleOnProgressAccumulatingAxis ,_lastTransformPositionRotationAngleOnProgressAccumulatingAxis);
                _lastTransformPositionRotationAngleOnProgressAccumulatingAxis = currentAngleOnProgressAccumulatingAxis;

                return -rotationChangeAngle;

            default:
                throw new ArgumentOutOfRangeException();
        }

    }

    private void AdjustToolPositionToAccountForRotationProgressMade(RotationProgressElement rotationProgressElement)
    {
        LockedAtPosition = rotationProgressElement.RotationProgressLengthDeterminedBy == RotationProgressLengthDeterminedByMode.RotationProgressElement 
            ? rotationProgressElement.RotationProgressEndPoint.position
            : rotationProgressElement.SnapRaycastOrigin.position;
        SetPositionAdjustedForSnapOrigin();
    }

    private bool IsRotatingInAccumulatingDirection(float driverMovedAngle)
    {
        return RotationProgressAccumulationDirection == RotationProgressAccumulationDirection.Both
                   || (RotationProgressAccumulationDirection == RotationProgressAccumulationDirection.Left && driverMovedAngle > 0)
               || (RotationProgressAccumulationDirection == RotationProgressAccumulationDirection.Right && driverMovedAngle < 0);
    }

    public override void SetTransformValuesForLockedPosition()
    {
        base.SetTransformValuesForLockedPosition();
        transform.SetRotation(_lastToolRotation);
    }

    private void UpdateToolUI()
    {
        if(ForceText != null) ForceText.text = RotationToolForce.ToString();
        if(DirectionText != null) DirectionText.text = RotationProgressAccumulationDirection == RotationProgressAccumulationDirection.Right ? "R" : "L";
    }

    private IEnumerator AttachInputEventHandlers()
    {
        yield return new WaitForEndOfFrame();

        if (!XRFrameworkToolInputs.Any())
        {
            FindAndSetXrFrameworkToolInputs();
        }

        foreach (var xrFrameworkToolInput in XRFrameworkToolInputs)
        {
            if (IsRotationForceAdjustable)
            {
                AddListenerExecuteIfElementDriverSameAsEventOriginator(xrFrameworkToolInput.IncreaseToolForce, IncreaseRotationForce);
                AddListenerExecuteIfElementDriverSameAsEventOriginator(xrFrameworkToolInput.DecreaseToolForce, DecreaseRotationForce);
            }

            if (CanAcumulationDirectionBeChanged)
            {
                AddListenerExecuteIfElementDriverSameAsEventOriginator(xrFrameworkToolInput.ChangeToolDirection, ToggleDirection);
            }
        }
    }

    private void AddListenerExecuteIfElementDriverSameAsEventOriginator(XRFrameworkToolInput.UnityEvent ev, Action executeFn)
    {
        ev.AddListener((originatorGo) =>
        {
            if (TransformPositionDriver != null && originatorGo == TransformPositionDriver.gameObject)
            {
                executeFn();
            }
        });
    }

    public override bool ShouldPreventDetach(float detachNotAllowedIfRotationProgressMoreThan)
    {
        if (IsActingAsNoToolRotationHandler)
        {
            return true;
        }
        return false;
    }

    public override bool IsConsideredTool { get; } = true;

    protected override void Reset()
    {
        base.Reset();

        FindAndSetXrFrameworkToolInputs();
    }

    [ContextMenu(nameof(FindAndSetXrFrameworkToolInputs))]
    private void FindAndSetXrFrameworkToolInputs()
    {
        XRFrameworkToolInputs = FindObjectsOfType<XRFrameworkToolInput>().ToList();
    }
}

public enum RotationProgressAccumulationDirection
{
    Left,
    Right,
    Both
}

public enum RotationProgressAccumulationDriverMethod
{
    AngleDueToPositionChange,
    AngleDueToRotationChange
}
