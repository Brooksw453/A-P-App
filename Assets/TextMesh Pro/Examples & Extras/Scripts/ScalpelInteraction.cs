using UnityEngine;

public class ScalpelInteraction : MonoBehaviour
{
    // Create a serializable struct that contains both the sphere and its corresponding menu.
    [System.Serializable]
    public struct SphereMenuPair
    {
        public GameObject sphere;
        public GameObject menu;
    }

    public SphereMenuPair[] sphereMenuPairs; // Array of Sphere-Menu pairs

    private GameObject currentActiveMenu = null;

    private void OnTriggerEnter(Collider other)
    {
        foreach (SphereMenuPair pair in sphereMenuPairs)
        {
            if (other.gameObject == pair.sphere)
            {
                if(currentActiveMenu != null)
                {
                    currentActiveMenu.SetActive(false); // Deactivate the currently active menu if there's any
                }

                pair.menu.SetActive(true); // Activate the corresponding menu
                currentActiveMenu = pair.menu;
                break;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        foreach (SphereMenuPair pair in sphereMenuPairs)
        {
            if (other.gameObject == pair.sphere)
            {
                pair.menu.SetActive(false); // Deactivate the corresponding menu
                currentActiveMenu = null; 
                break;
            }
        }
    }
}

