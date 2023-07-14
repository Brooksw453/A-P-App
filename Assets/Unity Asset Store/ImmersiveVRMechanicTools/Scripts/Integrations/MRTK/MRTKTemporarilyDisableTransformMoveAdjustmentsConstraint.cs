#if INTEGRATIONS_MRTK

using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;

public class MRTKTemporarilyDisableTransformMoveAdjustmentsConstraint : TransformConstraint
{
    public override TransformFlags ConstraintType { get; } = TransformFlags.Move;

    public override void ApplyConstraint(ref MixedRealityTransform transform)
    {
        transform.Position = worldPoseOnManipulationStart.Position;
    }
}

#endif