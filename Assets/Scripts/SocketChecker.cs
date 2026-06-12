using UnityEngine;

public class SocketChecker : MonoBehaviour
{
    public GameObject objectToCheck;
    public GameObject objectToDeactivate;

    [HideInInspector] public bool isObjectInSocket = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == objectToCheck)
        {
            isObjectInSocket = true;
            if (objectToDeactivate != null)
                objectToDeactivate.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == objectToCheck)
        {
            isObjectInSocket = false;
            if (objectToDeactivate != null)
                objectToDeactivate.SetActive(true);
        }
    }
}
