using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillator : MonoBehaviour {


    public float frequency;

    public float SinValue { get; set; }
    public float CosValue { get; set; }

    private float radians = 0;

    public float ClampedSinValue()
    {
        return (SinValue + 1) * 0.5f;
    }

    public float ClampedCosValue()
    {
        return (CosValue + 1) * 0.5f;
    }

    void FixedUpdate()
    {
        radians += Time.fixedDeltaTime * frequency * 2 * Mathf.PI;
        if (radians > 2 * Mathf.PI)
        {
            radians -= 2 * Mathf.PI;
        }
        SinValue = Mathf.Sin(radians);
        CosValue = Mathf.Cos(radians);
    }
}
