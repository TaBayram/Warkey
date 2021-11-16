using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class WaterChunk : SubChunk
{
    public WaterChunk(Chunk parent, Transform viewer, Material material) : base(parent) {

        subObject = new GameObject("Water");
        meshCollider = subObject.AddComponent<MeshCollider>();
        meshRenderer = subObject.AddComponent<MeshRenderer>();
        meshFilter = subObject.AddComponent<MeshFilter>();
        meshRenderer.material = material;
        subObject.layer = LayerMask.NameToLayer("Water");
        

        SetObject();
        subObject.transform.position = subObject.transform.position + Vector3.up * meshSettings.height;
    }
    public override void RequestMesh(LODMesh lODMesh) {
        lODMesh.RequestMesh(heightMap, meshSettings, LODMesh.MeshType.water);
    }
}
