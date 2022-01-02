using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameTracker
{
    private static GameTracker instance;

    public static GameTracker Instance {
        get {
            if (instance == null) 
                instance = new GameTracker(); 
            return instance; 
        }
    }

    //public List<PlayerTracker> PlayerTrackers { get => playerTrackers; set => playerTrackers = value; }

    private GameTracker() { }

    private List<PlayerTracker> playerTrackers = new List<PlayerTracker>();

    public NetworkManager NetworkManager;
    public WorldSettingsHolder WorldSettingsHolder;
    public bool isSceneChanging;


    public PlayerTracker AddPlayer(Player player) {
        PlayerTracker exists = GetPlayerTracker(player);
        if ((exists) != null) return exists;

        PlayerTracker tracker = new PlayerTracker(player);
        playerTrackers.Add(tracker);
        return tracker;
    }

    public void RemovePlayer(Player player) {
        PlayerTracker exists = GetPlayerTracker(player);
        if ((exists) != null) return;
        exists.Removed();
        playerTrackers.Remove(exists);
    }

    public void RemoveInactive() {
        for(int i = playerTrackers.Count-1; i >= 0; --i) {
            if (playerTrackers[i].Player.IsInactive)
                playerTrackers.RemoveAt(i);
        }
    }

    public PlayerTracker GetPlayerTracker(Player player) {
        foreach(PlayerTracker playerTracker in playerTrackers) {
            if(playerTracker.Player == player) {
                return playerTracker;
            }
        }

        return null;
    }

    public PlayerTracker GetLocalPlayerTracker() {
        return GetPlayerTracker(PhotonNetwork.LocalPlayer);
    }

    public List<PlayerTracker> GetPlayerTrackers() {
        return new List<PlayerTracker>(this.playerTrackers);
    }

}
