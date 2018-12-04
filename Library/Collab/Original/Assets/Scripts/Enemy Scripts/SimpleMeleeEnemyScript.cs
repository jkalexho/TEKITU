using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMeleeEnemyScript : GenericEnemyScript
{
    public int testHealthPoint = 10; 
    public int testSpeed = 2; 
    public int testDamage = 1; 
    public float testAttackRange = 0.1f; //FIXME: hardcoded for testing purposes

    #region Components
    public BoxCollider2D attackCollider;
    #endregion

    #region Parameters
    public float attackRange;
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
        attackCollider.enabled = true; //FIXME: change this to false after implementing attackCollider
        damageMade = false;
    }

    protected override void Start()
    {
        base.Start();
        healthPoint = testHealthPoint;
        speed = testSpeed;
        damage = testDamage;
        attackRange = testAttackRange;
        moveBody.Speed = speed;
    }

    void Update() // move is called from a fixedUpdate function.
    {
        int nextAction = GetNextAction();
        if (nextAction == State.Running)
        {
            Move(GetNextLocation());
        }
        else if (nextAction == State.Attacking)
        {
            attackCollider.enabled = true;
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

    /* https://medium.com/quick-code/how-to-make-2d-melee-combat-in-unity-practical-tutorials-c5aa0a26e621
     * Adapated the idea of DamageMaker in this url to allow more complex attack action
     * TODO: Location and .enabled of AttackCollider should match the animation in every frame. To be implemented in editor
     * TODO: Add a trigger box collider to Player
     */
    protected override void Attack()
    {
        Debug.Log("SimpleMeleeEnemy is attacking the player");

        if (attackCollider.enabled && !damageMade) {
            damageMade = true;
            Collider2D[] hittedObjects = new Collider2D[100];
            ContactFilter2D playerFilter = new ContactFilter2D
            {
                layerMask = LayerMask.GetMask("Player")
            };
            attackCollider.OverlapCollider(playerFilter, hittedObjects);
            for (int i = 0; i < hittedObjects.Length; i++)
            {
                //Debug.Log(hittedObjects[i].gameObject);
                if (hittedObjects[i] != null && hittedObjects[i].gameObject == player)
                {
                    player.GetComponent<PlayerControlsScript>().Hit(damage);
                    Debug.Log("Player is hit by an enemy!");
                }
            }
        } else if(!attackCollider.enabled && damageMade) 
        {
            damageMade = false;
            stateManager.ReturnToIdle(State.Attacking);
        }
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
        else if (stateManager.TrySetState(State.Running))
        {
            return State.Running;
        }
        return State.Idle;
    }
}
