using System;
using ReliableSolutions.Unity.Common.Debug;
using UnityEngine;

namespace ReliableSolutions.Unity.Common.Utilities
{
    public class AngleBetweenPoints
    {
        public static bool GlobalDebug = false;
        public static float GlobalScale = 1f;

        [Obsolete("Use GetThreePointAngleSigned180 instead")]
        public static float GetThreePointAngle(Vector3 first, Vector3 second, Vector3 pivot, Vector3 axis, bool debug = false)
        {
            var d1 = (pivot - first);
            var d2 = (pivot - second);

            var angleDifference = Mathf.Atan2(
                Vector3.Dot(axis, Vector3.Cross(d1, d2)),
                Vector3.Dot(d1, d2)) * Mathf.Rad2Deg;

            if (debug || GlobalDebug)
            {
                DebugDraw.Line(pivot, first, Color.green);
                DebugDraw.Point(first, Color.green, 0.008f * GlobalScale);

                DebugDraw.Line(pivot, second, Color.red);
                DebugDraw.Point(second, Color.red, 0.010f * GlobalScale);

                DebugDraw.Ray(pivot, axis, Color.yellow);
                DebugDraw.Point(pivot, Color.blue, 0.012f * GlobalScale);

                DebugDraw.Text(pivot + new Vector3(0, -0.1f, 0), $"Angle: {angleDifference}, axis: {axis}", Color.magenta);
            }

            return angleDifference;
        }

        public static float GetThreePointAngleSigned180(Vector3 first, Vector3 second, Vector3 pivot, Vector3 axis, bool debug = false)
        {
            return GetThreePointAngle(first, second, pivot, axis, debug);
        }

        public static float GetThreePointAngleUnsigned360(Vector3 first, Vector3 second, Vector3 pivot, Vector3 axis,  bool debug = false)
        {
            var angle180 = GetThreePointAngle(first, second, pivot, axis, debug);

            return AdjustSigned180AngleToUnsigned360(angle180);
        }

        public static float AdjustSigned180AngleToUnsigned360(float angle180)
        {
            //TODO: this is 'right/positive' direction? do that needs to be specified?
            var angle360 = angle180 < 0
                ? -angle180
                : angle180 <= 180
                    ? 360 - angle180
                    : -1;
            return angle360;
        }
    }
}
