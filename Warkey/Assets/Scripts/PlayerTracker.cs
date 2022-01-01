using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

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
    public GameObject Hero { get => hero; set { hero = value; onHeroChanged?.Invoke(value); GameTracker.Instance.NetworkManager?.SendHero(hero.GetComponent<PhotonView>().ViewID); } }
    public string Nickname { get => player.NickName; set => player.NickName = value; }
    public bool IsLocal { get => player.IsLocal; }
    public Player Player { get => player; set => player = value; }
    public int PrefabIndex { get => prefabIndex; }
    public GameObject HeroPrefab { get => heroPrefab; }

    public event System.Action<GameObject> onHeroChanged;

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

    public void AddExperience(float xp) {
        this.experience = (this.experience+xp) % levelExperienceCost;
        onExperienceChange?.Invoke(this);
        if(xp >= levelExperienceCost) {
            int levelAmount = (int)(xp / levelExperienceCost);
            AddLevel(levelAmount);
        }

        playerStorage.Save(playerStorageData);
    }

    public void AddLevel(int level) {
        level += level;
        onLevelUp?.Invoke(this);

        playerStorage.Save(playerStorageData);
    }

    public void LoadPlayer() {
        playerStorageData = playerStorage.Load();
        experience = playerStorageData.experience;
        level = playerStorageData.level;
        gold = playerStorageData.gold;
        prefabIndex = playerStorageData.heroIndex;

        playerStorageData.level = 1;
        playerStorageData.experience = 0;

        playerStorage.Save(playerStorageData);
        playerStorageData = playerStorage.Load();
        experience = playerStorageData.experience;
        level = playerStorageData.level;
        gold = playerStorageData.gold;
        prefabIndex = playerStorageData.heroIndex;


        heroPrefab = HeroesData.Instance.GetHeroByIndex(playerStorageData.heroIndex);
    }
}
