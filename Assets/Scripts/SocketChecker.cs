using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocketChecker : MonoBehaviour
{
    public GameObject objectToCheck;
    public bool isObjectInSocket = false;

    public GameObject objectToDeactivate; // Reference to the GameObject you want to deactivate.

    private void Update()
    {
        if (objectToCheck != null && IsObjectInSocket())
        {
            isObjectInSocket = true;
        }
        else
        {
            isObjectInSocket = false;
        }

        // Deactivate the other GameObject if isObjectInSocket is true.
        if (isObjectInSocket && objectToDeactivate != null)
        {
            objectToDeactivate.SetActive(false);
        }
        else 
        {
            objectToDeactivate.SetActive(true);
        }
    }

    private bool IsObjectInSocket()
    {
        // Your implementation to check if the objectToCheck is in the socket.
        // For example, you can use colliders to check if the object is inside the socket.
        // You need to implement the specific logic depending on your scene setup.
        // Here's a simple example using collider bounds check:

        if (objectToCheck != null && objectToCheck.GetComponent<Collider>() != null)
        {
            Collider socketCollider = GetComponent<Collider>();
            Collider objectCollider = objectToCheck.GetComponent<Collider>();

            if (socketCollider != null && objectCollider != null)
            {
                return socketCollider.bounds.Intersects(objectCollider.bounds);
            }
        }

        return false;
    }

  

}
