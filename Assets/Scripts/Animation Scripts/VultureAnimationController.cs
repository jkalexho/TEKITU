using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VultureAnimationController : GenericAnimationController
{
    [SerializeField]
    private FadeSpriteScript shadow;

    private bool hideShadow = false;

    protected override void Update()
    {
        base.Update();
        newState = stateManager.CurrentState;
        
        if ((newState == State.Invincible || newState == State.Perched || newState == State.SprayAttack) && !hideShadow)
        {
            hideShadow = true;
            shadow.FadeToAlpha(0, 0.25f);
        }
        if (newState != State.Invincible && newState != State.Perched && newState != State.SprayAttack && hideShadow)
        {
            hideShadow = false;
            shadow.FadeToAlpha(0.75f, 0.25f);
        }

        if (stateManager.newAttack)
        {
            animator.SetTrigger("Chargeup");
            stateManager.newAttack = false;
        }

        if (stateManager.dead && !dead)
        {
            dead = true;
            animator.SetTrigger("DoDie");
        }
        if (newState == State.Perched || newState == State.SprayAttack || newState == State.Invincible || newState == State.Falling)
        {
            spriteRenderer.sortingOrder = Layer.SortPlatforms;
        } else
        {
            spriteRenderer.sortingOrder = Layer.SortObjects;
        }

        originalState = newState;

        animator.SetInteger("state", originalState);
    }
}
