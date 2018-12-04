using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhirlwindTVEvent : GenericTVEvent
{
    [SerializeField]
    private FadeImageScript tooltip2;
    [SerializeField]
    private FadeImageScript tooltip3;
    public BoxCollider2D zone;

    public Transform encounterBoundary;

    private bool cleared = false;

    [SerializeField]
    private BarrierScript bs;

    private bool firstTooltip = true;
    private bool secondTooltip = false;
    private bool thirdTooltip = false;

    protected override void AdditionalEffects()
    {
        GameManager.gm.enableSpecials = true;
        PlayerControlsScript pc = GameManager.player.GetComponent<PlayerControlsScript>();
        pc.EnableSpecials = true;
        pc.SetEdge(12);
        encounterBoundary.position = this.transform.position;
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
            tooltip2.FadeOut();
            tooltip3.FadeIn();
            secondTooltip = false;
            StartCoroutine("WaitAndThirdTooltip");
        } else if (inputAttack && thirdTooltip)
        {
            TooltipManager.HideTooltip(tooltip3);
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

    private IEnumerator WaitAndThirdTooltip()
    {
        yield return new WaitForSeconds(1);
        thirdTooltip = true;
    }

    protected override void Update()
    {
        base.Update();
        if (!(ready || displaying || Done))
        {
            if (zone.bounds.Contains(GameManager.player.transform.position) && !cleared)
            {
                this.Activate();
                bs.Activate();
                cleared = true;
            }
        }
    }
}
