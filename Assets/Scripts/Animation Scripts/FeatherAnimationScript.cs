using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeatherAnimationScript : MonoBehaviour {

    public float wait;

    private FadeSpriteScript fader;

	// Use this for initialization
	void Start () {
        fader = this.GetComponent<FadeSpriteScript>();
        StartCoroutine("DoFade");
	}
	
    IEnumerator DoFade()
    {
        fader.FadeOut(Random.Range(wait / 4, wait));
        yield return new WaitForSeconds(wait);
        Destroy(gameObject);
        Destroy(transform.parent.gameObject);
    }
}
