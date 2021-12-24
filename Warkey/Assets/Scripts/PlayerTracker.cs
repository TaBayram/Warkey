using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PlayerTracker
{
    private string nickname;
    public const float levelExperienceCost = 200f;
    private float experience;
    private float level;
    private int gold;
    private GameObject prefabHero;
    private GameObject hero;
    private bool isMine;

    private PlayerStorage playerStorage;
    private PlayerStorage.PlayerStorageData playerStorageData;

    public float Experience { get => experience; set => experience = value; }
    public float Level { get => level; set => level = value; }
    public int Gold { get => gold; set => gold = value; }
    public GameObject Hero { get => hero; set => hero = value; }
    public string Nickname { get => nickname; set => nickname = value; }
    public GameObject PrefabHero { get => prefabHero; }
    public bool IsMine { get => isMine; set => isMine = value; }

    public PlayerTracker() {
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
