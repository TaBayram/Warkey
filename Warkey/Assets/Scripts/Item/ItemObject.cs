using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour, IItem
{
    [SerializeField] private Sprite sprite;
    [SerializeField] private IItem.type type;
    [SerializeField] private float amount;
    [SerializeField] private float time;

    private bool isPicked;

    public bool IsPicked { get => isPicked; }
    public IItem.type Type { get => type; }

    public ItemPicked PickUp()
    {
        Destroy(gameObject);
        isPicked = true;
        return new ItemPicked(sprite,Type,amount,time);
    }
}
