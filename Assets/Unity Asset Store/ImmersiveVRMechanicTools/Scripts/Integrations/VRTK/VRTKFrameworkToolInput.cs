#if INTEGRATIONS_VRTK

using System;
using UnityEngine;


public class VRTKFrameworkToolInput : XRFrameworkToolInput
{
    [SerializeField] private float _currentForceAxisValue;
    [SerializeField] private int _sideChangeButtonClickedInFrame;

    public void RegisterForceAxisValueChangeInverse(float value)
    {
        _currentForceAxisValue = value *-1;
    }

    public void RegisterForceAxisValueChange(float value)
    {
        _currentForceAxisValue = value;
    }

    public void RegisterSideChangeButtonChange(bool isClicked)
    {
        _sideChangeButtonClickedInFrame = isClicked ? Time.frameCount : 0;
    }

    protected override bool IsIncreasingToolForce()
    {
        return Math.Abs(_currentForceAxisValue - -1f) < 0.01f;
    }

    protected override bool IsDecreasingToolForce()
    {
        return Math.Abs(_currentForceAxisValue - 1f) < 0.01f;
    }

    protected override bool IsChangingToolDirection()
    {
        return Math.Abs(Time.frameCount - _sideChangeButtonClickedInFrame) <= 3;
    }
}

#endif