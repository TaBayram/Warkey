using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FiniteWorldGameGenerator : MonoBehaviour
{
    public const float spawnProtectionDistance = 100*10;

    public WorldPlayerManager gameEntities;
    public FiniteWorldGenerator finiteWorldGenerator;
    public WorldEntitySettings worldEntitySettings;
    private PathData pathData;
    

    [HideInInspector] public HeightMap heightMap;
    [HideInInspector] public GameObject[] players;

    private XY chunkMatrix;
    private XY chunkSize;

    [HideInInspector] public Vector2 endingChunk;
    [HideInInspector] public Vector2 startingChunk;

    private MissionEndManager missionEndManager;
    private GameObject startPosition;


    PhotonView PV;

    private List<GameObject> enemies;
    private void Start() {
        finiteWorldGenerator.onWorldReady += FiniteWorldGenerator_onWorldReady;
        enemies =new List<GameObject>();
    }

    private void FiniteWorldGenerator_onWorldReady() {
        pathData = finiteWorldGenerator.pathData;
        heightMap = finiteWorldGenerator.heightMap;
        chunkMatrix = finiteWorldGenerator.chunkSize;
        chunkSize = new XY(finiteWorldGenerator.meshSettings.VerticesPerLineCount);

        startPosition = new GameObject("StartPosition");
        startPosition.transform.position = new Vector3(pathData.start.x - pathData.sizeX / 2 + ((chunkMatrix.x % 2 == 0) ? chunkSize.x / 2 : 0), heightMap.values[(int)pathData.start.x, (int)pathData.start.y] + 5, -pathData.start.y + pathData.sizeY / 2 + ((chunkMatrix.y % 2 == 0) ? chunkSize.y / 2 : 0));

        players =  gameEntities.CreatePlayerHeroes();

        foreach (GameObject player in players) {
            player.GetComponent<CharacterController>().enabled = false;
            player.transform.position = (startPosition.transform.position) + Vector3.up*5;
            player.GetComponent<CharacterController>().enabled = true;

            finiteWorldGenerator.BindViewer(player.transform);
        }

        var end = Instantiate(worldEntitySettings.MissionEndAreaPrefab, GetPositionFromPathData(pathData.end), Quaternion.identity);
        

        if (PhotonNetwork.IsMasterClient) {
            InvokeRepeating(nameof(Spawn), 30, 20);
            return;
            CreateOnPath(pathData, true);
            CreateOnEnd();
        }
    }

    private void CreateOnPath(PathData pathData, bool isMainBranch) {
        foreach (PathData path in pathData.branches)
            CreateOnPath(path,false);
        
        foreach (Vector2 item in pathData.points) {
            if((item-pathData.start).sqrMagnitude > spawnProtectionDistance && Random.Range(0f, 1f) < ((isMainBranch)?0.15f:0.05f)) {
                float random = Random.Range(0f, 1f);
                EntitySettings entitySettings = new EntitySettings();
                bool canSpawn = false;
                foreach (EntitySettings entity in worldEntitySettings.entitySettings) {
                    if (entity.canStaticSpawn && entity.spawnChance < random) {
                        entitySettings = entity;
                        canSpawn = true;
                    }
                }
                if (!canSpawn) continue;
                GameObject enemy = InstantiateRoomObject(entitySettings.prefab.name, GetPositionFromPathData(item) + Vector3.up * 2f);
            }
        }
    }

    private void CreateOnEnd() {
        for(int i = 0; i < 10; i++) {
            float random = Random.Range(0f, 1f);
            EntitySettings entitySettings = new EntitySettings();
            bool canSpawn = false;
            foreach (EntitySettings entity in worldEntitySettings.entitySettings) {
                if (entity.canStaticSpawn && entity.spawnChance < random) {
                    entitySettings = entity;
                    canSpawn = true;
                }
            }
            if (!canSpawn) continue;
            GameObject enemy = InstantiateRoomObject(entitySettings.prefab.name, FindPosition(GetPositionFromPathData(pathData.end), 5f, 10) + Vector3.up * 2f);

        }
    }

    private void Spawn() {
        SpawnI(false);
    }

    private void SpawnI(bool isI = false) {
        float random = Random.Range(0f, 1f);
        EntitySettings entitySettings = new EntitySettings();
        bool canSpawn = false;
        foreach (EntitySettings entity in worldEntitySettings.entitySettings) {
            if(entity.canDynamicSpawn && entity.spawnChance < random) {
                entitySettings = entity;
                canSpawn = true;
            }
        }
        if (!canSpawn) return;

        foreach (GameObject player in players) {
            Vector3 position = FindPosition(player.transform.position, entitySettings.spawnDistance, 10);
            GameObject enemy = InstantiateRoomObject(entitySettings.prefab.name, position);
        }


        if (!isI) {
            for(int i = 0; i < count % 5; i++) {
                SpawnI(true);
            }
        }
    }

    int count = 0;

    private GameObject InstantiateRoomObject(string name, Vector3 position) {
        GameObject enemy = PhotonNetwork.InstantiateRoomObject(name, position, Quaternion.identity);
        enemy.transform.parent = this.transform;
        enemies.Add(enemy);
        return enemy;
    }

    private Vector3 FindPosition(Vector3 position,float distance ,int iteration) {
        Vector3 vector;
        float randomX = (Random.Range(0, 2) == 0) ? Random.Range(distance, distance * 1.5f) : Random.Range(-distance * 1.5f, -distance);
        float randomY = (Random.Range(0, 2) == 0) ? Random.Range(distance, distance * 1.5f) : Random.Range(-distance * 1.5f, -distance);
        vector.x = (int)(position.x + randomX);
        vector.z = (int)(position.z + randomY);
        vector.y = 100f;
        if (Physics.Raycast(vector, Vector3.down, out RaycastHit raycastHit, 100f, 1 << LayerMask.NameToLayer("Ground"))){
            return raycastHit.point;
        }
        else {
            if (iteration > 0)
                return FindPosition(position, distance, iteration - 1);
            else
                return transform.position;
        }
    }


    private Vector3 GetPositionFromPathData(Vector2 vector2) {
        int x = (int)(vector2.x - pathData.sizeX / 2 + ((chunkMatrix.x % 2 == 0) ? chunkSize.x / 2 : 0));
        int z = (int)(-vector2.y + pathData.sizeY / 2 + ((chunkMatrix.y % 2 == 0) ? chunkSize.y / 2 : 0));
        return new Vector3(x, heightMap.values[(int)vector2.x, (int)vector2.y], z);
    }
}
