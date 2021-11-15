using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class TerrainChunk: SubChunk
{
    public TerrainChunk(Chunk parent, Transform viewer, Material material) : base(parent, viewer){
        
        subObject = new GameObject("Terrain");
        meshCollider = subObject.AddComponent<MeshCollider>();
        meshRenderer = subObject.AddComponent<MeshRenderer>();
        meshFilter = subObject.AddComponent<MeshFilter>();
        meshRenderer.material = material;
        subObject.layer = LayerMask.NameToLayer("Ground");

        SetObject();
    }

    public override void RequestLODMesh(LODMesh lODMesh) {
        lODMesh.RequestMesh(heightMap, meshSettings);
    }
}

