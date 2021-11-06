using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public enum NormalizeMode {Local,Global};

    //lacunarity small feautures on map
    //persistance small features influence the map
    static int maxOffset = 100000;
    public static float[,] GenerateNoiseMap(int mapWidth,int mapHeight,NoiseSettings noiseSettings, Vector2 sampleCentre){
        float[,] noiseMap = new float[mapWidth,mapHeight];

        System.Random random = new System.Random(noiseSettings.seed);
        Vector2[] octaveOffsets = new Vector2[noiseSettings.octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < noiseSettings.octaves; i++) {
            float offsetX = random.Next(maxOffset * -1, maxOffset) + noiseSettings.offset.x + sampleCentre.x;
            float offsetY = random.Next(maxOffset * -1, maxOffset) - noiseSettings.offset.y - sampleCentre.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= noiseSettings.persistance;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for(int y = 0; y < mapHeight; y++){
            for(int x = 0; x < mapWidth; x++){

                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                for(int i = 0; i < noiseSettings.octaves; i++) {
                    float sampleX = (x-halfWidth + octaveOffsets[i].x) / noiseSettings.scale * frequency ;
                    float sampleY = (y-halfHeight + octaveOffsets[i].y) / noiseSettings.scale * frequency ;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 -1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= noiseSettings.persistance;
                    frequency *= noiseSettings.lacunarity;
                }
                maxLocalNoiseHeight = Mathf.Max(maxLocalNoiseHeight, noiseHeight);
                minLocalNoiseHeight = Mathf.Min(minLocalNoiseHeight, noiseHeight);
                noiseMap[x, y] = noiseHeight;

                if (noiseSettings.normalizeMode == NormalizeMode.Global) { 
                    float normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight / 0.90f);
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
            }
        }
        if (noiseSettings.normalizeMode == NormalizeMode.Local) {
            for (int y = 0; y < mapHeight; y++) {
                for (int x = 0; x < mapWidth; x++) {
                     noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                }
            }
        }

        return noiseMap;
    }
}

[System.Serializable]
public class NoiseSettings {
    public Noise.NormalizeMode normalizeMode;
    public float scale = 30;
    public int octaves = 3;
    [Range(0, 1)]
    public float persistance = 0.3f;
    public float lacunarity = 2;
    public int seed;
    public Vector2 offset;

    public void ValidateValues() {
        scale = Mathf.Max(scale, 0.01f);
        octaves = Mathf.Max(octaves, 1);
        lacunarity = Mathf.Max(lacunarity, 1);
        persistance = Mathf.Clamp01(persistance);
    }
}