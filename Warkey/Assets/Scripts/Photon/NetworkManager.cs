using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    GameTracker gameTracker = GameTracker.Instance;

    public event System.Action<PlayerTracker> onPlayerHeroReceived;
    public event System.Action<PlayerTracker> onPlayerLeft;
    public event System.Action<PlayerTracker> onPlayerJoin;
    public event System.Action<PlayerTracker> onMasterChanged;

    public int playerIndex = 0;

    private void Start() {
        gameTracker.NetworkManager = this;

        DontDestroyOnLoad(this.gameObject);
    }

    public override void OnJoinedRoom() {
        foreach (Player player in PhotonNetwork.PlayerList) {
            GameTracker.Instance.AddPlayer(player);
        }
        playerIndex = GameTracker.Instance.GetPlayerTrackers().Count - 1;
    }


    public override void OnPlayerEnteredRoom(Player newPlayer) {
        GameTracker.Instance.AddPlayer(newPlayer);
        onPlayerJoin?.Invoke(GameTracker.Instance.GetPlayerTracker(newPlayer));
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        gameTracker.RemovePlayer(otherPlayer);
        gameTracker.RemoveInactive();
        onPlayerLeft?.Invoke(GameTracker.Instance.GetPlayerTracker(otherPlayer));
    }

    public override void OnMasterClientSwitched(Player newMasterClient) {
        onMasterChanged?.Invoke(GameTracker.Instance.GetPlayerTracker(newMasterClient));
    }


    public void SendHero(int viewID) {
        this.photonView.RPC(nameof(BindHeroToPlayer), RpcTarget.OthersBuffered, viewID);
    }


    [PunRPC]
    public void BindHeroToPlayer(int viewID) {
        PhotonView view = PhotonView.Find(viewID);
        if (view != null) {
            var player = GameTracker.Instance.GetPlayerTracker(view.Owner);
            player.Hero = view.gameObject;
            onPlayerHeroReceived?.Invoke(player);
        }
    }

    internal void SendExperience(int v) {
        this.photonView.RPC(nameof(AddExperience), RpcTarget.All, v);
    }

    [PunRPC]
    public void AddExperience(int xp) {
        GameTracker.Instance.GetLocalPlayerTracker().AddExperience(xp);
    }
}
