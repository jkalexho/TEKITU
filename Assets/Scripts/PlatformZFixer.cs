using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformZFixer : MonoBehaviour {

	// Use this for initialization
	void Start () {
		this.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
		Destroy(this.GetComponent<PlatformZFixer>());
	}

}
