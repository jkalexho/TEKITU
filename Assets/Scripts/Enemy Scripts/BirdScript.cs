using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdScript : GenericEnemyScript {

    #region Editor Variables
    [Header("Bird Enemy Settings")]
    [SerializeField]
    private Collider2D attackCollider;
    [SerializeField]
    private float attackCooldown = 3;
    [Header("Dive Settings")]
    [SerializeField]
    private float diveRange;
    [SerializeField]
    private float diveMaxSpeed;
    [SerializeField]
    private float diveDuration;
    #endregion

    private float cooldown;

    private float moveCooldown;

    private EnemyAttackCollider attackColliderScript;

    protected override void Awake()
    {
        base.Awake();
        if (attackCollider != null)
        {
            attackColliderScript = attackCollider.gameObject.GetComponent<EnemyAttackCollider>();
        }
    }

    // Use this for initialization
    protected override void Start () {
        isFlying = true;
        cooldown = attackCooldown / 2;
        attackCollider.enabled = false;
        base.Start();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        Cooldown();
        if (CanAttack())
        {
            Attack();
        }
        else if (moveCooldown <= 0)
        {
            MakeMove();
        }
    }

    private void Cooldown()
    {
        if (stateManager.CurrentState < State.Attacking)
        {
            if (cooldown > 0)
            {
                cooldown -= Time.fixedDeltaTime;
            }
            if (moveCooldown > 0)
            {
                moveCooldown -= Time.fixedDeltaTime;
            }
        }
    }

    private bool CanAttack()
    {
        return cooldown <= 0 && DistanceToPlayer() < diveRange;
    }

    private void MakeMove()
    {
        if (stateManager.TrySetState(State.Running))
        {
            if (DistanceToPlayer() > diveRange)
            {
                Move(GameManager.player.transform.position);
            }
            else
            {
                Vector3 destination = Random.insideUnitCircle * 5;
                destination += this.transform.position;
                Move(destination);
            }
            moveCooldown = Random.Range(0.4f, cooldown + 0.5f);
        }
    }

    protected override void Move(Vector2 dest)
    {
        Vector2 destPos = new Vector2(dest.x, dest.y);
        moveBody.MoveToPoint(destPos);
    }

    protected override void Attack()
    {
        if (stateManager.TrySetState(State.Attacking))
        {
            StartCoroutine("DoAttack");
        }
    }

    private IEnumerator DoAttack()
    {
        stateManager.Direction = GameManager.player.transform.position + new Vector3(0, 0.4f, 0) - this.transform.position;
        yield return new WaitForSeconds(0.25f);
        if (attackCollider != null)
        {
            attackCollider.enabled = true;


            float diveAcceleration = diveMaxSpeed * 4 / diveDuration;
            float progress = 0;
            float diveSpeed = 0;
            attackCollider.offset = stateManager.Direction.normalized * 0.4f;
            attackColliderScript.Direction = stateManager.Direction;

            while (progress < diveDuration && stateManager.CurrentState == State.Attacking)
            {
                yield return new WaitForFixedUpdate();
                if (progress < diveDuration / 4)
                {
                    diveSpeed += diveAcceleration * Time.fixedDeltaTime;
                }
                progress += Time.fixedDeltaTime;
                if (progress > diveDuration * 3 / 4)
                {
                    diveSpeed -= diveAcceleration * Time.fixedDeltaTime;
                }
                moveBody.MoveInDirection(stateManager.Direction, diveSpeed);
            }
            attackCollider.enabled = false;
            cooldown = attackCooldown;
            stateManager.ReturnToIdle(State.Attacking);
        }
    }

    public override void Die()
    {
        if (inAir && isFlying)
        {
            if (stateManager.TrySetState(State.Falling))
            {
                StartCoroutine("DoFall");
                isFlying = false;
            }
        }
        else if (stateManager.TrySetState(State.Dying))
        {
            
            feetCollider.enabled = false;
            bodyCollider.enabled = false;
            base.Die();

            base.CreateCorpse(this.gameObject, deathAnimationTiming);
        }
    }
}
