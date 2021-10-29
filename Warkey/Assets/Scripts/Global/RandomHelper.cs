using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;


public static class RandomHelper
{
    static Random random;

    public static float Range(float minInclusive,float maxExclusive,ref Random random) {
        float value = (float)random.NextDouble();
        value = Mathf.Lerp(minInclusive, maxExclusive, value);

        return value;
    }

    public static int Range(int minInclusive, int maxExclusive, ref Random random) {
        int value = (int)random.Next(minInclusive,maxExclusive);
        return value;
    }

    public static float Range(ref Random random) {
        float value = (float)random.NextDouble();
        return value;
    }


}

