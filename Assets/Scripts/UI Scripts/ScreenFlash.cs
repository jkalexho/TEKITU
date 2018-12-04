using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFlash : MonoBehaviour {

    [SerializeField]
    [Tooltip("Duration of the flash")]
    private float flashDuration;

    [SerializeField]
    private Image screenFlash;

    [SerializeField]
    private Image screenCover;

    [SerializeField]
    private Image screenFlashWhite;

    [SerializeField]
    private bool fadeInFromWhite = false;

    private FadeCanvasScript canvasFader;
	// Use this for initialization
	void Start () {
        GameManager.SetScreenFlasher(this.gameObject);
        canvasFader = this.GetComponent<FadeCanvasScript>();
        if (canvasFader == null)
        {
            Debug.LogError(gameObject.ToString() + ": No canvas fader script found!");
        }
        DisableImages();
        if (fadeInFromWhite)
        {
            this.GetComponent<CanvasGroup>().alpha = 1;
            DisableImages();
            screenCover.enabled = true;
            screenCover.color = new Color(1, 1, 1, 1);
            StartCoroutine("WhiteStart");
        }
	}

    public void Reset()
    {
        StartCoroutine("DoReset");
    }

    private IEnumerator DoReset()
    {
        yield return new WaitForSeconds(2);
        canvasFader.FadeOut(2);
    }

    public void FadeToRed(float duration)
    {
        DisableImages();
        screenCover.enabled = true;
        screenCover.color = new Color(1, 0, 0, 1);
        canvasFader.FadeIn(duration);
    }

    public void FadeToWhite(float duration)
    {
        DisableImages();
        screenCover.enabled = true;
        screenCover.color = new Color(1, 1, 1, 1);
        canvasFader.FadeIn(duration);
    }

    public void FadeToColor(float duration, Color color)
    {
        DisableImages();
        screenCover.enabled = true;
        screenCover.color = color;
        canvasFader.FadeIn(duration);
    }
	
	public void FlashScreen()
    {
        FlashScreen(flashDuration);
    }

    public void FlashScreen(float duration)
    {
        DisableImages();
        canvasFader.FadeOut(duration);
        screenFlash.enabled = true;
    }

    public void FlashScreenWhite()
    {
        FlashScreenWhite(flashDuration);
    }

    public void FlashScreenWhite(float duration)
    {
        DisableImages();
        canvasFader.FadeOut(duration);
        screenFlashWhite.enabled = true;
    }

    private void DisableImages()
    {
        screenFlash.enabled = false;
        screenCover.enabled = false;
        screenFlashWhite.enabled = false;
    }

    IEnumerator WhiteStart()
    {
        yield return new WaitForSeconds(1);
        canvasFader.FadeOut(2);
    }
}
