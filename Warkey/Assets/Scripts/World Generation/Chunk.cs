using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public class Chunk
{
    public GameObject chunkObject;
    public Vector2 coordinate;
    public Vector2 worldPosition;
    public Vector2 chunkMatrix;
    private AdjacentChunks adjacentChunks = new AdjacentChunks();


    HeightMapSettings heightMapSettings;
    MeshSettings meshSettings;
    GroundSettings groundSettings;
    PathSettings pathSettings;
    SpawnableSettings spawnableSettings;

    int currentLOD;
    int previousLOD = -1;
    public LODSettings LODSettings;  

    public Bounds bounds;
    public HeightMap heightMap;
    
    public TerrainChunk terrainChunk;
    public WaterChunk waterChunk;
    public PathChunk pathChunk;
    
    Transform viewer;
    //public NavMeshSurface navMeshSurface;

    //World Generator
    public event System.Action<Chunk, bool> onVisibleChanged;
    public event System.Action<Chunk> onChunkLoaded;

    //Sub Chunk
    public event System.Action<int, bool> onLoadSubChunks;
    public event System.Action<int> onViewerUpdate;
    public event System.Action<float> onViewerColliderUpdate;
    event System.Action<bool> onSetVisibleChunk;
    event System.Action<HeightMap> onHeightMapSet;

    bool areSubsLoaded = false;
    bool loadAll = false;
    bool isHeightMapReceived = false;
    bool hasSetEnviromentObjects = false;
    bool hasRequestedEnviroment = false;
    bool hasRequestedSpawnables = false;
    bool hasSetSpawnables = false;
    List<EnviromentObjectData> enviromentObjectDatas = new List<EnviromentObjectData>();
    List<SpawnableObjectData> spawnableObjectDatas = new List<SpawnableObjectData>();

    float maxViewDistance;

    public HeightMapSettings HeightMapSettings { get => heightMapSettings; }
    public MeshSettings MeshSettings { get => meshSettings; }
    public GroundSettings GroundSettings { get => groundSettings; }
    public PathSettings PathSettings { get => pathSettings; }
    protected Vector2 ViewerPosition { get { return new Vector2(viewer.position.x, viewer.position.z); } }
    public AdjacentChunks AdjacentChunks { get => adjacentChunks; }
    public SpawnableSettings SpawnableSettings { get => spawnableSettings; }

    public Chunk(Vector2 coord,Vector2 chunkMatrix, HeightMapSettings heightMapSettings, MeshSettings meshSettings, GroundSettings groundSettings,SpawnableSettings spawnableSettings ,PathSettings pathSettings ,LODSettings lODSettings, Transform parent, Transform viewer, Material material, Material waterMaterial, Material pathMaterial,float[,] pathmap, PhysicMaterial physicMaterial) {
        this.coordinate = coord;
        this.chunkMatrix = chunkMatrix;
        this.LODSettings = lODSettings;
        this.heightMapSettings = heightMapSettings;
        this.meshSettings = meshSettings;
        this.groundSettings = groundSettings;
        this.spawnableSettings = spawnableSettings;
        this.pathSettings = pathSettings;
        this.viewer = viewer;

        this.chunkObject = new GameObject("Chunk");
        chunkObject.transform.parent = parent;
        

        worldPosition = coord * meshSettings.MeshWorldSize / meshSettings.scale;
        Vector2 position = coord * meshSettings.MeshWorldSize;
        chunkObject.transform.position = new Vector3(position.x, 0, position.y);

        bounds = new Bounds(position, Vector2.one * meshSettings.MeshWorldSize);

        terrainChunk = new TerrainChunk(this,material,physicMaterial);
        RegisterActions(terrainChunk);

        waterChunk = new WaterChunk(this, waterMaterial);
        RegisterActions(waterChunk);

        pathChunk = new PathChunk(this, pathMaterial, pathmap);
        RegisterActions(pathChunk);

        CreateCornerBlock();
        maxViewDistance = LODSettings.LODInfos[LODSettings.LODCount - 1].visibleDistanceThreshold;
    }

    internal void onWorldReady() {
        foreach (SpawnableObjectData objectData in spawnableObjectDatas) {
            if (objectData.Settings.needsNavMesh)
                objectData.CreateObjects(true);
        }

        UpdateChunk();
    }

    private void RegisterActions(SubChunk subChunk) {
        onLoadSubChunks += subChunk.LoadSubChunk;
        onSetVisibleChunk += subChunk.SetVisible;
        onHeightMapSet += subChunk.SetHeightMap;
        onViewerColliderUpdate += subChunk.UpdateCollisionMesh;
        onViewerUpdate += subChunk.UpdateSubChunk;
    }

    public void Load(float[,] fallOff,bool loadAll) {
        if (isHeightMapReceived) return;
        ThreadDataRequest.RequestData(() => HeightMapGenerator.GenerateHeightMap(meshSettings.VerticesPerLineCount, meshSettings.VerticesPerLineCount, heightMapSettings, worldPosition, coordinate, fallOff), OnHeightMapReceived);
        this.loadAll = loadAll;
    }

    protected void OnHeightMapReceived(object heightMap) {
        isHeightMapReceived = true;
        this.heightMap = (HeightMap)heightMap;
        onHeightMapSet(this.heightMap);
        if (loadAll) {
            onLoadSubChunks(0, true);
            /*RequestEnviromentObjectDatas();
            RequestSpawnableObjectDatas();*/
        }
        UpdateChunk();
    }

    private void RequestEnviromentObjectDatas() {
        if (hasRequestedEnviroment) return;
        groundSettings.poissonDiscSettings.sampleRegionSize = new Vector2(this.heightMap.values.GetLength(0), this.heightMap.values.GetLength(1));
        Transform transform = chunkObject.transform;
        ThreadDataRequest.RequestData(() => EnviromentObjectGenerator.GenerateEnviromentDatas(heightMap, groundSettings, transform, coordinate), OnEnviromentObjectDataListReceived);
        hasRequestedEnviroment = true;
    }

    private void OnEnviromentObjectDataListReceived(object enviromentObjects) {
        this.hasSetEnviromentObjects = true;
        this.enviromentObjectDatas = (List<EnviromentObjectData>)enviromentObjects;
        foreach (EnviromentObjectData objectData in enviromentObjectDatas)
            objectData.CreateObjects(true);

        
        UpdateChunk();
    }

    private void RequestSpawnableObjectDatas() {
        if (hasRequestedSpawnables) return;
        spawnableSettings.poissonDiscSettings.sampleRegionSize = new Vector2(this.heightMap.values.GetLength(0), this.heightMap.values.GetLength(1));
        Transform transform = chunkObject.transform;
        ThreadDataRequest.RequestData(() => SpawnableObjectGenerator.GenerateSpawnableDatas(heightMap, spawnableSettings, transform, coordinate), OnSpawnableObjectDataListReceived);
        hasRequestedSpawnables = true;
    }

    private void OnSpawnableObjectDataListReceived(object enviromentObjects) {
        this.hasRequestedSpawnables = true;
        this.spawnableObjectDatas = (List<SpawnableObjectData>)enviromentObjects;
        foreach (SpawnableObjectData objectData in spawnableObjectDatas) {
            if(!objectData.Settings.needsNavMesh)
                objectData.CreateObjects(true);
        }

        UpdateChunk();
    }

    public void UpdateChunk() {
        if (!isHeightMapReceived || viewer == null) return;
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
                onViewerUpdate(lodIndex);                
            }

            if (lodIndex <= LODSettings.LODInfos[LODSettings.enviromentLOD].lod) {
                if (!hasSetEnviromentObjects) {
                    RequestEnviromentObjectDatas();
                }
                else {
                    foreach (EnviromentObjectData objectData in enviromentObjectDatas) {
                        if(objectData.isObjectsLoaded)
                            objectData.Visible(true);
                    }
                }

                if (!hasRequestedSpawnables) {
                    RequestSpawnableObjectDatas();
                }
                else {
                    foreach (SpawnableObjectData objectData in spawnableObjectDatas) {
                        if (objectData.isObjectsLoaded) {
                            if (objectData.Settings.handleVisibility) {
                                objectData.ChangeVisiblity(lodIndex);
                            }
                            else {
                                objectData.Visible(true);
                            }
                        }
                            
                    }
                }
            }
            else {
                foreach (EnviromentObjectData objectData in enviromentObjectDatas) {
                    if (objectData.isObjectsLoaded)
                        objectData.Visible(false);
                }
                foreach (SpawnableObjectData objectData in spawnableObjectDatas) {
                    if (objectData.isObjectsLoaded) {
                        if (objectData.Settings.handleVisibility) {
                            objectData.ChangeVisiblity(lodIndex);
                        }
                        else {
                            objectData.Visible(false);
                        }
                    }
                }
            }
        }

        if (wasVisible != visible) {
            SetVisible(visible);
            onSetVisibleChunk(visible);
            if (onVisibleChanged != null) {
                onVisibleChanged(this, visible);
            }
        }
        
    }

    public void UpdateAdjacentChunks(Chunk chunk) {
        Vector2 vector = chunk.coordinate - coordinate;

        if(Mathf.Abs(vector.x) == 1) {
            if(coordinate.x < chunk.coordinate.x) 
                adjacentChunks.chunks[(int)AdjacentChunks.Position.right] = chunk;
            else
                adjacentChunks.chunks[(int)AdjacentChunks.Position.left] = chunk;
        }

        if(Mathf.Abs(vector.y) == 1) {
            if (coordinate.y < chunk.coordinate.y)
                adjacentChunks.chunks[(int)AdjacentChunks.Position.up] = chunk;
            else
                adjacentChunks.chunks[(int)AdjacentChunks.Position.down] = chunk;
        }

        pathChunk.CheckPath(chunk);
    }


    public void UpdateChunkCollisions() {
        float sqrDistanceFromViewerToEdge = bounds.SqrDistance(ViewerPosition);
        onViewerColliderUpdate(sqrDistanceFromViewerToEdge);
    }


    public void SetVisible(bool visible) {
        chunkObject.SetActive(visible);
    }

    public bool IsVisible() {
        return chunkObject.activeSelf;
    }


    public bool IsStartingCoord() {
        int maxX = (int)chunkMatrix.x / 2 + 1;
        int maxY = (int)chunkMatrix.y / 2 + 1;
        int xModifier = (chunkMatrix.x % 2 == 0) ? 1 : 0;
        int yModifier = (chunkMatrix.y % 2 == 0) ? 1 : 0;

        return coordinate.x == -maxX + xModifier && coordinate.y == maxY + yModifier;
    }

    GameObject colliders;

    public void CreateCornerBlock() {
        colliders = new GameObject("Corners");

        int maxX = (int)chunkMatrix.x / 2;
        int maxY = (int)chunkMatrix.y / 2;
        int xModifier = (chunkMatrix.x % 2 == 0) ? 1 : 0;
        int yModifier = (chunkMatrix.y % 2 == 0) ? 1 : 0;

        int right = maxX;
        int left = -maxX + xModifier;
        int up = maxY;
        int down = -maxY + yModifier;
        if (coordinate.x == right) {
            BoxCollider collider = colliders.AddComponent<BoxCollider>();
            var x = meshSettings.MeshWorldSize / 2;
            collider.center = chunkObject.gameObject.transform.position + Vector3.right * x;
            collider.size = new Vector3(5f, 300f, meshSettings.MeshWorldSize);
        }
        if (coordinate.x == left) {
            BoxCollider collider = colliders.AddComponent<BoxCollider>();
            var x = meshSettings.MeshWorldSize / 2;
            collider.center = chunkObject.gameObject.transform.position + Vector3.left * x;
            collider.size = new Vector3(5f, 300f, meshSettings.MeshWorldSize);
        }
        if (coordinate.y == up) {
            BoxCollider collider = colliders.AddComponent<BoxCollider>();
            var z = meshSettings.MeshWorldSize / 2;
            collider.center = chunkObject.gameObject.transform.position + Vector3.forward * z;
            collider.size = new Vector3(meshSettings.MeshWorldSize, 300f, 5f);
        }
        if (coordinate.y == down) {
            BoxCollider collider = colliders.AddComponent<BoxCollider>();
            var z = meshSettings.MeshWorldSize / 2;
            collider.center = chunkObject.gameObject.transform.position + Vector3.back * z;
            collider.size = new Vector3(meshSettings.MeshWorldSize, 300f, 5f);
        }
        colliders.transform.parent = this.chunkObject.transform;
    }

    int count = 0;
    public void SubchunkLoadCompletion(SubChunk subChunk) {
        count++;
        if(count == 3 && !areSubsLoaded) {
            areSubsLoaded = true;
            onChunkLoaded(this);
        }
    }


    public void BindViewer(Transform transform) {
        viewer = transform;
    }

}


public class AdjacentChunks
{
    public Chunk[] chunks = new Chunk[4];
    public enum Position
    {
        up = 0,
        right = 1,
        down = 2,
        left = 3
    }

    public AdjacentChunks() { }
}