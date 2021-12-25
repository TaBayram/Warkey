using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FallOffGenerator
{
    public static float[,] GenerateFalloffMap(int sizeX,int sizeY) {
        float[,] map = new float[sizeX, sizeY];

        for(int i = 0; i < sizeX; i++) {
            for(int j = 0; j < sizeY; j++) {
                float x = i / (float)sizeX * 2 - 1;
                float y = j / (float)sizeY * 2 - 1;

                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                map[i, j] = Evaluate(value);
            }
        }
        return map;
    }

    public static float[,] ApplyFalloffMap(float[,] values,float[,] falloff) {
        float maxValue = float.MinValue;
        float minValue = float.MaxValue;
        float sizeX = values.GetLength(0);
        float sizeY = values.GetLength(1);

        for (int i = 0; i < sizeX; i++) {
            for (int j = 0; j < sizeY; j++) {
                minValue = Mathf.Min(minValue, values[i, j]);
                maxValue = Mathf.Max(maxValue, values[i, j]);
            }
        }

        for (int i = 0; i < sizeX; i++) {
            for (int j = 0; j < sizeY; j++) {
                values[i, j] = Mathf.Clamp(values[i, j] - falloff[i, j], minValue, maxValue);
            }
        }
        return values;
    }

    public static float[,] ApplyFalloffMapOffset(float[,] values, float[,] falloff, Vector2 offset) {
        float maxValue = float.MinValue;
        float minValue = float.MaxValue;
        float sizeX = values.GetLength(0);
        float sizeY = values.GetLength(1);
        int maxX = (int)Mathf.Round(falloff.GetLength(0) / sizeX) - 1;
        int maxY = (int)Mathf.Round(falloff.GetLength(0) / sizeY) - 1;
        int coordOffsetX = maxX / 2;
        int coordOffsetY = maxY / 2;
        int fallOffSize = falloff.GetLength(0);

        int offsetX =(int)(Mathf.Round(offset.x + coordOffsetX)) *(int)(sizeX);
        int offsetY =(int)((maxY) - Mathf.Round(offset.y + coordOffsetY))*(int)(sizeY);
        for (int i = 0; i < sizeX; i++) {
            for (int j = 0; j < sizeY; j++) {
                minValue = Mathf.Min(minValue, values[i, j]);
                maxValue = Mathf.Max(maxValue, values[i, j]);
            }
        }

        for (int i = 0; i < sizeX; i++) {
            for (int j = 0; j < sizeX; j++) {
                values[i, j] = Mathf.Clamp(values[i, j] - falloff[i + offsetX, j + offsetY], 0, maxValue);
            }
        }
        return values;
    }

    static float Evaluate(float value) {
        float a = 3;
        float b = 4.5f;

        return  Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
    }
}
