using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;


public class PreparePool : MonoBehaviour
{
    public List<GameObject> Prefabs;

    void Awake() {
        DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;
        if (pool != null && Prefabs != null) {
            foreach (GameObject obj in Prefabs) {
                pool.ResourceCache.Add(obj.name, obj);
            }
        }
    }
}
