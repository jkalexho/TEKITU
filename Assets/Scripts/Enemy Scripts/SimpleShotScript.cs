using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleShotScript : MonoBehaviour
{
    public float speed;
    public int damage;
    public Vector3 Dest { get; set; }
    public bool knockdown = false;

    public float lifespan;

    protected Rigidbody2D rigidBody;
    protected Vector2 direction;

    public bool rotate = false;

    // this is the delay between when the shot is spawned and when it starts interacting with the environment. Done to fix a bug where
    // the bullet would die immediately on spawn because it was spawned on top of a wall.
    protected float spawnTime = 0.01f;

    protected virtual void Awake()
    {
        rigidBody = this.GetComponent<Rigidbody2D>();
        if (rigidBody == null)
        {
            Debug.LogError(gameObject.ToString() + ": No movebody script found!");
        }
    }

    protected virtual void Start()
    {
        direction = (Dest - this.transform.position);
        direction = direction.normalized; // put this on a seperate line because the previous line converts things from Vector3 to Vector2 and we want to use the Vector2.normalize function
        if (rotate)
        {
            transform.right = direction;
        }
        StartCoroutine("Spawn");
    }

    protected virtual void FixedUpdate()
    {
        rigidBody.MovePosition(rigidBody.position + direction * speed * Time.fixedDeltaTime);
    }    

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(spawnTime);
        yield return new WaitForFixedUpdate();
        spawnTime = 0;
        Destroy(gameObject, lifespan);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (spawnTime <= 0)
        {
            if (other.gameObject.layer == Layer.Player)
            {
                if (knockdown)
                {
                    GameManager.player.GetComponent<PlayerControlsScript>().Hit(damage, direction, 4);
                }
                else
                {
                    GameManager.player.GetComponent<PlayerControlsScript>().Hit(damage, direction);
                }
            }
            if ((other.gameObject.layer == Layer.Player || other.gameObject.layer == Layer.Walls))
            {
                Destroy(gameObject);
            }
        }        
    }
}
