using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class WorldEntitySettings : ScriptableObject
{
    public EntitySettings[] entitySettings;
    public EntitySettings[] bossSettings;
    public GameObject MissionEndAreaPrefab;
    public int staticSpawnAmount;
    
    public void SortByChance() {
        for(int i = 0; i < entitySettings.Length; i++) {
            for(int j = i+1; j < entitySettings.Length; j++) {
                if(entitySettings[i].spawnChance > entitySettings[j].spawnChance) {
                    var temp = entitySettings[i];
                    entitySettings[i] = entitySettings[j];
                    entitySettings[j] = temp;
                }
            }
        }
    }
}


[System.Serializable]
public struct EntitySettings
{
    public GameObject prefab;
    public bool canDynamicSpawn;
    public bool canStaticSpawn;
    [Range(0, 1)]
    public float spawnChance;
    public float spawnDistance;
}