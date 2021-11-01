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

    public Vector2 chunkSize;
    public int colliderLODIndex;
    public int objectCreationLODIndex;
    public LODInfo[] LODInfos;

    public Transform viewer;

    Vector2 viewerPosition;
    Vector2 viewerPositionOld;

    float meshWorldSize;
    int chunkSizeVisibleInViewDistance;

    bool hasFilled = false;

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> visibleTerrainChunks = new List<TerrainChunk>();

    public Material mapMaterial;
    public NavMeshSurface navMeshSurface;

    private void Start() {
        textureData.ApplyToMaterial(mapMaterial);
        textureData.UpdateMeshHeights(mapMaterial, heightMapSettings.MinHeight, heightMapSettings.MaxHeight);

        float maxViewDistance = LODInfos[LODInfos.Length - 1].visibleDistanceThreshold;
        meshWorldSize = meshSettings.MeshWorldSize;
        chunkSizeVisibleInViewDistance = Mathf.RoundToInt(maxViewDistance / meshWorldSize);

        UpdateVisibleChunks();
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
        foreach(TerrainChunk terrainChunk in visibleTerrainChunks) {
            if (!terrainChunk.meshIsSet) bake = false;
        }
        if (!hasFilled && visibleTerrainChunks.Count >= count && bake) {
            UpdateChunkCollisions();
            navMeshSurface.BuildNavMesh();
            hasFilled = true;
        }

    }

    private void UpdateChunkCollisions() {
        foreach (TerrainChunk terrainChunk in visibleTerrainChunks) {
            terrainChunk.UpdateCollisionMesh();
        }
    }

    private void UpdateVisibleChunks() {
        HashSet<Vector2> alreadyUpdatedChunkCoords = new HashSet<Vector2>();
        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / meshWorldSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / meshWorldSize);

        for (int i = visibleTerrainChunks.Count - 1; i >= 0; i--) {
            alreadyUpdatedChunkCoords.Add(visibleTerrainChunks[i].coord);
            visibleTerrainChunks[i].UpdateTerrainChunk();
        }

        for (int yOffset = -chunkSizeVisibleInViewDistance; yOffset <= chunkSizeVisibleInViewDistance; yOffset++) {
            for (int xOffset = -chunkSizeVisibleInViewDistance; xOffset <= chunkSizeVisibleInViewDistance; xOffset++) {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
                if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord)) {
                    if (terrainChunkDictionary.ContainsKey(viewedChunkCoord)) {
                        terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                    }
                    else {
                        if (CanCreateTerrainChunk(viewedChunkCoord)) {
                            TerrainChunk terrainChunk = new TerrainChunk(viewedChunkCoord, heightMapSettings, meshSettings,groundSettings ,LODInfos, colliderLODIndex, transform, viewer, mapMaterial);
                            terrainChunkDictionary.Add(viewedChunkCoord, terrainChunk);
                            terrainChunk.onVisibleChanged += OnTerrainChunkVisibilityChanged;
                            terrainChunk.Load();
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

    private void OnTerrainChunkVisibilityChanged(TerrainChunk chunk, bool isVisible) {
        if (isVisible) {
            visibleTerrainChunks.Add(chunk);
            /*int count = (int)chunkSize.x * (int)chunkSize.y;
            count = (count == 0) ? 4 : count;
            if(!hasFilled && visibleTerrainChunks.Count >= count ) {
                UpdateChunkCollisions();
                Debug.Log("build");
                navMeshSurface.BuildNavMesh();
                hasFilled = true;
            }*/
        }
        else {
            visibleTerrainChunks.Remove(chunk);
        }
    }

}
        

[System.Serializable]
public struct LODInfo
{
    [Range(0,MeshSettings.lodCount-1)]
    public int lod;
    public float visibleDistanceThreshold;

    public float sqrVisibleThreshold {
        get {
            return visibleDistanceThreshold * visibleDistanceThreshold;
        }
    }
}
