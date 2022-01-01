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
    PhotonView photonView;

    private bool isPicked;

    public bool IsPicked { get => isPicked; }
    public IItem.type Type { get => type; }

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    [PunRPC]
    public void DestroyItem()
    {
        if (photonView.Owner == PhotonNetwork.LocalPlayer)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
   
    public ItemPicked PickUp()
    {
        photonView.RPC(nameof(DestroyItem), RpcTarget.All);
        isPicked = true;
        return new ItemPicked(sprite,Type,amount,time);

    }



    

}
