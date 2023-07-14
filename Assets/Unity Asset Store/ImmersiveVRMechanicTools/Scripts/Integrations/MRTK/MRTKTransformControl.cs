using System;
using System.Collections;
using Common.Runtime.Utilities;
using UnityEngine;

#if INTEGRATIONS_MRTK
[RequireComponent(typeof(MRTKTemporarilyDisableTransformMoveAdjustmentsConstraint))]
[RequireComponent(typeof(MRTKTemporarilyDisableTransformRotateAdjustmentsConstraint))]
[RequireComponent(typeof(MRTKTemporarilyDisableTransformScaleAdjustmentsConstraint))]
#endif
public class MRTKTransformControl : XRFrameworkTransformControl
{
#if INTEGRATIONS_MRTK
    [SerializeField] private bool StartWithConstraintsEnabled = false;

    private bool _areConstraintsEnabled = false;
    private MRTKTemporarilyDisableTransformMoveAdjustmentsConstraint _moveAdjustmentsConstraint;
    private MRTKTemporarilyDisableTransformRotateAdjustmentsConstraint _rotateAdjustmentsConstraint;
    private MRTKTemporarilyDisableTransformScaleAdjustmentsConstraint _scaleAdjustmentsConstraint;
    
    private InsertElement _insertElement;
    private GuidedSnapEnabledElement _guidedSnapEnabledElement;
    
    public MRTKTemporarilyDisableTransformMoveAdjustmentsConstraint MoveAdjustmentsConstraint =>
        _moveAdjustmentsConstraint ?? (_moveAdjustmentsConstraint = GetComponent<MRTKTemporarilyDisableTransformMoveAdjustmentsConstraint>());

    public MRTKTemporarilyDisableTransformRotateAdjustmentsConstraint RotateAdjustmentsConstraint =>
        _rotateAdjustmentsConstraint ?? (_rotateAdjustmentsConstraint = GetComponent<MRTKTemporarilyDisableTransformRotateAdjustmentsConstraint>());

    public MRTKTemporarilyDisableTransformScaleAdjustmentsConstraint ScaleAdjustmentsConstraint =>
        _scaleAdjustmentsConstraint ?? (_scaleAdjustmentsConstraint = GetComponent<MRTKTemporarilyDisableTransformScaleAdjustmentsConstraint>());

    void Awake()
    {
        _insertElement = GetComponent<InsertElement>();
        _guidedSnapEnabledElement = GetComponent<GuidedSnapEnabledElement>();
        ChangeAllConstraintsState(StartWithConstraintsEnabled);
    }

    void Update()
    {
        EnsureNotStuckInConstraintsEnabledState();
    }

    private void EnsureNotStuckInConstraintsEnabledState()
    {
        if (_areConstraintsEnabled)
        {
            if (!_guidedSnapEnabledElement.TransformPositionDriver)
            {
                Debug.Log("Object Manipulator constraints are enabled but there's no TransformPositionDriver, likely object got stuck in constraints-enabled state, disabling...");
                ChangeAllConstraintsState(false);
            }
        }
    }

    public override void TakeControlFromXrFramework()
    {
        ChangeAllConstraintsState(true);
    }

    public override void PassControlBackToXrFramework()
    {
        ChangeAllConstraintsState(false);
    }

    private void ChangeAllConstraintsState(bool isEnabled)
    {
        _areConstraintsEnabled = isEnabled;
        MoveAdjustmentsConstraint.enabled = isEnabled;
        if (isEnabled)
        {
            if(_insertElement) _insertElement.SnappedInFinishPosition += OnSnappedInFinishPosition;
        }
        else
        {
            if(_insertElement) _insertElement.SnappedInFinishPosition -= OnSnappedInFinishPosition;
            RotateAdjustmentsConstraint.LockRotationTo = Quaternion.identity;
        }

        RotateAdjustmentsConstraint.enabled = isEnabled;
        ScaleAdjustmentsConstraint.enabled = isEnabled;
    }

    private void OnSnappedInFinishPosition(object sender, EventArgs e)
    {
        if (!_insertElement.IsConsideredTool)
        {
            //HACK: there's a bit of an oddity when sometimes items in fully locked position will change rotation, probably some issue with MRTK moving them, this will lock rotation to snapped-rotation which is correct
            RotateAdjustmentsConstraint.LockRotationTo = transform.rotation;
        }
    }
    
#else
    public override void TakeControlFromXrFramework()
    {
        throw new NotImplementedException();
    }

    public override void PassControlBackToXrFramework()
    {
        throw new NotImplementedException();
    }
#endif
}