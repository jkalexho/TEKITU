using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Intro1Shot7 : CinematicShot {

    [SerializeField]
    private RectTransform goblin1;
    [SerializeField]
    private RectTransform goblin2;
    [SerializeField]
    private RectTransform goblin1Target;
    [SerializeField]
    private RectTransform goblin2Target;

	// Use this for initialization
	void Start () {
        StartCoroutine("Play");
        this.GetComponent<FadeCanvasScript>().FadeIn();
    }
	
    IEnumerator Play()
    {
        yield return new WaitForSeconds(0.25f);
        float progress = 0;
        while (progress < 1)
        {
            yield return null;
            progress = Mathf.Clamp01(progress + Time.deltaTime * 1.2f);
            goblin1.anchoredPosition = Vector2.Lerp(goblin1.anchoredPosition, goblin1Target.anchoredPosition, progress);
            goblin2.anchoredPosition = Vector2.Lerp(goblin2.anchoredPosition, goblin2Target.anchoredPosition, progress);
        }
        yield return new WaitForSeconds(1f);
        Done = true;
    }
}
