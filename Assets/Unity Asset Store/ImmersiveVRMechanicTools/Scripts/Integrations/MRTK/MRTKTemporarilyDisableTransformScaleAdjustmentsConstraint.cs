#if INTEGRATIONS_MRTK

using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

public class MRTKTemporarilyDisableTransformScaleAdjustmentsConstraint : TransformConstraint
{
    public override TransformFlags ConstraintType { get; } = TransformFlags.Scale;

    private ChangeParentOnAttachmentStateChange _changeParentOnAttachmentStateChange;
    void Awake()
    {
        _changeParentOnAttachmentStateChange = GetComponent<ChangeParentOnAttachmentStateChange>();
    }

    public override void Initialize(MixedRealityTransform worldPose)
    {
        //if there's ChangeParentOnAttachmentStateChange initial pose needs to be adjusted, otherwise on grab (for item that was unparented) wrong scale will be remembered and applied when manipulating on guide
        worldPoseOnManipulationStart = _changeParentOnAttachmentStateChange != null && _changeParentOnAttachmentStateChange.LocalScaleBeforeUnparenting != Vector3.zero
            ? new MixedRealityTransform(worldPose.Position, worldPose.Rotation, _changeParentOnAttachmentStateChange.LocalScaleBeforeUnparenting) 
            : worldPose;
        
    }

    public override void ApplyConstraint(ref MixedRealityTransform transform)
    {
        transform.Scale = worldPoseOnManipulationStart.Scale;
    }
}

#endif