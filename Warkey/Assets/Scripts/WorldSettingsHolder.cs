using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WorldSettingsHolder : MonoBehaviour
{
    [SerializeField] private PhotonView photonView;
    public WorldSettings worldSettings;

    /*public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            stream.SendNext(worldSettings);
        }
        else {
            worldSettings = (WorldSettings)stream.ReceiveNext();
        }
    }*/
    private void Start() {
        GameTracker.Instance.WorldSettingsHolder = this;
        worldSettings.worldSize = new XY(2, 2);
    }

    public void SetWorldSettings() {
        worldSettings.seed = Random.Range(int.MinValue, int.MaxValue);
        worldSettings.worldSize = new XY(Random.Range(2, 4), Random.Range(2, 4));
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
}
