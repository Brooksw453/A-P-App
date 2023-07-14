#if INTEGRATIONS_VRTK

using UnityEngine;
using Zinnia.Haptics;

[RequireComponent(typeof(XRNodeHapticPulser))]
public class VRTKHapticFeedbackControl : HapticFeedbackControl
{
    private XRNodeHapticPulser _xrNodeHapticPulser;

    void Start()
    {
        _xrNodeHapticPulser = GetComponent<XRNodeHapticPulser>();
    }
    public override void SendHapticFeedback(float amplitude, float duration)
    {
        _xrNodeHapticPulser.Intensity = amplitude;
        _xrNodeHapticPulser.Duration = duration;
        _xrNodeHapticPulser.Begin();
    }
}

#endif