using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class InventoryUI : MonoBehaviour
{
    private Inventory inventory;
    private Transform InventorySlotContainer;
    private Transform ItemSlot;
    private Transform ItemSlot2;
    private Transform ItemSlot3;

    private void Start()
    {
        InventorySlotContainer = transform.Find("InventorySlotContainer");
        ItemSlot = InventorySlotContainer.Find("ItemSlot");
        ItemSlot2 = InventorySlotContainer.Find("ItemSlot2");
        ItemSlot3 = InventorySlotContainer.Find("ItemSlot3");
    }
    public void SetInventory(Inventory inventory)
    {
        this.inventory = inventory;
        RefreshInventoryItems();
    }

    private void RefreshInventoryItems()
    {
        int x = 0;
        float y = -2.5f;
        float itemSlotCellsize = 30f;
        
        foreach (Item item in inventory.GetItemList())
        {
            RectTransform itemSlotRectTransform = Instantiate(ItemSlot, InventorySlotContainer).GetComponent<RectTransform>();
            itemSlotRectTransform.gameObject.SetActive(true);
            itemSlotRectTransform.anchoredPosition = new Vector2(x* itemSlotCellsize, itemSlotRectTransform.gameObject.transform.position.y*y);
            x += 5;
            Image image = itemSlotRectTransform.Find("ItemIcon").GetComponent<Image>();
            image.sprite = item.GetSprite();
        }
    }
}
