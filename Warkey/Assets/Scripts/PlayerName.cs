using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerName : MonoBehaviour
{
    public PhotonView photonView;
    public TMP_Text text;
    void Start()
    {
        text.text = photonView.Owner.NickName;
    }
}
