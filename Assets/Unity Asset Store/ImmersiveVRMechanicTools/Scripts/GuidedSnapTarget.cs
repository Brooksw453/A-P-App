using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Common.Runtime;
using ReliableSolutions.Unity.Common.Extensions;
using ReliableSolutions.Unity.Common.PropertyDrawer;
using ReliableSolutions.Unity.Common.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using DebugDraw = ReliableSolutions.Unity.Common.Debug.DebugDraw;

public class GuidedSnapTarget : MonoBehaviour
{
    private const int YColliderDirectionIntRepresentation = 1;

    [Serializable] public class GuidedSnapEnabledElementEvent : UnityEngine.Events.UnityEvent<GuidedSnapEnabledElement> { }
    [Serializable] public class TryAttachEvent : UnityEngine.Events.UnityEvent<TryAttachResult> { }
    [Serializable] public class SnapElementProgressAlongDrivePathEvent : UnityEngine.Events.UnityEvent<SnapElementProgressAlongDrivePath>  { }

#pragma warning disable 649
    [Header("Guide Path")]
    [SerializeField] private Transform _targetOrigin;
    [SerializeField] [Range(0, 5)] private float GuideDistance = 1f;
    [SerializeField] [Range(0, 5)] private float GuideRadius = 0.5f;
    [SerializeField] private Transform WhileOnGuidePathFixRotationToInitialTransformRotation;

    [Header("Rotation Progress")]
    [SerializeField] private RotationProgressLengthDeterminedByMode _rotationProgressLengthDeterminedBy = RotationProgressLengthDeterminedByMode.RotationProgressElement;
    [ShowIf(nameof(IsRotationProgressLengthDeterminedBySnapTarget))] [SerializeField] private Transform _rotationProgressEndPoint;

    [Header("Snap Option")] 
    [SerializeField] [Range(0, 1)] private float ProgressPercentageToConsiderPathFinished = 0.95f;
    [SerializeField] [Range(0, 1)] private float AccelerationTowardsGuidePathRequiredToAttach = 0.05f;
    [SerializeField] [FormerlySerializedAs("ToolsAutoDetachDistance")] [Min(0)] protected Vector3 AutoDetachDistance = new Vector3(1, 1, 2);
    [SerializeField] [MinMaxSlider(0, 180)] private Vector2 AllowedAngleRange = new Vector2(0, 45);
    [SerializeField] [Range(0, 1)] private float DetachNotAllowedIfRotationProgressMoreThan = 0.1f;

    [Header("Snap Filters")]
    [SerializeField] [Tag] private List<string> LimitSnapTargetsToTags;

    [Header("Initial Snap")]
    [SerializeField] [FormerlySerializedAs("_initiallyAttachedRotationProgressElement")] private GuidedSnapEnabledElement _initiallyAttachedGuidedSnapEnabledElement;
    [SerializeField] [Range(0, 1)] private float _initiallyAttachedElementRotationProgress = 0f;

    [Header("Debug")]
    [SerializeField] private bool IsDebug;
    [ShowIf(nameof(IsDebug))] [SerializeField] private RotationProgressElement _currentlyAttachedRotationProgressElement;
    [ShowIf(nameof(IsDebug))] [SerializeField] private InsertElement _currentlyAttachedInsertElement;
    [ShowIf(nameof(IsDebug))] [SerializeField] private RotationTool _currentlyAttachedTool;
    [ShowIf(nameof(IsDebug))] [SerializeField] private StickyTool _currentlyAttachedStickyTool;
    [ShowIf(nameof(IsDebug))] [SerializeField] private GuidedSnapTargetCollider _guidedSnapTargetCollider;
    [ShowIf(nameof(IsDebug))] [SerializeField] private TryAttachStatus _lastTryAttachStatus;
#pragma warning restore 649

    private Quaternion _whileOnGuidePathRotationInitialTransformRotation;
    private List<GuidedSnapEnabledElement> _attachableElements;

    private List<GuidedSnapEnabledElement> AttachableElements => new List<GuidedSnapEnabledElement>
    {
        _currentlyAttachedRotationProgressElement,
        _currentlyAttachedInsertElement,
        _currentlyAttachedTool,
        _currentlyAttachedStickyTool
    };

    [Header("Events")]
    public GuidedSnapEnabledElementEvent ElementDetached = new GuidedSnapEnabledElementEvent();
    public GuidedSnapEnabledElementEvent ElementAttached = new GuidedSnapEnabledElementEvent();

    public TryAttachEvent TryAttachFailed = new TryAttachEvent();
    public TryAttachEvent TryAttachSucceeded = new TryAttachEvent();

    public SnapElementProgressAlongDrivePathEvent SnapElementProgressAlongDrivePathChanged = new SnapElementProgressAlongDrivePathEvent();
    public SnapElementProgressAlongDrivePathEvent SnapElementProgressAlongDrivePathFinished = new SnapElementProgressAlongDrivePathEvent();


    public Vector3 SnapDirection => TargetOrigin.transform.rotation * Vector3.up;
    private Vector3 DrivePathCenterPoint => (TargetOrigin.transform.position + MaxGuidePoint) / 2;
    public Transform TargetOrigin => _targetOrigin;
    public Vector3 MaxGuidePoint => GenerateEndGuidePoint(1f);
    public Transform RotationProgressEndPoint => _rotationProgressEndPoint;
    private bool IsRotationProgressLengthDeterminedBySnapTarget => RotationProgressLengthDeterminedBy == RotationProgressLengthDeterminedByMode.SnapTarget;
    public RotationProgressLengthDeterminedByMode RotationProgressLengthDeterminedBy => _rotationProgressLengthDeterminedBy;
    public GuidedSnapTargetCollider GuidedSnapTargetCollider => _guidedSnapTargetCollider;
    public RotationProgressElement CurrentlyAttachedRotationProgressElement =>  _currentlyAttachedRotationProgressElement;
    private bool IsAnyElementAttached => AttachableElements.Any(e => e != null);

    public InsertElement CurrentlyAttachedInsertElement => _currentlyAttachedInsertElement;

    private GuidedSnapEnabledElement FindFirstStickableElement() => AttachableElements
        .FirstOrDefault(e => e != null && e != _currentlyAttachedStickyTool);

    private ChangeSincePreviousFrameMonitor<Quaternion> _rotationChangeChangeSincePreviousFrameMonitor;

    void Awake()
    {
        _rotationChangeChangeSincePreviousFrameMonitor = new ChangeSincePreviousFrameMonitor<Quaternion>(() => transform.rotation);

        TrySetGuidedSnapTargetCollider();

        if (_initiallyAttachedGuidedSnapEnabledElement != null)
        {
            AttachGuidedSnapEnabledElement(_initiallyAttachedGuidedSnapEnabledElement);
        }

        if (WhileOnGuidePathFixRotationToInitialTransformRotation != null)
        {
            _whileOnGuidePathRotationInitialTransformRotation = WhileOnGuidePathFixRotationToInitialTransformRotation.localRotation;
        }
    }

    private void TrySetGuidedSnapTargetCollider()
    {
        if (_guidedSnapTargetCollider == null && TargetOrigin != null)
        {
            _guidedSnapTargetCollider = TargetOrigin.GetComponent<GuidedSnapTargetCollider>();
        }
    }


    void Update()
    {
        _rotationChangeChangeSincePreviousFrameMonitor.Update();

        foreach (var guidedSnapEnabledElement in AttachableElements)
        {
            ProcessAttachedSnapElement(guidedSnapEnabledElement);
        }
    }
    
    private Vector3 GenerateEndGuidePoint(float distanceMultiplier) => TargetOrigin.position + (SnapDirection * GuideDistance * distanceMultiplier);

    private void ProcessAttachedSnapElement(GuidedSnapEnabledElement snapEnabledElement)
    {
        if (snapEnabledElement == null) return;

        if (ShouldDetach(snapEnabledElement))
        {
            DetachElement(snapEnabledElement);
        }
        else
        {
            if (snapEnabledElement.TransformPositionDriver != null && !snapEnabledElement.IsLockedAtPosition)
            {
                var snapElementClampedPointOnGuide = ClamPositionToMoveAlongDrivePath(snapEnabledElement);

                if (WhileOnGuidePathFixRotationToInitialTransformRotation != null)
                    snapEnabledElement.transform.SetLocalRotation(_whileOnGuidePathRotationInitialTransformRotation);
                else
                    AdjustRotationToPointAtTransformDriver(snapEnabledElement, snapElementClampedPointOnGuide);

                //adjust position for origin, it has to be disjointed from initial position set as otherwise rotation would need to be adjusted for that
                snapEnabledElement.SetPositionAdjustedForSnapOrigin();

                ProcessSnappedElementProgressAlongDrivePath(snapEnabledElement);
            }
            else if (snapEnabledElement.TransformPositionDriver != null && snapEnabledElement is RotationTool rotationTool && rotationTool.IsRotating && CurrentlyAttachedRotationProgressElement)
            {
                rotationTool.ProcessRotationProgress(TargetOrigin.position, CurrentlyAttachedRotationProgressElement);
            }
            else if (snapEnabledElement.IsLockedAtPosition)
            {
                snapEnabledElement.SetTransformValuesForLockedPosition();
            }

            if (_rotationChangeChangeSincePreviousFrameMonitor.IsValueUpdatedSinceLastUpdateCall && snapEnabledElement.CurrentlySnappedToTarget != null)
            {
                var rotationToAlignSnapElementOriginWithTargetOrigin = GetRotationAlignedWithTargetPathDirection(snapEnabledElement);
                snapEnabledElement.RotationWhenLocked = rotationToAlignSnapElementOriginWithTargetOrigin;
                if (snapEnabledElement is RotationProgressElement rp) 
                    rp.UpdateLockedAtPositionBasedOnProgress();
            }
        }
    }
    private void DetachElement(GuidedSnapEnabledElement snapEnabledElement)
    {
        if (snapEnabledElement is RotationProgressElement) _currentlyAttachedRotationProgressElement = null;
        if (snapEnabledElement is RotationTool) _currentlyAttachedTool = null;
        if (snapEnabledElement is InsertElement && _currentlyAttachedInsertElement != null)
        {
            if(_currentlyAttachedInsertElement.IsElementDetachmentDisallowingChildElementsAttachmentDetachment) 
                _currentlyAttachedInsertElement.DisallowRotationProgressElementsAttachment();
            _currentlyAttachedInsertElement = null;
        }
        if (snapEnabledElement is StickyTool) _currentlyAttachedStickyTool = null;

        snapEnabledElement.Unsnap();
        ElementDetached?.Invoke(snapEnabledElement);
    }

    private Vector3 ClamPositionToMoveAlongDrivePath(GuidedSnapEnabledElement snapEnabledElement)
    {
        Vector3 startPoint;
        Vector3 endPoint;

        switch (RotationProgressLengthDeterminedBy)
        {
            case RotationProgressLengthDeterminedByMode.RotationProgressElement:
                startPoint = TargetOrigin.position;
                endPoint = MaxGuidePoint;
                break;
            case RotationProgressLengthDeterminedByMode.SnapTarget:
                startPoint = MaxGuidePoint;
                endPoint = RotationProgressEndPoint.position;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        var snapElementClampedPointOnGuide = VectorProjection.ClampPoint(
            snapEnabledElement.TransformPositionDriver.transform.position,
            startPoint,
            endPoint
        );

        snapEnabledElement.transform.position = snapElementClampedPointOnGuide;
        return snapElementClampedPointOnGuide;
    }

    private void AdjustRotationToPointAtTransformDriver(GuidedSnapEnabledElement snapEnabledElement, Vector3 snapElementClampedPointOnGuide)
    {
        var rotationToAlignSnapElementOriginWithTargetOrigin = GetRotationAlignedWithTargetPathDirection(snapEnabledElement);
        snapEnabledElement.transform.SetRotation(rotationToAlignSnapElementOriginWithTargetOrigin);

        //re-orient for tool to face transform driver
        var angleDifferenceToPointAtTransformDriver = AngleBetweenPoints.GetThreePointAngleSigned180(
            snapEnabledElement.DistanceFromOrigin.sqrMagnitude > 0.01f
                ? snapElementClampedPointOnGuide - snapEnabledElement.DistanceFromOrigin //need to adjust for new position as origin re-adjustment didn't happen yet
                : snapElementClampedPointOnGuide + (snapEnabledElement.transform.forward * 0.1f), //if origin is very similar as transform - use forward
            snapEnabledElement.TransformPositionDriver.transform.position,
            snapElementClampedPointOnGuide,
            SnapDirection,
            IsDebug
        );
        
        snapEnabledElement.transform.SetRotation(Quaternion.AngleAxis(angleDifferenceToPointAtTransformDriver, SnapDirection)
                                                * snapEnabledElement.transform.rotation);
    }

    private Quaternion GetRotationAlignedWithTargetPathDirection(GuidedSnapEnabledElement snapEnabledElement)
    {
        var rotationToAlignSnapElementOriginWithTargetOrigin =
            RotationHelper.GetQuaternionRotationChildRelativeParentApplicable(
                TargetOrigin.rotation,
                snapEnabledElement.transform.rotation,
                snapEnabledElement.SnapRaycastOrigin.rotation
            );
        return rotationToAlignSnapElementOriginWithTargetOrigin;
    }

    private void ProcessSnappedElementProgressAlongDrivePath(GuidedSnapEnabledElement snapEnabledElement)
    {
        var previousProgress = snapEnabledElement.CurrentAttachedSnapElementProgressAlongDrivePath;

        if (snapEnabledElement is RotationTool && CurrentlyAttachedRotationProgressElement != null && CurrentlyAttachedRotationProgressElement.IsLockedAtPosition)
        {
            //tool progress, need to consider size of screw and current progress
            Vector3 progressLineEndAdjustedForAttachedScrew;
            switch (CurrentlyAttachedRotationProgressElement.RotationProgressLengthDeterminedBy)
            {
                case RotationProgressLengthDeterminedByMode.RotationProgressElement:
                    //when progress element determine length we need to adjust for progress already made
                    progressLineEndAdjustedForAttachedScrew =
                        TargetOrigin.position - //adjust origin point
                        (CurrentlyAttachedRotationProgressElement.RaycastDirection * CurrentlyAttachedRotationProgressElement.RotationProgressLength //for rotation made (inserted)
                                                                  * (1 - Mathf.Min(1, CurrentlyAttachedRotationProgressElement.RotationProgress)));
                    break;
                case RotationProgressLengthDeterminedByMode.SnapTarget:
                    //no adjustment needed if progress is determined by SnapTarget
                    progressLineEndAdjustedForAttachedScrew = TargetOrigin.position;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (IsDebug)
            {
                DebugDraw.Point(MaxGuidePoint, Color.blue, 0.01f);
                DebugDraw.Point(progressLineEndAdjustedForAttachedScrew, Color.white, 0.01f);
            }

            snapEnabledElement.UpdateSnapElementProgressAlongDrivePath(VectorProjection.GetPercentagePointProgressAlongLine(
                MaxGuidePoint,
                progressLineEndAdjustedForAttachedScrew,
                snapEnabledElement.SnapRaycastOrigin.transform.position
            ));
        }
        else
        {
            snapEnabledElement.UpdateSnapElementProgressAlongDrivePath(VectorProjection.GetPercentagePointProgressAlongLine(
                MaxGuidePoint,
                TargetOrigin.position,
                snapEnabledElement.SnapRaycastOrigin.transform.position
            ));
        }

        if (Math.Abs(previousProgress - snapEnabledElement.CurrentAttachedSnapElementProgressAlongDrivePath) > 0.001f)
        {
            SnapElementProgressAlongDrivePathChanged?.Invoke(new SnapElementProgressAlongDrivePath(snapEnabledElement.CurrentAttachedSnapElementProgressAlongDrivePath, snapEnabledElement));

            if (snapEnabledElement.CurrentAttachedSnapElementProgressAlongDrivePath >= ProgressPercentageToConsiderPathFinished)
            {
                if (snapEnabledElement is StickyTool stickyTool) //different handling for sticky-tool, just stick and don't lock in place
                {
                    var firstStickableElement = FindFirstStickableElement();
                    if (!stickyTool.IsElementAlreadySticked && firstStickableElement != null)
                    {
                        DetachElement(firstStickableElement);
                        stickyTool.StickGuidedSnapElement(firstStickableElement);
                    }
                }
                else
                {
                    snapEnabledElement.LockInPlace(TargetOrigin.position, snapEnabledElement.transform.rotation);
                    if (snapEnabledElement is RotationProgressElement rotationProgressElement && rotationProgressElement.AllowAddingRotationProgressWithNoTools)
                    {
                        rotationProgressElement.StartUsingNoToolMode();
                    }

                    if (snapEnabledElement is RotationTool rotationTool)
                    {
                        rotationTool.LockInPlace(TargetOrigin.position, transform.rotation); //rotation tool is always locked in place
                        rotationTool.StartRotating(_currentlyAttachedRotationProgressElement);
                    }
                }

                snapEnabledElement.TriggerSnappedInFinishedPosition();
                SnapElementProgressAlongDrivePathFinished?.Invoke(new SnapElementProgressAlongDrivePath(snapEnabledElement.CurrentAttachedSnapElementProgressAlongDrivePath, snapEnabledElement));
            }
        }
    }



    private bool ShouldDetach(GuidedSnapEnabledElement snapEnabledElement)
    {
        bool IsBeoyndDetachDistance(Vector3 point, Vector3 deatachDistanceSize)
        {
            return (Mathf.Abs(point.x) > deatachDistanceSize.x / 2)
                   || (Mathf.Abs(point.y) > deatachDistanceSize.y / 2)
                   || (Mathf.Abs(point.z) > deatachDistanceSize.z / 2);
        }

        if (snapEnabledElement.TransformPositionDriver == null) return false;

        if (snapEnabledElement.ShouldPreventDetach(DetachNotAllowedIfRotationProgressMoreThan))
        {
            return false;
        }

        if (!snapEnabledElement.IsConsideredTool && snapEnabledElement.IsLockedAtPosition && snapEnabledElement.ElementDriverRegisteredInFrame == Time.frameCount)
        {
            return true;
        }

        var axisDistanceFromDriver = DrivePathCenterPoint - snapEnabledElement.TransformPositionDriver.transform.position;
        return IsBeoyndDetachDistance(axisDistanceFromDriver, AutoDetachDistance);
    }

    public void AttachGuidedSnapEnabledElement(GuidedSnapEnabledElement guidedSnapEnabledElement)
    {
        var rotationToAlignSnapElementOriginWithTargetOrigin = GetRotationAlignedWithTargetPathDirection(guidedSnapEnabledElement);

        guidedSnapEnabledElement.LockInPlace(TargetOrigin.position, rotationToAlignSnapElementOriginWithTargetOrigin);
        guidedSnapEnabledElement.CurrentlySnappedToTarget = this;
        guidedSnapEnabledElement.XRFrameworkTransformControl.TakeControlFromXrFramework(); 

        var changeParentOnAttachmentStateChangeForGuidedSnapEnabledElement = guidedSnapEnabledElement.GetComponent<ChangeParentOnAttachmentStateChange>();
        if (changeParentOnAttachmentStateChangeForGuidedSnapEnabledElement != null)
        {
            changeParentOnAttachmentStateChangeForGuidedSnapEnabledElement.ParentToGuidedSnapTarget(this);
        }

        StartCoroutine(SetInitiallyAttachedElementInitialRotation());

        SetCurrentlyAttachedElement(guidedSnapEnabledElement);
    }

    private void SetCurrentlyAttachedElement(GuidedSnapEnabledElement guidedSnapEnabledElement)
    {
        if (guidedSnapEnabledElement is RotationProgressElement rpe) _currentlyAttachedRotationProgressElement = rpe; 
        else if (guidedSnapEnabledElement is InsertElement ie) _currentlyAttachedInsertElement = ie;
        else if (guidedSnapEnabledElement is RotationTool rt) _currentlyAttachedTool = rt;
        else if (guidedSnapEnabledElement is StickyTool st) _currentlyAttachedStickyTool = st;
        else throw new Exception($"Unable to {nameof(SetCurrentlyAttachedElement)} - type not handled");
    }

    IEnumerator SetInitiallyAttachedElementInitialRotation()
    {
        yield return new WaitForEndOfFrame();

        if (_currentlyAttachedRotationProgressElement != null)
        {
            _currentlyAttachedRotationProgressElement.UpdateLockedAtPositionBasedOnProgress(_initiallyAttachedElementRotationProgress);
        }
    }
    public TryAttachResult TryAttach(GuidedSnapEnabledElement snapElement) => TryAttach(snapElement, null);
    public TryAttachResult TryAttach(GuidedSnapEnabledElement snapElement, List<TryAttachStatus> skipChecksFor)
    {
        bool IsCheckEnabled(TryAttachStatus check)
        {
            return skipChecksFor == null || !skipChecksFor.Contains(check);
        }
        
        if(IsCheckEnabled(TryAttachStatus.UnableToAttachTargetOnIgnoreRaycastLayer) && gameObject.layer == Layer.IgnoreRaycast)
            return CreateResultWithEventInvoke(snapElement, TryAttachStatus.UnableToAttachTargetOnIgnoreRaycastLayer, TryAttachFailed);

        if (IsCheckEnabled(TryAttachStatus.UnableToAttachInvalidTag) 
            && (LimitSnapTargetsToTags.Count > 0 && (!snapElement.HasTag() || !LimitSnapTargetsToTags.Contains(snapElement.tag)))
        )
            return CreateResultWithEventInvoke(snapElement, TryAttachStatus.UnableToAttachInvalidTag, TryAttachFailed);

        if (snapElement is RotationTool)
        {
            if(IsCheckEnabled(TryAttachStatus.UnableToAttachNoRotationProgressElementLocked) && CurrentlyAttachedRotationProgressElement == null)
                return CreateResultWithEventInvoke(snapElement, TryAttachStatus.UnableToAttachNoRotationProgressElementLocked, TryAttachFailed);

            if(IsCheckEnabled(TryAttachStatus.UnableToAttachRotationToolAsRotationProgressElementDoesNotAllowTag)
                && (snapElement.HasTag() && CurrentlyAttachedRotationProgressElement.LimitAttachToRotationToolsWithTag.Count > 0 
                && !CurrentlyAttachedRotationProgressElement.LimitAttachToRotationToolsWithTag.Contains(snapElement.tag))
            )
                return CreateResultWithEventInvoke(snapElement, TryAttachStatus.UnableToAttachRotationToolAsRotationProgressElementDoesNotAllowTag, TryAttachFailed);
        }

        if (snapElement is StickyTool st)
        {
            var firstStickableElement = FindFirstStickableElement();
            if (IsCheckEnabled(TryAttachStatus.UnableToAttachStickyToolNothingAttachableOnTarget) && firstStickableElement == null)
                return CreateResultWithEventInvoke(snapElement, TryAttachStatus.UnableToAttachStickyToolNothingAttachableOnTarget, TryAttachFailed);

            if (IsCheckEnabled(TryAttachStatus.UnableToAttachStickyToolSomethingAlreadyAttached) && st.IsElementAlreadySticked)
                return CreateResultWithEventInvoke(snapElement, TryAttachStatus.UnableToAttachStickyToolSomethingAlreadyAttached, TryAttachFailed);

            //TODO: edge case if InsertElement is on target need to check if it's not 'locked' eg insert already has bolts in
        }

        if (IsCheckEnabled(TryAttachStatus.SameElementAlreadyAttached)
            && (CurrentlyAttachedRotationProgressElement == snapElement || _currentlyAttachedTool == snapElement)
        )
            return CreateResultWithEventInvoke(snapElement, TryAttachStatus.SameElementAlreadyAttached, TryAttachFailed);

        if (IsCheckEnabled(TryAttachStatus.UnableToAttachOtherElementAlreadyAttached)
            && (snapElement is RotationProgressElement && (CurrentlyAttachedRotationProgressElement != null || _currentlyAttachedInsertElement != null)
            || snapElement is RotationTool && (_currentlyAttachedTool != null || _currentlyAttachedInsertElement != null)
            || snapElement is InsertElement && (CurrentlyAttachedRotationProgressElement != null || _currentlyAttachedInsertElement != null))
        )
            return CreateResultWithEventInvoke(snapElement, TryAttachStatus.UnableToAttachOtherElementAlreadyAttached, TryAttachFailed);

        var hitAngle = Vector3.Angle(SnapDirection, snapElement.SnapRaycastOrigin.transform.up);
        if (IsDebug) DebugDraw.Text(snapElement.SnapRaycastOrigin.position, $"TryAttach: Angle between target and origin UP: {hitAngle}, required: {AllowedAngleRange.x} to {AllowedAngleRange.y}", Color.yellow);
        if (!IsCheckEnabled(TryAttachStatus.UnableToAttachAngleNotWithinRange) || hitAngle >= AllowedAngleRange.x && hitAngle <= AllowedAngleRange.y)
        {
            var progressDiffAlongDrivePath = GetProgressDiffAlongDrivePathForTransformDriverMovement(snapElement);
            if (IsDebug) DebugDraw.Text(transform.position + new Vector3(0, -.1f, 0), $"Acceleration towards guide path: {progressDiffAlongDrivePath}, required min: {AccelerationTowardsGuidePathRequiredToAttach}", Color.magenta);
            if (IsCheckEnabled(TryAttachStatus.UnableToAttachNotCorrectDriverAcceleration)
                && (Math.Abs(progressDiffAlongDrivePath) < 0.001f || progressDiffAlongDrivePath < AccelerationTowardsGuidePathRequiredToAttach))
            {
                return CreateResultWithEventInvoke(snapElement, TryAttachStatus.UnableToAttachNotCorrectDriverAcceleration, TryAttachFailed);
            }

            if (snapElement is RotationProgressElement rotationProgressElement)
            {
                if (IsCheckEnabled(TryAttachStatus.UnableToAttachProgressElementLengthDeterminationModeMismatch)
                    && rotationProgressElement.RotationProgressLengthDeterminedBy != RotationProgressLengthDeterminedBy)
                {
                    return CreateResultWithEventInvoke(snapElement, TryAttachStatus.UnableToAttachProgressElementLengthDeterminationModeMismatch, TryAttachFailed);
                }

                _currentlyAttachedRotationProgressElement = rotationProgressElement;
            }
            else if (snapElement is RotationTool rotationTool)
            {
                _currentlyAttachedTool = rotationTool;
            }
            else if (snapElement is InsertElement guidedSnapEnabledElement)
            {
                _currentlyAttachedInsertElement = guidedSnapEnabledElement;
                _currentlyAttachedInsertElement.AllowRotationProgressElementsAttachment();
            }
            else if (snapElement is StickyTool stickyTool)
            {
                _currentlyAttachedStickyTool = stickyTool;
            }
            
            snapElement.Snap(this);
            ElementAttached?.Invoke(snapElement);
            return CreateResultWithEventInvoke(snapElement, TryAttachStatus.Attached, TryAttachSucceeded);
        }
        else
        {
            return CreateResultWithEventInvoke(snapElement, TryAttachStatus.UnableToAttachAngleNotWithinRange, TryAttachFailed);
        }
        
        return CreateResultWithEventInvoke(snapElement, TryAttachStatus.Unknown, TryAttachSucceeded);
    }

    private TryAttachResult CreateResultWithEventInvoke(GuidedSnapEnabledElement snapElement, TryAttachStatus tryAttachStatus, TryAttachEvent @event)
    {
        var result = new TryAttachResult(tryAttachStatus, snapElement);
        _lastTryAttachStatus = tryAttachStatus;
        @event?.Invoke(result);
        return result;
    }

    private float GetProgressDiffAlongDrivePathForTransformDriverMovement(GuidedSnapEnabledElement snapElement)
    {
        var velocityTrackerSamples = snapElement.TransformPositionDriver.VelocityTracker.GetSamples(2);
        if (velocityTrackerSamples.Count < 2) return 0f;

        const float guidePointEndExtensionMultiplier = 10f; //for short guide paths clamp might not be long enough to give correct value, this will make sure it is. Which prevents issues like NotCorrectDriverAcceleration
        var extendedGuidePoint = GenerateEndGuidePoint(guidePointEndExtensionMultiplier);
        var currentDriverProgressAlongDrivePath = VectorProjection.GetPercentagePointProgressAlongLine(
            MaxGuidePoint,
            TargetOrigin.position,
            VectorProjection.ClampPoint(
                snapElement.TransformPositionDriver.transform.position,
                TargetOrigin.position,
                extendedGuidePoint
            )
        );

        var previousDriverProgressAlongDrivePath = VectorProjection.GetPercentagePointProgressAlongLine(
            MaxGuidePoint,
            TargetOrigin.position,
            VectorProjection.ClampPoint(
                velocityTrackerSamples[1],
                TargetOrigin.position,
                extendedGuidePoint
            )
        );

        return currentDriverProgressAlongDrivePath - previousDriverProgressAlongDrivePath;
    }

    void Reset()
    {
        TrySetOriginObject();
        if (TargetOrigin == null)
        {
            var newGameObject = new GameObject
            {
                name = "Origin"
            };
            newGameObject.transform.parent = transform;
        }

        ResetCapsuleCollider();
    }

    [ContextMenu("Set Origin Object")]
    private void TrySetOriginObject()
    {
        var origin = gameObject.transform.Find("Origin");
        if (origin != null)
        {
            _targetOrigin = origin.transform;
        }
    }

    [ContextMenu("Reset Capsule Collider")]
    private void ResetCapsuleCollider()
    {
        var capsuleCollider = TargetOrigin.GetComponent<CapsuleCollider>();
        if (capsuleCollider == null)
        {
            var colliderScript = GuidedSnapTargetCollider.Create(TargetOrigin.gameObject, this);
            capsuleCollider = TargetOrigin.gameObject.AddComponent<CapsuleCollider>();
            _guidedSnapTargetCollider = colliderScript;
        }

        var scaleAdjustment = transform.lossyScale.x; //not ideal as it only looks at single axis
        capsuleCollider.center = TargetOrigin.transform.InverseTransformPoint(TargetOrigin.position + ((MaxGuidePoint - TargetOrigin.position) / 2));
        capsuleCollider.radius = GuideRadius / scaleAdjustment;
        capsuleCollider.height = (MaxGuidePoint - TargetOrigin.position).magnitude / scaleAdjustment;
        capsuleCollider.direction = YColliderDirectionIntRepresentation;
        capsuleCollider.isTrigger = true;
    }

    void OnValidate()
    {
        TrySetOriginObject();
        ResetCapsuleCollider();
    }

    void OnDrawGizmos()
    {
        if (IsDebug) {
#if UNITY_EDITOR
            var maxGuidePoint = MaxGuidePoint;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(TargetOrigin.position, maxGuidePoint);

            Handles.color = Color.green;
            Handles.DrawWireDisc(TargetOrigin.position, SnapDirection, GuideRadius);

            Handles.color = Color.yellow;
            Handles.DrawWireDisc(maxGuidePoint, SnapDirection, GuideRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(DrivePathCenterPoint, AutoDetachDistance);

            DebugDraw.LocalDirections(TargetOrigin);
#endif
        }
    }
}

public enum TryAttachStatus
{
    Unknown,
    SameElementAlreadyAttached,
    Attached,
    UnableToAttachAngleNotWithinRange,
    UnableToAttachInvalidTag,
    UnableToAttachOtherElementAlreadyAttached,
    UnableToAttachNotCorrectDriverAcceleration,
    UnableToAttachNoRotationProgressElementLocked,
    UnableToAttachRotationToolAsRotationProgressElementDoesNotAllowTag,
    UnableToAttachProgressElementLengthDeterminationModeMismatch, 
    UnableToAttachStickyToolNothingAttachableOnTarget, 
    UnableToAttachStickyToolSomethingAlreadyAttached,
    UnableToAttachTargetOnIgnoreRaycastLayer
}

public class RotationChanged
{
    public RotationProgressElement RotationProgressElement { get; }
    public float CurrentProgress { get; }
    public float MovedAngle { get; }
    public AddRotationProgressStatus AddRotationProgressStatus { get; }
    public TransformPositionDriver TransformPositionDriver { get; }
    public RotationTool RotationTool { get; }

    public RotationChanged(RotationProgressElement rotationProgressElement, float currentProgress, 
        AddRotationProgressStatus addRotationProgressStatus, float movedAngle, TransformPositionDriver transformPositionDriver, RotationTool rotationTool)
    {
        RotationProgressElement = rotationProgressElement;
        CurrentProgress = currentProgress;
        AddRotationProgressStatus = addRotationProgressStatus;
        MovedAngle = movedAngle;
        TransformPositionDriver = transformPositionDriver;
        RotationTool = rotationTool;
    }
}

public class SnapElementProgressAlongDrivePath
{
    public float Progress { get; }
    GuidedSnapEnabledElement GuidedSnapEnabledElement { get; }
    public bool IsFinished => Progress >= 1f;

    public SnapElementProgressAlongDrivePath(float progress, GuidedSnapEnabledElement guidedSnapEnabledElement)
    {
        Progress = progress;
        GuidedSnapEnabledElement = guidedSnapEnabledElement;
    }
}

public class TryAttachResult
{
    public TryAttachStatus TryAttachStatus { get; }
    public GuidedSnapEnabledElement GuidedSnapEnabledElement { get; }

    public TryAttachResult(TryAttachStatus tryAttachStatus, GuidedSnapEnabledElement guidedSnapEnabledElement)
    {
        TryAttachStatus = tryAttachStatus;
        GuidedSnapEnabledElement = guidedSnapEnabledElement;
    }
}