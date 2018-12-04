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
    public Collider2D feetCollider;
    public Collider2D bodyCollider;
    public float deathAnimationTiming;
    //public float attackRange; This is something that controls the AI of the enemy, but the AI of each enemy is different, so having this variable is pretty useless.
    public int healthPoint;
    public int damage;
    public bool isFlying = false;
    public bool isDead = false;
    protected int chasmCount = 0;

    protected bool inAir;
    private Vector3 originalPosition;

    public GameObject hitEffect;
    protected ParticleSystem[] hitEffects;
    protected GameObject hitEffectPermanent;

    [SerializeField]
    protected float spawnTime = 1.2f;

    //    public ParticleSystem hitEffect;
    #endregion

    protected virtual void Awake()
    {
        isDead = false;
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
        if (hitEffect != null)
        {
            hitEffects = hitEffect.GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < hitEffect.transform.childCount; i++)
            {
                if (hitEffect.transform.GetChild(i).CompareTag("permanentBlood"))
                {
                    hitEffectPermanent = hitEffect.transform.GetChild(i).gameObject;
                    break;
                }
            }
        }
    }

    protected virtual void Start()
    {
        this.gameObject.tag = "Enemy";
        this.gameObject.layer = Layer.Enemies;
        player = GameManager.player;
        StartCoroutine("DoSpawn");
    }
    

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
    

    protected float DistanceToPlayer()
    {
        return Vector3.Distance(this.transform.position, GameManager.player.transform.position);
    }

    /* Actions to take when the enemy dies
     * FIXME: Might need to call GameManager to manage death
     */
    public virtual void Die()
    {
        isDead = true;
        if (hitEffect != null)
        {
            hitEffectPermanent.transform.parent = hitEffect.transform.parent;
            hitEffectPermanent.transform.localScale = new Vector3(1, 1, 1);
            hitEffect.transform.parent = null;
            Destroy(hitEffect, hitEffects[0].main.duration);
        }
    }

    /* Actions to take when the enemy is hit. The direction and push strength determines how the object is moved upon being hit.
     */
    public virtual void Hit(int damage)
    {
        //hitEffect.Play();

        if (stateManager.TrySetState(State.Hit))
        {
            PlayHitEffects(new Vector3(0,1,0), damage);
            TakeDamage(damage);
            stateManager.ReturnToIdle(State.Hit);
        }
        else if (stateManager.CurrentState == State.UnstoppableAttack || stateManager.CurrentState == State.Pushed)
        {
            PlayHitEffects(new Vector3(0, 1, 0), damage);
            TakeDamage(damage);
        }
    }

    // this function should call the pushInDirection() function in the movebodyscript, using the pushStrength and direction.
    public virtual void Hit(int damage, Vector2 direction, float pushStrength)
    {
        //hitEffect.Play();

        if (stateManager.TrySetState(State.Pushed))
        {
            PlayHitEffects(direction, damage);
            TakeDamage(damage);
            moveBody.PushInDirection(direction, pushStrength);
            StopCoroutine("DoPush");
            StartCoroutine("DoPush", pushStrength);
        }
        else if (stateManager.CurrentState == State.UnstoppableAttack)
        {
            PlayHitEffects(direction, damage);
            TakeDamage(damage);
        }
    }

    protected virtual void PlayHitEffects(Vector2 direction, float strength)
    {
        if (hitEffect != null)
        {
            hitEffect.transform.localRotation = Quaternion.Euler(0, 0, Cardinal.VectorToAngle(direction));
            hitEffect.transform.localScale = new Vector3(strength * strength, 1, 1);
            foreach (ParticleSystem ps in hitEffects)
            {
                ps.Play();
            }
        }
    }

    protected virtual void TakeDamage(int damage)
    {
        stateManager.damaged = true;
        healthPoint -= damage;
        if (healthPoint <= 0)
        {
            stateManager.ReturnToIdle(State.Falling);
            Die();
        }
    }

    protected IEnumerator DoPush(float pushStrength)
    {
        if (!isFlying)
        {
            feetCollider.gameObject.layer = Layer.EnemyInAir;
        }
        float progress = pushStrength / moveBody.GetMass() / 2;
        yield return new WaitForSeconds(progress);
        while (progress > 0 && stateManager.CurrentState == State.Pushed)
        {
            yield return new WaitForFixedUpdate();
            progress -= Time.fixedDeltaTime;
            
            if (inAir && !isFlying)
            {
                if (stateManager.TrySetState(State.Falling))
                {
                    StartCoroutine("DoFall");
                }
            }
        }
        if (stateManager.CurrentState == State.Pushed)
        {
            stateManager.ReturnToIdle(State.Pushed);
            if (!isFlying)
            {
                feetCollider.gameObject.layer = Layer.EnemyFeet;
            }
        }
    }

    protected IEnumerator DoFall()
    {
        yield return new WaitForSeconds(1.5f);
        stateManager.ReturnToIdle(State.Falling);
        Die();
    }

    protected IEnumerator DoSpawn()
    {
        stateManager.TrySetState(State.Spawning);
        yield return new WaitForSeconds(spawnTime);
        stateManager.ReturnToIdle(State.Spawning);
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
                Destroy(comp, deathAnimationTiming*1.5f);
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
                // we preserve the permanent blood objects
                if (!objToCorpsify.transform.GetChild(i).CompareTag("permanentBlood"))
                {
                    CreateCorpse(objToCorpsify.transform.GetChild(i).gameObject, deathAnimationTiming);
                }
                
            }
        }

        //If you're empty and you don't have children with sprite renderers
        if (destroyObjItself == true && objToCorpsify.transform.childCount == 0)
        {
            // print("destory entire:" + objToCorpsify.name);
            Destroy(objToCorpsify);
        }
    }

    #region falling
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == Layer.Chasms && !inAir) // detects if the player is on the edge of the platform
        {
            //originalPosition = this.transform.position;
            chasmCount++;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == Layer.Chasms)
        {
            inAir = false;
            chasmCount--;
        }
    }

    // checks if the player is in the air or not
    public virtual void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer == Layer.Chasms)
        {
            if (other.bounds.Contains(feetCollider.bounds.center + new Vector3(0.07f,0.07f,0)) && other.bounds.Contains(feetCollider.bounds.center - new Vector3(0.07f,0.07f,0)))
            {
                inAir = true;
            }
            else if (chasmCount <= 1)
            {
                inAir = false;
            }
        }
    }
    #endregion

}
