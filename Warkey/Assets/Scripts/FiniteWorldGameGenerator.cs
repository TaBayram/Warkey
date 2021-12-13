using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FiniteWorldGenerator))]
public class FiniteWorldGameGenerator : MonoBehaviour
{
    public FiniteWorldGenerator finiteWorldGenerator;
    PathData pathData;
    public HeightMap heightMap;

    public GameObject[] players;

    XY chunkMatrix;
    XY chunkSize;

    public GameObject prefab;
    public GameObject playerPrefab;

    private void Start() {
        finiteWorldGenerator.onWorldReady += FiniteWorldGenerator_onWorldReady;
    }

    private void FiniteWorldGenerator_onWorldReady() {
        pathData = finiteWorldGenerator.pathData;
        heightMap = finiteWorldGenerator.heightMap;
        chunkMatrix = finiteWorldGenerator.chunkSize;
        chunkSize = new XY(finiteWorldGenerator.meshSettings.VerticesPerLineCount);

        GameObject startPosition = new GameObject("StartPosition");
        startPosition.transform.position = new Vector3(pathData.start.x - pathData.sizeX / 2 + ((chunkMatrix.x % 2 == 0) ? chunkSize.x / 2 : 0), heightMap.values[(int)pathData.start.x, (int)pathData.start.y] + 5, -pathData.start.y + pathData.sizeY / 2 + ((chunkMatrix.y % 2 == 0) ? chunkSize.y / 2 : 0));

        GameObject playr = Instantiate(playerPrefab, startPosition.transform.position, Quaternion.identity, this.transform);
        players = new GameObject[] { playr };

        foreach (GameObject player in players) {
            player.GetComponent<CharacterController>().enabled = false;
            player.transform.position = (startPosition.transform.position);
            player.GetComponent<CharacterController>().enabled = true;
        }

        foreach (Vector2 item in pathData.points) {
            if(Random.Range(0f,1f) < 0.1) {
                GameObject prefa = Instantiate(prefab, GetPositionFromPathData(item), Quaternion.identity, this.transform);
                prefa.GetComponent<ArtificialIntelligence>().player = players[0].transform;
            }
        }
    }

    private Vector3 GetPositionFromPathData(Vector2 vector2) {
        return new Vector3(vector2.x - pathData.sizeX / 2 + ((chunkMatrix.x % 2 == 0) ? chunkSize.x / 2 : 0), 
                           heightMap.values[(int)vector2.x, (int)vector2.y] + 5, 
                           -vector2.y + pathData.sizeY / 2 + ((chunkMatrix.y % 2 == 0) ? chunkSize.y / 2 : 0));
    }
}
