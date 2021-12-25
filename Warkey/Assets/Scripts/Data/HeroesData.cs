using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HeroesData : MonoBehaviour
{
    private static string prefabPath = "Data/Heroes";
    private static HeroesData instance;   

    public static HeroesData Instance {
        get {
            if (instance == null) {
                instance = GameObject.FindObjectOfType<HeroesData>();
                if (instance == null) {
                    instance = Resources.Load<GameObject>(prefabPath).GetComponent<HeroesData>();
                    instance.Construct();
                }
            }
            return instance;
        }
    }

    [SerializeField] Hero[] heroes;
    public Hero[] Heroes { get => heroes; }
    
    private void Construct() {
        for(int i = 0; i < heroes.Length; i++){
            heroes[i].uniqueName = heroes[i].prefab.name;
        }

        DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;
        if (pool != null && heroes != null) {
            foreach (Hero hero in heroes) {
                pool.ResourceCache.Add(hero.prefab.name, hero.prefab);
            }
        }
    }

    private void Start() {
        Construct();
        DontDestroyOnLoad(this.gameObject);
    }

    public GameObject GetHeroPrefab(string name) {
        foreach (Hero hero in heroes) {
            if(hero.uniqueName == name) {
                return hero.prefab;
            }
        }
        return null;
    }

    [System.Serializable]
    public struct Hero
    {
        [HideInInspector] public string uniqueName;
        public GameObject prefab;
    }
}
