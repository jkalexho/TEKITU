using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalShotScript : SimpleShotScript
{
    public Vector3 Direction { get; set; }

    protected override void Start()
    {
        direction = transform.rotation.eulerAngles + Direction;
        if (rotate)
        {
            transform.right = direction;
        }
        StartCoroutine("Spawn");
    }
    
}
