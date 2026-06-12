using UnityEngine;

public class SocketSelector : MonoBehaviour
{
    [SerializeField] private GameObject objectThatFits;

    [Header("Feedback")]
    [SerializeField] private AudioClip snapSound;
    [SerializeField] private AudioClip removeSound;
    [SerializeField] private GameObject snapParticleEffect;

    private bool objectInSocket;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!objectInSocket && other.gameObject == objectThatFits)
        {
            objectInSocket = true;
            other.transform.position = transform.position;
            other.transform.rotation = transform.rotation;

            Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
            if (rb != null)
                rb.isKinematic = true;

            if (snapSound != null)
                audioSource.PlayOneShot(snapSound);

            if (snapParticleEffect != null)
            {
                GameObject particles = Instantiate(snapParticleEffect, transform.position, Quaternion.identity);
                Destroy(particles, 3f);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (objectInSocket && other.gameObject == objectThatFits)
        {
            objectInSocket = false;

            Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
            if (rb != null)
                rb.isKinematic = false;

            if (removeSound != null)
                audioSource.PlayOneShot(removeSound);
        }
    }

    public bool IsObjectInSocket => objectInSocket;
}
