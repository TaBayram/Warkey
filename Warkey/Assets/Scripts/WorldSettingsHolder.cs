using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WorldSettingsHolder : MonoBehaviour
{
    [SerializeField] private PhotonView photonView;
    public WorldSettings worldSettings;

    private void Start() {
        GameTracker.Instance.WorldSettingsHolder = this;
        worldSettings.worldSize = new XY(1, 1);
    }

    public void SetWorldSettings() {
        worldSettings.seed = Random.Range(int.MinValue, int.MaxValue);
        worldSettings.worldSize = new XY(Random.Range(3, 3), Random.Range(3, 3));
        //worldSettings.worldSize = new XY(1, 1);
        worldSettings.biome = "Forest";
    }


    public void SendWorldSettings() {
        photonView.RPC(nameof(GetWorldSettings), RpcTarget.Others, worldSettings.seed,worldSettings.worldSize.x,worldSettings.worldSize.y,worldSettings.biome);

    }

    [PunRPC]
    void GetWorldSettings(int seed, int x , int y, string biome) {
        worldSettings.seed = seed;
        worldSettings.worldSize.x = x;
        worldSettings.worldSize.y = y;
        worldSettings.biome = biome;
    }
}

[System.Serializable]
public struct WorldSettings
{
    public int seed;
    public XY worldSize;
    public string biome;

    public WorldSettings(int seed, XY worldSize, string biome) {
        this.seed = seed;
        this.worldSize = worldSize;
        this.biome = biome;
    }
}
