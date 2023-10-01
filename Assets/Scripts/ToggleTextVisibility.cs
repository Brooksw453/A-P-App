using UnityEngine;

public class ToggleTextVisibility : MonoBehaviour
{
    public GameObject xrUIGameObject; // Drag your XR UI GameObject here in the inspector

    public void ToggleUI()
    {
        if (xrUIGameObject != null)
        {
            xrUIGameObject.SetActive(!xrUIGameObject.activeSelf); // Toggle the GameObject
        }
    }
}