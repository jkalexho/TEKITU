using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeTextScript : MonoBehaviour {

    public float durationInSeconds;

    private Text textRenderer;

    private float originalAlpha;

    private float targetAlpha;

    void Awake()
    {
        textRenderer = this.GetComponent<Text>();
        if (textRenderer == null)
        {
            Debug.Log(gameObject.ToString() + ": Cannot find Text Component");
        }
    }

    public void FadeIn()
    {
        FadeIn(durationInSeconds);
    }

    public void FadeIn(float duration)
    {
        textRenderer.color -= new Color(0, 0, 0, 1.0f);
        FadeToAlpha(1, duration);
    }

    public void FadeOut()
    {
        FadeOut(durationInSeconds);
    }

    public void FadeOut(float duration)
    {
        textRenderer.color += new Color(0, 0, 0, 1.0f);
        FadeToAlpha(0, duration);
    }

    public void FadeToAlpha(float alpha, float duration)
    {
        targetAlpha = alpha;
        originalAlpha = textRenderer.color.a;
        StartCoroutine("DoFade", duration);
    }

    IEnumerator DoFade(float duration)
    {
        float progress = 0;
        Color c = textRenderer.color;
        while (progress < 1)
        {
            progress += Time.deltaTime / duration;
            c.a = Mathf.Lerp(originalAlpha, targetAlpha, progress);
            textRenderer.color = c;
            yield return null;
        }
        yield return null;
    }
}
