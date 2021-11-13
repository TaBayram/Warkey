using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using System.Linq;

public class TerrainGenerator : MonoBehaviour
{
    const float viewerMoveThresholdForChunkUpdate = 5f;
    const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;

    public MeshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    public TextureSettings textureData;
    public GroundSettings groundSettings;

    public bool loadAtStart;
    public Vector2 chunkSize;
    public LODSettings LODSettings;

    public Transform viewer;

    Vector2 viewerPosition;
    Vector2 viewerPositionOld;

    float meshWorldSize;
    int chunkSizeVisibleInViewDistance;

    bool hasFilled = false;

    Dictionary<Vector2, Chunk> chunkDictionary = new Dictionary<Vector2, Chunk>();
    List<Chunk> visibleChunks = new List<Chunk>();


    public Material mapMaterial;
    public Material waterMaterial;
    public NavMeshSurface navMeshSurface;

    public float[,] fallOffMap;

    private void Start() {
        textureData.ApplyToMaterial(mapMaterial);
        textureData.UpdateMeshHeights(mapMaterial, heightMapSettings.MinHeight, heightMapSettings.MaxHeight);

        float maxViewDistance = LODSettings.LODInfos[LODSettings.LODCount - 1].visibleDistanceThreshold;
        meshWorldSize = meshSettings.MeshWorldSize;
        chunkSizeVisibleInViewDistance = Mathf.RoundToInt(maxViewDistance / meshWorldSize);

        if(chunkSize.x != 0 && chunkSize.y != 0 && heightMapSettings.useFallOff) {
            fallOffMap = FallOffGenerator.GenerateFalloffMap((int)(meshSettings.VerticesPerLineCount*chunkSize.x));
        }
        LoadAll();
        UpdateVisibleChunks();
    }

    private void LoadAll() {
        if (!loadAtStart || (chunkSize.x == 0 || chunkSize.y == 0)) return;
        int maxX = (int)chunkSize.x / 2 + 1;
        int maxY = (int)chunkSize.y / 2 + 1;
        int xModifier = (chunkSize.x % 2 == 0) ? 1 : 0;
        int yModifier = (chunkSize.y % 2 == 0) ? 1 : 0;

        for (int yOffset = -maxY + yModifier+1; yOffset < maxX; yOffset++) {
            for (int xOffset = -maxX + xModifier+1; xOffset < maxY; xOffset++) {
                Vector2 viewedChunkCoord = new Vector2(xOffset, yOffset);
                if (!chunkDictionary.ContainsKey(viewedChunkCoord)) {
                    if (CanCreateTerrainChunk(viewedChunkCoord)) {
                        Chunk chunk = new Chunk(viewedChunkCoord, heightMapSettings, meshSettings, groundSettings, LODSettings, transform, viewer, mapMaterial, waterMaterial);
                        chunkDictionary.Add(viewedChunkCoord, chunk);
                        chunk.onVisibleChanged += OnChunkVisibilityChanged;
                        chunk.Load(fallOffMap);
                        chunk.SetVisible(false);
                    }
                }
                
            }
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

        int count = (int)chunkSize.x * (int)chunkSize.y;
        count = (count == 0) ? 4 : count;
        bool bake = true;
        foreach(Chunk chunk in visibleChunks) {
            if (!chunk.terrainChunk.meshIsSet) bake = false;
        }
        if (!hasFilled && visibleChunks.Count >= count && bake) {
            UpdateChunkCollisions();
            navMeshSurface.BuildNavMesh();
            hasFilled = true;
        }

    }

    private void UpdateChunkCollisions() {
        foreach (Chunk chunk in visibleChunks) {
            chunk.terrainChunk.UpdateCollisionMesh();
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
                            Chunk chunk = new Chunk(viewedChunkCoord, heightMapSettings, meshSettings, groundSettings, LODSettings, transform, viewer, mapMaterial, waterMaterial);
                            chunkDictionary.Add(viewedChunkCoord, chunk);
                            chunk.onVisibleChanged += OnChunkVisibilityChanged;
                            chunk.Load(fallOffMap);
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
        
