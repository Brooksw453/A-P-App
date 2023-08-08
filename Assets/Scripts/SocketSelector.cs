using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocketSelector : MonoBehaviour
{
    [SerializeField]
    private GameObject objectThatFits; // The GameObject you set in the inspector that can fit in the socket

    private bool objectInSocket;

    private void OnTriggerEnter(Collider other)
    {
        if(!objectInSocket && other.gameObject == objectThatFits)
        {
            objectInSocket = true;
            other.transform.position = transform.position; // Snap the object to the socket's position
            other.transform.rotation = transform.rotation; // Align the object with the socket's rotation
            other.gameObject.GetComponent<Rigidbody>().isKinematic = true; // Prevent the object from falling or being affected by physics
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(objectInSocket && other.gameObject == objectThatFits)
        {
            objectInSocket = false;
            other.gameObject.GetComponent<Rigidbody>().isKinematic = false; // Re-enable physics for the object when it's removed from the socket
        }
    }
}
