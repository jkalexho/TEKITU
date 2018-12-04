using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KamikazeScript : GenericEnemyScript {

    [Header("Kamikaze Enemy Settings")]
    #region Components
    #endregion

    #region Parameters
    public ParticleSystem deathEffect;

    [Header("Explosion Settings")]
    public float explosionDelay;
    public int explosionDamage;
    public int explosionSpeed;
    public float explosionIgnitionRange;
    public int explosionProjecileNumber;
    public GameObject projectilePrefab;
    #endregion

    #region Private variables
    #endregion

    /* Initialize all variables. 
     * Make sure StateManger and MoveBodyScript are attached
     */
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
        int nextAction = GetNextAction();
        if (nextAction == State.Running)
        {
            Move(GetNextLocation());
        }
        else if (nextAction == State.Attacking)
        {
            StartCoroutine("DoExplosion");
        }
    }

    protected override void Move(Vector2 dest)
    {
        Vector2 destPos = new Vector2(dest.x, dest.y);
        moveBody.MoveToPoint(destPos);
    }

    protected bool CanAttack()
    {
        return Vector2.Distance(transform.position, player.transform.position) < explosionIgnitionRange;
    }

    private Vector2 GetNextLocation()
    {
        return player.transform.position;
    }

    protected override void Attack()
    {

    }

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

            feetCollider.enabled = false;
            bodyCollider.enabled = false;

            base.CreateCorpse(this.gameObject, deathAnimationTiming);
        }
    }

    private IEnumerator DoExplosion()
    {
        yield return new WaitForSeconds(explosionDelay);
        var temp = DoRangedAttackSingle(explosionProjecileNumber, 360, explosionSpeed, GameManager.player.transform.position);
        yield return StartCoroutine(temp);
        //Debug.Log("haha");
        if (stateManager.TrySetState(State.Dying))
        {
            feetCollider.enabled = false;
            bodyCollider.enabled = false;
            base.Die();
            base.CreateCorpse(this.gameObject, deathAnimationTiming);
        }
    }

    private IEnumerator DoRangedAttackSingle(int num, int range, int speed, Vector3 target)
    {
        yield return new WaitForSeconds(explosionDelay);
        if (stateManager.CurrentState != State.Dying)
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
                var shot = Instantiate(projectilePrefab, transform.position, transform.rotation);
                shot.GetComponent<DirectionalShotScript>().damage = explosionDamage;
                shot.GetComponent<DirectionalShotScript>().Direction = dir;
                shot.GetComponent<DirectionalShotScript>().speed = speed;
            }
        }
    }
}
