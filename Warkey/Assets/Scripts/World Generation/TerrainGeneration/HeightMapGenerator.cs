using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeightMapGenerator 
{
    public static HeightMap GenerateHeightMap(int width, int height, HeightMapSettings settings, Vector2 sampleCenter, Vector2 coord, float[,] fallOff) {
        float[,] values = Noise.GenerateNoiseMap(width, height, settings.noiseSettings, sampleCenter);
        AnimationCurve heightCurve = new AnimationCurve(settings.heightCurve.keys);
        float minValue = float.MaxValue;
        float maxValue = float.MinValue;
        float min01Value = float.MaxValue;
        float max01Value = float.MinValue;

        if (settings.useFallOff) {
            if(fallOff == null) {
                values = FallOffGenerator.ApplyFalloffMap(values, FallOffGenerator.GenerateFalloffMap(width,height));
            }
            else {
                values = FallOffGenerator.ApplyFalloffMapOffset(values, fallOff, coord);
            }
            //
            
        }

        float[,] values01 = (float[,])values.Clone();

        for (int i = 0; i < width; i++) {
            for(int j = 0; j < height; j++) {
                min01Value = Mathf.Min(min01Value, values[i, j]);
                max01Value = Mathf.Max(max01Value, values[i, j]);
                values[i, j] *= heightCurve.Evaluate(values[i, j]) * settings.heightMultiplier;
                minValue = Mathf.Min(minValue, values[i, j]);
                maxValue = Mathf.Max(maxValue, values[i, j]);
                
            }
        }


        //Debug.Log(minValue + " - " + maxValue + " : " + min01Value + " - " + max01Value);
        return new HeightMap(values, minValue, maxValue, values01, min01Value, max01Value);
    }

}

public struct HeightMap
{
    public readonly float[,] values;
    public readonly float minValue;
    public readonly float maxValue;
    public readonly float[,] values01;
    public readonly float min01Value;
    public readonly float max01Value;

    public HeightMap(float[,] values, float minValue, float maxValue, float[,] values01 = null, float min01Value = 0, float max01Value = 1) {
        this.values = values;
        this.minValue = minValue;
        this.maxValue = maxValue;
        this.min01Value = min01Value;
        this.max01Value = max01Value;
        this.values01 = values01;
    }

    public int sizeX { get => values.GetLength(0); }
    public int sizeY { get => values.GetLength(1); }
}