using System;
using UnityEngine;

[System.Serializable]
public class ItemPicked
{
	private Sprite sprite;
	private IItem.type type;
	private float amount;
    private float time;
	private ItemObject prefab;

    public ItemPicked(Sprite sprite, IItem.type type, float amount, float time) {
        this.Sprite = sprite;
        this.Type = type;
        this.Amount = amount;
        this.Time = time;
    }

    public Sprite Sprite { get => sprite; set => sprite = value; }
    public IItem.type Type { get => type; set => type = value; }
    public float Amount { get => amount; set => amount = value; }
    public float Time { get => time; set => time = value; }

    internal bool Use(Entity entity) {
        switch (type) {
            case IItem.type.health:
                entity.unit.Heal(amount);
                return true;
            case IItem.type.stamina:
                entity.unit.RegainStamina(amount);
                return true;
            case IItem.type.movement:
                entity.movement.IncreaseSpeed(amount, time);
                return true;
            default:
                return false;
        }
    }
}
