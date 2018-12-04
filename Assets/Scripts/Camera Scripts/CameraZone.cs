using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZone : MonoBehaviour {

    public Transform centerPoint;

    #region Editor variables
    [SerializeField]
    private bool horizontalLock = false;
    [SerializeField]
    private bool verticalLock = false;
    [SerializeField]
    [Tooltip("How much the camera sticks to the player. (1 = attached to player. 0 = attachd to centerpoint")]
    private float stickiness = 1;
    #endregion

    public Vector3 Pos { get; set; }

    public bool zoneActive = false;

    // Update is called once per frame
    void Update () {
        if (zoneActive)
        {
            Pos = Vector2.Lerp(centerPoint.transform.position, GameManager.player.transform.position, stickiness);
            if (verticalLock)
            {
                Pos = new Vector3(Pos.x, centerPoint.transform.position.y, Pos.z);
            }
            if (horizontalLock)
            {
                Pos = new Vector3(centerPoint.transform.position.x, Pos.y, Pos.z);
            }
        }
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("bodyCollider") && other.gameObject.layer == Layer.Player)
        {
            GameManager.SetCameraZone(this);
            zoneActive = true;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("bodyCollider") && other.gameObject.layer == Layer.Player && GameManager.cameraZone == null)
        {
            GameManager.SetCameraZone(this);
            zoneActive = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("bodyCollider") && other.gameObject.layer == Layer.Player)
        {
            if (GameManager.cameraZone == this)
            {
                GameManager.ClearCameraZone(this);
            }
            zoneActive = false;
        }
    }
}
