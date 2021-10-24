using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    const float viewerMoveThresholdForChunkUpdate = 5f;
    const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;

    public MeshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    public TextureData textureData;


    public int colliderLODIndex;
    public LODInfo[] LODInfos;


    public Transform viewer;

    Vector2 viewerPosition;
    Vector2 viewerPositionOld;

    float meshWorldSize;
    int chunkSizeVisibleInViewDistance;

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> visibleTerrainChunks = new List<TerrainChunk>();

    public Material mapMaterial;

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
        if (viewerPosition != viewerPositionOld) {
            foreach (TerrainChunk terrainChunk in visibleTerrainChunks) {
                terrainChunk.UpdateCollisionMesh();
            }
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
                        TerrainChunk terrainChunk = new TerrainChunk(viewedChunkCoord, heightMapSettings, meshSettings, LODInfos, colliderLODIndex, transform, viewer, mapMaterial);
                        terrainChunkDictionary.Add(viewedChunkCoord, terrainChunk);
                        terrainChunk.onVisibleChanged += OnTerrainChunkVisibilityChanged;
                        terrainChunk.Load();
                    }
                }
            }
        }
    }

    private void OnTerrainChunkVisibilityChanged(TerrainChunk chunk, bool isVisible) {
        if (isVisible) {
            visibleTerrainChunks.Add(chunk);
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
    public bool useForCollider;

    public float sqrVisibleThreshold {
        get {
            return visibleDistanceThreshold * visibleDistanceThreshold;
        }
    }
}
