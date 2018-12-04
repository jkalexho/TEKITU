using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : GenericAnimationController {

    public float dashProgress = 0;

    private int cardinal;
    private Vector2 direction;

    private string state;
    
    private float countdown;

    private bool countingDown;

    [SerializeField]
    [Tooltip("The shadow objects")]
    private List<GameObject> kuroShadows;

    [SerializeField]
    [Tooltip("The feather prefabs")]
    private List<GameObject> feathers;

    [SerializeField]
    [Tooltip("The special effects object")]
    private GameObject specialEffects;

    [SerializeField]
    [Tooltip("The rings")]
    private Transform[] ringEffects;

    [SerializeField]
    private float healDuration = 1.2f;

    private Animator specialEffectsAnimator;

    private ParticleSystem particles;

    private float iFrameDuration;

    public GameObject hitEffect;
    private ParticleSystem[] hitEffects;

    protected override void Awake()
    {
        base.Awake();
        
        foreach (GameObject g in kuroShadows)
        {
            g.transform.SetParent(null);
        }
        particles = this.GetComponent<ParticleSystem>();
        if (particles == null)
        {
            Debug.LogError(gameObject.ToString() + ": No particle system found!");
        }
        iFrameDuration = this.GetComponentInParent<PlayerControlsScript>().iFrameDuration;
        specialEffectsAnimator = specialEffects.GetComponent<Animator>();
        if (specialEffectsAnimator == null)
        {
            Debug.LogError(gameObject.ToString() + ": No special effects animator found!");
        }
        specialEffects.transform.SetParent(null);
        if (hitEffect != null)
        {
            hitEffects = hitEffect.GetComponentsInChildren<ParticleSystem>();
        }
        foreach (Transform t in ringEffects)
        {
            t.gameObject.SetActive(false);
        }
    }

    void Start()
    {
        cardinal = stateManager.CardinalDirection;
        originalState = 0;
        direction = stateManager.Direction;
        countdown = 0;
        countingDown = false;
        foreach (GameObject g in kuroShadows)
        {
            g.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
        }
    }

    protected override void Update()
    {
        base.Update();
        newState = stateManager.CurrentState;
        Vector2 newDirection = stateManager.Direction;
        
        if (stateManager.newDash)
        {
            animator.SetTrigger("DoDash");
            stateManager.newDash = false;
            StopCoroutine("DoDash");
            StartCoroutine("DoDash");
        }

        if (stateManager.newAttack && stateManager.CurrentState == State.Attacking)
        {
            animator.SetTrigger("DoLightAttack");
            animator.SetInteger("Combo", stateManager.combo);
            stateManager.newAttack = false;
        }

        if (stateManager.newPush && stateManager.CurrentState == State.Pushed)
        {
            PlayHitEffects();
            animator.SetInteger("HitDirection", Cardinal.VectorToCardinal4(stateManager.HitDirection));
            animator.SetTrigger("Knockup");
            stateManager.newPush = false;
            StopCoroutine("DoPush");
            StartCoroutine("DoPush");
            GameManager.screenFlasher.FlashScreen(stateManager.PushDuration);
        }

        if (stateManager.newAttack && stateManager.CurrentState == State.UnstoppableAttack)
        {
            animator.SetTrigger("Whirlwind");
            specialEffectsAnimator.SetTrigger("whirlwind");
            stateManager.newAttack = false;
            StopCoroutine("DoWhirlwind");
            StartCoroutine("DoWhirlwind");
        }

        if (stateManager.newHeal)
        {
            animator.SetBool("Heal", true);
            StopCoroutine("DoHeal");
            StartCoroutine("DoHeal");
            GameManager.screenFlasher.FlashScreenWhite();
            stateManager.newHeal = false;
        }

        if (stateManager.newHit && stateManager.CurrentState == State.Hit)
        {
            PlayHitEffects();
            animator.SetTrigger("Hit");
            int hitDirection = Cardinal.VectorToCardinal(stateManager.HitDirection);
            if (cardinal < 4)
            {
                if (hitDirection > 2 && hitDirection <= 6)
                {
                    animator.SetInteger("HitDirection", 3);
                } else
                {
                    animator.SetInteger("HitDirection", 1);
                }
            } else
            {
                if (hitDirection >= 2 && hitDirection < 6)
                {
                    animator.SetInteger("HitDirection", 5);
                } else
                {
                    animator.SetInteger("HitDirection", 7);
                }
            }
            stateManager.newHit = false;
            GameManager.screenFlasher.FlashScreen();
        } else if (stateManager.newHit)
        {
            GameManager.screenFlasher.FlashScreen();
            stateManager.newHit = false;
        }

        if (stateManager.dead && stateManager.CurrentState == State.Dying)
        {
            PlayHitEffects();
            stateManager.dead = false;
            animator.SetTrigger("Die");
            GameManager.screenFlasher.FadeToRed(1);
        }
        

        originalState = newState;

        // if the direction changed
        // the purpose of the countdown is so that the end of diagonal movement is smoother.
        if (Vector2.Angle(direction, newDirection) > 25)
        {
            if (originalState == State.Running || originalState == State.Attacking)
            {
                if (countdown < 0 && countingDown)
                {
                    direction = newDirection;
                    cardinal = Cardinal.VectorToCardinal(newDirection);
                    //direction = Cardinal.CardinalToVector(cardinal);
                    stateManager.CardinalDirection = cardinal;
                    countingDown = false;
                    animator.SetFloat("StartOffset", animator.GetFloat("CycleOffset"));
                }
                else if (countingDown == false)
                {
                    countdown = 0.05f;
                    countingDown = true;
                }
            }
            else if (originalState == State.Dashing)
            {
                direction = newDirection;
                cardinal = Cardinal.VectorToCardinal(direction);
                stateManager.CardinalDirection = cardinal;
                countingDown = false;
            }
        }

        if (countdown >= 0)
        {
            countdown -= Time.deltaTime;
        }

        animator.SetInteger("CardinalDirection4", Cardinal.VectorToCardinal4(newDirection));
        animator.SetInteger("State", originalState);
        animator.SetInteger("CardinalDirection", cardinal);
    }

    IEnumerator DoDash()
    {
        particles.Play();
        float progress = 0;
        float featherProgress = 0;
        float shadowProgress = 0;
        int shadowIndex = 0;
        if (stateManager.empowered)
        {
            specialEffects.transform.position = this.transform.position;
            animator.SetTrigger("empowered");
            specialEffectsAnimator.SetTrigger("dash");
            specialEffects.transform.rotation = Quaternion.Euler(0, 0, Cardinal.VectorToAngle(stateManager.Direction));
        }
        while (stateManager.CurrentState == State.Dashing)
        {
            yield return new WaitForFixedUpdate();
            specialEffects.transform.position = this.transform.position;
            progress += Time.fixedDeltaTime;
            if (progress > featherProgress)
            {
                if (stateManager.empowered)
                {
                    featherProgress -= 0.02f;
                }
                featherProgress += 0.04f;
                var fe = Instantiate(feathers[Random.Range(0, feathers.Capacity)]);
                fe.transform.position = this.GetComponentInParent<Transform>().position + Random.insideUnitSphere * 0.5f + new Vector3(0, 0.2f, 0);
            }
            if (progress > shadowProgress && shadowIndex < kuroShadows.Capacity)
            {
                kuroShadows[shadowIndex].transform.position = this.GetComponentInParent<Transform>().position;
                kuroShadows[shadowIndex].GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 1f);
                //kuroShadows[shadowIndex].GetComponent<Animator>().Play("dash" + cardinal);
                kuroShadows[shadowIndex].GetComponent<Animator>().SetInteger("CardinalDirection", cardinal);
                kuroShadows[shadowIndex].GetComponent<FadeSpriteScript>().FadeOut();
                shadowIndex++;
                shadowProgress += 0.075f;
            }
        }
        yield return null;
    }

    IEnumerator DoWhirlwind()
    {
        float progress = 0;
        float featherProgress = 0;
        specialEffects.transform.position = this.transform.position;
        specialEffects.transform.rotation = Quaternion.identity;
        while (stateManager.CurrentState == State.UnstoppableAttack)
        {
            yield return new WaitForFixedUpdate();
            progress += Time.fixedDeltaTime;
            if (progress > featherProgress)
            {
                featherProgress += 0.04f;
                Vector3 randomLocation = this.GetComponentInParent<Transform>().position + Random.insideUnitSphere * 2.3f + new Vector3(0, 0.2f, 0);
                Instantiate(feathers[Random.Range(0, feathers.Capacity)], randomLocation, Quaternion.identity);
            }
        }
        yield return null;
    }

    IEnumerator DoPush()
    {
        float duration = (stateManager.PushDuration - 0.5f) * 0.5f;
        yield return new WaitForSeconds(0.17f);
        // this segment checks if the player is still moving from the push effect
        Vector3 position;
        Vector3 position2 = this.transform.position;
        // we need two of these because the body's position needs that many in order to update for the first time.
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        position = position2;
        position2 = this.transform.position;
        float progress = 0;
        while (position != position2 && stateManager.CurrentState == State.Pushed && progress < duration)
        {
            yield return new WaitForFixedUpdate();
            progress += Time.fixedDeltaTime;
            position = position2;
            position2 = this.transform.position;
        }
        animator.SetTrigger("Knockdown");
    }

    IEnumerator DoHeal()
    {
        float progress = 0;
        int ring = 0;
        while (progress < healDuration)
        {
            yield return null;
            progress += Time.deltaTime;
            if (progress > healDuration / 5 * ring && ring < 3)
            {
                StartCoroutine("DoRingUp", ring);
                ring++;
            }
        }
        yield return null;
        animator.SetBool("Heal", false);
    }

    IEnumerator DoRingUp(int x)
    {
        float progress = 0;
        ringEffects[x].gameObject.SetActive(true);
        ringEffects[x].transform.localPosition = new Vector3(0, -0.4f, 0);
        while (progress < 1)
        {
            yield return null;
            progress += Time.deltaTime / (healDuration * 0.6f);
            ringEffects[x].transform.localPosition = Vector3.Lerp(new Vector3(0, -0.4f, 0), new Vector3(0, 0.4f, 0), progress);
        }
        ringEffects[x].gameObject.SetActive(false);
    }

    private void PlayHitEffects()
    {
        if (hitEffect != null)
        {
            hitEffect.transform.localRotation = Quaternion.Euler(0, 0, Cardinal.VectorToAngle(stateManager.HitDirection));
            foreach (ParticleSystem ps in hitEffects)
            {
                ps.Play();
            }
        }
    }
}
