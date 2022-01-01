using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName =nameof(SpawnableSettings),menuName = "World Generation/" + nameof(SpawnableSettings))]
public class SpawnableSettings : UpdateableData
{
    public PoissonDiscSettings poissonDiscSettings;
    public ObjectSettings[] objectSettings;

}


[System.Serializable]
public class ObjectSettings
{
    public bool enabled;
    public GameObject[] gameObjects;
    public int spawnLimit;
    public bool isNetworkObject;
    public bool handleVisibility;
    public int visibilityLODIndex;
    public bool needsNavMesh;
    public bool useNoise;
    public NoiseSettings noiseSettings;
    [Range(0, 1)]
    public float noiseMin;
    [Range(0, 1)]
    public float noiseMax;

    public float blockRadius;
    public float elevation = 0;
    [Range(0,2)]
    public float minThreshold;
    [Range(0, 2)]
    public float maxThreshold;
}
