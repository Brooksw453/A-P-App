using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SocketController : MonoBehaviour
{
    public GameObject objectToDetect; // The particular game object to detect
    public GameObject thirdObject; // The third game object to turn on/off

    private bool isObjectInSocket = false;
    private bool isObjectGrabbed = false;
    private bool isObjectSelected = false;
    private bool isTurningOffThirdObject = false;

    private void Update()
    {
        // If the object is in the socket, grabbed, or selected, turn off the third object
        if (isObjectInSocket || isObjectGrabbed || isObjectSelected)
        {
            thirdObject.SetActive(false);
            isTurningOffThirdObject = false;
        }
        // If the object is not in the socket, grabbed, or selected, turn on the third object
        else
        {
            if (!isTurningOffThirdObject)
            {
                StartCoroutine(TurnOnThirdObjectDelayed());
            }
        }
    }

    private System.Collections.IEnumerator TurnOnThirdObjectDelayed()
    {
        isTurningOffThirdObject = true;
        yield return new WaitForSeconds(0.1f); // Adjust the delay time as needed
        thirdObject.SetActive(true);
    }

    public void DeactivateThirdObject()
    {
        thirdObject.SetActive(false);
    }

    public void ReactivateThirdObject()
    {
        thirdObject.SetActive(true);
    }

    public void ObjectGrabbed(bool isGrabbed)
    {
        isObjectGrabbed = isGrabbed;

        // If the object is grabbed, set it as selected as well
        if (isGrabbed)
        {
            ObjectSelected(true);
        }
        // If the object is released or let go, mark it as not selected
        else
        {
            ObjectSelected(false);
        }
    }

    public void ObjectSelected(bool isSelected)
    {
        isObjectSelected = isSelected;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == objectToDetect)
        {
            isObjectInSocket = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == objectToDetect)
        {
            isObjectInSocket = false;
        }
    }
}
