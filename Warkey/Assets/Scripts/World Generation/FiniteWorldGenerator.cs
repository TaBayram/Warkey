using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public class FiniteWorldGenerator : MonoBehaviour
{
    const float viewerMoveThresholdForChunkUpdate = 5f;
    const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;

    private int seed;

    public MeshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    public TextureSettings textureData;
    public GroundSettings groundSettings;
    public PathSettings pathSettings;

    public Material mapMaterial;
    public Material waterMaterial;
    public Material pathMaterial;

    public PhysicMaterial groundMaterial;

    public XY chunkSize;
    public LODSettings LODSettings;

    public event System.Action<Chunk> onChunkCreated;

    int count = 0;
    Dictionary<Vector2, Chunk> chunkDictionary = new Dictionary<Vector2, Chunk>();
    Dictionary<Vector2, float[,]> pathDictionary = new Dictionary<Vector2, float[,]>();
    List<Chunk> visibleChunks = new List<Chunk>();
    List<Chunk> allChunks = new List<Chunk>();

    float meshWorldSize;
    int chunkSizeVisibleInViewDistance;


    public NavMeshSurface navMeshSurface;

    public float[,] fallOffMap;
    public HeightMap heightMap;
    public PathData pathData;


    public Transform viewer;
    Vector2 viewerPosition;
    Vector2 viewerPositionOld;
    private bool isWorldReady = false;

    public event System.Action onWorldReady;

    private void SetMap() {
        WorldSettings settings;
        if (GameTracker.Instance.WorldSettingsHolder != null)
            settings = GameTracker.Instance.WorldSettingsHolder.worldSettings;
        else
            settings = new WorldSettings(0,new XY(1,1), "WorldScene");

        groundSettings.poissonDiscSettings.seed = settings.seed;
        heightMapSettings.noiseSettings.seed = settings.seed;
        pathSettings.seed = settings.seed;

        chunkSize = settings.worldSize;
    }
    
    private void Awake() {
        SetMap();
    }

    private void Start() {
        textureData.ApplyToMaterial(mapMaterial);
        textureData.UpdateMeshHeights(mapMaterial, heightMapSettings.MinHeight, heightMapSettings.MaxHeight);

        float maxViewDistance = LODSettings.LODInfos[LODSettings.LODCount - 1].visibleDistanceThreshold;
        meshWorldSize = meshSettings.MeshWorldSize;
        chunkSizeVisibleInViewDistance = Mathf.RoundToInt(maxViewDistance / meshWorldSize);

        if (heightMapSettings.useFallOff) {
            fallOffMap = FallOffGenerator.GenerateFalloffMap((int)(meshSettings.VerticesPerLineCount * chunkSize.x), (int)(meshSettings.VerticesPerLineCount * chunkSize.y));
        }

        if(viewer == null) BindViewer(transform);
        GeneratePath();
        LoadAll();
        UpdateVisibleChunks();
    }
    private Chunk CreateChunk(Vector2 viewedChunkCoord) {
        Chunk chunk = new Chunk(viewedChunkCoord, chunkSize.ToVector(), heightMapSettings, meshSettings, groundSettings, pathSettings, LODSettings, transform, viewer, mapMaterial, waterMaterial, pathMaterial, pathDictionary[viewedChunkCoord], groundMaterial);
        chunkDictionary.Add(viewedChunkCoord, chunk);
        allChunks.Add(chunk);
        chunk.onVisibleChanged += OnChunkVisibilityChanged;
        chunk.onChunkLoaded += onChunkLoaded;
        chunk.Load(fallOffMap,true);
        
        onChunkCreated?.Invoke(chunk);
        onChunkCreated += chunk.UpdateAdjacentChunks;
        return chunk;
    }
    private void onChunkLoaded(Chunk chunk) {
        count++;
        if (count == (chunkSize.x * chunkSize.y)) {
            OnAllLoaded();
        }
    }

    private void GeneratePath() {
        float sizeX = meshSettings.VerticesPerLineCount * chunkSize.x;
        float sizeY = meshSettings.VerticesPerLineCount * chunkSize.y;
        heightMap = HeightMapGenerator.GenerateHeightMap((int)sizeX, (int)sizeY, heightMapSettings, Vector2.zero, Vector2.zero, fallOffMap);
        pathData = PathGenerator.GeneratePath(pathSettings, heightMap.values01, new Vector2(50, sizeY - 50), new Vector2(sizeX / Mathf.Max(sizeX, sizeY), -sizeY / Mathf.Max(sizeX, sizeY)));

        int maxX = (int)chunkSize.x / 2 + 1;
        int maxY = (int)chunkSize.y / 2 + 1;
        int xModifier = (chunkSize.x % 2 == 0) ? 1 : 0;
        int yModifier = (chunkSize.y % 2 == 0) ? 1 : 0;
        int y = 0;
        /*Debug.Log("maxX " + maxX + " maxY " + maxY);
        Debug.Log("xModifier " + xModifier + " yModifier " + yModifier);
        Debug.Log(pathData.pathMap.GetLength(0) + " " + pathData.pathMap.GetLength(1));*/
        for (int coordY = -maxY + yModifier + 1; coordY < maxY; coordY++) {
            int x = 0;
            for (int coordX = -maxX + xModifier + 1; coordX < maxX; coordX++) {
                // Debug.Log("coordX " + coordX + " coordY " + coordY);
                Vector2 viewedChunkCoord = new Vector2(coordX, coordY);
                //Debug.Log("x " + x + " y " + y);
                int arrayOffsetX = (int)((x)) * (int)(meshSettings.VerticesPerLineCount);
                int arrayOffsetY = (int)((chunkSize.y - 1 - y)) * (int)(meshSettings.VerticesPerLineCount);
                //Debug.Log("arrayOffsetX " + arrayOffsetX + " arrayOffsetY " + arrayOffsetY);

                float[,] heightMap2 = new float[meshSettings.VerticesPerLineCount, meshSettings.VerticesPerLineCount];

                for (int yp = 0; yp < meshSettings.VerticesPerLineCount; yp++) {
                    for (int xp = 0; xp < meshSettings.VerticesPerLineCount; xp++) {
                        heightMap2[xp, yp] = pathData.pathMap[arrayOffsetX + xp, arrayOffsetY + yp];
                    }
                }
                pathDictionary.Add(viewedChunkCoord, heightMap2);

                x++;
            }
            y++;
        }

    }
    private void LoadAll() {
        int maxX = (int)chunkSize.x / 2 + 1;
        int maxY = (int)chunkSize.y / 2 + 1;
        int xModifier = (chunkSize.x % 2 == 0) ? 1 : 0;
        int yModifier = (chunkSize.y % 2 == 0) ? 1 : 0;

        for (int yOffset = -maxY + yModifier+1; yOffset < maxY; yOffset++) {
            for (int xOffset = -maxX + xModifier+1; xOffset < maxX; xOffset++) {
                Vector2 viewedChunkCoord = new Vector2(xOffset, yOffset);
                if (!chunkDictionary.ContainsKey(viewedChunkCoord)) {
                    CreateChunk(viewedChunkCoord).SetVisible(true);
                }
                
            }
        }

        Invoke(nameof(OnAllLoaded), 10);
    }

    private void OnAllLoaded() {
        if (!isWorldReady) {
            isWorldReady = true;
            navMeshSurface.BuildNavMesh();
            onWorldReady();
        }
    }

    private void Update() {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
        
        if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate) {
            viewerPositionOld = viewerPosition;
            UpdateVisibleChunks();
        }
        bool isPosSame = viewerPosition == viewerPositionOld;
        if (!isPosSame) {
            UpdateChunkCollisions();   
        }
    }

    private void UpdateChunkCollisions() {
        foreach (Chunk chunk in visibleChunks) {
            chunk.UpdateChunkCollisions();
        }
    }
     
    private void UpdateVisibleChunks() {
        HashSet<Vector2> alreadyUpdatedChunkCoords = new HashSet<Vector2>();
        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / meshWorldSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / meshWorldSize);

        for (int i = visibleChunks.Count - 1; i >= 0; i--) {
            alreadyUpdatedChunkCoords.Add(visibleChunks[i].coordinate);
            visibleChunks[i].UpdateChunk();
        }

        for (int yOffset = -chunkSizeVisibleInViewDistance; yOffset <= chunkSizeVisibleInViewDistance; yOffset++) {
            for (int xOffset = -chunkSizeVisibleInViewDistance; xOffset <= chunkSizeVisibleInViewDistance; xOffset++) {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
                if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord)) {
                    if (chunkDictionary.ContainsKey(viewedChunkCoord)) {
                        chunkDictionary[viewedChunkCoord].UpdateChunk();
                    }
                    else {
                        if (CanCreateTerrainChunk(viewedChunkCoord)) {
                            CreateChunk(viewedChunkCoord);
                        }
                    }
                }
            }
        }
    }

    private bool CanCreateTerrainChunk(Vector2 nextChunkCoord) {
        if (chunkSize.x == 0 || chunkSize.y == 0) return true;
        int maxX = (int)chunkSize.x / 2 +1;
        int maxY = (int)chunkSize.y / 2 +1;
        int xModifier = (chunkSize.x % 2 == 0) ? 1 : 0;
        int yModifier = (chunkSize.y % 2 == 0) ? 1 : 0;

        return (nextChunkCoord.x < maxX && nextChunkCoord.x > -maxX+ xModifier) && (nextChunkCoord.y < maxY && nextChunkCoord.y > -maxY+ yModifier);
    }

    private void OnChunkVisibilityChanged(Chunk chunk, bool isVisible) {
        if (isVisible) {
            visibleChunks.Add(chunk);
        }
        else {
            visibleChunks.Remove(chunk);
        }
    }

    public void BindViewer(Transform transform) {
        viewer = transform;

        foreach(Chunk chunk in allChunks) {
            chunk.BindViewer(transform);
        }
    }

}
        
