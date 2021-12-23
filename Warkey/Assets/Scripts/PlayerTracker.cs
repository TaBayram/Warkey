using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTracker
{
    public const float levelExperienceCost = 200f;
    private float experience;
    private float level;
    private int gold;
    private GameObject prefabHero;
    private GameObject hero;

    private PlayerStorage playerStorage;
    private PlayerStorage.PlayerStorageData playerStorageData;

    public float Experience { get => experience; set => experience = value; }
    public float Level { get => level; set => level = value; }
    public int Gold { get => gold; set => gold = value; }
    public GameObject Hero { get => hero; }

    public PlayerTracker() {
        playerStorage = new PlayerStorage();
    }

    public GameObject CreatePlayerHero() {
        if(hero != null) {
            GameObject.Destroy(Hero);
        }
        return hero = GameObject.Instantiate<GameObject>(prefabHero);
    }

    public void LoadPlayer() {
        playerStorageData = playerStorage.Load();
        Experience = playerStorageData.experience;
        Level = playerStorageData.level;
        Gold = playerStorageData.gold;

        prefabHero = HeroesData.Instance.GetHeroPrefab(playerStorageData.playedHero);
    }
}
