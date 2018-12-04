using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Intro1Shot3 : CinematicShot {

    [SerializeField]
    private RectTransform kurogiri;
    [SerializeField]
    private FadeImageScript goblin1;
    [SerializeField]
    private FadeImageScript goblin2;

	// Use this for initialization
	void Start () {
        StartCoroutine("Play");
        this.GetComponent<FadeCanvasScript>().FadeIn();
    }
	
    IEnumerator Play()
    {
        yield return new WaitForSeconds(0.25f);
        float progress = 0;
        float scale = 1;
        while (progress < 1)
        {
            yield return null;
            progress = Mathf.Clamp01(progress + Time.deltaTime * 0.5f);
            scale = Mathf.Lerp(1, 0.67f, progress);
            kurogiri.localScale = new Vector3(scale, scale, 1);
        }
        yield return new WaitForSeconds(0.5f);
        goblin1.FadeIn(0.5f);
        yield return new WaitForSeconds(0.5f);
        goblin2.FadeIn(0.5f);
        yield return new WaitForSeconds(2.5f);
        Done = true;
    }
}
