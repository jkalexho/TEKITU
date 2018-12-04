using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenLightAnimator : MonoBehaviour {

    [SerializeField]
    private SpriteRenderer spotlight;

    private Animator animator;

    
	// Use this for initialization
	void Awake () {
        animator = this.GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError(gameObject.ToString() + ": No animator found!");
        }
        StartCoroutine("Flicker");
	}
	
    IEnumerator Flicker()
    {
        float progress = 0;
        while (true)
        {
            yield return new WaitForSeconds(progress);
            animator.SetBool("on", true);
            spotlight.enabled = true;
            progress = Random.Range(0f, 4); 
            if (progress < 3)
            {
                progress = Random.Range(0.1f, 1f);
            }
            yield return new WaitForSeconds(progress);
            animator.SetBool("on", false);
            progress = Random.Range(0.1f, 1);
            spotlight.enabled = false;
        }
    }
}
