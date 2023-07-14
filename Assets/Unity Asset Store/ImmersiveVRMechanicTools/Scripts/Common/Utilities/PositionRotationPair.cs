using UnityEngine;

namespace ReliableSolutions.Unity.Common.Utilities
{
    public struct PositionRotationPair
    {
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }

        public PositionRotationPair(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }
}