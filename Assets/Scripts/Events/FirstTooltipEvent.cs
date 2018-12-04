using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstTooltipEvent : SpecialEvent {

    public Transform wall;

    public FadeImageScript image;

    private bool started = false;

	// Use this for initialization
	protected override void Start () {
        base.Start();
        StartCoroutine(WaitAndStart());
        GameManager.pc.EnableMovement = false;
	}

    void Update()
    {
        bool inputAttack = Input.GetButton("Attack");
        if ((inputAttack || GameManager.player.transform.position.y > -5) && started)
        {
            TooltipManager.HideTooltip(image);
            StartCoroutine("ReEnable");
        }
    }

    private IEnumerator ReEnable()
    {
        yield return new WaitForSeconds(0.25f);
        GameManager.pc.EnableMovement = true;
        GameManager.pc.EnableAttack = true;
        Destroy(gameObject);
    }

    private IEnumerator WaitAndStart()
    {
        yield return new WaitForSeconds(0.1f);
        TooltipManager.DisplayTooltip(image);
        yield return new WaitForSeconds(1f);
        started = true;
    }
	

    public override void Activate()
    {
        
    }
}
