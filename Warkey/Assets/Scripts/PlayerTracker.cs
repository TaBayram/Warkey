using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerTracker
{
    private string nickname;
    public const float levelExperienceCost = 200f;
    private float experience;
    private float level;
    private int gold;
    private int prefabIndex;
    private GameObject hero;
    private Player player;
    private GameObject heroPrefab;


    private PlayerStorage playerStorage;
    private PlayerStorage.PlayerStorageData playerStorageData;

    public float Experience { get => experience; set => experience = value; }
    public float Level { get => level; set => level = value; }
    public int Gold { get => gold; set => gold = value; }
    public GameObject Hero { get => hero; set => hero = value; }
    public string Nickname { get => nickname; set => nickname = value; }
    public bool IsLocal { get => player.IsLocal; }
    public Player Player { get => player; set => player = value; }
    public int PrefabIndex { get => prefabIndex; }
    public GameObject HeroPrefab { get => heroPrefab; }

    public PlayerTracker(Player player) {
        this.player = player;
        playerStorage = new PlayerStorage();
        LoadPlayer();
    }

    public GameObject CreatePlayerHero() {
        if(hero != null) {
            GameObject.Destroy(Hero);
        }
        return hero = GameObject.Instantiate<GameObject>(HeroesData.Instance.GetHeroByIndex(prefabIndex));
    }

    public void ChangeHeroByIndex(int index) {
        prefabIndex = index;
        heroPrefab = HeroesData.Instance.GetHeroByIndex(prefabIndex);

        playerStorageData.heroIndex = index;
        playerStorageData.playedHero = heroPrefab.name;

        playerStorage.Save(playerStorageData);
    }



    public void LoadPlayer() {
        playerStorageData = playerStorage.Load();
        Experience = playerStorageData.experience;
        Level = playerStorageData.level;
        Gold = playerStorageData.gold;
        prefabIndex = playerStorageData.heroIndex;

        heroPrefab = HeroesData.Instance.GetHeroByIndex(playerStorageData.heroIndex);
    }
}
