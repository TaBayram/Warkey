using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu()]
public class GroundSettings : UpdateableData
{
    public PoissonDiscSettings poissonDiscSettings;
    public EnviromentObjectSettings[] enviromentObjects;

}


[System.Serializable]
public class EnviromentObjectSettings
{
    public bool enabled;
    [Range(0, MeshSettings.lodCount - 1)]
    public int visibleMaxLod;
    public GameObject gameObject;
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
