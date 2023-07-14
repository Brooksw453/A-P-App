using UnityEngine;

namespace ReliableSolutions.Unity.Common.Extensions
{
    public static class Vector3Extensions
    {
        public static Vector3 AddAdjustmentOnSingleAxis(this Vector3 vector, Vector3Axis axis, float adjustment)
        {
            return new Vector3(
                axis == Vector3Axis.X ? vector.x + adjustment : vector.x,
                axis == Vector3Axis.Y ? vector.y + adjustment : vector.y,
                axis == Vector3Axis.Z ? vector.z + adjustment : vector.z
            );
        }
    }
}