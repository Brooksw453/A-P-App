using UnityEngine;

namespace ReliableSolutions.Unity.Common.Utilities
{
    public class ParentChildMovementWithoutHierarchy
    {
        private readonly Transform _parent;
        private readonly Transform _child;

        private Vector3 _localParentRealativeChildPosition;
        private Vector3 _parentRelativeForwardDirection;
        private Vector3 _parentRelativeUpDirection;

        public ParentChildMovementWithoutHierarchy(Transform parent, Transform child)
        {
            _parent = parent;
            _child = child;
        }

        public void Initialize()
        {
            _localParentRealativeChildPosition = _parent.InverseTransformPoint(_child.position);
            _parentRelativeForwardDirection = _parent.InverseTransformDirection(_child.forward);
            _parentRelativeUpDirection = _parent.InverseTransformDirection(_child.up);
        }

        public PositionRotationPair GenerateParentAdjustedChildPositionRotationPair()
        {
            var newPosition = _parent.TransformPoint(_localParentRealativeChildPosition);
            var newForward = _parent.TransformDirection(_parentRelativeForwardDirection);
            var newUp = _parent.TransformDirection(_parentRelativeUpDirection);
            var newRotation = Quaternion.LookRotation(newForward, newUp);

            return new PositionRotationPair(newPosition, newRotation);
        }
    }
}