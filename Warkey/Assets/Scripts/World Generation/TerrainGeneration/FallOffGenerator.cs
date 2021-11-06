using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FallOffGenerator
{
    public static float[,] GenerateFalloffMap(int size) {
        float[,] map = new float[size, size];

        for(int i = 0; i < size; i++) {
            for(int j = 0; j < size; j++) {
                float x = i / (float)size * 2 - 1;
                float y = j / (float)size * 2 - 1;

                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                map[i, j] = Evaluate(value);
            }
        }
        return map;
    }

    public static float[,] ApplyFalloffMap(float[,] values,float[,] falloff) {
        float maxValue = float.MinValue;
        float minValue = float.MaxValue;
        float size = values.GetLength(0);

        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                minValue = Mathf.Min(minValue, values[i, j]);
                maxValue = Mathf.Max(maxValue, values[i, j]);
            }
        }

        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                values[i, j] = Mathf.Clamp(values[i, j] - falloff[i, j], minValue, maxValue);
            }
        }
        return values;
    }

    public static float[,] ApplyFalloffMapOffset(float[,] values, float[,] falloff, Vector2 offset) {
        float maxValue = float.MinValue;
        float minValue = float.MaxValue;
        float size = values.GetLength(0);
        int maxX = (int)Mathf.Round(falloff.GetLength(0) / size) - 1;
        int coordOffset = maxX / 2;
        int fallOffSize = falloff.GetLength(0);

        int offsetX =(int)(Mathf.Round(offset.x + coordOffset)) *(int)(size);
        int offsetY =(int)((maxX) - Mathf.Round(offset.y + coordOffset))*(int)(size);
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                minValue = Mathf.Min(minValue, values[i, j]);
                maxValue = Mathf.Max(maxValue, values[i, j]);
            }
        }

        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                values[i, j] = Mathf.Clamp(values[i, j] - falloff[i + offsetX, j + offsetY], 0, maxValue);
            }
        }
        return values;
    }

    static float Evaluate(float value) {
        float a = 3;
        float b = 4.5f;

        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
    }
}
