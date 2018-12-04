using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeImageScript : MonoBehaviour {
    public float durationInSeconds;

    private Image image;

    private float originalAlpha;

    private float targetAlpha;

    void Awake()
    {
        image = this.GetComponent<Image>();
        if (image == null)
        {
            Debug.Log(gameObject.ToString() + ": Cannot find Sprite Renderer");
        }
    }

    public void FadeIn()
    {
        FadeIn(durationInSeconds);
    }

    public void FadeIn(float duration)
    {
        image.color -= new Color(0, 0, 0, 1.0f);
        FadeToAlpha(1, duration);
    }

    public void FadeOut()
    {
        FadeOut(durationInSeconds);
    }

    public void FadeOut(float duration)
    {
        image.color += new Color(0, 0, 0, 1.0f);
        FadeToAlpha(0, duration);
    }

    public void FadeToAlpha(float alpha, float duration)
    {
        targetAlpha = alpha;
        originalAlpha = image.color.a;
        StartCoroutine("DoFade", duration);
    }

    IEnumerator DoFade(float duration)
    {
        float progress = 0;
        Color c = image.color;
        while (progress < 1)
        {
            progress += Time.deltaTime / duration;
            c.a = Mathf.Lerp(originalAlpha, targetAlpha, progress);
            image.color = c;
            yield return null;
        }
        yield return null;
    }
}
