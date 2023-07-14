using System;
using System.Collections;
using UnityEngine;

namespace Common.Runtime.Utilities
{
    public class DelayedExecutor
    {
        public static IEnumerator ExecuteOnNextFrame(Action action)
        {
            yield return new WaitForEndOfFrame();
            action();
        }
    }
}