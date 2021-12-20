using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryUI : MonoBehaviour
{
    private Inventory inventory;
    private Transform InventorySlotContainer;
    private Transform ItemSlot;

    private void Awake()
    {
        InventorySlotContainer = transform.Find("InventoryParentPanel");
        ItemSlot = InventorySlotContainer.Find("ItemSlot");
    }
    public void SetInventory(Inventory inventory)
    {
        this.inventory = inventory;
        RefreshInventoryItems();
    }

    private void RefreshInventoryItems()
    {
        int x = 0;
        int y = 0;
        float itemSlotCellsize = 30f;
        foreach (Item item in inventory.GetItemList())
        {
            RectTransform itemSlotRectTransform = Instantiate(ItemSlot, InventorySlotContainer).GetComponent<RectTransform>();
            itemSlotRectTransform.gameObject.SetActive(true);
            itemSlotRectTransform.anchoredPosition = new Vector2(x * itemSlotCellsize, y * itemSlotCellsize);
            x++;
            if (x>4)
            {
                x = 0;
                y++;
            }
        }
    }
}
