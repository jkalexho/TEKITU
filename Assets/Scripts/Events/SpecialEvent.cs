using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecialEvent : MonoBehaviour {

    public virtual bool Done { get; set; }
    public virtual bool Active { get; set; }
	// Use this for initialization
	protected virtual void Start () {
        Done = false;
        Active = false;
	}

    public abstract void Activate();
}
