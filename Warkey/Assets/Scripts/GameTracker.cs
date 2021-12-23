using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private GameTracker() { }

    private List<PlayerTracker> playerTrackers = new List<PlayerTracker>();


}
