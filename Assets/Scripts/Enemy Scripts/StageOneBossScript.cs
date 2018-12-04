using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageOneBossScript : GenericEnemyScript {

    #region Editor Variables
    public GameObject projectilePrefab;
    [SerializeField]
    private Collider2D meleeAttackCollider;
    [SerializeField]
    private Collider2D whirlwindAttackCollider;
    [SerializeField]
    private Collider2D environmentalAttackCollider;
    [SerializeField]
    private float attackCooldown = 3;
    [SerializeField]
    private BulletPool slowBulletPool;
    [SerializeField]
    private BulletPool featherBulletPool;

    [Header("Stage One Boss Settings")]
    #region Parameters
    public float normalSpeed;
    #endregion

    [Header("Dive Settings")]
    [SerializeField]
    private float diveRange;
    [SerializeField]
    private float diveMaxSpeed;
    [SerializeField]
    private float diveDuration;
    #endregion

    [Header("Melee Attack Settings")]
    public int meleeAttackDamage;
    public float meleeAttackDuration;
    public float meleeAttackCooldown;
    public float preMeleeAttackDelay;
    public float postMeleeAttackDelay;
    public float meleeAttackColliderDiameter;
    public float meleeAttackColliderOffset;
    public float meleeAttackRange;

    [Header("Whirlwind Attack Settings")]
    public int whirlwindAttackDamage;
    public float whirlwindAttackDuration;
    public float whirlwindAttackCooldown;
    public float preWhirlwindAttackDelay;
    public float postWhirlwindAttackDelay;
    public float whirlwindAttackRadius;
    public float whirlwindAttackMoveSpeed;

    [Header("Ranged Attack Settings")]
    public float preRangedAttackDelay;
    public float postRangedAttackDelay;
    public float projectileInterval;
    public float rangedAttackRange;
    public int rangedAttackDamage;
    public int rangedAttackProjectileNumber;
    public int rangedAttackAngleRange;
    public float rangedAttackCooldown;
    [Tooltip("Size has to be multiple of 3. [n] is the number of bullets. [n+1] is the range of angle (in degree). [n+2] is the speed of that wave.")]
    public int[] rangedAttackQueue;

    [Header("Summon Settings")]
    public float preSummonDelay;
    public float postSummonDelay;
    public int summonNumber;
    public int summonLimit;
    public float summonCooldown;
    public GameObject enemyPrefab;
    [Tooltip("The initial distance from the summoned enemy to boss.")]
    public float summonOffset;

    [Header("Environmental Attack Settings")]
    public GameObject[] Platforms;
    public float environmentalAttackRadius;
    public int environmentalAttackDamage;
    public float environmentalAttackDuration;
    public float environmentalAttackCooldown;
    public float preEnvironmentalAttackDelay;
    public float postEnvironmentalAttackDelay;
    public float environmentalAttackMoveSpeed;
    public float preEnvironmentalAttackMoveDuration;

    #region Private variables
    private float cooldown;

    private EnemyAttackCollider attackColliderScript;
    private bool damageMade;
    private float meleeAttackTimer;
    private float rangedAttackTimer;
    private float moveTimer;
    private float summonTimer;
    private int summonCounter;
    private float whirlwindAttackTimer;
    #endregion

    protected override void Awake()
    {
        base.Awake();
        if (meleeAttackCollider != null)
        {
            attackColliderScript = meleeAttackCollider.gameObject.GetComponent<EnemyAttackCollider>();
        }
        if (rangedAttackQueue.Length % 3 != 0)
        {
            Debug.LogError("The length of rangedAttackQueue is not multiple of 3.");
        }
    }

    protected override void Start () {
        base.Start();
        isFlying = true;
        cooldown = attackCooldown / 2;
        meleeAttackCollider.enabled = false;
        damageMade = false;
        meleeAttackCollider.GetComponent<CircleCollider2D>().radius = meleeAttackColliderDiameter / 2;
        whirlwindAttackCollider.GetComponent<CircleCollider2D>().radius = whirlwindAttackRadius;
        environmentalAttackCollider.GetComponent<CircleCollider2D>().radius = environmentalAttackRadius;

        moveBody.Speed = normalSpeed;
        rangedAttackTimer = rangedAttackCooldown;
        
        meleeAttackTimer = meleeAttackCooldown;
        summonTimer = summonCooldown;
        summonCounter = 0;
        whirlwindAttackTimer = whirlwindAttackCooldown;
    }
	
    void FixedUpdate()
    {
        if (stateManager.TrySetState(State.UnstoppableAttack))
        {
            //Debug.Log("Test");
            StartCoroutine("DoEnvironmentalAttack");
        }
    }

	void testFixedUpdate () {

        Cooldown();
        if (summonTimer <= 0 && summonCounter < summonLimit)
        {
            if (stateManager.TrySetState(State.UnstoppableAttack))
            {
                StartCoroutine("DoSummonEnemies");
                summonTimer = summonCooldown;
            }
        }
        else if (meleeAttackTimer <= 0 && DistanceToPlayer() < meleeAttackRange)
        {
            if (stateManager.TrySetState(State.Attacking))
            {
                StartCoroutine("DoMeleeAttack");
                meleeAttackTimer = meleeAttackCooldown;
            }
        }
        else if (rangedAttackTimer <= 0) 
        {
            if (stateManager.TrySetState(State.Attacking))
            {
                IEnumerator temp = DoRangedAttack(rangedAttackQueue, player.transform.position);
                StartCoroutine(temp);
                rangedAttackTimer = rangedAttackCooldown;
            }
        } else if (whirlwindAttackTimer <= 0)
        {
            if (stateManager.TrySetState(State.UnstoppableAttack))
            {
                StartCoroutine("DoWhirlwindAttack");
                whirlwindAttackTimer = whirlwindAttackCooldown;
            }
        }
        else
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
            if (rangedAttackTimer > 0)
            {
                rangedAttackTimer -= Time.fixedDeltaTime;
            }
            if (meleeAttackTimer > 0)
            {
                meleeAttackTimer -= Time.fixedDeltaTime;
            }
            if (summonTimer > 0)
            {
                summonTimer -= Time.fixedDeltaTime;
            }
            if (whirlwindAttackTimer > 0)
            {
                whirlwindAttackTimer -= Time.fixedDeltaTime;
            }
        }
    }

    private bool CanAttack()
    {
        return true;
    }

    private void MakeMove()
    {
        if (stateManager.TrySetState(State.Running))
        {
            Move(GameManager.player.transform.position);
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
            //if (rangedAttackRange > DistanceToPlayer())
            //{
            //    IEnumerator temp = DoRangedAttack(rangedAttackQueue, player.transform.position);
            //    StartCoroutine(temp);
            //} else {
            StartCoroutine("DoMeleeAttack");
            //}
        }
    }

    private IEnumerator DoRangedAttack(int[] actions, Vector3 target)
    {
        yield return new WaitForSeconds(preRangedAttackDelay);
        for (int i = 0; i < actions.Length; i += 3)
        {
            IEnumerator temp = DoRangedAttackSingle(actions[i], actions[i+1], actions[i+2], target);
            yield return StartCoroutine(temp);
        }
        yield return new WaitForSeconds(postRangedAttackDelay);
        if (stateManager.CurrentState == State.Attacking)
        {
            stateManager.ReturnToIdle(State.Attacking);
        }
    }

    private IEnumerator DoRangedAttackSingle(int num, int range, int speed, Vector3 target)
    {
        if (stateManager.CurrentState == State.Attacking)
        {
            int upperLimit = num % 2 == 0 ? num / 2 : num / 2 + 1;
            for (int i = -num / 2; i < upperLimit; i++)
            {
                float baseAngle = Vector2.SignedAngle(Vector2.right, target - transform.position);
                if (num % 2 == 0)
                {
                    baseAngle += range / num / 2;
                }
                Vector3 dir = new Vector2(Mathf.Cos((baseAngle + range / num * i) * Mathf.Deg2Rad), Mathf.Sin((baseAngle + range / num * i) * Mathf.Deg2Rad));
                slowBulletPool.Spawn(this.transform.position, dir);
            }
        }

        yield return new WaitForSeconds(projectileInterval);
    }

    private IEnumerator DoChargeAttack()
    {
        stateManager.Direction = GameManager.player.transform.position + new Vector3(0, 0.4f, 0) - this.transform.position;
        yield return new WaitForSeconds(0.25f);
        meleeAttackCollider.enabled = true;
        
        float diveAcceleration = diveMaxSpeed * 4 / diveDuration;
        float progress = 0;
        float diveSpeed = 0;
        meleeAttackCollider.offset = stateManager.Direction.normalized * 0.4f;
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
        meleeAttackCollider.enabled = false;
        cooldown = attackCooldown;
        stateManager.ReturnToIdle(State.Attacking);
    }

    private IEnumerator DoMeleeAttack()
    {
        yield return new WaitForSeconds(preMeleeAttackDelay);
        float progress = 0;

        while (progress < meleeAttackDuration && stateManager.CurrentState == State.Attacking)
        {
            yield return new WaitForFixedUpdate();

            progress += Time.fixedDeltaTime;
            meleeAttackCollider.enabled = true;
            Vector2 direction = stateManager.Direction;
            meleeAttackCollider.transform.localPosition = direction.normalized * meleeAttackColliderOffset;
            if (meleeAttackCollider.enabled && !damageMade)
            {
                Collider2D[] hitObjects = new Collider2D[100];
                ContactFilter2D playerFilter = new ContactFilter2D
                {
                    layerMask = LayerMask.GetMask("Player"),
                    useLayerMask = true,
                    useTriggers = true
                };
                int length = meleeAttackCollider.OverlapCollider(playerFilter, hitObjects);
                for (int i = 0; i < length; i++)
                {
                    damageMade = true;
                    if (hitObjects[i] != null && hitObjects[i].gameObject.tag == "bodyCollider" 
                        && GameManager.player.GetComponent<StateManager>().CurrentState != State.Dashing)
                    {
                        player.GetComponent<PlayerControlsScript>().Hit(damage, direction);
                    }
                }
            }
        }
        yield return new WaitForSeconds(postMeleeAttackDelay);
        meleeAttackCollider.enabled = false;
        damageMade = false;
        stateManager.ReturnToIdle(State.Attacking);
    }

    private IEnumerator DoWhirlwindAttack() {
        yield return new WaitForSeconds(preWhirlwindAttackDelay);
        float progress = 0;
        //Debug.Log("haha");
        //moveBody.Speed = whirlwindAttackMoveSpeed;
        while (progress < whirlwindAttackDuration && stateManager.CurrentState == State.UnstoppableAttack)
        {
            moveBody.MoveInDirection(GameManager.player.transform.position - transform.position, whirlwindAttackMoveSpeed);
            yield return new WaitForFixedUpdate();

            progress += Time.fixedDeltaTime;
            whirlwindAttackCollider.enabled = true;
            Vector2 direction = stateManager.Direction;
            if (whirlwindAttackCollider.enabled)
            {
                Collider2D[] hitObjects = new Collider2D[100];
                ContactFilter2D playerFilter = new ContactFilter2D
                {
                    layerMask = LayerMask.GetMask("Player"),
                    useLayerMask = true,
                    useTriggers = true
                };
                int length = whirlwindAttackCollider.OverlapCollider(playerFilter, hitObjects);
                for (int i = 0; i < length; i++)
                {
                    damageMade = true;
                    if (hitObjects[i] != null && hitObjects[i].gameObject.tag == "bodyCollider"
                        && GameManager.player.GetComponent<StateManager>().CurrentState != State.Dashing)
                    {
                        player.GetComponent<PlayerControlsScript>().Hit(damage, direction);
                    }
                }
            }
        }
        yield return new WaitForSeconds(postWhirlwindAttackDelay);
        whirlwindAttackCollider.enabled = false;
        moveBody.Speed = normalSpeed;
        stateManager.ReturnToIdle(State.UnstoppableAttack);
    }

    private IEnumerator DoSummonEnemies() {
        yield return new WaitForSeconds(preSummonDelay);
        for (int i = 0; i < summonNumber; i++)
        {
            if (summonCounter < summonLimit)
            {
                float angle = 360.0f / summonNumber * i;
                Vector3 offset = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * summonOffset;
                Instantiate(enemyPrefab, transform.position + offset, Quaternion.identity);
                summonCounter += 1; // TODO: Need to decrease the counter when enemy is dead.
            }
        }
        yield return new WaitForSeconds(postSummonDelay);
        stateManager.ReturnToIdle(State.UnstoppableAttack);
    }

    private IEnumerator DoComboAttack()
    {
        yield return new WaitForFixedUpdate();
    }

    private IEnumerator DoEnvironmentalAttack()
    {
        float progress = 0;

        while (progress < preEnvironmentalAttackMoveDuration)
        {
            moveBody.MoveInDirection(Vector2.up, environmentalAttackMoveSpeed * 3);
            //
            yield return new WaitForFixedUpdate();
            progress += Time.fixedDeltaTime;
        }
        //Vector2 startPosition = new Vector2(12, player.transform.position.y);
        //transform.position = startPosition;
        transform.SetPositionAndRotation(new Vector2(-12, player.transform.position.y), Quaternion.identity);
        //Debug.Log(transform.position);
        progress = 0;
        environmentalAttackCollider.enabled = true;
        //stateManager.Direction = Vector2.right;

        while (progress < environmentalAttackDuration && stateManager.CurrentState == State.UnstoppableAttack)
        {
            moveBody.MoveInDirection(Vector2.right, environmentalAttackMoveSpeed);
            //Debug.Log("Hello");
            yield return new WaitForFixedUpdate();
            //Debug.Log(progress);
            progress += Time.fixedDeltaTime;
            environmentalAttackCollider.enabled = true;
            //Vector2 direction = stateManager.Direction;
            if (environmentalAttackCollider.enabled)
            {
                Collider2D[] hitObjects = new Collider2D[100];
                ContactFilter2D playerFilter = new ContactFilter2D
                {
                    layerMask = LayerMask.GetMask("Player"),
                    useLayerMask = true,
                    useTriggers = true
                };
                int length = whirlwindAttackCollider.OverlapCollider(playerFilter, hitObjects);
                for (int i = 0; i < length; i++)
                {
                    damageMade = true;
                    if (hitObjects[i] != null && hitObjects[i].gameObject.tag == "bodyCollider"
                        && GameManager.player.GetComponent<StateManager>().CurrentState != State.Dashing)
                    {
                        player.GetComponent<PlayerControlsScript>().Hit(damage, Vector2.right, 6);
                    }
                }
            }
            Debug.Log(transform.position);
        }
        yield return new WaitForSeconds(postEnvironmentalAttackDelay);
        environmentalAttackCollider.enabled = false;
        moveBody.Speed = normalSpeed;
        stateManager.ReturnToIdle(State.UnstoppableAttack);
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
