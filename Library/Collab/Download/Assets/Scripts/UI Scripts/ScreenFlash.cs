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
	}

    public void FadeToRed(float duration)
    {
        DisableImages();
        screenCover.enabled = true;
        screenCover.color = new Color(1, 0, 0, 0.75f);
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
}
