#if INTEGRATIONS_VRIF
using BNG;

public class VRIFFrameworkToolInput : XRFrameworkToolInput
{
    private InputBridge _inputBridge;
    private InputBridge InputBridge => _inputBridge ?? (_inputBridge = FindObjectOfType<InputBridge>());

    protected override bool IsIncreasingToolForce()
    {
        return IsInputAxisVerticallyOverThreshold(InputAxis.LeftThumbStickAxis) || IsInputAxisVerticallyOverThreshold(InputAxis.RightThumbStickAxis);
    }

    protected override bool IsDecreasingToolForce()
    {
        return IsInputAxisVerticallyUnderThreshold(InputAxis.LeftThumbStickAxis) || IsInputAxisVerticallyUnderThreshold(InputAxis.RightThumbStickAxis);
    }

    protected override bool IsChangingToolDirection()
    {
        return InputBridge.GetControllerBindingValue(ControllerBinding.LeftThumbstickDown) || InputBridge.GetControllerBindingValue(ControllerBinding.RightThumbstickDown);
    }

    private bool IsInputAxisVerticallyOverThreshold(InputAxis axis)
    {
        var inputAxisValue = InputBridge.GetInputAxisValue(axis);
        if (inputAxisValue.y > 0.3f) return true;
        return false;
    }

    private bool IsInputAxisVerticallyUnderThreshold(InputAxis axis)
    {
        var inputAxisValue = InputBridge.GetInputAxisValue(axis);
        if (inputAxisValue.y < -0.3f) return true;
        return false;
    }
}

#endif