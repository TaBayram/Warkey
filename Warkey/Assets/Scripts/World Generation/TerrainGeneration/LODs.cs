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

    public enum MeshType
    {
        terrain = 0,
        water = 1,
        path = 2
    }

    public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings, MeshType meshType = MeshType.terrain) {
        hasRequestedMesh = true;
        switch (meshType) {
            case MeshType.terrain:
                ThreadDataRequest.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, lod), OnMeshDataRecieved);
                break;
            case MeshType.water:
                ThreadDataRequest.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.values01, meshSettings, lod, meshSettings.minValue, meshSettings.maxValue, true, meshSettings.height), OnMeshDataRecieved);
                break;
            case MeshType.path:
                ThreadDataRequest.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, lod, -1, float.MaxValue, false), OnMeshDataRecieved);
                break;
        }
    }
}


