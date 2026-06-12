using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A VR-interactable knob that outputs a normalized 0-1 value.
/// Works with any interaction system — connect grab/release via UnityEvents in the Inspector.
/// </summary>
public class VRKnob : MonoBehaviour
{
    [SerializeField]
    private Vector3 rotateAxis = Vector3.up;

    [SerializeField]
    [Range(0, 360)]
    private float maxRotation = 270f;

    [Serializable]
    public class FloatEvent : UnityEvent<float> { }

    [Header("Events")]
    public FloatEvent OnValueChanged = new FloatEvent();

    private bool _isGrabbed = false;
    private float _currentRotation = 0f;

    /// <summary>
    /// Call from interaction system when the knob is grabbed.
    /// </summary>
    public void OnGrabbed()
    {
        _isGrabbed = true;
        _currentRotation = Vector3.Angle(transform.up, Vector3.up);
    }

    /// <summary>
    /// Call from interaction system when the knob is released.
    /// </summary>
    public void OnReleased()
    {
        _isGrabbed = false;
    }

    private void Update()
    {
        if (!_isGrabbed) return;

        float newRotation = Vector3.Angle(transform.up, Vector3.up);
        float delta = newRotation - _currentRotation;
        RotateKnob(delta);
        _currentRotation = newRotation;
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
