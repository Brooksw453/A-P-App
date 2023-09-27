using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class VRKnob : MonoBehaviour
{
    [SerializeField]
    private Vector3 rotateAxis = Vector3.up;

    [SerializeField]
    [Range(0, 360)]
    private float maxRotation = 270f;

    [Serializable]
    public class FloatEvent : UnityEvent<float> { }

    public FloatEvent OnValueChanged = new FloatEvent();

    private XRGrabInteractable _grabInteractable;
    private float _currentRotation = 0f;

    private void Awake()
    {
        _grabInteractable = GetComponent<XRGrabInteractable>();
        _grabInteractable.onActivate.AddListener(StartRotate);
        _grabInteractable.onDeactivate.AddListener(StopRotate);
    }

    private void StartRotate(XRBaseInteractor obj)
    {
        _currentRotation = Vector3.Angle(transform.up, Vector3.up);
    }

    private void StopRotate(XRBaseInteractor obj)
    {
        // You can implement snapping or other behaviors when you release the knob here.
    }

    private void Update()
    {
        if (_grabInteractable.isSelected)
        {
            float newRotation = Vector3.Angle(transform.up, Vector3.up);
            float delta = newRotation - _currentRotation;
            RotateKnob(delta);
            _currentRotation = newRotation;
        }
    }

    private void RotateKnob(float delta)
    {
        transform.Rotate(rotateAxis, delta);
        
        // Clamp the rotation value
        float angle = Vector3.Angle(transform.up, Vector3.up);
        angle = Mathf.Clamp(angle, 0, maxRotation);
        transform.rotation = Quaternion.FromToRotation(Vector3.up, Quaternion.AngleAxis(angle, rotateAxis) * Vector3.up);
        
        // Notify value change
        float normalizedValue = angle / maxRotation;
        OnValueChanged.Invoke(normalizedValue);
    }
}
