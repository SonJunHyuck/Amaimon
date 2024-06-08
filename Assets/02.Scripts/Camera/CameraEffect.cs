using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffect : MonoBehaviour
{
    public UIManager uiM;

    private CameraSceneCover camSceneCover;  // in MainCam
    public Material[] matSceneCovers;

    public float fadeSpeed = 3.0f;
    public float grayScale = 0.0f;
    public float fadeValue = 0.0f;
    public float appliedTime = 2.0f;

    private void Start()
    {
        camSceneCover = transform.GetChild(0).GetComponent<CameraSceneCover>();
    }

    public IEnumerator gameOver()
    {
        float elapsedTime = 0.0f;

        while (elapsedTime < appliedTime)
        {
            elapsedTime += Time.deltaTime;

            grayScale = elapsedTime / appliedTime;
            matSceneCovers[0].SetFloat("_Grayscale", grayScale);
            yield return null;
        }

        uiM.GameOverUI();
    }
    
    public IEnumerator FadeINOUT()
    {
        camSceneCover.material = matSceneCovers[1];
        camSceneCover.enabled = true;

        uiM.SetActiveUI(false);

        float elapsedTime = 0.0f;
        appliedTime = fadeSpeed / 2;
        while (elapsedTime < appliedTime)
        {
            elapsedTime += Time.deltaTime;

            fadeValue = elapsedTime / appliedTime;
            matSceneCovers[1].SetFloat("_Fade", fadeValue);
            yield return null;
        }

        yield return new WaitForSeconds(fadeSpeed);

        while (elapsedTime > 0)
        {
            elapsedTime -= Time.deltaTime;

            fadeValue = elapsedTime / appliedTime;
            matSceneCovers[1].SetFloat("_Fade", fadeValue);
            yield return null;
        }

        camSceneCover.enabled = false;
        uiM.SetActiveUI(true);
    }
}