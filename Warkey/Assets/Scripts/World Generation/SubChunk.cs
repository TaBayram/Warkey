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

    protected MeshRenderer meshRenderer;
    protected MeshFilter meshFilter;
    protected MeshCollider meshCollider;

    protected LODSettings LODSettings;
    protected LODMesh[] lODMeshes;
    protected int previousLODIndex = -1;
    protected int requestedLODIndex = -1;

    protected HeightMap heightMap;
    protected bool isHeightMapReceived;
    protected bool hasSetCollider;
    protected bool mustSetCollider;
    protected float maxViewDistance;

    protected HeightMapSettings heightMapSettings;
    protected MeshSettings meshSettings;

    public bool meshIsSet = false;
    public bool setCollider;

    public event System.Action<SubChunk> onLoadFinish;
    public bool isLoaded = false;

    public SubChunk(Chunk parent, bool setCollider = true) {
        this.coordinate = parent.coordinate;
        this.LODSettings = parent.LODSettings;
        this.heightMapSettings = parent.HeightMapSettings;
        this.meshSettings = parent.MeshSettings;
        this.chunk = parent;
        this.setCollider = setCollider;
        this.onLoadFinish += parent.SubchunkLoadCompletion;
        SetLODMeshes();
    }
    protected void SetLODMeshes() {
        lODMeshes = new LODMesh[LODSettings.LODCount];
        for (int i = 0; i < LODSettings.LODCount; i++) {
            lODMeshes[i] = new LODMesh(LODSettings.LODInfos[i].lod,i);
            lODMeshes[i].updateCallback += OnLODMeshReceived;
            if (i == LODSettings.colliderLOD) {
                lODMeshes[i].updateCallback += onColliderMeshReceived;
            }
        }
        maxViewDistance = LODSettings.LODInfos[LODSettings.LODCount - 1].visibleDistanceThreshold;
    }
    protected void SetObject() {
        Vector2 position = coordinate * meshSettings.MeshWorldSize;
        subObject.transform.position = new Vector3(position.x, 0, position.y);
        subObject.transform.parent = chunk.chunkObject.transform;
        //SetVisible(false);
    }    
    public virtual void SetHeightMap(HeightMap heightMap) {
        this.heightMap = heightMap;
        isHeightMapReceived = true;
    }
    public abstract void RequestMesh(LODMesh lODMesh);
    public void RequestLODMesh(LODMesh lODMesh) {
        requestedLODIndex = lODMesh.lodIndex;
        RequestMesh(lODMesh);
    }
    public void OnLODMeshReceived(int lodIndex) {
        if (lodIndex == requestedLODIndex) {
            LODMesh lodMesh = lODMeshes[lodIndex];
            if (lodMesh.hasMesh) {
                previousLODIndex = lodIndex;
                meshFilter.mesh = lodMesh.mesh;
                meshIsSet = true;
                if (!isLoaded) {
                    isLoaded = true;
                    onLoadFinish(this);
                }
                
            }
        }
    }
    public void onColliderMeshReceived(int lodIndex) {
        if (!hasSetCollider && setCollider) {
            meshCollider.sharedMesh = lODMeshes[lodIndex].mesh;
        }
    }
    public void UpdateSubChunk(int lodIndex) {
        if (!isHeightMapReceived) return;
        meshIsSet = false;
        if (lodIndex != previousLODIndex) {
            LODMesh lodMesh = lODMeshes[lodIndex];
            if (lodMesh.hasMesh) {
                previousLODIndex = lodIndex;
                meshFilter.mesh = lodMesh.mesh;
                meshIsSet = true;
                if (!isLoaded) {
                    isLoaded = true;
                    onLoadFinish(this);
                }
            }
            else if (!lodMesh.hasRequestedMesh) {
                RequestLODMesh(lodMesh);
            }
        }
    }
    public void UpdateCollisionMesh(float sqrNearestDistance) {
        if (hasSetCollider || !setCollider) return;
        if (sqrNearestDistance < LODSettings.LODInfos[LODSettings.colliderLOD].sqrVisibleThreshold) {
            if (!lODMeshes[LODSettings.colliderLOD].hasRequestedMesh) {
                RequestLODMesh(lODMeshes[LODSettings.colliderLOD]);
            }
        }
        if (sqrNearestDistance < colliderGenerationDistanceThreshold * colliderGenerationDistanceThreshold) {
            if (lODMeshes[LODSettings.colliderLOD].hasMesh) {
                meshCollider.sharedMesh = lODMeshes[LODSettings.colliderLOD].mesh;
                hasSetCollider = true;
            }
            else
                mustSetCollider = true;
        }
        else
            mustSetCollider = false;
    }
    public void LoadSubChunk(int lodIndex,bool loadCollider) {
        if (!isHeightMapReceived) return;
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
        if (setCollider && loadCollider && !hasSetCollider && !lODMeshes[LODSettings.colliderLOD].hasRequestedMesh) {
            RequestLODMesh(lODMeshes[LODSettings.colliderLOD]);
        }
    }

    public void SetVisible(bool visible) {
        subObject.SetActive(visible);
    }

    public bool IsVisible() {
        return subObject.activeSelf;
    }
}


