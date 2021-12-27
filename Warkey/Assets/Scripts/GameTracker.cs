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
        playerTrackers.Remove(exists);
    }

    public PlayerTracker GetPlayerTracker(Player player) {
        foreach(PlayerTracker playerTracker in playerTrackers) {
            if(playerTracker.Player.ActorNumber == player.ActorNumber) {
                return playerTracker;
            }
        }

        return null;
    }


    public List<PlayerTracker> GetPlayerTrackers() {
        return new List<PlayerTracker>(this.playerTrackers);
    }

}
