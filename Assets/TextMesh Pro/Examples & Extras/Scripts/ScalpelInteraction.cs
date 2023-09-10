using UnityEngine;

public class ScalpelInteraction : MonoBehaviour
{
    // Create a serializable struct that contains the sphere, its corresponding menu, and its audio clip.
    [System.Serializable]
    public struct SphereMenuPair
    {
        public GameObject sphere;
        public GameObject menu;
        public AudioClip sound; // Sound to play when interacting with the sphere
    }

    public SphereMenuPair[] sphereMenuPairs; // Array of Sphere-Menu pairs

    private GameObject currentActiveMenu = null;
    private AudioSource audioSource;

    private void Start()
    {
        // Initialize the audio source component. If it doesn't exist, add one.
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        foreach (SphereMenuPair pair in sphereMenuPairs)
        {
            if (other.gameObject == pair.sphere)
            {
                if (currentActiveMenu != null)
                {
                    currentActiveMenu.SetActive(false); // Deactivate the currently active menu if there's any
                }

                pair.menu.SetActive(true); // Activate the corresponding menu
                currentActiveMenu = pair.menu;

                // Play the corresponding sound
                if (pair.sound)
                {
                    audioSource.clip = pair.sound;
                    audioSource.Play();
                }

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


