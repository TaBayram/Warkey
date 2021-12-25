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
    private GameObject prefabHero;
    private GameObject hero;
    private Player player;


    private PlayerStorage playerStorage;
    private PlayerStorage.PlayerStorageData playerStorageData;

    public float Experience { get => experience; set => experience = value; }
    public float Level { get => level; set => level = value; }
    public int Gold { get => gold; set => gold = value; }
    public GameObject Hero { get => hero; set => hero = value; }
    public string Nickname { get => nickname; set => nickname = value; }
    public GameObject PrefabHero { get => prefabHero; }
    public bool IsLocal { get => player.IsLocal; }
    public Player Player { get => player; set => player = value; }

    public PlayerTracker(Player player) {
        this.player = player;
        playerStorage = new PlayerStorage();
        LoadPlayer();
    }

    public GameObject CreatePlayerHero() {
        if(hero != null) {
            GameObject.Destroy(Hero);
        }
        return hero = GameObject.Instantiate<GameObject>(PrefabHero);
    }



    public void LoadPlayer() {
        playerStorageData = playerStorage.Load();
        Experience = playerStorageData.experience;
        Level = playerStorageData.level;
        Gold = playerStorageData.gold;

        prefabHero = HeroesData.Instance.GetHeroPrefab(playerStorageData.playedHero);
    }
}
