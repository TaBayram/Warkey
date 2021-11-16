using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnviromentObjectData
{
    const int rotationSampleRadius = 5;
    public readonly Transform parent;
    private GameObject enviromentHolder;
    private EnviromentObjectSettings settings;
    private List<ObjectPlace> objectPlaces = new List<ObjectPlace>();
    private List<GameObject> gameObjects = new List<GameObject>();

    public bool isObjectsLoaded = false;

    public EnviromentObjectSettings Settings { get => settings;}

    public EnviromentObjectData(List<ValidPoint> validGrid, EnviromentObjectSettings enviromentObject, Transform parent, float[,] heightMap) {
        this.settings = enviromentObject;
        this.parent = parent;

        CreateObjectPlaces(validGrid,heightMap);
    }

    private void CreateObjectPlaces(List<ValidPoint> validPoints, float[,] heightMap) {

        for (int i = 0; i < validPoints.Count; i++) {
            Vector3 position = Vector3.zero;
            Vector3 rotation = Vector3.zero;

            Vector2 vector2 = validPoints[i].point;
            Vector2 jitter = validPoints[i].jitter;
            Vector2 point = validPoints[i].point + validPoints[i].jitter;

            Vector2 heightIndex = new Vector2((int)(point.x), (int)(point.y));
            float height = IndexExists(heightIndex.x, heightIndex.y, heightMap) ? heightMap[(int)heightIndex.x, (int)heightIndex.y] : heightMap[(int)vector2.x, (int)vector2.y];
            position = new Vector3(point.x - heightMap.GetLength(0) / 2, height + settings.elevation, -point.y + heightMap.GetLength(1) / 2);

            if (settings.correctRotation) {

                Vector2[] xRotationIndex = new Vector2[] { new Vector2(point.x + rotationSampleRadius, point.y), new Vector2(point.x - rotationSampleRadius, point.y) };
                Vector2[] zRotationIndex = new Vector2[] { new Vector2(point.x, point.y + rotationSampleRadius), new Vector2(point.x, point.y - rotationSampleRadius) };

                if (IndexExists(xRotationIndex[0].x, xRotationIndex[0].y, heightMap) && IndexExists(xRotationIndex[1].x, xRotationIndex[1].y, heightMap)) {
                    float[] xHeights = new float[] { heightMap[(int)xRotationIndex[0].x, (int)xRotationIndex[0].y], heightMap[(int)xRotationIndex[1].x, (int)xRotationIndex[1].y] };
                    float rotationZ = (xHeights[0] - xHeights[1]) / (xRotationIndex[0].x - xRotationIndex[1].x);
                    rotation.z = Mathf.Tan(rotationZ) * 180 / Mathf.PI;
                }

                if (IndexExists(zRotationIndex[0].x, zRotationIndex[0].y, heightMap) && IndexExists(zRotationIndex[1].x, zRotationIndex[1].y, heightMap)) {
                    float[] zHeights = new float[] { heightMap[(int)zRotationIndex[0].x, (int)zRotationIndex[0].y], heightMap[(int)zRotationIndex[1].x, (int)zRotationIndex[1].y] };
                    float rotationX = (zHeights[0] - zHeights[1]) / (zRotationIndex[0].y - zRotationIndex[1].y);
                    rotation.x = Mathf.Tan(rotationX) * 180 / Mathf.PI;
                }
            }
            objectPlaces.Add(new ObjectPlace(validPoints[i], position, rotation));

        }
    }


    public void CreateObjects(bool visible = false) {
        enviromentHolder = new GameObject("Enviroment Holder");
        enviromentHolder.transform.parent = parent;
        enviromentHolder.transform.position = parent.position;
        for (int i = 0; i < objectPlaces.Count; i++) {
            GameObject gameObject = MonoBehaviour.Instantiate(settings.gameObject, enviromentHolder.transform);
            gameObject.transform.localPosition = objectPlaces[i].position;
            gameObject.transform.Rotate(objectPlaces[i].rotation.x, 0, objectPlaces[i].rotation.z);
            gameObjects.Add(gameObject);
        }
        isObjectsLoaded = true;
        enviromentHolder.SetActive(visible);
    }

    private bool IndexExists(float x, float y, float[,] heightMap) {
        return (x >= 0 && x < heightMap.GetLength(0) && y >= 0 && y < heightMap.GetLength(1));
    }

    public void Visible(bool visible) {
        enviromentHolder.SetActive(visible);
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



public struct ObjectPlace
{
    public readonly ValidPoint validPoint;
    public readonly Vector3 position;
    public readonly Vector3 rotation;

    public ObjectPlace(ValidPoint validPoint, Vector3 position, Vector3 rotation) {
        this.validPoint = validPoint;
        this.position = position;
        this.rotation = rotation;
    }
}