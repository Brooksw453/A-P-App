#if INTEGRATIONS_XRTOOLKIT

using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class XRToolkitTransformControl : XRFrameworkTransformControl
{
    [SerializeField] private XRGrabInteractable XRGrabInteractable;
    private bool _originalTrackPosition;
    private bool _originalTrackRotation;

    public bool IsFrameworkControllingTransform { get; private set; } = true;

    public override void TakeControlFromXrFramework()
    {
        _originalTrackPosition = XRGrabInteractable.trackPosition;
        XRGrabInteractable.trackPosition = false;

        _originalTrackRotation = XRGrabInteractable.trackRotation;
        XRGrabInteractable.trackRotation = false;

        IsFrameworkControllingTransform = false;
    }

    public override void PassControlBackToXrFramework()
    {
        XRGrabInteractable.trackPosition = _originalTrackPosition;
        XRGrabInteractable.trackRotation = _originalTrackRotation;
        IsFrameworkControllingTransform = true;
    }

    void Reset()
    {
        TrySetXrGrabInteractable();
    }

    void Start()
    {
        TrySetXrGrabInteractable();
    }

    private void TrySetXrGrabInteractable()
    {
        if(XRGrabInteractable == null) XRGrabInteractable = GetComponent<XRGrabInteractable>();
    }
}

#endif