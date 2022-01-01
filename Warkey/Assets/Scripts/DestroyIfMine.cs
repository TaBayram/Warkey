using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyIfMine : MonoBehaviour
{
    public PhotonView photonView;
    void Start()
    {
        if (photonView.IsMine) {
            Destroy(this.gameObject);
        }
    }


}
