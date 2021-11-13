using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public struct LODSettings
{
    public int colliderLOD;
    public int enviromentLOD;
    public LODInfo[] LODInfos;

    public int LODCount { get => LODInfos.Length; }
}



[System.Serializable]
public struct LODInfo
{
    [Range(0, MeshSettings.lodCount - 1)]
    public int lod;
    public float visibleDistanceThreshold;

    public float sqrVisibleThreshold {
        get {
            return visibleDistanceThreshold * visibleDistanceThreshold;
        }
    }
}

public class LODMesh
{
    public Mesh mesh;
    public bool hasRequestedMesh;
    public bool hasMesh;
    int lod;

    public event System.Action updateCallback;

    public LODMesh(int lod) {
        this.lod = lod;
    }

    public void OnMeshDataRecieved(object meshData) {
        hasMesh = true;
        this.mesh = ((MeshData)meshData).CreateMesh();
        updateCallback();
    }

    public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings, bool isWater = false) {
        hasRequestedMesh = true;
        if (isWater)
            ThreadDataRequest.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, lod, meshSettings.minValue, meshSettings.maxValue, meshSettings.height), OnMeshDataRecieved);
        else
            ThreadDataRequest.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, lod), OnMeshDataRecieved);
    }
}


