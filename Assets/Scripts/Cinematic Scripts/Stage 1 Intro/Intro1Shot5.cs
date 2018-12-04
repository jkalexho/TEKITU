using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Intro1Shot5 : CinematicShot {

    [SerializeField]
    private FadeImageScript panel5;
    [SerializeField]
    private FadeImageScript panel6;

	// Use this for initialization
	void Start () {
        StartCoroutine("Play");
        this.GetComponent<FadeCanvasScript>().FadeIn();
	}
	
    IEnumerator Play()
    {
        yield return new WaitForSeconds(1);
        panel5.FadeIn(0.25f);
        yield return new WaitForSeconds(1);
        panel5.FadeOut(0.4f);
        panel6.FadeIn(0.25f);
        yield return new WaitForSeconds(2);
        Done = true;
    }
}
