using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkScript : GenericEnemyScript
{

    #region Editor Variables
    [Header("Shark Enemy Settings")]
    [SerializeField]
    private Collider2D attackCollider;
    [SerializeField]
    private Transform centerPoint;
    [SerializeField]
    private float patrolMaxRange;
    [SerializeField]
    private float attackCooldown = 7;
    [Header("Charge Settings")]
    [SerializeField]
    private float chargeRange;
    [SerializeField]
    private float chargeMaxSpeed;
    [SerializeField]
    private float chargeDuration;
    #endregion

    private Vector3 centerPosition;

    private float angle;

    private Vector3 destination;

    private float cooldown;
    
    private EnemyAttackCollider attackColliderScript;

    protected override void Awake()
    {
        base.Awake();
        if (attackCollider != null)
        {
            attackColliderScript = attackCollider.gameObject.GetComponent<EnemyAttackCollider>();
        }
        //centerPosition = centerPoint.position;
    }

    // Use this for initialization
    protected override void Start()
    {
        cooldown = attackCooldown / 2;
        attackCollider.enabled = false;
        destination = this.transform.position;
        bodyCollider.enabled = false;
        base.Start();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Cooldown();
        if (CanAttack())
        {
            Attack();
        }
        else if (Vector3.Distance(this.transform.position, destination) <= 1)
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
        }
    }

    private bool CanAttack()
    {
        return cooldown <= 0 && DistanceToPlayer() < chargeRange;
    }

    private void MakeMove()
    {
        if (stateManager.TrySetState(State.Running))
        {
            angle += 0.506f;
            if (angle > 2 * Mathf.PI)
            {
                angle -= 2 * Mathf.PI;
            }
            float distance = Random.Range(patrolMaxRange * 0.5f, patrolMaxRange);
            destination = centerPosition + new Vector3(Mathf.Cos(angle) * distance, Mathf.Sin(angle) * distance, 0);
            Move(destination);
        }
    }

    protected override void Move(Vector2 dest)
    {
        Vector2 destPos = new Vector2(dest.x, dest.y);
        moveBody.MoveToPoint(destPos);
    }

    protected override void Attack()
    {
        if (stateManager.TrySetState(State.UnstoppableAttack))
        {
            StartCoroutine("DoAttack");
        }
    }

    private IEnumerator DoAttack()
    {
        bodyCollider.enabled = true;
        float progress = 0;
        Vector3 originalDestination = destination;
        while (progress < 1)
        {
            yield return new WaitForFixedUpdate();
            progress += Time.fixedDeltaTime * 2;
            destination = Vector3.Lerp(originalDestination, GameManager.player.transform.position, progress);
            moveBody.MoveInDirection(destination - this.transform.position);
            stateManager.Direction = destination - this.transform.position;
        }
        
        attackCollider.enabled = true;

        float chargeAcceleration = (chargeMaxSpeed - moveBody.Speed) * 4 / chargeDuration;
        progress = 0;
        float chargeSpeed = 0;
        attackCollider.offset = stateManager.Direction.normalized * 0.4f;
        attackColliderScript.Direction = stateManager.Direction;
        while (progress < chargeDuration && stateManager.CurrentState == State.UnstoppableAttack)
        {
            yield return new WaitForFixedUpdate();
            if (progress < chargeDuration / 4)
            {
                chargeSpeed += chargeAcceleration * Time.fixedDeltaTime;
            }
            progress += Time.fixedDeltaTime;
            if (progress > chargeDuration * 3 / 4)
            {
                chargeSpeed -= chargeAcceleration * Time.fixedDeltaTime;
            }
            moveBody.MoveInDirection(stateManager.Direction, chargeSpeed + moveBody.Speed);
        }
        
        attackCollider.enabled = false;
        cooldown = attackCooldown;
        if (stateManager.CurrentState != State.UnstoppableAttack)
        {
            yield return new WaitForSeconds(3);
            cooldown += 3;
        }
        stateManager.ReturnToIdle(State.UnstoppableAttack);
        if (stateManager.TrySetState(State.Running))
        {
            //angle = Cardinal.VectorToAngle(this.transform.position - centerPosition);
            angle = Vector3.SignedAngle(new Vector3(1, 0, 0), this.transform.position - centerPosition, new Vector3(0,0,1)) * Mathf.Deg2Rad;
            MakeMove();
        }
        bodyCollider.enabled = false;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == Layer.Walls)
        {
            if (stateManager.CurrentState == State.UnstoppableAttack)
            {
                
                stateManager.ReturnToIdle(State.UnstoppableAttack);
                stateManager.TrySetState(State.Pushed);
                ContactPoint2D[] contacts = new ContactPoint2D[3];
                attackCollider.GetContacts(contacts);
                moveBody.PushInDirection((Vector2) this.transform.position - contacts[0].point - moveBody.Direction, moveBody.GetMass()*3);
                TakeDamage(0);
                if (TekituCameraShaker.Instance != null)
                {
                    TekituShakeParameters shakeSettings = new TekituShakeParameters(1, new Vector3(0,1,0), 0.5f, 0.4f);
                    StartCoroutine(TekituCameraShaker.Instance.Shake(shakeSettings));
                }
            }
        }
    }

    public override void Hit(int damage, Vector2 direction, float pushStrength)
    {
        //hitEffect.Play();
        if (stateManager.CurrentState == State.UnstoppableAttack || stateManager.CurrentState == State.Pushed)
        {
            PlayHitEffects(direction, damage);
            TakeDamage(damage);
        }
    }

    public void SetCenterPoint(Vector3 point)
    {
        centerPosition = point;
        patrolMaxRange = Vector3.Distance(this.transform.position, centerPosition);
        angle = Vector3.SignedAngle(new Vector3(1, 0, 0), this.transform.position - centerPosition, new Vector3(0, 0, 1)) * Mathf.Deg2Rad;
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
