using UnityEngine;
using UnityEngine.SceneManagement;

// Kept for backward compatibility with existing scene references.
// New code should use LoadScene.ReloadCurrentScene() instead.
public class SceneReloader : MonoBehaviour
{
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
