using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class MeshSettings : UpdateableData 
{
    public const int lodCount = 5;
    public const int chunkSizesCount = 9;
    public const int flatShadedChunkSizesCount = 3;
    public static readonly int[] chunkSizes = { 48, 72, 96, 120, 144, 168, 192, 216, 240 };

    public float scale = 1f;
    public bool useFlatShading;

    [Range(0, chunkSizesCount - 1)]
    public int chunkSizeIndex;
    [Range(0, flatShadedChunkSizesCount - 1)]
    public int flatShadedChunkSizeIndex;

    [Header("Water")]
    [Range(0,1)]
    public float minValue;
    [Range(0, 1)]
    public float maxValue;
    public float height;


    //Number of vertices per line of mesh rendered at LOD = 0.
    //Includes 2 extra vertices that are exluded for final mesh but used for calculating normals
    public int VerticesPerLineCount {
        get {
            return chunkSizes[(useFlatShading) ? flatShadedChunkSizeIndex : chunkSizeIndex] + 5;
        }
    }

    public float MeshWorldSize {
        get {
            return (VerticesPerLineCount - 3) * scale;
        }
    }


}
