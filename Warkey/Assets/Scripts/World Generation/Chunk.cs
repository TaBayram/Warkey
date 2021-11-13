using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public class Chunk
{
    public GameObject chunkObject;
    public Vector2 coordinate;
    public Vector2 worldPosition;

    HeightMapSettings heightMapSettings;
    MeshSettings meshSettings;
    GroundSettings groundSettings;

    int currentLOD;
    int previousLOD = -1;
    public LODSettings LODSettings;  

    public Bounds bounds;
    public HeightMap heightMap;
    
    public TerrainChunk terrainChunk;
    public WaterChunk waterChunk;
    
    Transform viewer;
    Material terrainMaterial;
    Material waterMaterial;
    //public NavMeshSurface navMeshSurface;

    public event System.Action<Chunk, bool> onVisibleChanged;
    public event System.Action<int> onUpdateChunk;
    event System.Action<bool> onSetVisibleChunk;
    event System.Action<HeightMap> onHeightMapSet;

    bool isHeightMapReceived = false;
    bool hasSetEnviromentObjects = false;
    List<EnviromentObjectData> enviromentObjectDatas = new List<EnviromentObjectData>();

    float maxViewDistance;

    public HeightMapSettings HeightMapSettings { get => heightMapSettings; }
    public MeshSettings MeshSettings { get => meshSettings; }
    public GroundSettings GroundSettings { get => groundSettings; }
    protected Vector2 ViewerPosition { get { return new Vector2(viewer.position.x, viewer.position.z); } }


    public Chunk(Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings, GroundSettings groundSettings, LODSettings lODSettings, Transform parent, Transform viewer, Material material, Material waterMaterial) {
        this.coordinate = coord;
        this.LODSettings = lODSettings;
        this.heightMapSettings = heightMapSettings;
        this.meshSettings = meshSettings;
        this.groundSettings = groundSettings;
        this.viewer = viewer;

        this.waterMaterial = waterMaterial;
        this.terrainMaterial = material;

        this.chunkObject = new GameObject("Chunk");
        chunkObject.transform.parent = parent;
        SetVisible(false);

        worldPosition = coord * meshSettings.MeshWorldSize / meshSettings.scale;
        Vector2 position = coord * meshSettings.MeshWorldSize;
        chunkObject.transform.position = new Vector3(position.x, 0, position.y);

        bounds = new Bounds(position, Vector2.one * meshSettings.MeshWorldSize);

        terrainChunk = new TerrainChunk(this,viewer,material);
        RegisterActions(terrainChunk);

        waterChunk = new WaterChunk(this, viewer, waterMaterial);
        RegisterActions(waterChunk);


        maxViewDistance = LODSettings.LODInfos[LODSettings.LODCount - 1].visibleDistanceThreshold;
    }

    private void RegisterActions(SubChunk subChunk) {
        onUpdateChunk += subChunk.ForceLODMesh;
        onSetVisibleChunk += subChunk.SetVisible;
        onHeightMapSet += subChunk.SetHeightMap;

    }

    public void Load(float[,] fallOff) {
        ThreadDataRequest.RequestData(() => HeightMapGenerator.GenerateHeightMap(meshSettings.VerticesPerLineCount, meshSettings.VerticesPerLineCount, heightMapSettings, worldPosition, coordinate, fallOff), OnHeightMapReceived);
    }

    protected void OnHeightMapReceived(object heightMap) {
        isHeightMapReceived = true;
        this.heightMap = (HeightMap)heightMap;
        onHeightMapSet(this.heightMap);
        UpdateChunk();
    }

    private void RequestEnviromentObjectDatas() {
        groundSettings.poissonDiscSettings.sampleRegionSize = new Vector2(this.heightMap.values.GetLength(0), this.heightMap.values.GetLength(1));
        Transform transform = chunkObject.transform;
        ThreadDataRequest.RequestData(() => EnviromentObjectGenerator.GenerateEnviromentDatas(heightMap, groundSettings, transform), OnEnviromentObjectDataListReceived);
    }

    private void OnEnviromentObjectDataListReceived(object enviromentObjects) {
        this.hasSetEnviromentObjects = true;
        this.enviromentObjectDatas = (List<EnviromentObjectData>)enviromentObjects;
        foreach (EnviromentObjectData objectData in enviromentObjectDatas)
            objectData.CreateObjects();
        UpdateChunk();
    }

    public void UpdateChunk() {
        if (!isHeightMapReceived) return;
        float viewerDistanceFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(ViewerPosition));
        bool wasVisible = IsVisible();
        bool visible = viewerDistanceFromNearestEdge <= maxViewDistance;

        if (visible) {
            int lodIndex = 0;
            for (int i = 0; i < LODSettings.LODCount - 1; i++) {
                if (viewerDistanceFromNearestEdge > LODSettings.LODInfos[i].visibleDistanceThreshold)
                    lodIndex = i + 1;
                else break;
            }
            if (lodIndex != previousLOD) {
                previousLOD = lodIndex;
                onUpdateChunk(lodIndex);                
            }
            if (lodIndex <= LODSettings.enviromentLOD) {
                if (!hasSetEnviromentObjects) {
                    RequestEnviromentObjectDatas();
                }
                else {
                    foreach (EnviromentObjectData objectData in enviromentObjectDatas) {
                        if(objectData.isObjectsLoaded)
                            objectData.Visible(true);
                    }
                }
            }
            else {
                foreach (EnviromentObjectData objectData in enviromentObjectDatas) {
                    if (objectData.isObjectsLoaded)
                        objectData.Visible(false);
                }
            }
        }

        if (wasVisible != visible) {
            SetVisible(visible);
            onSetVisibleChunk(true);
            if (onVisibleChanged != null) {
                onVisibleChanged(this, visible);
            }
        }
        
    }



    public void UpdateChunkCollisions() {

    }


    public void SetVisible(bool visible) {
        chunkObject.SetActive(visible);
    }

    public bool IsVisible() {
        return chunkObject.activeSelf;
    }
}
