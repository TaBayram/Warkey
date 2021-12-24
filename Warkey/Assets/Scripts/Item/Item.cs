using UnityEngine;

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
			case ItemType.HealthPotion: return ItemAssets.Instance.healthPotionSprite;
            case ItemType.StaminaPotion: return ItemAssets.Instance.staminaPotionSprite;
            case ItemType.Bread: return ItemAssets.Instance.BreadSprite;
			default: return ItemAssets.Instance.BreadSprite;
		}
    }
	



}
