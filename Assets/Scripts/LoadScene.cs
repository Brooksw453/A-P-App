using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadScene : MonoBehaviour
{
    [SerializeField] private FadeCanvas fadeCanvas;
    [SerializeField] private float fadeDelay = 1.0f;

    public void LoadSceneUsingName(string sceneName)
    {
        if (fadeCanvas != null)
            StartCoroutine(FadeAndLoad(sceneName));
        else
            SceneManager.LoadScene(sceneName);
    }

    public void ReloadCurrentScene()
    {
        LoadSceneUsingName(SceneManager.GetActiveScene().name);
    }

    private IEnumerator FadeAndLoad(string sceneName)
    {
        fadeCanvas.StartFadeIn();
        yield return new WaitForSeconds(fadeDelay);
        SceneManager.LoadScene(sceneName);
    }
}
