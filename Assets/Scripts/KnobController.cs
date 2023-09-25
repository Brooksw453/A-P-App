using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class KnobController : MonoBehaviour
{
    [Serializable]
    public class FloatEvent : UnityEvent<float> { }

    public FloatEvent onValueChangedEvent = new FloatEvent();

    private XRGrabInteractable grabInteractable;
    private XRBaseInteractor handInteractor;
    private bool isGrabbed = false;

    private float initialHandRotation;
    private float initialKnobRotation;

    private Vector3 initialLocalPosition;
private Transform panelTransform;

    public float rotationScale = 12f;  // Adjust this value to get the desired rotation amplification.

private void Start()
{
    grabInteractable = GetComponent<XRGrabInteractable>();
    grabInteractable.onSelectEntered.AddListener(OnGrab);
    grabInteractable.onSelectExited.AddListener(OnRelease);

    // Ensure the knob's position is never tracked, only its rotation.
    grabInteractable.trackPosition = false;
    grabInteractable.trackRotation = true;

    // Store the knob's initial local position relative to the panel.
    initialLocalPosition = transform.localPosition;

    // Assuming the panel is the parent of the knob in the hierarchy.
    panelTransform = transform.parent;
}

    private void OnDestroy()
    {
        grabInteractable.onSelectEntered.RemoveListener(OnGrab);
        grabInteractable.onSelectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(XRBaseInteractor interactor)
    {
        isGrabbed = true;
        handInteractor = interactor;

        // Record the initial rotation of the hand and the knob when first grabbed.
        initialHandRotation = handInteractor.transform.localEulerAngles.y;
        initialKnobRotation = transform.localEulerAngles.y;
    }

    private void OnRelease(XRBaseInteractor interactor)
    {
        isGrabbed = false;
        handInteractor = null;
    }

private void Update()
{
    // If the knob is grabbed, ensure its position remains fixed relative to the panel.
    if (isGrabbed)
    {
        transform.position = panelTransform.TransformPoint(initialLocalPosition);
    }
    
if (isGrabbed && handInteractor)
    {
        float currentHandRotation = handInteractor.transform.localEulerAngles.y;
        float rotationDifference = -(initialHandRotation - currentHandRotation) * rotationScale;

        // Rotate around local Y-axis.
        transform.Rotate(0, rotationDifference, 0, Space.Self); 

        // Map the knob's rotation to the [0, 1] range for volume.
        float currentRotation = (transform.localEulerAngles.y > 180) ? transform.localEulerAngles.y - 360 : transform.localEulerAngles.y;
        float normalizedValue = Mathf.InverseLerp(0, 180, currentRotation);
        onValueChangedEvent.Invoke(normalizedValue);
    }
}

}

