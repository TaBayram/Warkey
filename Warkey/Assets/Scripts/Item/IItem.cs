using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItem
{
    public enum type
    {
        health = 0,
        stamina = 1,
        movement = 2
    }
    ItemPicked PickUp();
}
