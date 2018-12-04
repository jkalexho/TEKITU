using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealTVEvent : GenericTVEvent {

    [SerializeField]
    private FadeImageScript tooltip2;

    private bool firstTooltip = true;
    private bool secondTooltip = false;

	protected override void AdditionalEffects()
    {
        GameManager.gm.enableHeal = true;
        PlayerControlsScript pc = GameManager.player.GetComponent<PlayerControlsScript>();
        pc.EnableHeal = true;
        pc.SetEdge(12);
        
    }

    protected override void CheckDeactivateTV()
    {
        bool inputAttack = Input.GetButton("Attack");
        if (inputAttack && firstTooltip)
        {
            tooltip.FadeOut();
            tooltip2.FadeIn();
            firstTooltip = false;
            StartCoroutine("WaitAndSecondTooltip");
        } else if (inputAttack && secondTooltip)
        {
            TooltipManager.HideTooltip(tooltip2);
            displaying = false;
            animator.Deactivate();
            Done = true;
            AdditionalEffects();
            StartCoroutine("ReEnable");
        }
    }

    private IEnumerator WaitAndSecondTooltip()
    {
        yield return new WaitForSeconds(1);
        secondTooltip = true;
    }
}
