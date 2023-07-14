#if INTEGRATIONS_VRIF

using System.Reflection;
using BNG;
using UnityEngine;

[RequireComponent(typeof(Grabbable))]
public class VRIFTransformControl : XRFrameworkTransformControl
{
    [SerializeField] private Grabbable Grabbable;
    private bool _originalRemoteGrabbing;
    private bool _originalBeingHeld;

    public bool IsFrameworkControllingTransform { get; private set; } = true;

    public static FieldInfo RemoteGrabbingField = typeof(Grabbable).GetField("remoteGrabbing", BindingFlags.Instance | BindingFlags.NonPublic);

    public override void TakeControlFromXrFramework()
    {
        _originalRemoteGrabbing = (bool)RemoteGrabbingField.GetValue(Grabbable);
        RemoteGrabbingField.SetValue(Grabbable, false);;

        _originalBeingHeld = Grabbable.BeingHeld;
        Grabbable.BeingHeld = false;

        IsFrameworkControllingTransform = false;
    }

    public override void PassControlBackToXrFramework()
    {
        RemoteGrabbingField.SetValue(Grabbable, _originalRemoteGrabbing);
        Grabbable.BeingHeld = _originalBeingHeld;

        IsFrameworkControllingTransform = true;
    }

    void Reset()
    {
        TrySetGrabbable();
    }

    void Start()
    {
        TrySetGrabbable();
    }

    private void TrySetGrabbable()
    {
        if(Grabbable == null) Grabbable = GetComponent<Grabbable>();
    }
}

#endif