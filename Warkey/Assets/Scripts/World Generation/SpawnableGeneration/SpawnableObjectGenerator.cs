using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpawnableObjectGenerator
{
    public const float maxDistanceThreshold = 5f;
    public const float sqrMaxDistanceThreshold = maxDistanceThreshold*maxDistanceThreshold;


    public static List<ValidPoint> GenerateValidPoints(ObjectSettings settings, float[,] heightMap, PoissonDiscSettings poissonDiscSettings, Vector2 coord) {
        float[,] noiseMap = null;
        if (settings.useNoise) {
            noiseMap = Noise.GenerateNoiseMap(heightMap.GetLength(0), heightMap.GetLength(1), settings.noiseSettings, coord);
        }

        List<Vector2> poissonDiscGrid = PoissonDiscSampling.GeneratePoints(poissonDiscSettings,settings.blockRadius);
        List<ValidPoint> validGrid = new List<ValidPoint>();
        int mapWidth = heightMap.GetLength(0);
        int mapHeight = heightMap.GetLength(1);

        for (int i = poissonDiscGrid.Count-1; i >= 0; --i) {
            Vector2 validPoint = poissonDiscGrid[i];
            if (mapHeight > validPoint.y && mapWidth > validPoint.x) {
                float height = heightMap[(int)validPoint.x, (int)validPoint.y];
                
                if (height >= settings.minThreshold && height <= settings.maxThreshold) {
                    if (settings.useNoise) {
                        float noiseHeight = noiseMap[(int)validPoint.x, (int)validPoint.y];
                        if (settings.noiseMin > noiseHeight || settings.noiseMax < noiseHeight) continue;
                    }
                    validGrid.Add(new ValidPoint(validPoint, Vector2.zero));
                    poissonDiscGrid.RemoveAt(i);
                }
            }
        }
        System.Random random = new System.Random(poissonDiscSettings.seed);
        while (settings.spawnLimit != 0 && validGrid.Count > settings.spawnLimit) {
            int index = RandomHelper.Range(0, validGrid.Count, ref random);
            validGrid.RemoveAt(index);
        }

        return validGrid;
    }

    public static List<SpawnableObjectData> GenerateSpawnableDatas(HeightMap heightMap,SpawnableSettings settings,Transform parent,Vector2 coord) {
        List<SpawnableObjectData> spawnableObjectDatas = new List<SpawnableObjectData>();
        for (int i = 0; i < settings.objectSettings.Length; i++) {
            if (settings.objectSettings[i].enabled) {
                List<ValidPoint> grid = SpawnableObjectGenerator.GenerateValidPoints(settings.objectSettings[i], heightMap.values01, settings.poissonDiscSettings, coord);
                SpawnableObjectData spawnableObjectData = new SpawnableObjectData(grid, settings.objectSettings[i], parent,heightMap.values, settings.poissonDiscSettings.seed);
                spawnableObjectDatas.Add(spawnableObjectData);
            }
        }
        return spawnableObjectDatas;
    }
}