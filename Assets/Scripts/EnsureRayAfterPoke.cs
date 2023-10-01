using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class EnsureRayAfterPoke : MonoBehaviour
{
    private XRDirectInteractor directInteractor;
    private XRRayInteractor rayInteractor;

    private void Awake()
    {
        directInteractor = GetComponent<XRDirectInteractor>();
        rayInteractor = GetComponent<XRRayInteractor>();

        if (directInteractor)
        {
            directInteractor.onSelectExited.AddListener(OnPokeEnd);
        }
    }

    private void OnDestroy()
    {
        if (directInteractor)
        {
            directInteractor.onSelectExited.RemoveListener(OnPokeEnd);
        }
    }

    private void OnPokeEnd(XRBaseInteractable interactable)
    {
        if (rayInteractor)
        {
            rayInteractor.enabled = true; // Ensure the ray interactor is enabled after a poke interaction ends
        }
    }
}

