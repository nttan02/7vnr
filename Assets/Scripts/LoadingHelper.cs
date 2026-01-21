using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LoadingHelper : MonoBehaviour
{
    public Image loadingBar;
    public Text loadingStatus;
    private AsyncOperation sceneAO;
    public string sceneName;
    private bool loadedFirebase;

    private void Start()
    {
        base.StartCoroutine(this.loadSceneCor());
    }
    private IEnumerator loadSceneCor()
    {
        this.sceneAO = SceneManager.LoadSceneAsync(this.sceneName);
        if (this.sceneAO == null)
        {
            Debug.LogError($"Failed to load scene: {this.sceneName}");
            yield break;
        }
        this.sceneAO.allowSceneActivation = false;
        while (!this.sceneAO.isDone)
        {
            float progress = Mathf.Clamp01(this.sceneAO.progress / 0.1f);
            this.loadingBar.fillAmount = progress;
            this.loadingStatus.text = string.Format("Loading... {0}%", (int)(progress * 100f));

            if (this.sceneAO.progress >= 0.1f && !this.loadedFirebase)
            {
                this.loadedFirebase = true;
                this.loadingStatus.text = "Finalizing...";
                yield return new WaitForSeconds(0.5f);
                yield return StartCoroutine(FadeManager.Instance.FadeOut(0.2f));
                yield return new WaitForSeconds(2f);
                this.sceneAO.allowSceneActivation = true;
            }
            yield return null;
        }
        yield break;
    }
}