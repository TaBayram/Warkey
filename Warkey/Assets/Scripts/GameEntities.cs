using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntities : MonoBehaviour
{
    public int playerCount = 1;
    public GameObject playerPrefab;
    public HUDPlayerContainer hUDPlayerContainer;

    private void Start() {
        
    }

    public void BindPlayerToHUD(GameObject obj) {
        hUDPlayerContainer.BindUnit(obj.GetComponent<Unit>());
        hUDPlayerContainer.SubscribeInventory(obj.GetComponentInChildren<ItemPicker>().Inventory);
        
    }

    public GameObject[] CreatePlayerHeroes() {
        GameObject[] heroes = new GameObject[playerCount];

        for(int i = 0; i < playerCount; i++) {
            heroes[i] = Instantiate(playerPrefab, transform.position, Quaternion.identity, this.transform);
            BindPlayerToHUD(heroes[i]);
        }

        return heroes;
    }
}
