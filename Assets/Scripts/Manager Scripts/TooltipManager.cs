using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour {

    public static TooltipManager ttm;

    public static TooltipBackgroundAnimator tooltipBGAnimator;

    public static ContinueNotifier cNotifier;

    public static InteractNotifier iNotifier;

	// Use this for initialization
	void Awake () {
		if (ttm == null)
        {
            ttm = this;
        } else if (ttm != this)
        {
            Destroy(gameObject);
        }
	}

    public static void SetTooltipBackground(TooltipBackgroundAnimator bg)
    {
        tooltipBGAnimator = bg;
    }

    public static void SetContinueNotifier(ContinueNotifier cn)
    {
        cNotifier = cn;
    }

    public static void SetInteractNotifier(InteractNotifier inot)
    {
        iNotifier = inot;
    }
	
	public static void DisplayTooltip(FadeImageScript tooltip)
    {
        ttm.StartCoroutine(DoShow(tooltip, true));
    }

    public static void DisplayInteractNotifier()
    {
        iNotifier.FadeIn();
    }

    public static void HideInteractNotifier()
    {
        iNotifier.FadeOut();
    }

    public static void HideTooltip(FadeImageScript tooltip)
    {
        ttm.StartCoroutine(DoShow(tooltip, false));
    }

    private static IEnumerator DoShow(FadeImageScript tooltip, bool display)
    {
        if (display)
        {
            tooltipBGAnimator.Activate();
            yield return new WaitForSeconds(tooltipBGAnimator.animationDuration);
            tooltip.FadeIn();
            cNotifier.FadeIn();
        } else
        {
            tooltip.FadeOut();
            cNotifier.FadeOut();
            yield return new WaitForSeconds(tooltip.durationInSeconds);
            tooltipBGAnimator.Deactivate();
        }
    }
}
