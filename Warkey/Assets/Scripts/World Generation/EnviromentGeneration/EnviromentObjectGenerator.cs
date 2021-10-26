using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnviromentObjectGenerator
{
    public const float maxDistanceThreshold = 5f;
    public const float sqrMaxDistanceThreshold = maxDistanceThreshold*maxDistanceThreshold;


    public static List<Vector2> GenerateEnviroment(EnviromentObject enviromentObject, float[,] heightMap, PoissonDiscSettings poissonDiscSettings) {
        List<Vector2> poissonDiscGrid = PoissonDiscSampling.GeneratePoints(poissonDiscSettings,enviromentObject.blockRadius);
        List<Vector2> validGrid = new List<Vector2>();
        int mapWidth = heightMap.GetLength(0);
        int mapHeight = heightMap.GetLength(1);

        for (int i = poissonDiscGrid.Count-1; i >= 0; --i) {
            Vector2 validPoint = poissonDiscGrid[i];
            if (mapHeight > validPoint.y && mapWidth > validPoint.x) {
                float height = heightMap[(int)validPoint.x, (int)validPoint.y];
                if (height >= enviromentObject.minThreshold && height <= enviromentObject.maxThreshold) {
                    validGrid.Add(validPoint);
                    poissonDiscGrid.RemoveAt(i);
                }
            }
        }

                

        return validGrid;
    }


}
public class EnviromentObjectData
{
    List<Vector2> validGrid;
    EnviromentObject enviromentObject;
    List<GameObject> gameObjects = new List<GameObject>();

    public EnviromentObjectData(List<Vector2> validGrid, EnviromentObject enviromentObject, Transform parent) {
        this.validGrid = validGrid;
        this.enviromentObject = enviromentObject;

    }


    public void CreateObjects(float[,] heightMap,Transform parent) {
        for(int i = 0; i < validGrid.Count; i++) {
            Vector2 vector2 = validGrid[i];
            
            GameObject gameObject = MonoBehaviour.Instantiate(enviromentObject.gameObject,parent);
            gameObject.transform.localPosition = new Vector3(vector2.x - heightMap.GetLength(0)/2, heightMap[(int)vector2.x, (int)vector2.y], -vector2.y + heightMap.GetLength(1)/2);
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
