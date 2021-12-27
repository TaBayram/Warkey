using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    GameTracker gameTracker = GameTracker.Instance;

    private void Start() {
        gameTracker.NetworkManager = this;

        DontDestroyOnLoad(this.gameObject);
    }


    public override void OnPlayerLeftRoom(Player otherPlayer) {
        gameTracker.RemovePlayer(otherPlayer);
    }

}
