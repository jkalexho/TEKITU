using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Intro1Shot4 : CinematicShot {

    [SerializeField]
    private FadeImageScript panel4;
    [SerializeField]
    private FadeImageScript panel5;

	// Use this for initialization
	void Start () {
        StartCoroutine("Play");
        this.GetComponent<FadeCanvasScript>().FadeIn();
    }
	
	IEnumerator Play()
    {
        yield return new WaitForSeconds(2);
        panel4.FadeOut();
        panel5.FadeIn();
        yield return new WaitForSeconds(1);
        Done = true;
    }
}
