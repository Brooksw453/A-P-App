using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.TorqueWrenchTools.Scripts.Utilities
{
    public class UnityEventToolsHelper
    {
        public static bool AddPersistentListenerIfNotExists<T>(UnityEvent<T> unityEvent, UnityAction<T> unityAction, UnityEngine.Object target)
        {
#if UNITY_EDITOR
            if (IterateExistingEvents(unityEvent).Any(e => e.MethodName == unityAction.Method.Name && e.Target == target))
            {
                return false;
            }
            UnityEditor.Events.UnityEventTools.AddPersistentListener(unityEvent, unityAction);
            return true;
#elif DEBUG
            Debug.LogWarning("Can't run in non-editor mode");
            return false;
#else
            return false;
#endif
        }

        public static IEnumerable<UnityEventMetadata> IterateExistingEvents<T>(UnityEvent<T> unityEvent)
        {
#if UNITY_EDITOR
            for (var i = 0; i < unityEvent.GetPersistentEventCount(); ++i)
            {
                yield return new UnityEventMetadata(
                    unityEvent.GetPersistentTarget(i),
                    unityEvent.GetPersistentMethodName(i)
                );
            }
#elif DEBUG
            Debug.LogWarning("Can't run in non-editor mode");
            return Enumerable.Empty<UnityEventMetadata>();
#else
            return Enumerable.Empty<UnityEventMetadata>();
#endif
        }

        public class UnityEventMetadata
        {
            public UnityEngine.Object Target { get; }
            public string MethodName { get; }

            public UnityEventMetadata(UnityEngine.Object target, string methodName)
            {
                Target = target;
                MethodName = methodName;
            }
        }
    }
}
