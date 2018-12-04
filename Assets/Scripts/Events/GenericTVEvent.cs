using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericTVEvent : SpecialEvent {

    protected bool ready = false;

    protected bool displaying = false;

    protected bool clickable = false;

    protected TVAnimator animator;

    protected CircleCollider2D circle;

    [SerializeField]
    protected FadeImageScript tooltip;

    void Awake()
    {
        animator = this.GetComponent<TVAnimator>();
        if (animator == null)
        {
            Debug.LogError(gameObject.ToString() + ": No TV Animator found!");
        }
    }
	// Use this for initialization
	protected override void Start () {
        base.Start();
        circle = this.GetComponent<CircleCollider2D>();
        circle.enabled = false;
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		if (ready && !Done)
        {
            CheckActivateTV();
        }
        if (displaying && !Done)
        {
            CheckDeactivateTV();
        }
    }

    protected virtual void CheckActivateTV()
    {
        bool inputAttack = Input.GetButtonDown("Attack");
        if (inputAttack && clickable)
        {
            clickable = false;
            TooltipManager.HideInteractNotifier();
            TooltipManager.DisplayTooltip(tooltip);
            StartCoroutine("DoDisplay");
            ready = false;
            GameManager.pc.EnableMovement = false;
        }
    }

    protected virtual void CheckDeactivateTV()
    {
        bool inputAttack = Input.GetButtonDown("Attack");
        if (inputAttack)
        {
            TooltipManager.HideTooltip(tooltip);
            animator.Deactivate();
            Done = true;
            displaying = false;
            StartCoroutine("ReEnable");
            AdditionalEffects();
        }
    }

    protected abstract void AdditionalEffects();

    public override void Activate()
    {
        StartCoroutine("DoStart");
        Active = true;
    }

    protected IEnumerator DoStart()
    {
        yield return new WaitForSeconds(1);
        ready = true;
        animator.Activate();
        circle.enabled = true;
    }

    protected IEnumerator DoDisplay()
    {
        yield return new WaitForSeconds(1);
        displaying = true;
    }

    protected IEnumerator ReEnable()
    {
        yield return new WaitForSeconds(0.25f);
        GameManager.pc.EnableMovement = true;
        GameManager.pc.EnableAttack = true;
        Destroy(gameObject.GetComponent<GenericTVEvent>());
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == Layer.Player && ready)
        {
            clickable = true;
            TooltipManager.DisplayInteractNotifier();
            GameManager.pc.EnableAttack = false;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == Layer.Player && ready)
        {
            clickable = false;
            TooltipManager.HideInteractNotifier();
            GameManager.pc.EnableAttack = true;
        }
    }
}
