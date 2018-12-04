using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVAnimator : MonoBehaviour {

    [SerializeField]
    private Animator televisionAnimator;
    [SerializeField]
    private Animator speechBubbleAnimator;

	public void Activate()
    {
        televisionAnimator.SetBool("on", true);
        speechBubbleAnimator.SetBool("on", true);
    }

    public void Deactivate()
    {
        televisionAnimator.SetBool("on", false);
        speechBubbleAnimator.SetBool("on", false);
    }
}
