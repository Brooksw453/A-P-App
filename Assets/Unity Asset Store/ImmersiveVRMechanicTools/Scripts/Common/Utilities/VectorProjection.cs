using ReliableSolutions.Unity.Common.Debug;
using UnityEngine;

namespace ReliableSolutions.Unity.Common.Utilities
{
    public static class VectorProjection
    {
        public static bool GlobalDebug = false; 
        public static float GlobalScale = 1f;

        public static float GetPercentagePointProgressAlongLine(Vector3 lineStart, Vector3 lineEnd, Vector3 point, bool isDebug = false)
        {
            if(isDebug || GlobalDebug) DrawDebug(point, lineStart, lineEnd);

            var ab = lineEnd - lineStart;
            var ac = point - lineStart;
            return Vector3.Dot(ac, ab.normalized) / ab.magnitude;
        }

        public static Vector3 ClampPoint(Vector3 point, Vector3 segmentStart, Vector3 segmentEnd, bool isDebug = false)
        {
            if (isDebug) DrawDebug(point, segmentStart, segmentEnd);

            return ClampProjection(ProjectPoint(point, segmentStart, segmentEnd), segmentStart, segmentEnd);
        }

        public static Vector3 ProjectPoint(Vector3 point, Vector3 segmentStart, Vector3 segmentEnd, bool isDebug = false)
        {
            if (isDebug || GlobalDebug) DrawDebug(point, segmentStart, segmentEnd);

            return segmentStart + Vector3.Project(point - segmentStart, segmentEnd - segmentStart);
        }

        private static Vector3 ClampProjection(Vector3 point, Vector3 start, Vector3 end)
        {
            var toStart = (point - start).sqrMagnitude;
            var toEnd = (point - end).sqrMagnitude;
            var segment = (start - end).sqrMagnitude;

            var clampedPoint = point;
            if (toStart > segment || toEnd > segment) clampedPoint = toStart > toEnd ? end : start;

            Debug.DebugDraw.Point(clampedPoint, Color.cyan, 0.005f * GlobalScale);

            return clampedPoint;
        }

        private static void DrawDebug(Vector3 point, Vector3 start, Vector3 end)
        {
            Debug.DebugDraw.Point(start, Color.green, 0.008f * GlobalScale);
            Debug.DebugDraw.Point(end, Color.red, 0.012f * GlobalScale);
            Debug.DebugDraw.Point(point, Color.blue, 0.010f * GlobalScale);
            Debug.DebugDraw.Line(start, end, Color.black);
        }
    }
}
