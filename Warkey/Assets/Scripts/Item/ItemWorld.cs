using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWorld : MonoBehaviour
{
    public static ItemWorld SpawnHealthPotion(Vector3 position, Item item)
    {
        GameObject transform = Instantiate(ItemAssets.Instance.healthPotionObject, position, Quaternion.identity);
        ItemWorld itemWorld = transform.GetComponent<ItemWorld>();
        itemWorld?.SetItem(item);
        return itemWorld;
    }
    public static ItemWorld SpawnStaminaPotion(Vector3 position, Item item)
    {
        GameObject transform = Instantiate(ItemAssets.Instance.staminaPotionObject, position, Quaternion.identity);
        ItemWorld itemWorld = transform.GetComponent<ItemWorld>();
        itemWorld?.SetItem(item);
        return itemWorld;
    }
    public static ItemWorld SpawnBread(Vector3 position, Item item)
    {
        GameObject transform = Instantiate(ItemAssets.Instance.BreadObject, position, Quaternion.identity);
        ItemWorld itemWorld = transform.GetComponent<ItemWorld>();
        itemWorld?.SetItem(item);
        return itemWorld;
    }


    private Item item;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void SetItem(Item item)
    {
        this.item = item;
        Debug.Log("item created");
        spriteRenderer.sprite = item.GetSprite();
    }
    public Item GetItem()
    {
        return item;
    }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
