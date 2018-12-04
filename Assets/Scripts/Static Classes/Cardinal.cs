using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Cardinal {
    public static readonly int E = 0;
    public static readonly int NE = 1;
    public static readonly int N = 2;
    public static readonly int NW = 3;
    public static readonly int W = 4;
    public static readonly int SW = 5;
    public static readonly int S = 6;
    public static readonly int SE = 7;

    public static int VectorToCardinal(Vector2 direction)
    {
        float angle = Vector2.SignedAngle(new Vector2(1, 0), direction);
        return AngleToCardinal(angle);
    }

    public static int VectorToCardinal41(Vector2 direction)
    {
        float angle = Vector2.SignedAngle(new Vector2(1, 0), direction) - 45;
        float a = (angle < 0) ? angle + 360 : angle;
        int c = Mathf.RoundToInt(a / 90) * 2 + 1;
        return (c == 9) ? 1 : c;
    }

    public static int VectorToCardinal4(Vector2 direction)
    {
        float angle = Vector2.SignedAngle(new Vector2(1, 0), direction);
        float a = (angle < 0) ? angle + 360 : angle;
        int c = (Mathf.RoundToInt(a / 90) * 2);
        return (c == 8) ? 0 : c;
    }

    public static float VectorToAngle(Vector2 direction)
    {
        return Vector2.SignedAngle(new Vector2(1, 0), direction);
    }

    public static int VectorToCardinal2(Vector2 direction)
    {
        float angle = Vector2.SignedAngle(new Vector2(1, 0), direction);
        return (angle < 90 && angle >= -90) ? 0 : 1;
    }

    // angle can be anywhere within [-180, 180]
    public static int AngleToCardinal(float angle)
    {
        float a = (angle < 0) ? angle + 360 : angle;
        int c = Mathf.RoundToInt(a / 45);
        return (c == 8) ? 0 : c;
    }

    public static float CardinalToAngle(int c)
    {
        return c * 45;
    }

    // returns a unit vector in the cardinal direction
    public static Vector2 CardinalToVector(int c)
    {
        float a = CardinalToAngle(c) * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(a), Mathf.Sin(a));
    }
}
