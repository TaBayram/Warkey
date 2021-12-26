using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WorldPlayerManager : MonoBehaviour
{
    public int playerCount = 1;
    public HUDPlayerContainer hUDPlayerContainer;

    private void Start() {
        
    }

    public void BindPlayerUnit(Unit unit) {
        hUDPlayerContainer.BindUnit(unit);
    }

    public GameObject[] CreatePlayerHeroes() {
        GameObject[] heroes = new GameObject[playerCount];

        foreach(PlayerTracker player in GameTracker.Instance.GetPlayerTrackers()) {
            if (player.IsLocal) {
                player.Hero = PhotonNetwork.Instantiate(player.PrefabHero.name, transform.position, Quaternion.identity);
                player.Hero.transform.parent = this.transform;
                BindPlayerUnit(player.Hero.GetComponent<Unit>());
                heroes[0] = player.Hero;
            }
        }

        return heroes;
    }
}
