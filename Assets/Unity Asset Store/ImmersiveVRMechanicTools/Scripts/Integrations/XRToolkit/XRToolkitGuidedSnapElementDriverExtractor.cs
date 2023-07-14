#if INTEGRATIONS_XRTOOLKIT

using System;
using Assets.TorqueWrenchTools.Scripts.Utilities;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class XRToolkitGuidedSnapElementDriverExtractor : MonoBehaviour
{
    [Serializable] public class DriverExtractedEvent : UnityEngine.Events.UnityEvent<TransformPositionDriver> { }
    public DriverExtractedEvent DriverExtractedForGrab = new DriverExtractedEvent();
    public DriverExtractedEvent DriverExtractedForUngrab = new DriverExtractedEvent();

    public void ExctractGuidedSnapElementDriverForGrab(XRBaseInteractor xrBaseInteractor)
    {
        ExtractDriverAndProcessEvent(xrBaseInteractor, DriverExtractedForGrab);
    }

    public void ExctractGuidedSnapElementDriverForUngrab(XRBaseInteractor xrBaseInteractor)
    {
        ExtractDriverAndProcessEvent(xrBaseInteractor, DriverExtractedForUngrab);
    }

    private void ExtractDriverAndProcessEvent(XRBaseInteractor xrBaseInteractor, DriverExtractedEvent driverExtractedEvent)
    {
        var driver = xrBaseInteractor.GetComponent<TransformPositionDriver>();
        if (driver != null)
        {
            driverExtractedEvent?.Invoke(driver);
        }
    }

    void Reset()
    {
#if UNITY_EDITOR
        SetUpEventCommunication();
#endif
    }

    void Start()
    {
#if UNITY_EDITOR
        var isAnyAdded = SetUpEventCommunication();
        if (isAnyAdded)
        {
            Debug.LogWarning($"Events for {nameof(XRToolkitGuidedSnapElementDriverExtractor)} were added automatically, this will only happen in Editor." +
                             $"Please click on this message and set up event communication. You can right click on script and use '{nameof(SetUpEventCommunication)}' context menu option to do that." +
                             $"If you don't set event communication it'll fail at runtime", gameObject);
        }
#endif
    }

#if UNITY_EDITOR
    [ContextMenu(nameof(SetUpEventCommunication))]
    private bool SetUpEventCommunication()
    {
        var grabInteractable = GetComponent<XRGrabInteractable>();
        var isOnSelectEnterAdded = UnityEventToolsHelper.AddPersistentListenerIfNotExists(grabInteractable.onSelectEntered, ExctractGuidedSnapElementDriverForGrab, this);
        var isOnSelectEnterExitAdded = UnityEventToolsHelper.AddPersistentListenerIfNotExists(grabInteractable.onSelectExited, ExctractGuidedSnapElementDriverForUngrab, this);

        var guidedSnapEnabledElement = GetComponent<GuidedSnapEnabledElement>();
        var isDriverExtractedForGrabAdded = UnityEventToolsHelper.AddPersistentListenerIfNotExists(DriverExtractedForGrab, guidedSnapEnabledElement.RegisterElementDriver, guidedSnapEnabledElement);
        var isDriverExtractedForUngrabAdded = UnityEventToolsHelper.AddPersistentListenerIfNotExists(DriverExtractedForUngrab, guidedSnapEnabledElement.UnregisterElementDriver, guidedSnapEnabledElement);

        return isOnSelectEnterAdded || isOnSelectEnterExitAdded || isDriverExtractedForGrabAdded || isDriverExtractedForUngrabAdded;
    }
#endif
}

#endif