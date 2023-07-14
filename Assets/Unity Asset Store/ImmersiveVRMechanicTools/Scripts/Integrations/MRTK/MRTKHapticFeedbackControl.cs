#if INTEGRATIONS_MRTK

using Microsoft.MixedReality.Toolkit.Input;

public class MRTKHapticFeedbackControl : HapticFeedbackControl
{
    private IMixedRealityHapticFeedback _hapticsEnabledController;

    private IMixedRealityHapticFeedback HapticsEnabledController => _hapticsEnabledController ??= gameObject.GetComponent<BaseControllerPointer>().Controller as IMixedRealityHapticFeedback;

    public override void SendHapticFeedback(float amplitude, float duration)
    {
        HapticsEnabledController?.StartHapticImpulse(amplitude, duration);
    }
}

#endif