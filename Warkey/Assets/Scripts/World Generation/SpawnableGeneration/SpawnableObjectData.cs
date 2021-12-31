using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class SpawnableObjectData
{
    public readonly Transform parent;
    public readonly int seed;
    private GameObject enviromentHolder;
    private ObjectSettings settings;
    private List<ObjectPlace> objectPlaces = new List<ObjectPlace>();
    private List<GameObject> gameObjects = new List<GameObject>();

    public bool isObjectsLoaded = false;

    public ObjectSettings Settings { get => settings;}

    public SpawnableObjectData(List<ValidPoint> validGrid, ObjectSettings settings, Transform parent, float[,] heightMap, int seed) {
        this.settings = settings;
        this.parent = parent;
        this.seed = seed;

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

            objectPlaces.Add(new ObjectPlace(validPoints[i], position, rotation));

        }
    }


    public void CreateObjects(bool visible = false) {
        enviromentHolder = new GameObject("Spawnable Holder");
        enviromentHolder.transform.parent = parent;
        enviromentHolder.transform.position = parent.position;
        System.Random random = new System.Random(seed + 1);

        for (int i = 0; i < objectPlaces.Count; i++) {
            GameObject gameObject;
            int index = RandomHelper.Range(0, settings.gameObjects.Length, ref random);
            if (settings.isNetworkObject) {
                gameObject = PhotonNetwork.Instantiate(settings.gameObjects[index].name, enviromentHolder.transform.position, Quaternion.identity);
                gameObject.transform.parent = enviromentHolder.transform;
            }
            else {
                gameObject = MonoBehaviour.Instantiate(settings.gameObjects[index], enviromentHolder.transform);
            }

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