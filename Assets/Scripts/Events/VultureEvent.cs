using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VultureEvent : SpecialEvent {

    private BoxCollider2D bounds;
    
    [SerializeField]
    private Animator fakeVulture;

    [SerializeField]
    private Transform encounterBounds;

    void Awake()
    {
        bounds = this.GetComponentInChildren<BoxCollider2D>();
        Active = false;
    }

    void Update()
    {
        if (!Active)
        {
            if (bounds.bounds.Contains(GameManager.player.transform.position))
            {
                Activate();
            }
        }
    }

    public override void Activate()
    {
        Active = true;
        GameManager.pc.SetEdge(12);
        StartCoroutine("DoActivate");
    }

    IEnumerator DoActivate()
    {
        yield return null;
        GameManager.freeCamera = true;
        GameManager.pc.EnableMovement = false;
        GameManager.followCamScript.desiredPos = fakeVulture.transform.position;
        yield return new WaitForSeconds(3);
        fakeVulture.SetTrigger("animate");
        yield return new WaitForSeconds(3);
        encounterBounds.position = this.transform.position;
        GameManager.pc.EnableMovement = true;
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
