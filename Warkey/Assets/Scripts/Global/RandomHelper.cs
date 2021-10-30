using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;


public static class RandomHelper
{
    static List<RandomWrapper> randomWrappers = new List<RandomWrapper>();
    public static RandomWrapper GetRandom(int seed) {
        foreach(RandomWrapper wrapper in randomWrappers) {
            if(wrapper.Seed == seed) {
                return wrapper;
            }
        }
        RandomWrapper random = new RandomWrapper(new Random(seed), seed);
        randomWrappers.Add(random);
        return random;
    }

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


public class RandomWrapper
{
    readonly int seed;
    Random random;
    public int Seed => seed;
    public RandomWrapper(Random random, int seed) {
        this.random = random;
        this.seed = seed;
    }
    

    public float Range(float minInclusive, float maxExclusive) {
        float value = (float)random.NextDouble();
        value = Mathf.Lerp(minInclusive, maxExclusive, value);

        return value;
    }

    public int Range(int minInclusive, int maxExclusive) {
        int value = (int)random.Next(minInclusive, maxExclusive);
        return value;
    }

    public float Range() {
        float value = (float)random.NextDouble();
        return value;
    }
}
