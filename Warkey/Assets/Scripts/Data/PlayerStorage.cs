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
    private string nickname;

    public PlayerStorage(string nickname) {
        this.nickname = nickname;
        storageHandler = new StorageHandler();
    }

    

    public PlayerStorageData Load() {
        var obj = storageHandler.LoadData(filename + nickname);
        if(obj == null) {
            loaded = new PlayerStorageData();
            loaded.experience = 0;
            loaded.level = 1;
            loaded.playedHero = HeroesData.Instance.Heroes[0].uniqueName;
            loaded.gold = 0;
            loaded.heroIndex = UnityEngine.Random.Range(0,1);
        }
        else {
            loaded = (PlayerStorageData)obj;
        }
        return loaded;
    }

    public bool Save(PlayerStorageData save) {
        try {
            saved = save;
            storageHandler.SaveData(save, filename + nickname);
            return true;
        }
        catch (Exception) {
            return false;
        }
    }


    [Serializable]
    public struct PlayerStorageData
    {
        public float experience;
        public int level;
        public int gold;
        public string playedHero;
        public int heroIndex;
    }
}
