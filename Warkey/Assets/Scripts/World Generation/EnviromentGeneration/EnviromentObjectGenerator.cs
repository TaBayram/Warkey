using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnviromentObjectGenerator
{
    public const float maxDistanceThreshold = 5f;
    public const float sqrMaxDistanceThreshold = maxDistanceThreshold*maxDistanceThreshold;


    public static List<ValidPoint> GenerateValidPoints(EnviromentObjectSettings enviromentObject, float[,] heightMap, PoissonDiscSettings poissonDiscSettings, Vector2 coord) {
        float[,] noiseMap = null;
        if (enviromentObject.useNoise) {
            noiseMap = Noise.GenerateNoiseMap(heightMap.GetLength(0), heightMap.GetLength(1), enviromentObject.noiseSettings, coord);
        }


        List<Vector2> poissonDiscGrid = PoissonDiscSampling.GeneratePoints(poissonDiscSettings,enviromentObject.blockRadius);
        List<ValidPoint> validGrid = new List<ValidPoint>();
        int mapWidth = heightMap.GetLength(0);
        int mapHeight = heightMap.GetLength(1);

        System.Random random = new System.Random(poissonDiscSettings.seed);

        float upperBound = enviromentObject.maxThreshold;
        float lowerBound = enviromentObject.minThreshold;
        float midBound = (upperBound+lowerBound) / 2;


        for (int i = poissonDiscGrid.Count-1; i >= 0; --i) {
            Vector2 validPoint = poissonDiscGrid[i];
            if (mapHeight > validPoint.y && mapWidth > validPoint.x) {
                float height = heightMap[(int)validPoint.x, (int)validPoint.y];
                
                if (height >= enviromentObject.minThreshold && height <= enviromentObject.maxThreshold) {
                    float edgePercent = (Mathf.Abs(height - midBound)) / (midBound- lowerBound);
                    if (enviromentObject.lessenTowardsEdges && RandomHelper.Range(0f,1f,ref random) < enviromentObject.lessenScale && RandomHelper.Range(0f,0.90f,ref random) < edgePercent)  {
                        continue;
                    }
                    else {
                        if (enviromentObject.useNoise) {
                            float noiseHeight = noiseMap[(int)validPoint.x, (int)validPoint.y];
                            if (enviromentObject.noiseMin > noiseHeight || enviromentObject.noiseMax < noiseHeight) continue;
                        }

                        Vector2 jitter =  Jitter(enviromentObject.blockRadius, enviromentObject.jitterScale,ref random);
                        validGrid.Add(new ValidPoint(validPoint, jitter));
                        poissonDiscGrid.RemoveAt(i);
                    }
                    
                }
            }
        }
        return validGrid;
    }

    public static List<EnviromentObjectData> GenerateEnviromentDatas(HeightMap heightMap,GroundSettings groundSettings,Transform parent,Vector2 coord) {
        List<EnviromentObjectData> enviromentObjectDatas = new List<EnviromentObjectData>();
        for (int i = 0; i < groundSettings.enviromentObjects.Length; i++) {
            if (groundSettings.enviromentObjects[i].enabled) {
                List<ValidPoint> grid = EnviromentObjectGenerator.GenerateValidPoints(groundSettings.enviromentObjects[i], heightMap.values01, groundSettings.poissonDiscSettings, coord);
                EnviromentObjectData enviromentObjectData = new EnviromentObjectData(grid, groundSettings.enviromentObjects[i], parent,heightMap.values);
                enviromentObjectDatas.Add(enviromentObjectData);
            }
        }
        return enviromentObjectDatas;
    }


    private static Vector2 Jitter(float radius,float scale,ref System.Random random) {
        float angle = (float)(RandomHelper.Range(ref random) * Mathf.PI * 2);
        Vector2 direction = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
        return direction * RandomHelper.Range(0, radius,ref random) * scale;
    }


}

public struct ValidPoint
{
    public readonly Vector2 point;
    public readonly Vector2 jitter;

    public ValidPoint(Vector2 point, Vector2 jitter) {
        this.point = point;
        this.jitter = jitter;
    }
}