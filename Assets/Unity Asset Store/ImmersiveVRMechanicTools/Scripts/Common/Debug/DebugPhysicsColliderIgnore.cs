using System;
using System.Linq;
using ReliableSolutions.Unity.Common.Extensions;
using UnityEngine;

namespace ReliableSolutions.Unity.Common.Debug
{
    public class DebugPhysicsColliderIgnore : MonoBehaviour
    {

        [ContextMenu("Log to console")]
        public void LogToConsole()
        {
#if DEBUG_ADDITIONAL_NATIVE_UNITY_CALLS_TRACKING_VIA_EXTENSION_METHODS
            var refrences = UnityNativeObjectSetFieldIndirectorExtensions._DebugCollisionIgnoreReferences.ToList();
            UnityEngine.Debug.Log($"Ignoring collisions via Physics.IgnoreCollision:" + Environment.NewLine
                                  + string.Join(Environment.NewLine, refrences.Select(r => $"{r.FirstCollider.gameObject.name} - {r.SecondCollider.gameObject.name}"))); 
#else

            UnityEngine.Debug.LogWarning("DEBUG_ADDITIONAL_NATIVE_UNITY_CALLS_TRACKING_VIA_EXTENSION_METHODS - not specified as build symbol, tracking disabled");
#endif
        }
    }
}