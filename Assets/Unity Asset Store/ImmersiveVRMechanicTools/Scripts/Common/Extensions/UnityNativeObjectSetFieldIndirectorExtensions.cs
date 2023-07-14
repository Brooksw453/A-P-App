using System.Collections.Generic;
using UnityEngine;

namespace ReliableSolutions.Unity.Common.Extensions
{
    public static class UnityNativeObjectSetFieldIndirectorExtensions
    {
        #region GameObject

        public static void SetLayer(this GameObject go, int layer) => go.layer = layer;
        public static void SetActiveTracked(this GameObject go, bool isActive) => go.SetActive(isActive);

        #endregion

        #region Transform

        public static void SetPosition(this Transform t, Vector3 position) => t.position = position;
        public static void SetLocalPosition(this Transform t, Vector3 localPosition) => t.localPosition = localPosition;

        public static void SetRotation(this Transform t, Quaternion rotation) => t.rotation = rotation;
        public static void SetLocalRotation(this Transform t, Quaternion localRotation) => t.localRotation = localRotation;

        public static void SetLocalScale(this Transform t, Vector3 localScale) => t.localScale = localScale;

        //Name needs to be different as there is already SetParent on transform
        public static void SetParentTracked(this Transform t, Transform parent) => t.SetParentTracked(parent, true);
        public static void SetParentTracked(this Transform t, Transform parent, bool worldPositionStays) => t.SetParent(parent, worldPositionStays);

        public static void RotateAroundTracked(this Transform t, Vector3 point, Vector3 axis, float angle) => t.RotateAround(point, axis, angle);
        public static void RotateTracked(this Transform t, Vector3 axis, float angle, Space relativeTo) => t.Rotate(axis, angle, relativeTo);
        
        #endregion

        #region Collider

#if DEBUG_ADDITIONAL_NATIVE_UNITY_CALLS_TRACKING_VIA_EXTENSION_METHODS
        public class CollisionIgnoreReference
        {
            public Collider FirstCollider { get; }
            public Collider SecondCollider { get; }

            public CollisionIgnoreReference(Collider firstCollider, Collider secondCollider)
            {
                FirstCollider = firstCollider;
                SecondCollider = secondCollider;
            }

            public override int GetHashCode()
            {
                return FirstCollider.GetHashCode() + SecondCollider.GetHashCode();
            }
        }

        public static readonly HashSet<CollisionIgnoreReference> _DebugCollisionIgnoreReferences = new HashSet<CollisionIgnoreReference>();
#endif

        //Bit of a reach for class name, but would rather keep them in single file, raname?
        public static void IgnoreCollision(this Collider collider1, Collider collider2, bool ignore)
        {
            Physics.IgnoreCollision(collider1, collider2, ignore);

#if DEBUG_ADDITIONAL_NATIVE_UNITY_CALLS_TRACKING_VIA_EXTENSION_METHODS
            var reference = new CollisionIgnoreReference(collider1, collider2);
            if (ignore) _DebugCollisionIgnoreReferences.Add(reference);
            else _DebugCollisionIgnoreReferences.Remove(reference);

            UnityEngine.Debug.Log($"{(ignore ? "IGNORING" : "NOT IGNORING")} collisions via Physics.IgnoreCollision: {collider1.gameObject.name} - {collider2.gameObject.name}");
#endif
        }

#endregion

    }
}
