#if INTEGRATIONS_VRTK

using System;
using Assets.TorqueWrenchTools.Scripts.Utilities;
using UnityEngine;
using VRTK.Prefabs.Interactions.Interactables;
using VRTK.Prefabs.Interactions.Interactors;

[RequireComponent(typeof(InteractableFacade))]
public class VRTKGuidedSnapElementDriverExtractor : MonoBehaviour
{
    [Serializable] public class DriverExtractedEvent : UnityEngine.Events.UnityEvent<TransformPositionDriver> { }
    public DriverExtractedEvent DriverExtractedForGrab = new DriverExtractedEvent();
    public DriverExtractedEvent DriverExtractedForUngrab = new DriverExtractedEvent();

    //VRTK is removing those events automatically, re-set on start
    void Start()
    {
        SetUpEventCommunication();
    }

    public void ExctractGuidedSnapElementDriverForGrab(InteractorFacade interactorFacade)
    {
        ExtractDriverAndProcessEvent(interactorFacade, DriverExtractedForGrab);
    }

    public void ExctractGuidedSnapElementDriverForUngrab(InteractorFacade interactorFacade)
    {
        ExtractDriverAndProcessEvent(interactorFacade, DriverExtractedForUngrab);
    }

    private void ExtractDriverAndProcessEvent(InteractorFacade interactorFacade, DriverExtractedEvent driverExtractedEvent)
    {
        var driver = interactorFacade.GetComponentInParent<TransformPositionDriver>();
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

#if UNITY_EDITOR
    [ContextMenu(nameof(SetUpEventCommunication))]
    private void SetUpEventCommunication()
    {
        var interactableFacade = GetComponentInChildren<InteractableFacade>();
        UnityEventToolsHelper.AddPersistentListenerIfNotExists(interactableFacade.Grabbed, ExctractGuidedSnapElementDriverForGrab, this);
        UnityEventToolsHelper.AddPersistentListenerIfNotExists(interactableFacade.Ungrabbed, ExctractGuidedSnapElementDriverForUngrab, this);

        var guidedSnapEnabledElement = GetComponentInChildren<GuidedSnapEnabledElement>();
        UnityEventToolsHelper.AddPersistentListenerIfNotExists(DriverExtractedForGrab, guidedSnapEnabledElement.RegisterElementDriver, guidedSnapEnabledElement);
        UnityEventToolsHelper.AddPersistentListenerIfNotExists(DriverExtractedForUngrab, guidedSnapEnabledElement.UnregisterElementDriver, guidedSnapEnabledElement);
    }
#endif
}

#endif