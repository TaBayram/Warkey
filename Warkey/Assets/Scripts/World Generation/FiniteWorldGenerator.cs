using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using System.Linq;

public class FiniteWorldGenerator : MonoBehaviour
{
    const float viewerMoveThresholdForChunkUpdate = 5f;
    const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;

    public MeshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    public TextureSettings textureData;
    public GroundSettings groundSettings;
    public PathSettings pathSettings;

    public Material mapMaterial;
    public Material waterMaterial;
    public Material pathMaterial;

    public XY chunkSize;
    public LODSettings LODSettings;

    public event System.Action<Chunk> onChunkCreated;

    int count = 0;
    Dictionary<Vector2, Chunk> chunkDictionary = new Dictionary<Vector2, Chunk>();
    Dictionary<Vector2, float[,]> pathDictionary = new Dictionary<Vector2, float[,]>();
    List<Chunk> visibleChunks = new List<Chunk>();

    float meshWorldSize;
    int chunkSizeVisibleInViewDistance;


    public NavMeshSurface navMeshSurface;

    public float[,] fallOffMap;
    public HeightMap heightMap;
    public PathData pathData;


    public Transform viewer;
    Vector2 viewerPosition;
    Vector2 viewerPositionOld;

    private void Start() {
        textureData.ApplyToMaterial(mapMaterial);
        textureData.UpdateMeshHeights(mapMaterial, heightMapSettings.MinHeight, heightMapSettings.MaxHeight);

        float maxViewDistance = LODSettings.LODInfos[LODSettings.LODCount - 1].visibleDistanceThreshold;
        meshWorldSize = meshSettings.MeshWorldSize;
        chunkSizeVisibleInViewDistance = Mathf.RoundToInt(maxViewDistance / meshWorldSize);

        if(heightMapSettings.useFallOff) {
            fallOffMap = FallOffGenerator.GenerateFalloffMap((int)(meshSettings.VerticesPerLineCount*chunkSize.x), (int)(meshSettings.VerticesPerLineCount * chunkSize.y));
        }
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
                int arrayOffsetY = (int)((chunkSize.y-1 - y)) * (int)(meshSettings.VerticesPerLineCount);
                //Debug.Log("arrayOffsetX " + arrayOffsetX + " arrayOffsetY " + arrayOffsetY);
                
                float[,] heightMap2 = new float[meshSettings.VerticesPerLineCount, meshSettings.VerticesPerLineCount];

                for (int yp = 0; yp < meshSettings.VerticesPerLineCount; yp++) {
                    for (int xp = 0; xp < meshSettings.VerticesPerLineCount; xp++) {
                        heightMap2[xp,yp] = pathData.pathMap[arrayOffsetX + xp, arrayOffsetY + yp];
                    }
                }
                pathDictionary.Add(viewedChunkCoord, heightMap2);

                x++;
            }
            y++;
        }


        LoadAll();
        UpdateVisibleChunks();
    }
    private Chunk CreateChunk(Vector2 viewedChunkCoord) {
        Chunk chunk = new Chunk(viewedChunkCoord, chunkSize.ToVector(), heightMapSettings, meshSettings, groundSettings, pathSettings, LODSettings, transform, viewer, mapMaterial, waterMaterial, pathMaterial, pathDictionary[viewedChunkCoord]);
        chunkDictionary.Add(viewedChunkCoord, chunk);
        chunk.onVisibleChanged += OnChunkVisibilityChanged;
        chunk.onChunkLoaded += onChunkLoaded;
        chunk.Load(fallOffMap,true);
        
        if(onChunkCreated != null)
            onChunkCreated(chunk);

        onChunkCreated += chunk.UpdateAdjacentChunks;
        return chunk;
    }
    private void onChunkLoaded(Chunk chunk) {
        count++;
        if (count == (chunkSize.x * chunkSize.y)) {
            GameObject gameObject = new GameObject("Pos");
            gameObject.transform.position = new Vector3(pathData.start.x - heightMap.sizeX / 2  + ((chunkSize.x % 2 == 0)?meshSettings.VerticesPerLineCount/2:0) , heightMap.values[(int)pathData.start.x , (int)pathData.start.y] + 10, -pathData.start.y + heightMap.sizeY / 2 + ((chunkSize.y % 2 == 0) ? meshSettings.VerticesPerLineCount / 2 : 0));
            OnAllLoaded();
        }
            
    }
    private void LoadAll() {
        int maxX = (int)chunkSize.x / 2 + 1;
        int maxY = (int)chunkSize.y / 2 + 1;
        int xModifier = (chunkSize.x % 2 == 0) ? 1 : 0;
        int yModifier = (chunkSize.y % 2 == 0) ? 1 : 0;

        for (int yOffset = -maxY + yModifier+1; yOffset < maxX; yOffset++) {
            for (int xOffset = -maxX + xModifier+1; xOffset < maxY; xOffset++) {
                Vector2 viewedChunkCoord = new Vector2(xOffset, yOffset);
                if (!chunkDictionary.ContainsKey(viewedChunkCoord)) {
                    if (CanCreateTerrainChunk(viewedChunkCoord)) {
                        CreateChunk(viewedChunkCoord).SetVisible(false);
                    }
                }
                
            }
        }
    }

    private void OnAllLoaded() {
        navMeshSurface.BuildNavMesh();       

        /*
        float[,] heightMap = new float[(int)Mathf.Ceil(sizeX), (int)Mathf.Ceil(sizeY)];

        
        int maxX = (int)Mathf.Round(sizeX / meshSettings.VerticesPerLineCount) - 1;
        int maxY = (int)Mathf.Round(sizeY / meshSettings.VerticesPerLineCount) - 1;
        int coordOffsetX = maxX / 2;
        int coordOffsetY = maxY / 2;

        

        foreach (Chunk chunk in visibleChunks) {
            int offsetX = (int)(Mathf.Round(chunk.coordinate.x + coordOffsetX)) * (int)(meshSettings.VerticesPerLineCount);
            int offsetY = (int)((maxY) - Mathf.Round(chunk.coordinate.y + coordOffsetY)) * (int)(meshSettings.VerticesPerLineCount);
            for (int y = 0; y < chunk.heightMap.values01.GetLength(1); y++) {
                for (int x = 0; x < chunk.heightMap.values01.GetLength(0); x++) {
                    heightMap[offsetX + x, offsetY + y] = chunk.heightMap.values01[x, y];
                }
            }
        }


        pathData = PathGenerator.GeneratePath(pathSettings, heightMap, new Vector2(10, sizeY - 10), new Vector2(sizeX/Mathf.Max(sizeX,sizeY), -sizeY / Mathf.Max(sizeX, sizeY)));

        foreach (Chunk chunk in visibleChunks) {
            int offsetX = (int)(Mathf.Round(chunk.coordinate.x + coordOffsetX)) * (int)(meshSettings.VerticesPerLineCount);
            int offsetY = (int)((maxY) - Mathf.Round(chunk.coordinate.y + coordOffsetY)) * (int)(meshSettings.VerticesPerLineCount);
            for (int y = 0; y < chunk.heightMap.values01.GetLength(1); y++) {
                for (int x = 0; x < chunk.heightMap.values01.GetLength(0); x++) {
                    heightMap[offsetX + x, offsetY + y] = chunk.heightMap.values01[x, y];
                }
            }
        }


        foreach (Chunk chunk in visibleChunks) {
            int offsetX = (int)(Mathf.Round(chunk.coordinate.x + coordOffsetX)) * (int)(meshSettings.VerticesPerLineCount);
            int offsetY = (int)((maxY) - Mathf.Round(chunk.coordinate.y + coordOffsetY)) * (int)(meshSettings.VerticesPerLineCount);
            for (int y = 0; y < chunk.heightMap.values01.GetLength(1); y++) {
                for (int x = 0; x < chunk.heightMap.values01.GetLength(0); x++) {
                    heightMap[offsetX + x, offsetY + y] = chunk.heightMap.values01[x, y];
                }
            }
        }*/

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

}
        
