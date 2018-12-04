using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMeleeEnemyScript : GenericEnemyScript
{
    [Header("Melee Enemy Settings")]
    #region Components
    public Collider2D attackCollider;
    #endregion

    #region Parameters
    [Tooltip("Player is reacheable within attackRange. *DIAMETER* of attackCollider")]
    public float attackRange;
    public float attackDuration;
    public float attackColliderOffset;   
    public ParticleSystem deathEffect;
    #endregion

    #region Private variables
    private bool damageMade;
    #endregion

    /* Initialize all variables. 
     * Make sure StateManger and MoveBodyScript are attached
     */
    protected override void Awake()
    {
        base.Awake();
        attackCollider.enabled = false;
        damageMade = false;
    }

    protected override void Start()
    {
        base.Start();
        attackCollider.GetComponent<CircleCollider2D>().radius = attackRange / 2;
    }

    void FixedUpdate()
    {
        int nextAction = GetNextAction();
        if (nextAction == State.Running)
        {
            Move(GetNextLocation());
        }
        else if (nextAction == State.Attacking)
        {
            Attack();
        }
    }

    protected override void Move(Vector2 dest)
    {
        Vector2 destPos = new Vector2(dest.x, dest.y);
        moveBody.MoveToPoint(destPos);
    }

    protected bool CanAttack()
    {
        return Vector2.Distance(transform.position, player.transform.position) < attackRange;
    }

    private Vector2 GetNextLocation()
    {
        return player.transform.position;
    }

    protected override void Attack()
    {
        StopCoroutine("DoAttack");
        StartCoroutine("DoAttack");
    }

    /* Move the attackCollider and check if it hits the player
     * Adapted from PlayerControlsScript
     */

    IEnumerator DoAttack()
    {
        yield return new WaitForSeconds(0.58f);
        float progress = 0;

        while (progress < attackDuration && stateManager.CurrentState == State.Attacking)
        {
            yield return new WaitForFixedUpdate();

            progress += Time.fixedDeltaTime;
            attackCollider.enabled = true;
            Vector2 direction = stateManager.Direction;
            attackCollider.transform.localPosition = direction.normalized * attackColliderOffset;
            if (attackCollider.enabled && !damageMade)
            {
                //Debug.Log("hi");
                Collider2D[] hitObjects = new Collider2D[100];
                ContactFilter2D playerFilter = new ContactFilter2D
                {
                    layerMask = LayerMask.GetMask("Player"),
                    useLayerMask = true,
                    useTriggers = true
                };
                int length = attackCollider.OverlapCollider(playerFilter, hitObjects);
                if (length >= 1)
                {
                    // Debug.Log(hitObjects[0]);
                }
                for (int i = 0; i < length; i++)
                {
                    damageMade = true;
                    //Debug.Log(hittedObjects[i].gameObject);
                    if (hitObjects[i] != null && hitObjects[i].gameObject.tag == "bodyCollider" && GameManager.player.GetComponent<StateManager>().CurrentState != State.Dashing)
                    {
                        player.GetComponent<PlayerControlsScript>().Hit(damage, direction);
                        //Debug.Log("Player is hit by SME.");
                    }
                }
            }
        }
        if (attackCollider != null)
        {
            attackCollider.enabled = false;
        }
        damageMade = false;
        yield return new WaitForSeconds(0.5f - attackDuration);
        stateManager.ReturnToIdle(State.Attacking);
    }

    //TODO: TrySetState is the final check. Should check other conditions first. Move the function call to some other places
    protected int GetNextAction()
    {
        if (CanAttack())
        {
            if (stateManager.TrySetState(State.Attacking))
            {
                return State.Attacking;
            }
        }

        if (stateManager.TrySetState(State.Running))
        {
            return State.Running;
        }

        return State.Idle;
    }

    public override void Die()
    {
        if (stateManager.TrySetState(State.Dying))
        {
            //deathEffect.Play();
            feetCollider.enabled = false;
            bodyCollider.enabled = false;
            base.Die();

            base.CreateCorpse(this.gameObject, deathAnimationTiming);
        }
    }

    
}
