using UnityEngine;

namespace ReliableSolutions.Unity.Common.Debug
{
    public class DebugLogger : MonoBehaviour
    {
        [SerializeField] private string MessagePrefix = "CustomDebug: ";

        public void Log(string message)
        {
            UnityEngine.Debug.Log(MessagePrefix + message);
        }
    }
}