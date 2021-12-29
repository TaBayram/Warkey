using UnityEngine;

public class ItemPicked
{
	private Sprite sprite;
	private IItem.type type;
	private float amount;
	private ItemObject prefab;

    public ItemPicked(Sprite sprite, IItem.type type, float amount)
    {
        this.Sprite = sprite;
        this.Type = type;
        this.Amount = amount;
    }

    public Sprite Sprite { get => sprite; set => sprite = value; }
    public IItem.type Type { get => type; set => type = value; }
    public float Amount { get => amount; set => amount = value; }
}
