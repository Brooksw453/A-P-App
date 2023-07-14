using System;
using UnityEngine;

public abstract class HapticFeedbackControl : MonoBehaviour
{
    public abstract void SendHapticFeedback(float amplitude, float duration);
}
