using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VultureDashAttack : MonoBehaviour {

    [SerializeField]
    private Animator runway;

    [SerializeField]
    private Animator vulture;

    [SerializeField]
    private Vector2 direction;

    private ParticleSystem particles;

    private BoxCollider2D box;

    void Awake()
    {
        particles = vulture.GetComponent<ParticleSystem>();
        vulture.GetComponentInChildren<EnemyAttackCollider>().Direction = direction;
        box = this.GetComponent<BoxCollider2D>();
    }

    public bool ContainsPlayer()
    {
        return box.bounds.Contains(GameManager.player.transform.position);
    }

    public void Reset()
    {
        vulture.SetTrigger("reset");
        runway.SetTrigger("reset");
    }

	public void Attack()
    {
        StartCoroutine("DoAttack");
    }

    IEnumerator DoAttack()
    {
        yield return new WaitForEndOfFrame();
        runway.SetTrigger("reveal");
        yield return new WaitForSeconds(1.5f);
        vulture.SetTrigger("dash");
        particles.Play();
    }
}
