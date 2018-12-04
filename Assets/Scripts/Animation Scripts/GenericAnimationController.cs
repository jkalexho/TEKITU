using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericAnimationController : MonoBehaviour {

    public float orderOffset = -0.5f;

    protected bool dead = false;
    // Use this for initialization
    protected StateManager stateManager;

    protected Animator animator;

    protected SpriteRenderer spriteRenderer;

    protected int originalState;
    protected int newState;

    float progress = 0;

    public float orderUpdateDelay = 0.1f;

    protected bool isFalling;

    protected Transform parent;

    protected virtual void Awake()
    {
        stateManager = this.GetComponentInParent<StateManager>();
        if (stateManager == null)
        {
            Debug.LogError(gameObject.ToString() + ": No state manager found in parent!");
        }
        animator = this.GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError(gameObject.ToString() + ": No animator found!");
        }
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError(gameObject.ToString() + ": No sprite Renderer found!");
        }
        parent = this.transform.parent;
    }

    protected virtual void Update()
    {
        if (stateManager.damaged)
        {
            stateManager.damaged = false;
            DamageFlash();
        }
        if (stateManager.CurrentState == State.Falling && !isFalling)
        {
            isFalling = true;
            spriteRenderer.sortingOrder = Layer.SortPlatforms;
        } else if (stateManager.CurrentState != State.Falling && isFalling)
        {
            isFalling = false;
            spriteRenderer.sortingOrder = Layer.SortObjects;
        }
    }

    protected virtual void FixedUpdate()
    {
        progress += Time.fixedDeltaTime;
        if (progress >= orderUpdateDelay && stateManager.CurrentState != State.Dying)
        {
            progress = 0;
            UpdateOrder();
        }
    }

    protected void UpdateOrder()
    {
        if (stateManager.CurrentState != State.Falling)
        {
            //spriteRenderer.sortingOrder = -Mathf.RoundToInt(stateManager.gameObject.transform.position.y * 10);
            parent.transform.position = new Vector3(parent.parent.transform.position.x, parent.parent.transform.position.y, stateManager.gameObject.transform.position.y + orderOffset);
        }
    }

    protected void OnDestroy()
    {
        spriteRenderer.sortingOrder = Layer.SortDeadBodies;
    }

    protected void DamageFlash()
    {
        StopCoroutine("DoDamageFlash");
        StartCoroutine("DoDamageFlash");
    }

    protected IEnumerator DoDamageFlash()
    {
        animator.SetBool("Damaged", true);
        yield return new WaitForSeconds(0.2f);
        animator.SetBool("Damaged", false);
    }
}
