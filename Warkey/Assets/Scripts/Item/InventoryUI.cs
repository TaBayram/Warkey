using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class InventoryUI : MonoBehaviour
{
    private Inventory inventory;

    [SerializeField] private Transform[] slots;

    private void Start()
    {
    }
    public void SetInventory(Inventory inventory)
    {
        this.inventory = inventory;
        RefreshInventoryItems();
    }

    public void RefreshInventoryItems()
    {
        foreach (ItemPicked item in inventory.GetItemList())
        {
            Image image = slots[(int)item.Type].GetComponent<Image>();
            image.sprite = item.Sprite;
        }
    }
}
