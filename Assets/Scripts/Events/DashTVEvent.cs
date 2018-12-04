using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashTVEvent : GenericTVEvent {

    public GenericEnemyScript[] enemies;

    private bool cleared = false;

    protected override void AdditionalEffects()
    {
        GameManager.gm.enableDash = true;
        GameManager.player.GetComponent<PlayerControlsScript>().EnableDash = true;
    }

    protected override void Update()
    {
        base.Update();
        if (!(ready || displaying || Done))
        {
            bool c = true;
            foreach (GenericEnemyScript e in enemies)
            {
                c = c && e.isDead;
            }
            if (c && !cleared)
            {
                this.Activate();
                cleared = true;
            }
        }
    }
}
