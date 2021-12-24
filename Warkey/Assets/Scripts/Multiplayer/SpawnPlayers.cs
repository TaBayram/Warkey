using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab;

    public float minX;
    public float maxX;
    public float minZ;
    public float maxZ;
    public float exactX;

    public List<GameObject> spawnedPlayers = new List<GameObject>();

    private void Start()
    {
        Vector3 randomPosition = new Vector3(Random.Range(minX, maxX), exactX, Random.Range(minZ, maxZ));
        spawnedPlayers.Add(PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, Quaternion.identity)); 
    }
}
