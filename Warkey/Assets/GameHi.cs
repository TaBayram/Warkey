using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class GameHi : MonoBehaviour
{
    public TMP_Text text;

    void Start()
    {
        text.text = PhotonNetwork.CurrentRoom.Name;
    }

    
}
