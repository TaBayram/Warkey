using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PoissonDiscSampling
{
    
    public static List<Vector2> GeneratePoints(PoissonDiscSettings settings, float radius) {
        float cellSize = radius / Mathf.Sqrt(2);

        System.Random random = new System.Random(settings.seed);

        int[,] grid = new int[Mathf.CeilToInt(settings.sampleRegionSize.x / cellSize), Mathf.CeilToInt(settings.sampleRegionSize.y / cellSize)];
        List<Vector2> points = new List<Vector2>();
        List<Vector2> spawnPoints = new List<Vector2>();

        spawnPoints.Add(settings.sampleRegionSize / 2);
        while(spawnPoints.Count > 0) {
            int spawnIndex = RandomHelper.Range(0, spawnPoints.Count,ref random);
            Vector2 spawnCenter = spawnPoints[spawnIndex];
            bool candidateAccepted = false;

            for (int i = 0; i < settings.sampleCountBeforeRejection; i++) {
                float angle = (float)(RandomHelper.Range(ref random) * Mathf.PI * 2);
                Vector2 direction = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                Vector2 candidate = spawnCenter + direction * RandomHelper.Range(radius, 2* radius,ref random);
                
                if (IsValid(candidate, settings.sampleRegionSize,cellSize, radius,points,grid)) {
                    points.Add(candidate);
                    spawnPoints.Add(candidate);
                    grid[(int)(candidate.x / cellSize), (int)(candidate.y / cellSize)] = points.Count;
                    candidateAccepted = true;
                    break;
                }
            }

            if (!candidateAccepted) {
                spawnPoints.RemoveAt(spawnIndex);
            }
        }

        return points;
    }

    private static bool IsValid(Vector2 candidate, Vector2 sampleRegionSize, float cellSize,float radius, List<Vector2> points, int[,] grid) {
        if(candidate.x >= 0 && candidate.x < sampleRegionSize.x && candidate.y >= 0 && candidate.y < sampleRegionSize.y) {
            int cellX = (int)(candidate.x / cellSize);
            int cellY = (int)(candidate.y / cellSize);

            int searchStartX = Mathf.Max(0, cellX - 2);
            int searchEndX = Mathf.Min(grid.GetLength(0)-1, cellX + 2);
            int searchStartY = Mathf.Max(0, cellY - 2);
            int searchEndY = Mathf.Min(grid.GetLength(1) - 1, cellY + 2);

            for(int x = searchStartX; x <= searchEndX; x++) {
                for(int y = searchStartY; y <= searchEndY; y++) {
                    int pointIndex = grid[x, y] - 1;
                    if(pointIndex != -1) {
                        float sqrDistance = (candidate - points[pointIndex]).sqrMagnitude;
                        if(sqrDistance < radius*radius) {
                            return false;
                        }
                    }
                }
            }

            return true;

        }

        return false;
    }


}


[System.Serializable]
public class PoissonDiscSettings
{
    public int seed;
    public Vector2 sampleRegionSize;
    public int sampleCountBeforeRejection;
}
