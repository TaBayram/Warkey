using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class InventoryUI : MonoBehaviour
{
    private Inventory inventory;
    [SerializeField] private Sprite empty;
    [SerializeField] private Transform[] slots;

    private void Start()
    {
    }

    public void SubscribeInventory(Inventory inventory) {
        this.inventory = inventory;
        inventory.onItemAdded += Inventory_onItemAdded;
        inventory.onItemRemoved += Inventory_onItemRemoved;
        RefreshInventoryItems();
    }

    private void Inventory_onItemRemoved(ItemPicked obj) {
        RefreshInventoryItems();
    }

    private void Inventory_onItemAdded(ItemPicked obj) {
        RefreshInventoryItems();
    }


    public void RefreshInventoryItems()
    {
        foreach (Transform slot in slots) {
            Image image = slot.GetComponent<Image>();
            image.sprite = empty;
        }
        foreach (ItemPicked item in inventory.GetItemList())
        {
            Image image = slots[(int)item.Type].GetComponent<Image>();
            image.sprite = item.Sprite;
        }
    }
}
