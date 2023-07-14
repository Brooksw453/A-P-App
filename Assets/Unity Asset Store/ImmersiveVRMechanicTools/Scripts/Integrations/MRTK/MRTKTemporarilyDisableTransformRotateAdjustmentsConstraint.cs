#if INTEGRATIONS_MRTK

using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

public class MRTKTemporarilyDisableTransformRotateAdjustmentsConstraint : TransformConstraint
{
    public override TransformFlags ConstraintType { get; } = TransformFlags.Rotate;

    public Quaternion LockRotationTo { get; set; } = Quaternion.identity;

    public override void ApplyConstraint(ref MixedRealityTransform transform)
    {
        //WARN: using worldPoseOnManipulationStart is giving some odd rotation results, looks like something is already applied and in some cases that results with incorrect alignment
        transform.Rotation = LockRotationTo != Quaternion.identity ? LockRotationTo : this.transform.rotation;
    }
}

#endif