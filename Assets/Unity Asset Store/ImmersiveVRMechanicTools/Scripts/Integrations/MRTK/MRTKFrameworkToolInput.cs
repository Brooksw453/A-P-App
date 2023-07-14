#if INTEGRATIONS_MRTK

using System;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;


public class MRTKFrameworkToolInput : XRFrameworkToolInput, IMixedRealityInputActionHandler
{
    private IMixedRealityInputSource _mixedRealityInputSource;
    private int _toolDirectionPressedFrame = -1;

    private bool _isInitialized;
    private MixedRealityInputAction _changeToolDirectionAction;

    public void Initialize(MixedRealityInputAction changeToolDirectionAction)
    {
        _isInitialized = true;
        _changeToolDirectionAction = changeToolDirectionAction;
    }

    void Start()
    {
        if (!_isInitialized) throw new Exception("You need to call initialize first");
        _mixedRealityInputSource = gameObject.GetComponent<BaseControllerPointer>().Controller.InputSource;
    }

    private void OnEnable()
    {
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityInputActionHandler>(this);
    }

    private void OnDisable()
    {
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityInputActionHandler>(this);
    }

    protected override bool IsIncreasingToolForce()
    {
        return false; //TODO implement
    }

    protected override bool IsDecreasingToolForce()
    {
        return false; //TODO implement
    }

    protected override bool IsChangingToolDirection()
    {
        return Time.frameCount == _toolDirectionPressedFrame;
    }

    public void OnActionStarted(BaseInputEventData eventData)
    {
        if (eventData.InputSource == _mixedRealityInputSource)
        {
            if (eventData.MixedRealityInputAction == _changeToolDirectionAction)
            {
                _toolDirectionPressedFrame = Time.frameCount;
            }
        }

    }

    public void OnActionEnded(BaseInputEventData eventData)
    {
    }
}

#endif