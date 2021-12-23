using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerStorage
{
    private const string filename = "PlayerData";
    private StorageHandler storageHandler;
    private PlayerStorageData loaded;
    private PlayerStorageData saved;

    public PlayerStorage() {
        storageHandler = new StorageHandler();
    }

    

    public PlayerStorageData Load() {
        return loaded = (PlayerStorageData)storageHandler.LoadData(filename);
    }

    public bool Save(PlayerStorageData save) {
        try {
            saved = save;
            storageHandler.SaveData(save, filename);
            return true;
        }
        catch(Exception exception) {
            return false;
        }
    }


    [Serializable]
    public struct PlayerStorageData
    {
        public float experience;
        public float level;
        public int gold;
        public string playedHero;
    }
}
