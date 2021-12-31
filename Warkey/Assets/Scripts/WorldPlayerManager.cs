using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class WorldPlayerManager : MonoBehaviour
{
    public int playerCount = 1;
    public HUDPlayerContainer hUDPlayerContainer;
    [HideInInspector] public Transform revivingPlace;

    private void Start() {
        
    }

    public void BindPlayerToHUD(GameObject obj) {
        try {
            hUDPlayerContainer.BindUnit(obj.GetComponent<Unit>());
            if (obj.GetComponentInChildren<ItemPicker>() != null)
                hUDPlayerContainer.SubscribeInventory(obj.GetComponentInChildren<ItemPicker>().Inventory);
        }
        catch (Exception _) {

        }
        

    }

    public GameObject[] CreatePlayerHeroes() {
        GameObject[] heroes = new GameObject[1];

        foreach(PlayerTracker player in GameTracker.Instance.GetPlayerTrackers()) {
            if (player.IsLocal) {
                player.Hero = PhotonNetwork.Instantiate(player.HeroPrefab.name, transform.position, Quaternion.identity);
                player.Hero.transform.parent = this.transform;
                BindPlayerToHUD(player.Hero);
                heroes[0] = player.Hero;
                player.Hero.GetComponent<Entity>().onUnitStateChange += WorldPlayerManager_onUnitStateChange;
            }
        }

        return heroes;
    }

    private void WorldPlayerManager_onUnitStateChange(Entity arg1, IWidget.State arg2) {
        if(arg2 == IWidget.State.dead) {
            arg1.gameObject.transform.position = revivingPlace.position;
            arg1.unit.Heal(50);
            arg1.unit.State = IWidget.State.alive;
        }

        
    }
}
