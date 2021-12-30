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

    public void BindPlayerToHUD(GameObject obj) {
        hUDPlayerContainer.BindUnit(obj.GetComponent<Unit>());
        hUDPlayerContainer.SubscribeInventory(obj.GetComponentInChildren<ItemPicker>().Inventory);

    }

    public GameObject[] CreatePlayerHeroes() {
        GameObject[] heroes = new GameObject[GameTracker.Instance.GetPlayerTrackers().Count];

        foreach(PlayerTracker player in GameTracker.Instance.GetPlayerTrackers()) {
            if (player.IsLocal) {
                player.Hero = PhotonNetwork.Instantiate(player.PrefabHero.name, transform.position, Quaternion.identity);
                player.Hero.transform.parent = this.transform;
                BindPlayerToHUD(player.Hero);
                heroes[0] = player.Hero;
            }
        }

        return heroes;
    }
}
