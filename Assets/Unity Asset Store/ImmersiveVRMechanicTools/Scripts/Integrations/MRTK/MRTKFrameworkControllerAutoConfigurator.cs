#if INTEGRATIONS_MRTK

using System.Collections.Generic;
using System.Linq;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using ReliableSolutions.Unity.Common.Extensions;
using UnityEngine;

//MRTK controller game objects are created at runtime, it's not possible to easily add required scripts to them, instead this class will capture registrations and add required scripts
public class MRTKFrameworkControllerAutoConfigurator : MonoBehaviour, IMixedRealitySourceStateHandler
{
    [SerializeField] private List<string> PointerNamesToConfigure;
    [SerializeField] private MixedRealityInputAction ChangeToolDirectionAction = MixedRealityInputAction.None;

    private void OnEnable()
    {
        CoreServices.InputSystem?.RegisterHandler<IMixedRealitySourceStateHandler>(this);
    }

    private void OnDisable()
    {
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealitySourceStateHandler>(this);
    }

    public void OnSourceDetected(SourceStateEventData eventData)
    {
        var pointersToConfigure = eventData.InputSource?.Pointers?
            .Where(p => PointerNamesToConfigure.Contains(p.PointerName));
        foreach (var pointer in pointersToConfigure)
        {
            if (pointer is MonoBehaviour goPointer)
            {
                var toolInput = goPointer.gameObject.GetOrAddComponent<MRTKFrameworkToolInput>();
                toolInput.Initialize(ChangeToolDirectionAction);
                goPointer.gameObject.GetOrAddComponent<TransformPositionDriver>(); 
                goPointer.gameObject.GetOrAddComponent<MRTKHapticFeedbackControl>();
            }
        }
    }

    public void OnSourceLost(SourceStateEventData eventData)
    {

    }
}

#endif