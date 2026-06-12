using UnityEngine;

/// <summary>
/// This script was used with XRI to re-enable ray interactor after poke.
/// With Meta Interaction SDK, the OVRCameraRigInteraction prefab handles
/// interactor switching automatically. This script is no longer needed
/// but is kept for backward compatibility.
/// </summary>
public class EnsureRayAfterPoke : MonoBehaviour
{
    // Meta Interaction SDK handles poke/ray switching automatically
    // via its built-in interactor group logic. No custom code needed.
}
