using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

//This script exists on the attack collider of the player only
//as it should detect OnTriggerEnter2D from the sword collider only and not the player's collider or any other collider.
public class AttackOnCollideScript : MonoBehaviour {

	public PlayerControlsScript controlsScript;
	private int attackDamage;
	private Vector3 playerPos;
    [SerializeField]
	private float pushStrength = 1.75f;

    private float defaultPushStrength;
    private int defaultAttackDamage;

    private bool dash = false;
    private bool whirlwind = false;

    private float defaultShakeDuration;
    private float defaultShakeStrength;
    [SerializeField]
    private float shakeDuration = 0.05f;
    [SerializeField]
    private float shakeStrength = 0.2f;

    void Start()
    {
        attackDamage = controlsScript.attackDamage;
        defaultPushStrength = pushStrength;
        defaultShakeDuration = shakeDuration;
        defaultShakeStrength = shakeStrength;
    }

	void Update()
	{
		defaultAttackDamage = controlsScript.attackDamage;
		playerPos = controlsScript.transform.position;
		// pushStrength = controlsScript.pushStrength; could be implemented to support attack push modifiers.
	}
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            Vector3 direction;
            if (dash)
            {
                direction = this.transform.position - playerPos;
            }
            else if (whirlwind)
            {
                direction = other.transform.position - playerPos;
            }
            else
            {
                direction = other.transform.position - playerPos;
                controlsScript.HitSuccess(); // if it hits an enemy, it tells the player controls script.
                attackDamage = defaultAttackDamage;
            }
            //Initialises a camera shake coroutine when hitting an enemy.
            if (TekituCameraShaker.Instance != null)
            {
                TekituShakeParameters shakeSettings = new TekituShakeParameters(shakeDuration, direction, 0.5f, shakeStrength);
                StartCoroutine(TekituCameraShaker.Instance.Shake(shakeSettings));
            }
            other.gameObject.GetComponent<GenericEnemyScript>().Hit(attackDamage, direction, pushStrength);
        }
    }

    public void StartDashAttack(int damage, float pushStrength, float cameraShakeDuration, float cameraShakeStrength)
    {
        attackDamage = damage;
        this.pushStrength = pushStrength;
        shakeDuration = cameraShakeDuration;
        shakeStrength = cameraShakeStrength;
        dash = true;
    }

    public void StartWhirlwindAttack(int damage, float pushStrength, float cameraShakeDuration, float cameraShakeStrength)
    {
        attackDamage = damage;
        this.pushStrength = pushStrength;
        shakeDuration = cameraShakeDuration;
        shakeStrength = cameraShakeStrength;
        whirlwind = true;
    }

    public void ResetValues()
    {
        dash = false;
        whirlwind = false;
        attackDamage = defaultAttackDamage;
        pushStrength = defaultPushStrength;
        shakeDuration = defaultShakeDuration;
        shakeStrength = defaultShakeStrength;
    }
}
