using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VultureScript : GenericEnemyScript {

    #region Editor Variables
    [Header("Bullet Pools")]
    [SerializeField]
    private BulletPool slowBulletPool;
    [SerializeField]
    private BulletPool featherBulletPool;
    [Header("Environment")]
    [SerializeField]
    private Transform centerPoint;
    [SerializeField]
    private GameObject[] perches;
    [Header("Dashes")]
    [SerializeField]
    private VultureDashAttack[] columns;
    [SerializeField]
    private VultureDashAttack[] rows;
    [Header("Misc")]
    [SerializeField]
    private GameObject birdPrefab;
    #endregion

    //private bool invincible = false;

    private List<string> coroutines;

    private int nextCoroutine = 0;

    private int maxHitPoints;

    private List<GenericEnemyScript> listOfLiveEnemies;
    private List<GameObject> listOfDeadEnemies;

    protected override void Awake()
    {
        base.Awake();
        coroutines = new List<string>() { "MoveToRandomLocation", "Volley", "MoveToRandomLocation", "Volley", "MoveToNearestPerch", "BulletSpray", "DashAttack", "Summon"};
        maxHitPoints = healthPoint;
        listOfLiveEnemies = new List<GenericEnemyScript>();
        listOfDeadEnemies = new List<GameObject>();
    }

    // Use this for initialization
    protected override void Start()
    {
        isFlying = true;
        this.gameObject.tag = "Enemy";
        this.gameObject.layer = Layer.Enemies;
        stateManager.TrySetState(State.Invincible);
        foreach (VultureDashAttack v in columns)
        {
            v.gameObject.SetActive(false);
        }
        foreach (VultureDashAttack v in rows)
        {
            v.gameObject.SetActive(false);
        }
    }

    public void Activate()
    {
        nextCoroutine = 0;
        StartCoroutine("DoPatterns");
        StartCoroutine("UpdateEnemies");
        this.transform.position = centerPoint.position - new Vector3(0,1,0);
        foreach (VultureDashAttack v in columns)
        {
            v.gameObject.SetActive(true);
        }
        foreach (VultureDashAttack v in rows)
        {
            v.gameObject.SetActive(true);
        }
        player = GameManager.player;
    }

    public void Reset()
    {
        StopAllCoroutines();
        stateManager.ReturnToIdle(State.Spawning);
        stateManager.TrySetState(State.Invincible);
        healthPoint = maxHitPoints;
        foreach (VultureDashAttack v in columns)
        {
            v.Reset();
            v.gameObject.SetActive(false);
        }
        foreach (VultureDashAttack v in rows)
        {
            v.Reset();
            v.gameObject.SetActive(false);
        }
        if (listOfDeadEnemies.Count > 0)
        {
            foreach (GameObject go in listOfDeadEnemies)
            {
                Destroy(go);
            }
        }
        if (listOfLiveEnemies.Count > 0)
        {
            foreach (GenericEnemyScript go in listOfLiveEnemies)
            {
                Destroy(go.gameObject);
            }
        }
        listOfDeadEnemies.Clear();
        listOfLiveEnemies.Clear();
        slowBulletPool.Reset();
        featherBulletPool.Reset();
        this.transform.position = centerPoint.position - new Vector3(0, 1, 0);
    }
    
    private IEnumerator DoPatterns()
    {
        stateManager.ReturnToIdle(State.Invincible);
        yield return new WaitForSeconds(spawnTime); // spawn delay
        while (true)
        {
            StopCoroutine(coroutines[nextCoroutine]);
            yield return StartCoroutine(coroutines[nextCoroutine]);
            nextCoroutine++;
            
            if (nextCoroutine >= 6 && healthPoint > maxHitPoints * 0.8f)
            {
                nextCoroutine = 0;
            }
            
            if (nextCoroutine >= coroutines.Capacity)
            {
                nextCoroutine = 0;
            }
        }
    }

    private IEnumerator UpdateEnemies()
    {
        while (true)
        {
            yield return null;
            if (listOfLiveEnemies.Count != 0)
            {
                for (int i = 0; i < listOfLiveEnemies.Count; i++)
                {
                    if (listOfLiveEnemies[i].isDead == true)
                    {
                        listOfDeadEnemies.Add(listOfLiveEnemies[i].gameObject);
                        listOfLiveEnemies.Remove(listOfLiveEnemies[i]);
                    }
                }
            }
        }
    }

    #region Coroutines
    IEnumerator MoveToRandomLocation()
    {
        if (stateManager.TrySetState(State.Running))
        {
            Vector2 destination = Random.insideUnitCircle * 9 + (Vector2) centerPoint.position;
            moveBody.MoveToPoint(destination);
            yield return null;
        }
    }

    IEnumerator MoveToNearestPerch()
    {
        float closestDistance = Mathf.Infinity;
        float currentDistance = 0;
        Vector2 target = this.transform.position;
        foreach(GameObject t in perches)
        {
            currentDistance = Vector3.Distance(t.transform.position, this.transform.position);
            if (currentDistance < closestDistance)
            {
                target = t.transform.position;
                closestDistance = currentDistance;
            }
        }
        if (stateManager.TrySetState(State.Running))
        {
            moveBody.MoveToPoint(target);
            while (stateManager.CurrentState != State.Idle)
            {
                // wait for move to finish
                yield return null;
            }
        }
        stateManager.TrySetState(State.Perched);
        yield return new WaitForSeconds(1);
    }

    IEnumerator Summon()
    {
        if (stateManager.TrySetState(State.Running))
        {
            yield return new WaitForSeconds(2);
            if (listOfLiveEnemies.Count < 3)
            {
                GameObject enemy = Instantiate(birdPrefab, this.transform.position + new Vector3(3, 0, 0), Quaternion.identity);
                listOfLiveEnemies.Add(enemy.GetComponent<GenericEnemyScript>());
                enemy = Instantiate(birdPrefab, this.transform.position + new Vector3(-3, 0, 0), Quaternion.identity);
                listOfLiveEnemies.Add(enemy.GetComponent<GenericEnemyScript>());
            }
            if (healthPoint < maxHitPoints * 0.5f && listOfLiveEnemies.Count < 4)
            {
                GameObject enemy = Instantiate(birdPrefab, this.transform.position + new Vector3(0, -3, 0), Quaternion.identity);
                listOfLiveEnemies.Add(enemy.GetComponent<GenericEnemyScript>());
            }
            if (healthPoint < maxHitPoints * 0.2f && listOfLiveEnemies.Count < 5)
            {
                GameObject enemy = Instantiate(birdPrefab, this.transform.position + new Vector3(0, 3, 0), Quaternion.identity);
                listOfLiveEnemies.Add(enemy.GetComponent<GenericEnemyScript>());
            }
        }
    }

    // this is for camera stuff
    IEnumerator StickToPlayer()
    {
        while (stateManager.CurrentState == State.Invincible)
        {
            this.transform.position = GameManager.player.transform.position;
            yield return null;
        }
    }

    IEnumerator DashAttack()
    {
        if (stateManager.TrySetState(State.Invincible))
        {
            yield return new WaitForSeconds(1);
            StartCoroutine("StickToPlayer");
            if (healthPoint > maxHitPoints * 0.4f)
            {
                foreach (VultureDashAttack v in columns)
                {
                    if (v.ContainsPlayer())
                    {
                        v.Attack();
                    }
                }
                yield return new WaitForSeconds(2);
                foreach (VultureDashAttack v in rows)
                {
                    if (v.ContainsPlayer())
                    {
                        v.Attack();
                    }
                }
                yield return new WaitForSeconds(2);
                stateManager.ReturnToIdle(State.Invincible);
                this.transform.position = centerPoint.position + new Vector3(0,2,0);
            }
            else
            {
                // all three columns and rows get attacked. We determine the direction randomly using math
                int x = Random.Range(0, 2); // if x is 0, then positive direction
                int direction = -2 * x + 1; // either -1 or 1.
                int i = 2 * x;
                int target = 3 - (4 * x);
                while (i != target)
                {
                    columns[i].Attack();
                    yield return new WaitForSeconds(0.5f);
                    i += direction;
                }
                yield return new WaitForSeconds(1);
                x = Random.Range(0, 2);
                direction = -2 * x + 1;
                i = 2 * x;
                target = 3 - (4 * x);
                while (i != target)
                {
                    rows[i].Attack();
                    yield return new WaitForSeconds(0.5f);
                    i += direction;
                }
                yield return new WaitForSeconds(2);
                stateManager.ReturnToIdle(State.Invincible);
                this.transform.position = centerPoint.position + new Vector3(0,2,0);
            }
        }
        yield return null;
    }

    IEnumerator BulletSpray()
    {
        yield return new WaitForSeconds(1f);
        if (stateManager.TrySetState(State.SprayAttack)) {
            float startAngle = 90 * Mathf.Deg2Rad;
            float angleMod = 12 * Mathf.Deg2Rad;
            float delay = 0.14f;
            int shotCount = 28;
            if (healthPoint <= maxHitPoints * 0.7f)
            {
                shotCount += 20;
                delay *= 0.7f;
            }
            if (healthPoint <= maxHitPoints * 0.5f)
            {
                shotCount += 20;
                delay *= 0.7f;
            }
            for (int i = 0; i < shotCount; i++)
            {
                slowBulletPool.Spawn(this.transform.position, new Vector2(Mathf.Cos(startAngle + angleMod * i), Mathf.Sin(startAngle + angleMod * i)));
                slowBulletPool.Spawn(this.transform.position, new Vector2(Mathf.Cos(startAngle - angleMod * i), Mathf.Sin(startAngle - angleMod * i)));
                yield return new WaitForSeconds(delay);
            }
        }
        stateManager.ReturnToIdle(State.SprayAttack);
    }

    IEnumerator Volley()
    {
        if (stateManager.TrySetState(State.Running))
        {
            yield return new WaitForSeconds(1);
            int bulletCount = 1;
            if (healthPoint <= maxHitPoints * 0.6f)
            {
                bulletCount += 2;
            }
            if (healthPoint <= maxHitPoints * 0.4f)
            {
                bulletCount += 2;
            }
            for (int i = 0; i < 3; i++)
            {
                stateManager.newAttack = true;
                yield return new WaitForSeconds(0.5f);
                ShootWaveAtPlayer(bulletCount, 15);

                yield return new WaitForSeconds(1);
                bulletCount++;
            }
        }
    }
    #endregion
    // angleBetween is in degrees
    private void ShootWaveAtPlayer(int count, float angleBetween)
    {
        angleBetween *= Mathf.Deg2Rad;
        float totalAngle = (count - 1) * angleBetween;
        float targetAngle = Vector2.SignedAngle(Vector2.right, player.transform.position - this.transform.position) * Mathf.Deg2Rad; // angle to player in radians
        float startAngle = targetAngle - totalAngle * 0.5f; // start angle in radians

        float angle;
        Vector2 direction;

        for (int i = 0; i < count; i++)
        {
            angle = startAngle + i * angleBetween;
            direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            featherBulletPool.Spawn(this.transform.position, direction);
        }
    }

    #region Useless
    protected override void Move(Vector2 dest)
    {
        
    }

    protected override void Attack()
    {
        
    }
    #endregion

    public override void Hit(int damage, Vector2 direction, float pushStrength)
    {
        if (stateManager.CurrentState != State.Invincible)
        {
            PlayHitEffects(direction, damage);
            TakeDamage(damage);
        }
    }

    public override void Die()
    {
        if (stateManager.TrySetState(State.Falling))
        {

            feetCollider.enabled = false;
            bodyCollider.enabled = false;
            base.Die();
            StartCoroutine("DoDie");
        }
    }

    IEnumerator DoDie()
    {
        
        GameManager.pc.isInvincible = true;
        Time.timeScale = 0.5f;
        yield return new WaitForSeconds(0.5f);
        GameManager.screenFlasher.FadeToWhite(1);
        yield return new WaitForSeconds(1);
        Time.timeScale = 1;
        SceneManager.LoadScene("Stage 1 Outro");
    }
}
