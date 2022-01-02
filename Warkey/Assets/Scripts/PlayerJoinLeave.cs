using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class PlayerJoinLeave : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    void Start()
    {
        text.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name;
        GameTracker.Instance.NetworkManager.onPlayerLeft += NetworkManager_onPlayerLeft;
        GameTracker.Instance.NetworkManager.onPlayerJoin += NetworkManager_onPlayerJoin;
        GameTracker.Instance.NetworkManager.onMasterChanged += NetworkManager_onMasterChanged;
    }

    private void NetworkManager_onMasterChanged(PlayerTracker obj) {
        text.text += (obj.Player.NickName + " is the new host!");
    }

    private void NetworkManager_onPlayerJoin(PlayerTracker obj) {
        text.text += (obj.Player.NickName +" has joined!");
    }

    private void NetworkManager_onPlayerLeft(PlayerTracker obj) {
        text.text += (obj.Player.NickName + " has left!");
    }
}
