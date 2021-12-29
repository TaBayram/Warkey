using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWorld : MonoBehaviour
{
    public static ItemWorld SpawnHealthPotion(Vector3 position, ItemPicked item)
    {
        GameObject transform = Instantiate(ItemAssets.Instance.healthPotionObject, position, Quaternion.identity);
        ItemWorld itemWorld = transform.GetComponent<ItemWorld>();
        itemWorld?.SetItem(item);
        return itemWorld;
    }
    public static ItemWorld SpawnStaminaPotion(Vector3 position, ItemPicked item)
    {
        GameObject transform = Instantiate(ItemAssets.Instance.staminaPotionObject, position, Quaternion.identity);
        ItemWorld itemWorld = transform.GetComponent<ItemWorld>();
        itemWorld?.SetItem(item);
        return itemWorld;
    }
    public static ItemWorld SpawnBread(Vector3 position, ItemPicked item)
    {
        GameObject transform = Instantiate(ItemAssets.Instance.BreadObject, position, Quaternion.identity);
        ItemWorld itemWorld = transform.GetComponent<ItemWorld>();
        itemWorld?.SetItem(item);
        return itemWorld;
    }


    private ItemPicked item;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void SetItem(ItemPicked item)
    {
        this.item = item;
        Debug.Log("item created");
        spriteRenderer.sprite = item.Sprite;
    }
    public ItemPicked GetItem()
    {
        return item;
    }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
