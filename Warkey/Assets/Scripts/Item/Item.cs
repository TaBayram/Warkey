using UnityEngine;


//[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item
{
	public enum ItemType
    {
		HealthPotion,
		StaminaPotion,
		Bread
    }
	public ItemType itemType;
	public int amount;

	public Sprite GetSprite()
    {
        switch (itemType)
        {
			case ItemType.HealthPotion: return ItemAssets.Instance.healthPotion;
            case ItemType.StaminaPotion: return ItemAssets.Instance.staminaPotion;
            case ItemType.Bread: return ItemAssets.Instance.Bread;
			default: return ItemAssets.Instance.Bread;
		}
    }

	/*
	new public string name = "New Item";    // Name of the item
	public Sprite icon = null;              // Item icon
	public bool isDefaultItem = false;      // Is the item default wear?

	// Called when the item is pressed in the inventory
	public virtual void Use()
	{
		// Use the item
		// Something might happen

		Debug.Log("Using " + name);
	}

	public void RemoveFromInventory()
	{
		Inventory.instance.Remove(this);
	}
	*/


}
