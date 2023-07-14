#if INTEGRATIONS_VRTK

using UnityEngine;
using VRTK.Prefabs.Interactions.Interactables;
using Zinnia.Process.Moment;
using Zinnia.Tracking.Follow;

public class VRTKTransformControl : XRFrameworkTransformControl
{
    [SerializeField] private MomentProcess ObjectFollowerMomentProcess;
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;

    public override void TakeControlFromXrFramework()
    {
        ObjectFollowerMomentProcess.enabled = false;
        _initialPosition = transform.position;
        _initialRotation = transform.rotation;
    }

    public override void PassControlBackToXrFramework()
    {
        ObjectFollowerMomentProcess.enabled = true;
        transform.position = _initialPosition;
        transform.rotation = _initialRotation;
    }

    void Reset()
    {
        var guidedSnapEnabledElement = GetComponent<GuidedSnapEnabledElement>();
        if (guidedSnapEnabledElement != null)
        {
            guidedSnapEnabledElement.FindAndSetXRFrameworkTransformControl();
        }

        var interactableFacade = GetComponentInParent<InteractableFacade>();
        var objectFollower = interactableFacade.GetComponentInChildren<ObjectFollower>();
        ObjectFollowerMomentProcess = objectFollower?.GetComponent<MomentProcess>();
        if (ObjectFollowerMomentProcess == null)
            Debug.LogError($"No ObjectFollower / Moment Processor, this is required to correctly take control over transform from VRTK, " +
                      $"if different method is used you'll have to write custom implementation of {nameof(XRFrameworkTransformControl)}");
    }
}

#endif