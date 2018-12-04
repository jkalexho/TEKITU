using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolShotScript : SimpleShotScript {

    public bool Active { get; set; }

    protected override void Start()
    {
        Deactivate();
    }

    public void Shoot(Vector3 location, Vector2 dir)
    {
        Active = true;
        spawnTime = 0.01f;
        direction = dir;
        if (rotate)
        {
            transform.right = direction;
        }
        this.transform.position = location;
        StopCoroutine("Spawn");
        StartCoroutine("Spawn");
    }

    protected override void FixedUpdate()
    {
        if (Active)
        {
            rigidBody.MovePosition(rigidBody.position + direction * speed * Time.fixedDeltaTime);
        }
    }

    private IEnumerator Spawn()
    {
        yield return new WaitForSeconds(spawnTime);
        yield return new WaitForFixedUpdate();
        spawnTime = 0;
        yield return new WaitForSeconds(lifespan);
        Deactivate();
    }

    public void Deactivate()
    {
        Active = false;
        this.gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (spawnTime <= 0 && Active)
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
                Deactivate();
            }
        }
    }
}
