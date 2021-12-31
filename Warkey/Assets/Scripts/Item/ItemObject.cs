using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

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
        PhotonNetwork.Destroy(gameObject);
        isPicked = true;
        return new ItemPicked(sprite,Type,amount,time);
    }
}
