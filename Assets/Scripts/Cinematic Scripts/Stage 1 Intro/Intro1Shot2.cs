using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Intro1Shot2 : CinematicShot {

    [SerializeField]
    private RectTransform sky;
    [SerializeField]
    private RectTransform panel2;
    [SerializeField]
    private RectTransform panel3;

    private FadeImageScript panel2Fader;
    private FadeImageScript panel3Fader;

    protected override void Awake()
    {
        base.Awake();
        panel2Fader = panel2.GetComponent<FadeImageScript>();
        panel3Fader = panel3.GetComponent<FadeImageScript>();
    }

	// Use this for initialization
	void Start () {
        StartCoroutine("Play");
        this.GetComponent<FadeCanvasScript>().FadeIn();
    }

    IEnumerator Play()
    {
        float progress = 0;
        Vector2 kurogiriOriginalPos = panel2.anchoredPosition;
        Vector2 skyOriginalPos = sky.anchoredPosition;
        Vector2 skyTargetPos = panel3.anchoredPosition + new Vector2(0, sky.anchoredPosition.y - panel2.anchoredPosition.y + 150);
        while (progress < 1)
        {
            yield return null;
            progress = Mathf.Clamp01(progress + Time.deltaTime * 0.125f);
            panel2.anchoredPosition = Vector2.Lerp(kurogiriOriginalPos, panel3.anchoredPosition, progress);
            sky.anchoredPosition = Vector2.Lerp(skyOriginalPos, skyTargetPos, progress);
        }
        yield return new WaitForSeconds(1.5f);
        panel2Fader.FadeOut(0.5f);
        panel3Fader.FadeIn(0.25f);
        yield return new WaitForSeconds(2);
        Done = true;
    }
	
}
