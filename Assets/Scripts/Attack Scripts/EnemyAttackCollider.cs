using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackCollider : MonoBehaviour {

    [SerializeField]
    private int damage;

    [SerializeField]
    private bool knockdown;

    [SerializeField]
    private float pushStrength = 4;

    public Vector2 Direction { get; set; }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == Layer.Player)
        {
            if (knockdown)
            {
                GameManager.player.GetComponent<PlayerControlsScript>().Hit(damage, Direction, pushStrength);
            }
            else
            {
                GameManager.player.GetComponent<PlayerControlsScript>().Hit(damage, Direction);
            }
        }
    }
}
