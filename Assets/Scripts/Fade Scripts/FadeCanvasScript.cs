using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeCanvasScript : MonoBehaviour {

    public float durationInSeconds;

    private CanvasGroup canvasGroup;

    private float originalAlpha;

    private float targetAlpha;

    void Awake()
    {
        canvasGroup = this.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            Debug.Log(gameObject.ToString() + ": Cannot find Canvas Group component");
        }
    }

	public void FadeIn()
    {
        FadeIn(durationInSeconds);
    }

    public void FadeIn(float duration)
    {
        canvasGroup.alpha = 0.0f;
        FadeToAlpha(1, duration);
    }

    public void FadeOut()
    {
        FadeOut(durationInSeconds);
    }

    public void FadeOut(float duration)
    {
        canvasGroup.alpha = 1.0f;
        FadeToAlpha(0, duration);
    }

    public void FadeToAlpha(float alpha, float duration)
    {
        targetAlpha = alpha;
        originalAlpha = canvasGroup.alpha;
        StartCoroutine("DoFade", duration);
    }

    IEnumerator DoFade (float duration)
    {
        float progress = 0;
        while (progress < 1)
        {
            progress += Time.deltaTime / duration;
            canvasGroup.alpha = Mathf.Lerp(originalAlpha, targetAlpha, progress);
            yield return null;
        }
        yield return null;
    }
}
