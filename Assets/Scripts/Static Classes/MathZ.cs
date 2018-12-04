using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathZ {

    // the proper mathematical modulus operator. Always returns a positive number
	public static int Modulo(int a, int b)
    {
        return ((a %= b) < 0) ? a + b : a;
    }

    public static float Modulo(float a, float b)
    {
        return ((a %= b) < 0) ? a + b : a;
    }
}
