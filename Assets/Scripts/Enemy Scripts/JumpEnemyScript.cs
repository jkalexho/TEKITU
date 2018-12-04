using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpEnemyScript : GenericEnemyScript
{
    [Header("Jump Enemy Settings")]
    #region Components
    public Collider2D attackCollider;
    #endregion

    #region Parameters
    public float normalSpeed;

    [Header("Attack Settings")]
    public float attackDuration;
    public float preAttackDelay;
    public float postAttackDelay;
    public float attackColliderDiameter;
    public float attackColliderOffset;

    [Header("Jump Settings")]
    public float jumpMaxSpeed; // Not accurate
    public float jumpCooldown;
    public float jumpDuration;
    [Tooltip("If distToPlayer is within noJumpRange, enemy will walk to player and attack.")]
    public float noJumpRange;
    #endregion

    #region Private variables
    private bool damageMade;
    private bool isJumping;
    private float jumpCooldownTimer;
    #endregion

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        attackCollider.enabled = false;
        damageMade = false;
        attackCollider.GetComponent<CircleCollider2D>().radius = attackColliderDiameter / 2;
        isJumping = false;
        jumpCooldownTimer = 0.0f;
        moveBody.Speed = normalSpeed;
    }

    void FixedUpdate()
    {
        CoolDown();
        if (!isJumping && jumpCooldownTimer <= 0.0f && CanAttack() && stateManager.TrySetState(State.Attacking))
        {
            //Debug.Log("Attacking");
            Attack();
        } else if (!isJumping 
                    && jumpCooldownTimer <= 0.0f 
                    && DistanceToPlayer() < noJumpRange 
                    && stateManager.TrySetState(State.Running))
        {
            //Debug.Log("Moving");
            Move(GetNextLocation());
        } else if (!isJumping && jumpCooldownTimer <= 0.0f && stateManager.TrySetState(State.Running))
        {
            //Debug.Log("Jumping starts");
            Jump(); //TODO: new state?
        }
    }

    private void Jump()
    {
        isJumping = true;
        StartCoroutine("DoJump");
    }

    private void CoolDown()
    {
        if (jumpCooldownTimer > 0)
        {
            jumpCooldownTimer -= Time.fixedDeltaTime;
        }
    }

    protected override void Move(Vector2 dest) 
    {
        Vector2 destPos = new Vector2(dest.x, dest.y);
        moveBody.MoveToPoint(destPos);
    }

    protected bool CanAttack()
    {
        return Vector2.Distance(transform.position, player.transform.position) < attackColliderDiameter;
    }

    private Vector2 GetNextLocation()
    {
        return player.transform.position;
    }

    protected override void Attack()
    {
        StartCoroutine("DoAttack");
    }

    private IEnumerator DoJump()
    {
        yield return new WaitForFixedUpdate();
        stateManager.Direction = GetJumpDirection();
        float progress = 0;
        float jumpSpeed = 0;
        float jumpAcceleration = jumpMaxSpeed / (jumpDuration / 6); // From BirdScript, a = v / t
        while (progress < jumpDuration && stateManager.CurrentState == State.Running)
        {
            yield return new WaitForFixedUpdate();
            //Debug.Log(jumpSpeed);
            if (progress <= jumpDuration / 6)
            {
                jumpSpeed += jumpAcceleration * Time.fixedDeltaTime;
            }
            if (progress >= jumpDuration * 5 / 6)
            {
                jumpSpeed -= jumpAcceleration * Time.fixedDeltaTime;
            }
            progress += Time.fixedDeltaTime;
            moveBody.MoveInDirection(stateManager.Direction, jumpSpeed);
        }
        jumpCooldownTimer = jumpCooldown;
        isJumping = false;
        moveBody.Speed = normalSpeed;
        stateManager.ReturnToIdle(State.Running);
    }

    private Vector2 GetJumpDirection() 
    {
        Vector2 relativePlayerPosition = (player.transform.position - transform.position).normalized;
        float baseAngle = Vector2.SignedAngle(new Vector2(1, 0), relativePlayerPosition);
        float randomAngle = Random.Range(-45, 45);
        float resultAngle = baseAngle + randomAngle;
        Vector2 result = new Vector2(Mathf.Cos(resultAngle * Mathf.Deg2Rad), Mathf.Sin(resultAngle * Mathf.Deg2Rad));
        return result;
    }

    IEnumerator DoAttack()
    {
        yield return new WaitForSeconds(preAttackDelay);
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
                Collider2D[] hitObjects = new Collider2D[100];
                ContactFilter2D playerFilter = new ContactFilter2D
                {
                    layerMask = LayerMask.GetMask("Player"),
                    useLayerMask = true,
                    useTriggers = true
                };
                int length = attackCollider.OverlapCollider(playerFilter, hitObjects);
                for (int i = 0; i < length; i++)
                {
                    damageMade = true;
                    if (hitObjects[i] != null && hitObjects[i].gameObject.tag == "bodyCollider" && GameManager.player.GetComponent<StateManager>().CurrentState != State.Dashing)
                    {
                        player.GetComponent<PlayerControlsScript>().Hit(damage, direction);
                    }
                }
            }
        }
        yield return new WaitForSeconds(postAttackDelay);
        attackCollider.enabled = false;
        damageMade = false;
        stateManager.ReturnToIdle(State.Attacking);
    }

    public override void Die()
    {
        if (stateManager.TrySetState(State.Dying))
        {
            
            feetCollider.enabled = false;
            bodyCollider.enabled = false;

            base.CreateCorpse(this.gameObject, deathAnimationTiming);
        }
    }
}
