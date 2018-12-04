using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericEnemyScript : MonoBehaviour
{
    #region Components
    protected MoveBodyScript moveBody;
    protected StateManager stateManager;
    #endregion

    #region Common attributes
    // attributes that every enemy should have
    protected GameObject player;
    //public float attackRange; This is something that controls the AI of the enemy, but the AI of each enemy is different, so having this variable is pretty useless.
    public int healthPoint;
    public int damage;

    //    public ParticleSystem hitEffect;
    #endregion

    protected virtual void Awake()
    {
        stateManager = GetComponent<StateManager>();
        if (stateManager == null)
        {
            Debug.LogError(gameObject.ToString() + ": No state manager found!");
        }
        moveBody = this.GetComponent<MoveBodyScript>();
        if (moveBody == null)
        {
            Debug.LogError(gameObject.ToString() + ": No move body script found!");
        }
    }

    protected virtual void Start()
    {
        this.gameObject.tag = "Enemy";
        this.gameObject.layer = 11;
        player = GameManager.player;
    }

    /* This function is redundant. It is only ever called once in any creature's script.
     
    protected virtual int GetNextAction()
    {
        /* Determine the next action
         * - if the player can be attacked, attack. Otherwise, move
         /
        if (CanAttack())
        {
            if (stateManager.TrySetState(State.Attacking))
            {
                return State.Attacking;
            }
        } else if (stateManager.TrySetState(State.Running))
            {
                return State.Running;
            }
        return State.Idle;
    }
    */

    /* Action: Move
     * Move towards the postion of destination
     */
    protected abstract void Move(Vector2 dest);

    /* Determine the position to move to in the next frame
     * 
     */
    //protected abstract Vector2 GetNextLocation(); // this is also a fairly useless function. 

    /* Action: Attack
     */
    protected abstract void Attack();

    /* Determine if the player can be attacked
     * True if the player is
     * - within attack range for melee enemies
     * - in line of sight for ranged enemies
     */
    //protected abstract bool CanAttack(); // NOTE: this doesn't seem pretty useful. The idea of having a method in the inheritable class is so that anything can call that function. This is something that is pretty unique for each enemy, so having it as a common variable but then having to change it in the end is pretty redundant.

    /* Actions to take when the enemy dies
     * FIXME: Might need to call GameManager to manage death
     */
    public virtual void Die()
    {

    }

    /* Actions to take when the enemy is hit. The direction and push strength determines how the object is moved upon being hit.
     */
    public virtual void Hit(int damage)
    {
        //hitEffect.Play();

        if (stateManager.TrySetState(State.Hit))
        {
            TakeDamage(damage);
            stateManager.ReturnToIdle(State.Hit);
        }
        else if (stateManager.CurrentState == State.UnstoppableAttack)
        {
            TakeDamage(damage);
        }
    }

    // this function should call the pushInDirection() function in the movebodyscript, using the pushStrength and direction.
    public virtual void Hit(int damage, Vector2 direction, float pushStrength)
    {
        //hitEffect.Play();

        if (stateManager.TrySetState(State.Pushed))
        {
            TakeDamage(damage);
            moveBody.PushInDirection(direction, pushStrength);
            StopCoroutine("DoPush");
            StartCoroutine("DoPush", pushStrength);
        }
        else if (stateManager.CurrentState == State.UnstoppableAttack)
        {
            TakeDamage(damage);
        }
    }

    protected virtual void TakeDamage(int damage)
    {
        healthPoint -= damage;
        if (healthPoint <= 0)
        {
            Die();
        }
    }

    protected IEnumerator DoPush(float pushStrength)
    {
        yield return new WaitForSeconds(pushStrength / moveBody.GetMass());

        stateManager.ReturnToIdle(State.Pushed);
    }

    //This function strips all components except the sprite renderers to create a dead sprite on death.
    protected virtual void CreateCorpse(GameObject objToCorpsify, float deathAnimationTiming)
    {
        Component[] components = objToCorpsify.GetComponents<Component>();
        bool destroyObjItself = true;

        foreach (Component comp in components)
        {
            if (comp.GetType() != typeof(SpriteRenderer) && comp.GetType() != typeof(Transform))
            {
                Destroy(comp, deathAnimationTiming);
            }

            //When the whole gameobject doesn't even have a sprite renderer, we should just destroy the whole thing.
            if (comp.GetType() == typeof(SpriteRenderer))
            {
                destroyObjItself = false;
            }

        }

        if (objToCorpsify.transform.childCount != 0)
        {
            // print("obj has children to corpsify");
            for (int i = 0; i < objToCorpsify.transform.childCount; i++)
            {
                // print("corpsifying child" + i + ": " + objToCorpsify.transform.GetChild(i).name);      
                CreateCorpse(objToCorpsify.transform.GetChild(i).gameObject, deathAnimationTiming);
            }
        }

        //If you're empty and you don't have children with sprite renderers
        if (destroyObjItself == true && objToCorpsify.transform.childCount == 0)
        {
            // print("destory entire:" + objToCorpsify.name);
            Destroy(objToCorpsify);
        }
    }

}
