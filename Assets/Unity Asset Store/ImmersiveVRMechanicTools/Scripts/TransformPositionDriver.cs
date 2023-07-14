using UnityEngine;

[RequireComponent(typeof(VelocityTracker))]
public class TransformPositionDriver : MonoBehaviour
{ 
    private VelocityTracker _velocityTracker;
    public VelocityTracker VelocityTracker => _velocityTracker;
    
    [SerializeField] private HapticFeedbackControl _hapticFeedbackControl;
    public HapticFeedbackControl HapticFeedbackControl => _hapticFeedbackControl;

    void Start()
    {
        _velocityTracker = GetComponent<VelocityTracker>();
        _hapticFeedbackControl = GetComponent<HapticFeedbackControl>();
    }
}
