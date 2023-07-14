#if INTEGRATIONS_XRTOOLKIT

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRController))]
public class XRToolkitFrameworkToolInput : XRFrameworkToolInput
{
    [SerializeField] private XRController XRController;

    protected override bool IsIncreasingToolForce()
    {
        if (XRController.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out var v2))
        {
            if (v2.y > 0.3f) return true;
        }

        return false;
    }

    protected override bool IsDecreasingToolForce() 
    {
        if (XRController.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out var v2))
        {
            if (v2.y < -0.3f) return true;
        }

        return false;
    }

    protected override bool IsChangingToolDirection()
    {
        if (XRController.inputDevice.IsPressed(InputHelpers.Button.Primary2DAxisClick, out var isPressed))
        {
            return isPressed;
        }
        return false;
    }

    void Reset()
    {
        XRController = GetComponent<XRController>();
    }
}

#endif