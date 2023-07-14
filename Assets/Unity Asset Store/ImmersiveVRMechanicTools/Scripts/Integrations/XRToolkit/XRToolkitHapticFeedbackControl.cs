#if INTEGRATIONS_XRTOOLKIT

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRController))]
public class XRToolkitHapticFeedbackControl : HapticFeedbackControl
{
    private XRController _xrController;

    void Start()
    {
        _xrController = GetComponent<XRController>();
    }


    public override void SendHapticFeedback(float amplitude, float duration)
    {
        _xrController.SendHapticImpulse(amplitude, duration);
    }
}

#endif