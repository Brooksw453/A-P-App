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

    private void Update()
    {
        // If the object is in the socket, grabbed, or selected, turn off the third object
        if (isObjectInSocket || isObjectGrabbed || isObjectSelected)
        {
            thirdObject.SetActive(false);
        }
        // If the object is not in the socket, grabbed, or selected, turn on the third object
        else
        {
            thirdObject.SetActive(true);
        }
    }

    public void DeactivateThirdObject()
    {
        thirdObject.SetActive(false);
    }

    public void ReactivateThirdObject()
    {
        // If the object is not in the socket, grabbed, or selected, turn on the third object
        if (!isObjectInSocket && !isObjectGrabbed && !isObjectSelected)
        {
            thirdObject.SetActive(true);
        }
    }

    public void ObjectGrabbed(bool isGrabbed)
    {
        isObjectGrabbed = isGrabbed;
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
