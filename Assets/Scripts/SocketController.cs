using System.Collections;
using UnityEngine;

/// <summary>
/// Detects when an object enters a socket zone and controls visibility of a related object.
/// Framework-agnostic — uses physics triggers, works with Meta Interaction SDK or any system.
/// </summary>
public class SocketController : MonoBehaviour
{
    [Header("References")]
    public GameObject objectToDetect;
    public GameObject thirdObject;

    [Header("Settings")]
    [SerializeField] private float reactivateDelay = 0.1f;

    private bool isObjectInSocket = false;
    private bool isObjectGrabbed = false;
    private Coroutine delayedReactivateCoroutine;

    private void Update()
    {
        bool shouldHide = isObjectInSocket || isObjectGrabbed;

        if (shouldHide)
        {
            if (delayedReactivateCoroutine != null)
            {
                StopCoroutine(delayedReactivateCoroutine);
                delayedReactivateCoroutine = null;
            }
            thirdObject.SetActive(false);
        }
        else if (!thirdObject.activeSelf && delayedReactivateCoroutine == null)
        {
            delayedReactivateCoroutine = StartCoroutine(ReactivateAfterDelay());
        }
    }

    private IEnumerator ReactivateAfterDelay()
    {
        yield return new WaitForSeconds(reactivateDelay);
        thirdObject.SetActive(true);
        delayedReactivateCoroutine = null;
    }

    /// <summary>
    /// Call from Meta Interaction SDK's Grabbable WhenPointerEventRaised or UnityEvents.
    /// </summary>
    public void SetGrabbed(bool grabbed)
    {
        isObjectGrabbed = grabbed;
    }

    public void DeactivateThirdObject() => thirdObject.SetActive(false);
    public void ReactivateThirdObject() => thirdObject.SetActive(true);

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == objectToDetect)
            isObjectInSocket = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == objectToDetect)
            isObjectInSocket = false;
    }
}
