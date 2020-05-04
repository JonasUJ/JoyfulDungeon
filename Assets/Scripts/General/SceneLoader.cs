using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public Animator animator;

    public void LoadScene(string sceneName)
    {
        StartCoroutine(loadScene(sceneName));
    }

    IEnumerator loadScene(string sceneName)
    {
        animator.SetTrigger("Start");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
            yield return null;
        // yield return new WaitForSeconds(1);
        // SceneManager.LoadScene(sceneName);
        animator.SetTrigger("Stop");
    }
}
