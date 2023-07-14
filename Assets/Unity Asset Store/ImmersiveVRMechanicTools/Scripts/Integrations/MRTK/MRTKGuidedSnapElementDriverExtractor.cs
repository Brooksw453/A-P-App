using System;
using System.Collections.Generic;
using Assets.TorqueWrenchTools.Scripts.Utilities;
using UnityEngine;

#if INTEGRATIONS_MRTK
using Microsoft.MixedReality.Toolkit.UI;
[RequireComponent(typeof(ObjectManipulator))]
#endif
public class MRTKGuidedSnapElementDriverExtractor : MonoBehaviour
{
#if INTEGRATIONS_MRTK
    [Serializable] public class DriverExtractedEvent : UnityEngine.Events.UnityEvent<TransformPositionDriver> { }
    public DriverExtractedEvent DriverExtractedForGrab = new DriverExtractedEvent();
    public DriverExtractedEvent DriverExtractedForUngrab = new DriverExtractedEvent();

    private TransformPositionDriver _cachedExtractedTransformPositionDriver;

    public void ExctractGuidedSnapElementDriverForGrab(ManipulationEventData eventData)
    {
        if (eventData.Pointer is MonoBehaviour goPointer)
        {
            _cachedExtractedTransformPositionDriver = goPointer.GetComponent<TransformPositionDriver>();
            if (_cachedExtractedTransformPositionDriver != null)
            {
                DriverExtractedForGrab?.Invoke(_cachedExtractedTransformPositionDriver);
            }
        }
    }

    public void ExctractGuidedSnapElementDriverForUngrab(ManipulationEventData eventData)
    {
        DriverExtractedForUngrab?.Invoke(_cachedExtractedTransformPositionDriver); //on ungrab eventData does not contain pointer and it's unable to retrieve transform-driver
    }
    void Reset()
    {
#if UNITY_EDITOR
        SetUpEventCommunication();
#endif
    }

    //TODO: VRTK is removing those events automatically, those can not be readded at runtime does that break integration?
    [ContextMenu(nameof(SetUpEventCommunication))]
    private void SetUpEventCommunication()
    {
#if UNITY_EDITOR
        var objectManipulator = GetComponentInChildren<ObjectManipulator>();
        
        UnityEventToolsHelper.AddPersistentListenerIfNotExists(objectManipulator.OnManipulationStarted, ExctractGuidedSnapElementDriverForGrab, this);
        UnityEventToolsHelper.AddPersistentListenerIfNotExists(objectManipulator.OnManipulationEnded, ExctractGuidedSnapElementDriverForUngrab, this);

        var guidedSnapEnabledElement = GetComponentInChildren<GuidedSnapEnabledElement>();
        UnityEventToolsHelper.AddPersistentListenerIfNotExists(DriverExtractedForGrab, guidedSnapEnabledElement.RegisterElementDriver, guidedSnapEnabledElement);
        UnityEventToolsHelper.AddPersistentListenerIfNotExists(DriverExtractedForUngrab, guidedSnapEnabledElement.UnregisterElementDriver, guidedSnapEnabledElement);
#endif
    }

#endif
}

