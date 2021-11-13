using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SubChunk
{
    protected const float colliderGenerationDistanceThreshold = 5;
    public Vector2 coordinate;

    protected GameObject subObject;
    protected Chunk chunk;
    protected Vector2 worldPosition;
    protected Bounds bounds;

    protected MeshRenderer meshRenderer;
    protected MeshFilter meshFilter;
    protected MeshCollider meshCollider;

    protected LODSettings LODSettings;
    protected LODMesh[] lODMeshes;
    protected int previousLODIndex = -1;

    protected HeightMap heightMap;
    protected bool isMapDataReceived;
    protected bool hasSetCollider;
    protected float maxViewDistance;

    protected HeightMapSettings heightMapSettings;
    protected MeshSettings meshSettings;

    protected Transform viewer;
    public bool meshIsSet = false;

    protected Vector2 ViewerPosition {
        get {
            return new Vector2(viewer.position.x, viewer.position.z);
        }
    }

    public SubChunk(Chunk parent, Transform viewer) {
        this.coordinate = parent.coordinate;
        this.LODSettings = parent.LODSettings;
        this.heightMapSettings = parent.HeightMapSettings;
        this.meshSettings = parent.MeshSettings;
        this.viewer = viewer;
        this.bounds = parent.bounds;
        this.chunk = parent;
        SetLODMeshes();
    }

    public void SetHeightMap(HeightMap heightMap) {
        this.heightMap = heightMap;
        isMapDataReceived = true;
    }

    public abstract void RequestLODMesh(LODMesh lODMesh);

    protected void SetObject() {
        Vector2 position = coordinate * meshSettings.MeshWorldSize;
        subObject.transform.position = new Vector3(position.x, 0, position.y);
        subObject.transform.parent = chunk.chunkObject.transform;
        SetVisible(false);
    }

    protected void SetLODMeshes() {
        lODMeshes = new LODMesh[LODSettings.LODCount];
        for (int i = 0; i < LODSettings.LODCount; i++) {
            lODMeshes[i] = new LODMesh(LODSettings.LODInfos[i].lod);
            lODMeshes[i].updateCallback += UpdateSubChunk;
            if (i == LODSettings.colliderLOD) {
                lODMeshes[i].updateCallback += UpdateCollisionMesh;
            }
        }
        maxViewDistance = LODSettings.LODInfos[LODSettings.LODCount - 1].visibleDistanceThreshold;
    }

    public void UpdateSubChunk() {
        if (!isMapDataReceived) return;
        meshIsSet = false;
        float viewerDistanceFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(ViewerPosition));
        bool visible = viewerDistanceFromNearestEdge <= maxViewDistance;

        if (visible) {
            int lodIndex = 0;
            for (int i = 0; i < LODSettings.LODCount - 1; i++) {
                if (viewerDistanceFromNearestEdge > LODSettings.LODInfos[i].visibleDistanceThreshold)
                    lodIndex = i + 1;
                else break;
            }
            if (lodIndex != previousLODIndex) {
                LODMesh lodMesh = lODMeshes[lodIndex];
                if (lodMesh.hasMesh) {
                    previousLODIndex = lodIndex;
                    meshFilter.mesh = lodMesh.mesh;
                    meshIsSet = true;
                }
                else if (!lodMesh.hasRequestedMesh) {
                    RequestLODMesh(lodMesh);
                }

            }
        }
    }


    public void ForceLODMesh(int lodIndex) {
        if (!isMapDataReceived) return;
        meshIsSet = false;
        LODMesh lodMesh = lODMeshes[lodIndex];
        if (lodMesh.hasMesh) {
            previousLODIndex = lodIndex;
            meshFilter.mesh = lodMesh.mesh;
            meshIsSet = true;
        }
        if (!lodMesh.hasRequestedMesh) {
            RequestLODMesh(lodMesh);
        }
    }

    public void UpdateCollisionMesh() {
        if (hasSetCollider) return;
        float sqrDistanceFromViewerToEdge = bounds.SqrDistance(ViewerPosition);
        if (sqrDistanceFromViewerToEdge < LODSettings.LODInfos[LODSettings.colliderLOD].sqrVisibleThreshold) {
            if (!lODMeshes[LODSettings.colliderLOD].hasRequestedMesh) {
                RequestLODMesh(lODMeshes[LODSettings.colliderLOD]);
            }
        }

        if (sqrDistanceFromViewerToEdge < colliderGenerationDistanceThreshold * colliderGenerationDistanceThreshold) {
            if (lODMeshes[LODSettings.colliderLOD].hasMesh) {
                meshCollider.sharedMesh = lODMeshes[LODSettings.colliderLOD].mesh;
                hasSetCollider = true;
            }
        }

    }

    public void SetVisible(bool visible) {
        subObject.SetActive(visible);
    }

    public bool IsVisible() {
        return subObject.activeSelf;
    }
}


