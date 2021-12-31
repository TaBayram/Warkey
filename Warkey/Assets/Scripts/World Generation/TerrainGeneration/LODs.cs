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
    public int lodIndex;

    public int Lod { get => lod; }

    public event System.Action<int> updateCallback;

    public LODMesh(int lod, int lodIndex) {
        this.lod = lod;
        this.lodIndex = lodIndex;
    }

    public void OnMeshDataReceived(object meshData) {
        hasMesh = true;
        this.mesh = ((MeshData)meshData).CreateMesh();
        updateCallback(lodIndex);
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
                ThreadDataRequest.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, lod), OnMeshDataReceived);
                break;
            case MeshType.water:
                ThreadDataRequest.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.values01, meshSettings, lod, meshSettings.minValue, meshSettings.maxValue, true, meshSettings.height), OnMeshDataReceived);
                break;
            case MeshType.path:
                ThreadDataRequest.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, lod, 0, float.MaxValue, false), OnMeshDataReceived);
                break;
        }
    }
}


