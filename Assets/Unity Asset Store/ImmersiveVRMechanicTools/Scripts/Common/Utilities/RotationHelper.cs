
using UnityEngine;

namespace ReliableSolutions.Unity.Common.Utilities
{
    public static class RotationHelper
    {
        public static Quaternion GetQuaternionRotationChildRelativeParentApplicable(Quaternion targetRotation, Quaternion parentRotation, Quaternion childRotation)
        {
            var relativeDirectionPointRotationToParent = Quaternion.Inverse(parentRotation) * childRotation;
            var newRotation = targetRotation * Quaternion.Inverse(relativeDirectionPointRotationToParent);
            return newRotation;
        }

        public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
        {
            return Quaternion.Euler(angles) * (point - pivot) + pivot;
        }

        public static PositionRotationPair RotateAround(PositionRotationPair original, Vector3 center, Vector3 axis, float angle)
        {
            var rot = Quaternion.AngleAxis(angle, axis); // get the desired rotation
            var dir = original.Position - center; // find current direction relative to center
            dir = rot * dir; // rotate the direction
            var resultPosition = center + dir; // define new position
            // rotate object to keep looking at the center:
            var myRot = original.Rotation;
            var resultRotation = original.Rotation * Quaternion.Inverse(myRot) * rot * myRot;

            return new PositionRotationPair(resultPosition, resultRotation);
        }
    }
}
