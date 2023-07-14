using UnityEngine;

namespace ReliableSolutions.Unity.Common.Extensions
{
    public static class ComponentExtensions
    {
        public static bool HasTag(this Component component)
        {
            return !string.IsNullOrWhiteSpace(component.tag) && component.tag != "Untagged";
        }
    }
}
