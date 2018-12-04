using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinAnimationController : GenericAnimationController {

   

    protected override void Update()
    {
        base.Update();
        newState = stateManager.CurrentState;

        if (stateManager.newAttack && stateManager.CurrentState == State.Attacking)
        {
            animator.SetTrigger("DoAttack");
            stateManager.newAttack = false;
        }


        if (stateManager.dead && !dead)
        {
            dead = true;
            animator.SetTrigger("DoDie");
        }

        originalState = newState;

        animator.SetInteger("cardinal", Cardinal.VectorToCardinal2(stateManager.Direction));
        animator.SetInteger("state", originalState);
    }
}
