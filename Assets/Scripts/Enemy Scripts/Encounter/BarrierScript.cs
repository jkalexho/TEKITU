using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierScript : MonoBehaviour {

    [SerializeField]
    private GameObject effectsObject;

    [SerializeField]
    private BoxCollider2D wallCollider;

    private bool barrierActive = false;

    private Animator animator;

    private ParticleSystem particles;

	// Use this for initialization
	void Start () {
        wallCollider.enabled = false;
        particles = effectsObject.GetComponent<ParticleSystem>();
        if (particles == null)
        {
            Debug.LogError(gameObject.ToString() + ": No particle effects found in effects object!");
        }
        animator = effectsObject.GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError(gameObject.ToString() + ": No animator found in effects object!");
        }
        effectsObject.transform.position = new Vector3(effectsObject.transform.position.x, effectsObject.transform.position.y, this.transform.position.y);
	}
	
    public void Activate()
    {
        if (!barrierActive)
        {
            barrierActive = true;
            wallCollider.enabled = true;
            particles.Play();
            animator.SetBool("active", true);
        }
    }

    public void Deactivate()
    {
        if (barrierActive)
        {
            barrierActive = false;
            wallCollider.enabled = false;
            particles.Stop();
            animator.SetBool("active", false);
        }
    }
}
