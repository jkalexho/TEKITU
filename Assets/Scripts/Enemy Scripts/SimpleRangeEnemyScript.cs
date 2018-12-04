using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRangeEnemyScript : GenericEnemyScript
{
    #region Simple Ranged Enemy Attributes
    [Header("Ranged Enemy Settings")]
    public GameObject projectilePrefab;
    public float shotCooldown = 2f;
    public float attackRange = 1f;
    public ParticleSystem deathEffect;
    #endregion

    //public ParticleSystem hitEffect;

    float timer;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    void FixedUpdate()
    {
        stateManager.Direction = GameManager.player.transform.position - this.transform.position;
        if (stateManager.CurrentState == State.Idle || stateManager.CurrentState == State.Running)
        {
            timer += Time.fixedDeltaTime;
        }
        if (InRange()) {
            if (timer >= shotCooldown) {
                Attack();
            } else
            {
                stateManager.ReturnToIdle(State.Running);
            }
        } else {
            Move(player.transform.position);
        }
    }

    public override void Die()
    {
        if (stateManager.TrySetState(State.Dying))
        {
            // print("dying");
            GameObject effectToDestroy = deathEffect.gameObject;
            Destroy(effectToDestroy, deathEffect.main.duration);
            deathEffect.transform.parent = null;
            deathEffect.Play();
            feetCollider.enabled = false;
            bodyCollider.enabled = false;
            base.Die();

            base.CreateCorpse(this.gameObject, deathAnimationTiming);
        }
    }

    protected override void Move(Vector2 dest) 
    {
        if (stateManager.TrySetState(State.Running))
        {
            moveBody.MoveInDirection(dest - (Vector2)this.transform.position);
        }
    }

    private bool InRange()
    {
        float distance = Vector2.Distance(transform.position, player.transform.position);
        return distance <= attackRange ? true : false;
    }

    protected override void Attack() {
        if (stateManager.TrySetState(State.Attacking))
        {
            
            timer = 0f;
            StartCoroutine("DoAttack");
        }
    }

    private IEnumerator DoAttack()
    {
        yield return new WaitForSeconds(0.25f);
        Vector3 destination = player.transform.position;
        yield return new WaitForSeconds(0.25f);
        // shoot
        if (stateManager.CurrentState == State.Attacking)
        {
            var shot = Instantiate(projectilePrefab, transform.position, transform.rotation);
            shot.GetComponent<SimpleShotScript>().damage = damage;
            shot.GetComponent<SimpleShotScript>().Dest = destination;
        }
        
        yield return new WaitForSeconds(0.5f);
        if (stateManager.CurrentState == State.Attacking)
        {
            stateManager.ReturnToIdle(State.Attacking);
        }
    }
    

}
