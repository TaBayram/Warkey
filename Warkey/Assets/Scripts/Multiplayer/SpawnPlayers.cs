using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab;

    public float xWidth;
    public float zWidth;

    private void Start()
    {
        Vector3 randomPosition = this.transform.position + new Vector3(Random.Range(-xWidth, xWidth), 0, Random.Range(-zWidth, zWidth));
        var prefab = PhotonNetwork.Instantiate("Paladin", randomPosition, Quaternion.identity);
    }
}
