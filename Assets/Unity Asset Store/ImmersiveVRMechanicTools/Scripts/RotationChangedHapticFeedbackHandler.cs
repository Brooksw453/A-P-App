using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RotationChangedHapticFeedbackHandler : MonoBehaviour
{
    [SerializeField] [Range(0,1)] private float AddedHapticFeedbackAmplitude = 0.25f;
    [SerializeField] [Range(0,1)] private float FinalRotationHapticFeedbackAmplitude = 0.5f;
    [SerializeField] [Range(0,1)] private float OverRotatingHapticFeedbackAmplitude = 0.9f;
    [SerializeField] [Range(0, 1)] private float OverRotatingBreakingPointHapticFeedbackAmplitude = 1f;

    private static readonly Dictionary<AddRotationProgressStatus, Func<RotationChangedHapticFeedbackHandler, float>> AddRotationProgressStatusToGetAmplitudeMap = new Dictionary<AddRotationProgressStatus, Func<RotationChangedHapticFeedbackHandler, float>>()
    {
        [AddRotationProgressStatus.Added] = h => h.AddedHapticFeedbackAmplitude,
        [AddRotationProgressStatus.FinalRotationStage] = h => h.FinalRotationHapticFeedbackAmplitude,
        [AddRotationProgressStatus.OverRotating] = h => h.OverRotatingHapticFeedbackAmplitude
    };

    public void HandleRotationChanged(RotationChanged rotationChangedArgs)
    {
        var hapticFeedbackControl = rotationChangedArgs.TransformPositionDriver?.HapticFeedbackControl;
        if(hapticFeedbackControl == null) return;

        if (AddRotationProgressStatusToGetAmplitudeMap.TryGetValue(rotationChangedArgs.AddRotationProgressStatus, out var getAmplitude))
        {
            hapticFeedbackControl.SendHapticFeedback(getAmplitude(this), Time.deltaTime);
        }
        else if (rotationChangedArgs.AddRotationProgressStatus == AddRotationProgressStatus.OverRotatingBreakingPoint)
        {
            hapticFeedbackControl.SendHapticFeedback(OverRotatingBreakingPointHapticFeedbackAmplitude, 1f);
        }
    }
}