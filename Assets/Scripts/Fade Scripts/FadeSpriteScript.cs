using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeSpriteScript : MonoBehaviour {

    public float durationInSeconds;

    private SpriteRenderer spriteRenderer;

    private float originalAlpha;

    private float targetAlpha;

    void Awake()
    {
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
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
        spriteRenderer.color -= new Color(0, 0, 0, 1.0f);
        FadeToAlpha(1, duration);
    }

    public void FadeOut()
    {
        FadeOut(durationInSeconds);
    }

    public void FadeOut(float duration)
    {
        spriteRenderer.color += new Color(0, 0, 0, 1.0f);
        FadeToAlpha(0, duration);
    }

    public void FadeToAlpha(float alpha, float duration)
    {
        targetAlpha = alpha;
        originalAlpha = spriteRenderer.color.a;
        StartCoroutine("DoFade", duration);
    }

    IEnumerator DoFade(float duration)
    {
        float progress = 0;
        Color c = spriteRenderer.color;
        while (progress < 1)
        {
            progress += Time.deltaTime / duration;
            c.a = Mathf.Lerp(originalAlpha, targetAlpha, progress);
            spriteRenderer.color = c;
            yield return null;
        }
        yield return null;
    }
}
