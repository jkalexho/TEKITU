using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Intro1Shot6 : CinematicShot {


    [SerializeField]
    private FadeImageScript goblins1;

    [SerializeField]
    private FadeImageScript goblins2;

	// Use this for initialization
	void Start () {
        StartCoroutine("Play");
        this.GetComponent<FadeCanvasScript>().FadeIn();
    }
	
	IEnumerator Play()
    {
        yield return new WaitForSeconds(1f);
        goblins1.FadeOut(0.4f);
        goblins2.FadeIn(0.25f);
        yield return new WaitForSeconds(1f);
        Done = true;
    }
}
