using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CinematicShot : MonoBehaviour {

	public virtual bool Done { get; set; }

    protected virtual void Awake()
    {
        Done = false;
    }
}
