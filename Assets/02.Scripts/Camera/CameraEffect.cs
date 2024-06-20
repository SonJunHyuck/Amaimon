using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraEffect : MonoBehaviour
{
    public UIManager uiM;

    private CameraSceneCover camSceneCover;
    public Material[] matSceneCovers;

    [Header("Fade Params")]
    public float fadeSpeed = 1.0f;
    public float fadeTime = 2.0f;

    [Header("GameOver Params")]
    public float graySpeed = 1.0f;
    public float grayTime = 2.0f;

    private void Start()
    {
        camSceneCover = transform.GetComponentInChildren<CameraSceneCover>();
        matSceneCovers[0].SetFloat("_Grayscale", 0);
        matSceneCovers[1].SetFloat("_Fade", 0);
    }

    public void MakeToTransparent(Renderer renderer, UtilityKit.BlendMode blendMode, float opacity)
    {
        Material mat = renderer.material;
        UtilityKit.ChangeRenderMode(mat, blendMode);

        Color matColor = mat.color;
        matColor.a = opacity;
        mat.color = matColor;
    }

    public IEnumerator GrayScale()
    {
        camSceneCover.material = matSceneCovers[0];
        camSceneCover.enabled = true;

        float elapsedTime = 0.0f;
        float grayScale = 0.0f;
        while (elapsedTime < grayTime)
        {
            elapsedTime += Time.deltaTime;

            grayScale = elapsedTime / grayTime;
            matSceneCovers[0].SetFloat("_Grayscale", grayScale);
            yield return null;
        }
    }

    public IEnumerator GameOver()
    {
        uiM.SetActiveUI(false);

        camSceneCover.material = matSceneCovers[0];
        camSceneCover.enabled = true;

        float elapsedTime = 0.0f;
        float grayScale = 0.0f;
        while (elapsedTime < grayTime)
        {
            elapsedTime += Time.deltaTime;

            grayScale = elapsedTime / grayTime;
            matSceneCovers[0].SetFloat("_Grayscale", grayScale);
            yield return null;
        }

        uiM.SetActiveUI(true);
        uiM.GameOverUI();
    }

    public IEnumerator FadeIn()
    {
        uiM.SetActiveUI(false);
        camSceneCover.material = matSceneCovers[1];
        camSceneCover.enabled = true;

        float elapsedTime = 0.0f;
        float fadeValue = 0.0f;
        while (elapsedTime < fadeTime)
        {
            fadeValue = elapsedTime / fadeTime;
            matSceneCovers[1].SetFloat("_Fade", fadeValue);
            elapsedTime += Time.deltaTime * fadeSpeed;

            yield return null;
        }

        camSceneCover.enabled = false;
        uiM.SetActiveUI(true);
    }

    public IEnumerator FadeOut()
    {
        uiM.SetActiveUI(false);

        camSceneCover.material = matSceneCovers[1];
        camSceneCover.enabled = true;

        float elapsedTime = 0.0f;
        float fadeValue = 0.0f;
        while (elapsedTime < fadeTime)
        {
            fadeValue = 1 - (elapsedTime / fadeTime);
            matSceneCovers[1].SetFloat("_Fade", fadeValue);
            elapsedTime += Time.deltaTime * fadeSpeed;

            yield return null;
        }

        camSceneCover.enabled = false;
        uiM.SetActiveUI(true);
    }

    public IEnumerator FadeInOut(float delay)
    {
        // FadeIn & FadeOut �ڷ�ƾ�� ���ļ� ����� �� ���� ������ �ʿ���.
        // UI�� ���� ���¸� �����ϸ� FadeIn �� Out�� �ž� �ϱ� ����

        yield return StartCoroutine(FadeIn());

        // FadeIn ������ Cover Off �ϴµ�, On���� ���� ����
        camSceneCover.enabled = true;
        uiM.SetActiveUI(false);

        yield return new WaitForSeconds(delay);

        yield return StartCoroutine(FadeOut());
    }
}