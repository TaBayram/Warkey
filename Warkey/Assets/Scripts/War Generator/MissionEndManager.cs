using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionEndManager : MonoBehaviour
{
    private List<GameObject> playersInside = new List<GameObject>();

    public event System.Action onPlayerEnter;

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")) {
            playersInside.Add(other.gameObject);
            CheckMissionEnd();
            onPlayerEnter?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {
            playersInside.Remove(other.gameObject);
        }
    }

    private void CheckMissionEnd() {
        if(playersInside.Count == GameTracker.Instance.GetPlayerTrackers().Count) {
            Debug.Log("End");
        }
    }
}
