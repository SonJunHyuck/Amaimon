using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{
    [Header("Title")]
    public GameObject title;

    [Header("Loading")]
    public GameObject loading;
    public Image progressBar;
    public Text loadingTxt;

    string nextScene;

    private void Awake()
    {
        nextScene = "InGame";
    }

    public void LoadScene()
    {
        //nextScene = sceneName;
        title.SetActive(false);
        loading.SetActive(true);
        StartCoroutine(LoadingScene());
    }

    IEnumerator LoadingScene()
    {
        yield return null;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(nextScene);
        //asyncOperation.allowSceneActivation = false;

        float timer = 0.0f;

        while (!asyncOperation.isDone)
        {
            timer += Time.deltaTime;

            loadingTxt.text = (Mathf.Round(progressBar.fillAmount * 100.0f)).ToString() + "%";

            progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);

            // .. Explane : allowSceneActivation == true -> isDone == true
            // asyncOperation.allowSceneActivation = true;

            yield return null;
        }
    }
}
