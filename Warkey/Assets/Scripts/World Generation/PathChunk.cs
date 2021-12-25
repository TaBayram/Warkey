using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathChunk : SubChunk
{
    PathSettings pathSettings;
    PathData pathData;
    AdjacentChunks adjacentChunks;
    bool isStartingChunk;
    bool hasSetPath;

    public PathData PathData { set => pathData = value; get => pathData; }

    public PathChunk(Chunk parent, Material material, float[,] pathmap) : base(parent,false) {
        this.pathSettings = parent.PathSettings;
        this.adjacentChunks = parent.AdjacentChunks;
        pathData = new PathData(pathmap);

        subObject = new GameObject("Path");
        meshCollider = subObject.AddComponent<MeshCollider>();
        meshRenderer = subObject.AddComponent<MeshRenderer>();
        meshFilter = subObject.AddComponent<MeshFilter>();
        meshRenderer.material = material;
        subObject.layer = LayerMask.NameToLayer("Ground");

        isStartingChunk = parent.IsStartingCoord();

        SetObject();
    }
    public override void RequestMesh(LODMesh lODMesh) {
        pathData.heightMap = PathGenerator.SetPathHeight(pathData.pathMap, this.heightMap.values, pathSettings);
        lODMesh.RequestMesh(new HeightMap(this.pathData.heightMap,0,0), meshSettings, LODMesh.MeshType.path);
    }

    public override void SetHeightMap(HeightMap heightMap) {
        this.heightMap = heightMap;
        this.isHeightMapReceived = true;
        //RequestPathData();
    }

    private void RequestPathData() {
        if(isStartingChunk)
            ThreadDataRequest.RequestData(() => PathGenerator.GeneratePath(pathSettings, heightMap, new Vector2(5, 240), new Vector2(1, -1)), OnPathDataReceived);
        else {
            //pathData = new PathData(heightMap.values.GetLength(0), heightMap.values.GetLength(1));
        }
    }

    public void CheckPath(Chunk chunk) {
        return;
        if (chunk.pathChunk.PathData == null) return;
        Vector2 corner =  chunk.pathChunk.pathData.endingCorner;

        if(chunk == adjacentChunks.chunks[(int)AdjacentChunks.Position.up]) {
            if(corner.y == 1) {
                pathData.SetStart(new Vector2(chunk.pathChunk.pathData.end.x, 0));
                ThreadDataRequest.RequestData(() => PathGenerator.AddPath(pathSettings, pathData ,heightMap), OnPathDataReceived);
            }
        }
        else if (chunk == adjacentChunks.chunks[(int)AdjacentChunks.Position.right]) {
            if (corner.x == 0) {

            }
        }
        else if (chunk == adjacentChunks.chunks[(int)AdjacentChunks.Position.down]) {
            if (corner.y == 0) {
                pathData.SetStart(new Vector2(chunk.pathChunk.pathData.end.x, heightMap.values.GetLength(0)));
                ThreadDataRequest.RequestData(() => PathGenerator.AddPath(pathSettings, pathData, heightMap), OnPathDataReceived);
            }
        }
        else if (chunk == adjacentChunks.chunks[(int)AdjacentChunks.Position.left]) {
            if (corner.x == 1) {

            }
        }
    }

    private void OnPathDataReceived(object pathData) {
       // this.pathData = (PathData)pathData;
        lODMeshes[previousLODIndex].RequestMesh(new HeightMap(this.pathData.heightMap, 0, 0), meshSettings, LODMesh.MeshType.path);
        hasSetPath = true;
    }
}

