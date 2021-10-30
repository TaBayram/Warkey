using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class TerrainChunk
{
    const float colliderGenerationDistanceThreshold = 5;
    public event System.Action<TerrainChunk, bool> onVisibleChanged;
    public Vector2 coord;

    GameObject meshObject;
    Vector2 sampleCenter;
    Bounds bounds;

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    MeshCollider meshCollider;
    NavMeshSurface navMeshSurface;
    
    

    LODInfo[] detailLevels;
    LODMesh[] lODMeshes;
    int colliderLODIndex;

    HeightMap heightMap;
    bool isMapDataRecieved;
    int previousLODIndex = -1;
    bool hasSetCollider;
    float maxViewDistance;
    bool hasSetObjects;
    List<EnviromentObjectData> enviromentObjectDatas = new List<EnviromentObjectData>();


    HeightMapSettings heightMapSettings;
    MeshSettings meshSettings;
    GroundSettings groundSettings;

    Transform viewer;

    Vector2 ViewerPosition {
        get {
            return new Vector2(viewer.position.x, viewer.position.z);
        }
    }

    public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings,GroundSettings groundSettings , LODInfo[] detailLevels, int colliderLODIndex, Transform parent, Transform viewer, Material material) {
        this.coord = coord;
        this.detailLevels = detailLevels;
        this.colliderLODIndex = colliderLODIndex;
        this.heightMapSettings = heightMapSettings;
        this.meshSettings = meshSettings;
        this.groundSettings = groundSettings;
        this.viewer = viewer;

        sampleCenter = coord * meshSettings.MeshWorldSize / meshSettings.scale;
        Vector2 position = coord * meshSettings.MeshWorldSize;
        bounds = new Bounds(position, Vector2.one * meshSettings.MeshWorldSize);


        GameObject gameObject = new GameObject("Chunk");
        
        meshObject = new GameObject("Terrain");
        meshObject.transform.parent = gameObject.transform;
        

        navMeshSurface = gameObject.AddComponent<NavMeshSurface>();
        meshCollider = meshObject.AddComponent<MeshCollider>();
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshFilter = meshObject.AddComponent<MeshFilter>();
        meshRenderer.material = material;
        meshObject.layer = LayerMask.NameToLayer("Ground");


        meshObject.transform.position = new Vector3(position.x, 0, position.y);
        gameObject.transform.parent = parent;
        SetVisible(false);

        lODMeshes = new LODMesh[detailLevels.Length];
        for (int i = 0; i < detailLevels.Length; i++) {
            lODMeshes[i] = new LODMesh(detailLevels[i].lod);
            lODMeshes[i].updateCallback += UpdateTerrainChunk;
            if (i == colliderLODIndex) {
                lODMeshes[i].updateCallback += UpdateCollisionMesh;
            }
        }
        maxViewDistance = detailLevels[detailLevels.Length - 1].visibleDistanceThreshold;
    }
    public void Load() {
        ThreadDataRequest.RequestData(() => HeightMapGenerator.GenerateHeightMap(meshSettings.VerticesPerLineCount, meshSettings.VerticesPerLineCount, heightMapSettings, sampleCenter), OnHeightMapReceived);
    }

    private void OnHeightMapReceived(object heightMap) {
        isMapDataRecieved = true;
        this.heightMap = (HeightMap)heightMap;
        RequestEnviromentObjectDatas();
        UpdateTerrainChunk();
    }

    private void RequestEnviromentObjectDatas() {
        groundSettings.poissonDiscSettings.sampleRegionSize = new Vector2(this.heightMap.values.GetLength(0), this.heightMap.values.GetLength(1));
        Transform transform = meshObject.transform;
        ThreadDataRequest.RequestData(() => EnviromentObjectGenerator.GenerateEnviromentDatas(heightMap, groundSettings, transform), OnEnviromentObjectDataListRecieved);
    }

    private void OnEnviromentObjectDataListRecieved(object enviromentObjects) {
        this.hasSetObjects = true;
        this.enviromentObjectDatas = (List<EnviromentObjectData>)enviromentObjects;
        foreach(EnviromentObjectData objectData in enviromentObjectDatas) {
            objectData.CreateObjects(false);
        }
    }

   

    public void UpdateTerrainChunk() {
        if (!isMapDataRecieved) return;
        float viewerDistanceFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(ViewerPosition));

        bool wasVisible = IsVisible();
        bool visible = viewerDistanceFromNearestEdge <= maxViewDistance;

        if (visible) {
            int lodIndex = 0;
            for (int i = 0; i < detailLevels.Length - 1; i++) {
                if (viewerDistanceFromNearestEdge > detailLevels[i].visibleDistanceThreshold) {
                    lodIndex = i + 1;
                }
                else
                    break;
            }

            if (lodIndex != previousLODIndex) {
                LODMesh lodMesh = lODMeshes[lodIndex];
                if (lodMesh.hasMesh) {
                    previousLODIndex = lodIndex;
                    meshFilter.mesh = lodMesh.mesh;
                    if(lodIndex <= 1) {
                        if (hasSetObjects) {
                            foreach(EnviromentObjectData objectData in enviromentObjectDatas) {
                                objectData.Visible(true);
                            }

                            //navMeshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
                            //navMeshSurface.layerMask = LayerMask.NameToLayer("Ground");
                            navMeshSurface.collectObjects = CollectObjects.Children;
                            navMeshSurface.BuildNavMesh();
                        }
                        else {

                        }
                    }
                    else {
                        foreach (EnviromentObjectData objectData in enviromentObjectDatas) {
                            objectData.Visible(false);
                        }
                    }
                }
                else if (!lodMesh.hasRequestedMesh) {
                    lodMesh.RequestMesh(heightMap,meshSettings);
                }
            }
        }

        if (wasVisible != visible) {
            SetVisible(visible);
            if (onVisibleChanged != null) {
                onVisibleChanged(this, visible);
            }
        }

    }

    public void UpdateCollisionMesh() {
        if (hasSetCollider) return;
        float sqrDistanceFromViewerToEdge = bounds.SqrDistance(ViewerPosition);
        if (sqrDistanceFromViewerToEdge < detailLevels[colliderLODIndex].sqrVisibleThreshold) {
            if (!lODMeshes[colliderLODIndex].hasRequestedMesh) {
                lODMeshes[colliderLODIndex].RequestMesh(heightMap, meshSettings);
            }
        }

        if (sqrDistanceFromViewerToEdge < colliderGenerationDistanceThreshold * colliderGenerationDistanceThreshold) {
            if (lODMeshes[colliderLODIndex].hasMesh) {
                meshCollider.sharedMesh = lODMeshes[colliderLODIndex].mesh;
                hasSetCollider = true;
            }
        }

    }

    public void SetVisible(bool visible) {
        meshObject.SetActive(visible);
    }

    public bool IsVisible() {
        return meshObject.activeSelf;
    }
}

class LODMesh
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

    public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings) {
        hasRequestedMesh = true;
        ThreadDataRequest.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, lod), OnMeshDataRecieved);
    }
}

