using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;


public class MissionEndManager : MonoBehaviour
{
    private List<GameObject> playersInside = new List<GameObject>();

    public event System.Action onPlayerEnter;

    [SerializeField] private TMP_Text playerText;

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")) {
            playersInside.Add(other.gameObject);
            playerText.text =""+ playersInside.Count;
            CheckMissionEnd();
            onPlayerEnter?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {
            playersInside.Remove(other.gameObject);
            playerText.text = "" + playersInside.Count;
        }
    }

    private void CheckMissionEnd() {
        if(PhotonNetwork.IsMasterClient && playersInside.Count == GameTracker.Instance.GetPlayerTrackers().Count) {
            GameTracker.Instance.NetworkManager.SendExperience(225);
            PhotonNetwork.LoadLevel((int)LoadScene.Scenes.Camp);

        }
    }
}
