using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
   
    public static MeshData GenerateTerrainMesh(float[,] heightMap, MeshSettings meshSettings, int levelOfDetail) {
        int skipIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
        int verticesPerLineCount = meshSettings.VerticesPerLineCount;

        Vector2 topLeft = new Vector2(-1, 1) * meshSettings.MeshWorldSize / 2f;
        MeshData meshData = new MeshData(verticesPerLineCount,skipIncrement, meshSettings.useFlatShading);


        //int verticesPerLine = (meshSize - 1) / skipIncrement + 1;
        

        int[,] vertexIndicesMap = new int[verticesPerLineCount, verticesPerLineCount];
        int meshVertexIndex = 0;
        int outofMeshVertexIndex = -1;

        for (int y = 0; y < verticesPerLineCount; y ++) {
            for (int x = 0; x < verticesPerLineCount; x ++) {
                bool isOutofMeshVertex = y == 0 || y == verticesPerLineCount - 1 || x == 0 || x == verticesPerLineCount - 1;
                bool isSkippedVertex = x > 2 && x < verticesPerLineCount - 3 && y > 2 && y < verticesPerLineCount - 3 && ((x-2)%skipIncrement != 0 || (y-2)%skipIncrement != 0);

                if (isOutofMeshVertex) {
                    vertexIndicesMap[x, y] = outofMeshVertexIndex;
                    outofMeshVertexIndex--;
                }
                else if(!isSkippedVertex){
                    vertexIndicesMap[x, y] = meshVertexIndex;
                    meshVertexIndex++;
                }
            }
        }


        for (int y = 0; y < verticesPerLineCount; y++) {
            for(int x = 0; x < verticesPerLineCount; x++) {
                bool isSkippedVertex = x > 2 && x < verticesPerLineCount - 3 && y > 2 && y < verticesPerLineCount - 3 && ((x - 2) % skipIncrement != 0 || (y - 2) % skipIncrement != 0);

                if (!isSkippedVertex) {
                    bool isOutofMeshVertex = y == 0 || y == verticesPerLineCount - 1 || x == 0 || x == verticesPerLineCount - 1;
                    bool isMeshEdgeVertex = (y == 1 || y == verticesPerLineCount - 2 || x == 1 || x == verticesPerLineCount - 2) && !isOutofMeshVertex;
                    bool isMainVertex = (x - 2) % skipIncrement == 0 && (y - 2) % skipIncrement == 0 && !isOutofMeshVertex && !isMeshEdgeVertex;
                    bool isEdgeConnectionVertex = (y == 2 || y == verticesPerLineCount - 3 || x == 2 || x == verticesPerLineCount - 3) && !isMainVertex && !isOutofMeshVertex && !isMeshEdgeVertex;


                    int vertexIndex = vertexIndicesMap[x, y];
                    Vector2 vertexUV = new Vector2((x - 1), (y - 1))/(verticesPerLineCount-3);
                    Vector2 vertexPosition2D = topLeft + new Vector2(vertexUV.x,-vertexUV.y) * meshSettings.MeshWorldSize;
                    float height = heightMap[x, y];

                    if (isEdgeConnectionVertex) {
                        bool isVertical = x == 2 || x == verticesPerLineCount - 3;
                        int distanceToMainVertexA = ((isVertical)? y - 2: x - 2) % skipIncrement;
                        int distanceToMainVertexB = skipIncrement - distanceToMainVertexA;
                        float distancePercentFromAtoB = distanceToMainVertexA / (float)skipIncrement;

                        float heightMainVertexA = heightMap[(isVertical) ? x : x - distanceToMainVertexA, (isVertical) ? y - distanceToMainVertexA : y];
                        float heightMainVertexB = heightMap[(isVertical) ? x : x + distanceToMainVertexB, (isVertical) ? y + distanceToMainVertexB : y];

                        height = heightMainVertexA * (1 - distancePercentFromAtoB) + heightMainVertexB * distancePercentFromAtoB;
                    }


                    meshData.AddVertex(new Vector3(vertexPosition2D.x,height,vertexPosition2D.y), vertexUV, vertexIndex);

                    bool createTriangle = x < verticesPerLineCount - 1 && y < verticesPerLineCount - 1 && (!isEdgeConnectionVertex || (x != 2 && y != 2));

                    if (createTriangle) {
                        int currentIncrement = (isMainVertex && x != verticesPerLineCount-3 && y != verticesPerLineCount - 3) ? skipIncrement : 1;

                        int a = vertexIndicesMap[x, y];
                        int b = vertexIndicesMap[x + currentIncrement, y];
                        int c = vertexIndicesMap[x, y + currentIncrement];
                        int d = vertexIndicesMap[x + currentIncrement, y + currentIncrement];

                        meshData.AddTriangle(a, d, c);
                        meshData.AddTriangle(d, a, b);
                    }
                }
            }
        }
        meshData.ProcessMesh();

        return meshData;
    }



    public static MeshData GenerateTerrainMesh(float[,] heightMap, MeshSettings meshSettings, int levelOfDetail,float minHeight, float maxHeight,float defaultValue = 0) {
        int skipIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
        int verticesPerLineCount = meshSettings.VerticesPerLineCount;

        Vector2 topLeft = new Vector2(-1, 1) * meshSettings.MeshWorldSize / 2f;
        MeshData meshData = new MeshData(verticesPerLineCount, skipIncrement, meshSettings.useFlatShading);


        //int verticesPerLine = (meshSize - 1) / skipIncrement + 1;


        int[,] vertexIndicesMap = new int[verticesPerLineCount, verticesPerLineCount];
        int meshVertexIndex = 0;
        int outofMeshVertexIndex = -1;

        for (int y = 0; y < verticesPerLineCount; y++) {
            for (int x = 0; x < verticesPerLineCount; x++) {
                bool isOutofMeshVertex = y == 0 || y == verticesPerLineCount - 1 || x == 0 || x == verticesPerLineCount - 1;
                bool isSkippedVertex = x > 2 && x < verticesPerLineCount - 3 && y > 2 && y < verticesPerLineCount - 3 && ((x - 2) % skipIncrement != 0 || (y - 2) % skipIncrement != 0);

                if (isOutofMeshVertex) {
                    vertexIndicesMap[x, y] = outofMeshVertexIndex;
                    outofMeshVertexIndex--;
                }
                else if (!isSkippedVertex) {
                    vertexIndicesMap[x, y] = meshVertexIndex;
                    meshVertexIndex++;
                }
            }
        }


        for (int y = 0; y < verticesPerLineCount; y++) {
            for (int x = 0; x < verticesPerLineCount; x++) {
                bool isSkippedVertex = x > 2 && x < verticesPerLineCount - 3 && y > 2 && y < verticesPerLineCount - 3 && ((x - 2) % skipIncrement != 0 || (y - 2) % skipIncrement != 0);

                if (!isSkippedVertex) {
                    bool isOutofMeshVertex = y == 0 || y == verticesPerLineCount - 1 || x == 0 || x == verticesPerLineCount - 1;
                    bool isMeshEdgeVertex = (y == 1 || y == verticesPerLineCount - 2 || x == 1 || x == verticesPerLineCount - 2) && !isOutofMeshVertex;
                    bool isMainVertex = (x - 2) % skipIncrement == 0 && (y - 2) % skipIncrement == 0 && !isOutofMeshVertex && !isMeshEdgeVertex;
                    bool isEdgeConnectionVertex = (y == 2 || y == verticesPerLineCount - 3 || x == 2 || x == verticesPerLineCount - 3) && !isMainVertex && !isOutofMeshVertex && !isMeshEdgeVertex;


                    int vertexIndex = vertexIndicesMap[x, y];
                    Vector2 vertexUV = new Vector2((x - 1), (y - 1)) / (verticesPerLineCount - 3);
                    Vector2 vertexPosition2D = topLeft + new Vector2(vertexUV.x, -vertexUV.y) * meshSettings.MeshWorldSize;
                    float height = heightMap[x, y];

                    

                    if (isEdgeConnectionVertex) {
                        bool isVertical = x == 2 || x == verticesPerLineCount - 3;
                        int distanceToMainVertexA = ((isVertical) ? y - 2 : x - 2) % skipIncrement;
                        int distanceToMainVertexB = skipIncrement - distanceToMainVertexA;
                        float distancePercentFromAtoB = distanceToMainVertexA / (float)skipIncrement;

                        float heightMainVertexA = heightMap[(isVertical) ? x : x - distanceToMainVertexA, (isVertical) ? y - distanceToMainVertexA : y];
                        float heightMainVertexB = heightMap[(isVertical) ? x : x + distanceToMainVertexB, (isVertical) ? y + distanceToMainVertexB : y];

                        height = heightMainVertexA * (1 - distancePercentFromAtoB) + heightMainVertexB * distancePercentFromAtoB;
                    }


                    meshData.AddVertex(new Vector3(vertexPosition2D.x, defaultValue, vertexPosition2D.y), vertexUV, vertexIndex);
                    if (height < minHeight || height > maxHeight) continue;
                    bool createTriangle = x < verticesPerLineCount - 1 && y < verticesPerLineCount - 1 && (!isEdgeConnectionVertex || (x != 2 && y != 2));

                    if (createTriangle) {
                        int currentIncrement = (isMainVertex && x != verticesPerLineCount - 3 && y != verticesPerLineCount - 3) ? skipIncrement : 1;

                        int a = vertexIndicesMap[x, y];
                        int b = vertexIndicesMap[x + currentIncrement, y];
                        int c = vertexIndicesMap[x, y + currentIncrement];
                        int d = vertexIndicesMap[x + currentIncrement, y + currentIncrement];

                        meshData.AddTriangle(a, d, c);
                        meshData.AddTriangle(d, a, b);
                    }
                }
            }
        }
        meshData.ProcessMesh();

        return meshData;
    }
}

public class MeshData
{
    Vector3[] vertices;
    int[] triangles;
    Vector2[] uvs;
    Vector3[] bakedNormals;

    Vector3[] outOfMeshVertices;
    int[] outOfMeshTriangles;

    int triangleIndex;
    int outOfMeshTriangleIndex;

    bool useFlatShading;

    public MeshData(int verticesPerLine,int skipIncrement,bool useFlatShading) {
        this.useFlatShading = useFlatShading;

        int meshEdgeVerticesCount = (verticesPerLine - 2) * 4 - 4;
        int edgeConnectionVerticesCount = (skipIncrement - 1) * (verticesPerLine - 5) / skipIncrement * 4;
        int mainVerticesPerLineCount = (verticesPerLine - 5) / skipIncrement + 1;
        int mainVerticesCount = mainVerticesPerLineCount * mainVerticesPerLineCount;
        
        int meshEdgeTrianglesCount = (verticesPerLine-4) * 8;
        int mainTrianglesCount = (mainVerticesPerLineCount - 1) * (mainVerticesPerLineCount - 1) * 2;

        vertices = new Vector3[meshEdgeVerticesCount + edgeConnectionVerticesCount + mainVerticesCount];
        uvs = new Vector2[vertices.Length];
        triangles = new int[(meshEdgeTrianglesCount + mainTrianglesCount)*3];

        outOfMeshVertices = new Vector3[verticesPerLine * 4 - 4];
        outOfMeshTriangles = new int[24*(verticesPerLine-2)];

    }

    public void AddVertex(Vector3 vertexPosition, Vector2 uv, int vertexIndex) {
        if(vertexIndex < 0) {
            outOfMeshVertices[-vertexIndex - 1] = vertexPosition;
        }
        else {
            vertices[vertexIndex] = vertexPosition;
            uvs[vertexIndex] = uv;
        }
    }

    public void AddTriangle(int a, int b, int c) {
        if(a < 0 || b < 0 || c < 0) {
            outOfMeshTriangles[outOfMeshTriangleIndex] = a;
            outOfMeshTriangles[outOfMeshTriangleIndex + 1] = b;
            outOfMeshTriangles[outOfMeshTriangleIndex + 2] = c;
            outOfMeshTriangleIndex += 3;
        }
        else {
            triangles[triangleIndex] = a;
            triangles[triangleIndex + 1] = b;
            triangles[triangleIndex + 2] = c;
            triangleIndex += 3;
        }
        
    }

    Vector3[] CalculateNormals() {
        Vector3[] vertexNormals = new Vector3[vertices.Length];
        int triangleCount = triangles.Length/3;
        for(int i = 0; i < triangleCount; i++) {
            int normalTriangleIndex = i * 3;
            int vertexIndexA = triangles[normalTriangleIndex];
            int vertexIndexB = triangles[normalTriangleIndex+1];
            int vertexIndexC = triangles[normalTriangleIndex+2];

            Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
            vertexNormals[vertexIndexA] += triangleNormal;
            vertexNormals[vertexIndexB] += triangleNormal;
            vertexNormals[vertexIndexC] += triangleNormal;
        }

        int borderTriangleCount = outOfMeshTriangles.Length / 3;
        for (int i = 0; i < borderTriangleCount; i++) {
            int normalTriangleIndex = i * 3;
            int vertexIndexA = outOfMeshTriangles[normalTriangleIndex];
            int vertexIndexB = outOfMeshTriangles[normalTriangleIndex + 1];
            int vertexIndexC = outOfMeshTriangles[normalTriangleIndex + 2];

            Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
            if(vertexIndexA >= 0) {
                vertexNormals[vertexIndexA] += triangleNormal;
            }
            if (vertexIndexB >= 0) {
                vertexNormals[vertexIndexB] += triangleNormal;
            }
            if (vertexIndexC >= 0) {
                vertexNormals[vertexIndexC] += triangleNormal;
            }
        }

        for (int i = 0; i < vertexNormals.Length; i++) {
            vertexNormals[i].Normalize();
        }

        return vertexNormals;
    }

    Vector3 SurfaceNormalFromIndices(int indexA,int indexB,int indexC) {
        Vector3 pointA = (indexA < 0) ? outOfMeshVertices[-indexA - 1] : vertices[indexA];
        Vector3 pointB = (indexB < 0) ? outOfMeshVertices[-indexB - 1] : vertices[indexB];
        Vector3 pointC = (indexC < 0) ? outOfMeshVertices[-indexC - 1] : vertices[indexC];

        Vector3 sideAB = pointB - pointA;
        Vector3 sideAC = pointC - pointA;
        return Vector3.Cross(sideAB, sideAC).normalized;
    }

    public void ProcessMesh() {
        if (useFlatShading) {
            FlatShading();
        }
        else {
            BakeNormals();
        }
    }

    void BakeNormals() {
        bakedNormals = CalculateNormals();
    }

    void FlatShading() {
        Vector3[] flatShadedVertices = new Vector3[triangles.Length];
        Vector2[] flatShadedUvs = new Vector2[triangles.Length];

        for (int i = 0; i < triangles.Length; i++) {
            flatShadedVertices[i] = vertices[triangles[i]];
            flatShadedUvs[i] = uvs[triangles[i]];
            triangles[i] = i;
        }

        vertices = flatShadedVertices;
        uvs = flatShadedUvs;
    }

    public Mesh CreateMesh() {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        if (useFlatShading) {
            mesh.RecalculateNormals();
        }
        else
            mesh.normals = bakedNormals;
        return mesh;
    }
}