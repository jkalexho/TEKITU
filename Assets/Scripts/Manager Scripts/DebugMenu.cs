using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugMenu : MonoBehaviour {

	public Text inputXText;
    public Text inputYText;
    public Text inputDashText;
    public Text inputAttackText;
	public Text invincibilityText;
	public Text currentStateText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		inputXText.text = Input.GetAxis("Horizontal").ToString();
        inputYText.text = Input.GetAxis("Vertical").ToString();
        inputDashText.text = Input.GetButton("Dash").ToString();
        inputAttackText.text = Input.GetButton("Attack").ToString();
		invincibilityText.text = GameManager.player.GetComponent<PlayerControlsScript>().isInvincible.ToString();
		currentStateText.text = State.toString[GameManager.player.GetComponent<StateManager>().CurrentState];
	}
}
