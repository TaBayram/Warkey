using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class TerrainChunk: SubChunk
{
    public TerrainChunk(Chunk parent, Material material, PhysicMaterial physicMaterial) : base(parent){
        
        subObject = new GameObject("Terrain");
        meshCollider = subObject.AddComponent<MeshCollider>();
        meshCollider.material = physicMaterial;
        meshRenderer = subObject.AddComponent<MeshRenderer>();
        meshFilter = subObject.AddComponent<MeshFilter>();
        meshRenderer.material = material;
        subObject.layer = LayerMask.NameToLayer("Ground");

        SetObject();
    }

    public override void RequestMesh(LODMesh lODMesh) {
        lODMesh.RequestMesh(heightMap, meshSettings);
    }
}

