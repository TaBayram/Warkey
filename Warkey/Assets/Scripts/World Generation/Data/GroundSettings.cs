using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class GroundSettings : UpdateableData
{
    public PoissonDiscSettings poissonDiscSettings;
    public EnviromentObject[] enviromentObjects;

}


[System.Serializable]
public class EnviromentObject
{
    public GameObject gameObject;
    public float blockRadius;
    [Range(0,1)]
    public float density = 0;
    [Range(0,2)]
    public float minThreshold;
    [Range(0, 2)]
    public float maxThreshold;
    public bool lessenTowardsEdges;
    [Range(0, 1)]
    public float lessenScale;

}
