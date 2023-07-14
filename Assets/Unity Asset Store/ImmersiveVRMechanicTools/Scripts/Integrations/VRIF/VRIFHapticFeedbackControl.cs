#if INTEGRATIONS_VRIF

using BNG;
using UnityEngine;

[RequireComponent(typeof(InputBridge))]
public class VRIFHapticFeedbackControl : HapticFeedbackControl
{
    [SerializeField] private float _vibrateFrequency = 1;

    private InputBridge _inputBridge;
    private InputBridge InputBridge => _inputBridge ?? (_inputBridge = GetComponent<InputBridge>());


    public override void SendHapticFeedback(float amplitude, float duration)
    {
        InputBridge.VibrateController(_vibrateFrequency, amplitude, duration, ControllerHand.Left);
        InputBridge.VibrateController(_vibrateFrequency, amplitude, duration, ControllerHand.Right);
    }
}

#endif