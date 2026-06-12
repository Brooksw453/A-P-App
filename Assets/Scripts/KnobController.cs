using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Controls a VR knob that rotates based on hand/controller input.
/// Works with any interaction system (Meta Interaction SDK, XRI, etc.)
/// Set up grab events via the Inspector using UnityEvents.
/// </summary>
public class KnobController : MonoBehaviour
{
    [Serializable]
    public class FloatEvent : UnityEvent<float> { }

    [Header("Knob Settings")]
    public float rotationScale = 12f;

    [Header("Events")]
    public FloatEvent onValueChangedEvent = new FloatEvent();

    private Transform currentInteractor;
    private bool isGrabbed = false;

    private float initialHandRotation;
    private float initialKnobRotation;
    private Vector3 initialLocalPosition;
    private Transform panelTransform;

    private void Start()
    {
        initialLocalPosition = transform.localPosition;
        panelTransform = transform.parent;
    }

    /// <summary>
    /// Call this from your interaction system's Select/Grab event.
    /// Pass the hand or controller Transform that is grabbing.
    /// </summary>
    public void OnGrab(Transform interactorTransform)
    {
        isGrabbed = true;
        currentInteractor = interactorTransform;
        initialHandRotation = currentInteractor.localEulerAngles.y;
        initialKnobRotation = transform.localEulerAngles.y;
    }

    /// <summary>
    /// Call this from your interaction system's Deselect/Release event.
    /// </summary>
    public void OnRelease()
    {
        isGrabbed = false;
        currentInteractor = null;
    }

    private void Update()
    {
        if (!isGrabbed || currentInteractor == null) return;

        // Lock position to panel
        if (panelTransform != null)
            transform.position = panelTransform.TransformPoint(initialLocalPosition);

        float currentHandRotation = currentInteractor.localEulerAngles.y;
        float rotationDifference = -(initialHandRotation - currentHandRotation) * rotationScale;

        transform.Rotate(0, rotationDifference, 0, Space.Self);

        // Map rotation to [0, 1] range
        float currentRotation = (transform.localEulerAngles.y > 180)
            ? transform.localEulerAngles.y - 360
            : transform.localEulerAngles.y;
        float normalizedValue = Mathf.InverseLerp(0, 180, currentRotation);
        onValueChangedEvent.Invoke(normalizedValue);

        initialHandRotation = currentHandRotation;
    }
}
