using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnviromentObjectGenerator
{
    public const float maxDistanceThreshold = 5f;
    public const float sqrMaxDistanceThreshold = maxDistanceThreshold*maxDistanceThreshold;


    public static List<ValidPoint> GenerateEnviroment(EnviromentObject enviromentObject, float[,] heightMap, PoissonDiscSettings poissonDiscSettings) {
        List<Vector2> poissonDiscGrid = PoissonDiscSampling.GeneratePoints(poissonDiscSettings,enviromentObject.blockRadius);
        List<ValidPoint> validGrid = new List<ValidPoint>();
        int mapWidth = heightMap.GetLength(0);
        int mapHeight = heightMap.GetLength(1);

        Random.InitState(poissonDiscSettings.seed);

        float upperBound = enviromentObject.maxThreshold;
        float lowerBound = enviromentObject.minThreshold;
        float midBound = (upperBound+lowerBound) / 2;


        for (int i = poissonDiscGrid.Count-1; i >= 0; --i) {
            Vector2 validPoint = poissonDiscGrid[i];
            if (mapHeight > validPoint.y && mapWidth > validPoint.x) {
                float height = heightMap[(int)validPoint.x, (int)validPoint.y];
                
                if (height >= enviromentObject.minThreshold && height <= enviromentObject.maxThreshold) {
                    float edgePercent = (Mathf.Abs(height - midBound)) / (midBound- lowerBound);
                    if (enviromentObject.lessenTowardsEdges && Random.Range(0f,1f) < enviromentObject.lessenScale && Random.Range(0f,0.90f) < edgePercent)  {
                        continue;
                    }
                    else {
                        Vector2 jitter =  Jitter(enviromentObject.blockRadius, enviromentObject.jitterScale);
                        validGrid.Add(new ValidPoint(validPoint, jitter));
                        poissonDiscGrid.RemoveAt(i);
                    }
                    
                }
            }
        }

                

        return validGrid;
    }

    private static Vector2 Jitter(float radius,float scale) {
        float angle = (float)(Random.value * Mathf.PI * 2);
        Vector2 direction = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
        return direction * Random.Range(0, radius) * scale;
    }


}
public class EnviromentObjectData
{
    List<ValidPoint> validPoints;
    EnviromentObject enviromentObject;
    List<GameObject> gameObjects = new List<GameObject>();

    public EnviromentObjectData(List<ValidPoint> validGrid, EnviromentObject enviromentObject, Transform parent) {
        this.validPoints = validGrid;
        this.enviromentObject = enviromentObject;

    }


    public void CreateObjects(float[,] heightMap,Transform parent) {
        for(int i = 0; i < validPoints.Count; i++) {
            Vector2 vector2 = validPoints[i].point;
            Vector2 jitter = validPoints[i].jitter;

            GameObject gameObject = MonoBehaviour.Instantiate(enviromentObject.gameObject,parent);
            gameObject.transform.localPosition = new Vector3((vector2.x + jitter.x) - heightMap.GetLength(0)/2, heightMap[(int)vector2.x, (int)vector2.y], -(vector2.y + jitter.y) + heightMap.GetLength(1)/2);
            gameObjects.Add(gameObject);
        }
    }

    public void DestroyObjects() {
        foreach (GameObject gameObject in gameObjects) {
#if UNITY_EDITOR
            MonoBehaviour.DestroyImmediate(gameObject);
#else
            MonoBehaviour.Destroy(gameObject);
#endif
        }
        gameObjects.Clear();
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
