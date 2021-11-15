using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MinMax
{
    public float min;
    public float max;
    public MinMax(float min, float max) {
        this.min = min;
        this.max = max;
    }
}

public struct XY
{
    public int x;
    public int y;

    public XY(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public Vector2 ToVector() {
        return new Vector2(x, y);
    }
}
