using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FiniteWorldGameGenerator : MonoBehaviour
{
    public const float spawnProtectionDistance = 100*10;

    public WorldPlayerManager playerManager;
    public FiniteWorldGenerator finiteWorldGenerator;
    public WorldEntitySettings worldEntitySettings;
    private PathData pathData;
    [SerializeField] private GameObject defaultCamera;

    [HideInInspector] public HeightMap heightMap;
    [HideInInspector] public GameObject[] players;

    private XY chunkMatrix;
    private XY chunkSize;

    [HideInInspector] public Vector2 endingChunk;
    [HideInInspector] public Vector2 startingChunk;

    private MissionEndManager missionEndManager;
    private GameObject startPosition;
    private GameObject endPosition;


    PhotonView PV;

    private List<GameObject> enemies;
    private void Start() {
        finiteWorldGenerator.onWorldReady += FiniteWorldGenerator_onWorldReady;
        enemies = new List<GameObject>();
    }

    public Transform GenerateStartPosition() {
        pathData = finiteWorldGenerator.pathData;
        heightMap = finiteWorldGenerator.heightMap;
        chunkMatrix = finiteWorldGenerator.chunkSize;
        chunkSize = new XY(finiteWorldGenerator.meshSettings.VerticesPerLineCount);

        startPosition = Instantiate(worldEntitySettings.MissionStartAreaPrefab, GetPositionFromPathData(pathData.start), Quaternion.identity);
        startPosition.transform.parent = this.transform.parent;
        defaultCamera.transform.position = new Vector3(startPosition.transform.position.x, startPosition.transform.position.y, startPosition.transform.position.z);
        return startPosition.transform;
    }

    private void FiniteWorldGenerator_onWorldReady() {
        SetStartHeight();

        playerManager.revivingPlace = startPosition.transform;
        players =  playerManager.CreatePlayerHeroes();

        foreach (GameObject player in players) {
            player.GetComponent<CharacterController>().enabled = false;
            player.transform.position = (startPosition.transform.position);
            player.GetComponent<CharacterController>().enabled = true;

            finiteWorldGenerator.BindViewer(player.transform);
        }

        endPosition = Instantiate(worldEntitySettings.MissionEndAreaPrefab, GetPositionFromPathData(pathData.end), Quaternion.identity);
        endPosition.transform.parent = this.transform.parent;

        if (PhotonNetwork.IsMasterClient) {
            InvokeRepeating(nameof(Spawn), 30, 30);
            return;
            CreateOnEnd();
            CreateOnPath(pathData, true);
        }
    }

    private void CreateOnPath(PathData pathData, bool isMainBranch) {
        foreach (PathData path in pathData.branches)
            CreateOnPath(path,false);
        
        foreach (Vector2 item in pathData.points) {
            if((item-pathData.start).sqrMagnitude > spawnProtectionDistance && Random.Range(0f, 1f) < ((isMainBranch)?0.15f:0.05f)) {
                EntitySettings entitySettings = new EntitySettings();
                if (!GetPrefab(out entitySettings, true, false)) return;
                GameObject enemy = InstantiateRoomObject(entitySettings.prefab.name, GetPositionFromPathData(item) + Vector3.up * 2f);
            }
        }
    }

    private void CreateOnEnd() {
        for(int i = 0; i < 10; i++) {
            EntitySettings entitySettings = new EntitySettings();
            if (!GetPrefab(out entitySettings, true, false)) return;
            GameObject enemy = InstantiateRoomObject(entitySettings.prefab.name, FindPosition(GetPositionFromPathData(pathData.end), 5f, 10) + Vector3.up * 2f);

        }
    }

    private void Spawn() {
        SpawnI(false);
    }

    private void SpawnIteration() {
        SpawnI(true);
    }

    private void SpawnI(bool isIteration = false) {
        EntitySettings entitySettings = new EntitySettings();
        if (!GetPrefab(out entitySettings,false,true)) return;

        var players = GameTracker.Instance.GetPlayerTrackers();
        var player = players[Random.Range(0, players.Count)];
        Vector3 position = FindPosition(player.Hero.transform.position, entitySettings.spawnDistance, 10);
        GameObject enemy = InstantiateRoomObject(entitySettings.prefab.name, position);
        

        
        if (!isIteration) {
            count++;
            for (int i = 0; i < count / 5; i++) {
                Invoke(nameof(SpawnIteration), .5f);
            }
        }
    }

    int count = 0;

    private bool GetPrefab(out EntitySettings entitySettings, bool isStatic, bool isDynamic) {
        float random = Random.Range(0f, 1f);
        entitySettings = new EntitySettings();
        bool canSpawn = false;
        foreach (EntitySettings entity in worldEntitySettings.entitySettings) {
            if ((isStatic && !entity.canStaticSpawn)) continue;
            if ((isDynamic && !entity.canDynamicSpawn)) continue;
            if (entity.spawnChance >= random) {
                entitySettings = entity;
                canSpawn = true;
                break;
            }
        }
        return canSpawn;
    }

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
        int x = (int)(vector2.x - pathData.sizeX / 2f + ((chunkMatrix.x % 2 == 0) ? chunkSize.x / 2 : 0));
        int z = (int)(-vector2.y + pathData.sizeY / 2f + ((chunkMatrix.y % 2 == 0) ? chunkSize.y / 2 : 0));
        return new Vector3(x, heightMap.values[(int)vector2.x, (int)vector2.y], z);
    }

    private void SetStartHeight() {
        int iteration = 0;
        while (!Physics.Raycast(startPosition.transform.position + Vector3.down*20, Vector3.down, out RaycastHit raycastHit, 1000f, 1 << LayerMask.NameToLayer("Ground")) && iteration < 100) {
            startPosition.transform.position = new Vector3(startPosition.transform.position.x, startPosition.transform.position.y + 2, startPosition.transform.position.z);
            iteration++;
        }
    }
}
