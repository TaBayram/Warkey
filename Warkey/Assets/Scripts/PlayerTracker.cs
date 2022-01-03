using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class PlayerTracker
{
    public const float levelExperienceCost = 200f;
    private float experience;
    private int level;
    private int gold;
    private int prefabIndex;
    private GameObject hero;
    private Player player;
    private GameObject heroPrefab;


    private PlayerStorage playerStorage;
    private PlayerStorage.PlayerStorageData playerStorageData;

    public event System.Action<PlayerTracker> onLevelUp;
    public event System.Action<PlayerTracker> onExperienceChange;

    public float Experience { get => experience; }
    public int Level { get => level; }
    public int Gold { get => gold; }
    public GameObject Hero { get => hero; 
        set { 
            hero = value; 
            onHeroChanged?.Invoke(value); 
            if(player == PhotonNetwork.LocalPlayer)
                GameTracker.Instance.NetworkManager?.SendHero(hero.GetComponent<PhotonView>().ViewID); 
        } 
    }
    public string Nickname { get => player.NickName; set => player.NickName = value; }
    public bool IsLocal { get => player.IsLocal; }

    internal void Removed() {
        if(hero != null && (PhotonNetwork.IsMasterClient && PhotonNetwork.LocalPlayer == player)) {
            PhotonNetwork.Destroy(hero);
        }
    }

    public Player Player { get => player; set => player = value; }
    public int PrefabIndex { get => prefabIndex; }
    public GameObject HeroPrefab { get => heroPrefab; }

    public event System.Action<GameObject> onHeroChanged;

    public PlayerTracker(Player player) {
        this.player = player;
        playerStorage = new PlayerStorage(player.NickName);
        LoadPlayer();
    }

    public void ChangeHeroByIndex(int index) {
        prefabIndex = index;
        heroPrefab = HeroesData.Instance.GetHeroByIndex(prefabIndex);

        playerStorageData.heroIndex = index;
        playerStorageData.playedHero = heroPrefab.name;

        playerStorage.Save(playerStorageData);
    }

    public void AddExperience(float xp) {
        this.experience += xp;
        if(this.experience >= levelExperienceCost) {
            int levelAmount = (int)(this.experience / levelExperienceCost);
            AddLevel(levelAmount);
        }
        this.experience %= levelExperienceCost;
        onExperienceChange?.Invoke(this);

        playerStorageData.experience = this.experience;
        playerStorage.Save(playerStorageData);
    }

    public void AddLevel(int level) {
        this.level += level;
        onLevelUp?.Invoke(this);

        playerStorageData.level = this.level;
        playerStorage.Save(playerStorageData);
    }

    public void LoadPlayer() {
        playerStorageData = playerStorage.Load();
        experience = playerStorageData.experience;
        level = playerStorageData.level;
        gold = playerStorageData.gold;
        prefabIndex = playerStorageData.heroIndex;

        heroPrefab = HeroesData.Instance.GetHeroByIndex(playerStorageData.heroIndex);
    }
}
