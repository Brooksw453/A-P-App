#if INTEGRATIONS_VRIF

using BNG;
using UnityEngine;

[RequireComponent(typeof(Grabber))]
[RequireComponent(typeof(TransformPositionDriver))]
public class VRIFGuidedSnapElementDriverExtractor : MonoBehaviour
{
    private Grabber _grabber;
    private Grabber Grabber => _grabber ?? (_grabber = GetComponent<Grabber>());


    private TransformPositionDriver _transformPositionDriver;
    private TransformPositionDriver TransformPositionDriver => _transformPositionDriver ?? (_transformPositionDriver = GetComponent<TransformPositionDriver>());

    void OnEnable()
    {
        Grabber.onGrabEvent.AddListener(OnGrab);
        Grabber.onReleaseEvent.AddListener(OnUnGrab);
    }

    void OnDisable()
    {
        Grabber.onGrabEvent.RemoveListener(OnGrab);
        Grabber.onReleaseEvent.RemoveListener(OnUnGrab);
    }

    private void OnGrab(Grabbable grabbable)
    {
        var guidedSnapEnabledElement = grabbable.GetComponent<GuidedSnapEnabledElement>();
        if (guidedSnapEnabledElement != null)
        {
            guidedSnapEnabledElement.RegisterElementDriver(TransformPositionDriver);
        }
    }

    private void OnUnGrab(Grabbable grabbable)
    {
        var guidedSnapEnabledElement = grabbable.GetComponent<GuidedSnapEnabledElement>();
        if (guidedSnapEnabledElement != null)
        {
            guidedSnapEnabledElement.UnregisterElementDriver(TransformPositionDriver);
        }
    }
}

#endif