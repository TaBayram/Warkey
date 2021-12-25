using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName =nameof(GroundSettings),menuName = "World Generation/" + nameof(GroundSettings))]
public class GroundSettings : UpdateableData
{
    public PoissonDiscSettings poissonDiscSettings;
    public EnviromentObjectSettings[] enviromentObjects;

}


[System.Serializable]
public class EnviromentObjectSettings
{
    public bool enabled;
    public GameObject gameObject;
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
    public bool lessenTowardsEdges;
    [Range(0, 1)]
    public float lessenScale;
    [Range(0, 1)]
    public float jitterScale;
    public bool correctRotation;

}
