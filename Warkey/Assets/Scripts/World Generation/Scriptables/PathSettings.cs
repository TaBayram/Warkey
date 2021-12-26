using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(PathSettings), menuName = "World Generation/" + nameof(PathSettings))]
public class PathSettings : UpdateableData
{
    public bool useMesh;
    public int seed;
    [Range(0, 100000)]
    public int maxIteration;
    [Range(0, 100000)]
    public int maxSubIteration;
    public MinMax distance;
    [Range(0.0f,0.1f)]
    public float maxSteepChange;
    public MinMax height;
    [Range(0, 1)]
    public float negativeStrength;

    public BranchData[] branchSettings;

    public float startRadius;
    public float pathWidth;
    public float pathHeightOffset;
    public float smoothWeight;
}

[System.Serializable]
public struct BranchData
{
    [Header("Branch")]
    [Range(0, 1)]
    public float branchChance;
    [Range(0, 100000)]
    public int branchIteration;
    public float branchWidth;
    [Range(0, 1)]
    public float branchNegativeStrength;

}