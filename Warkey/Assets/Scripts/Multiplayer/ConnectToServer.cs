using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using UnityEngine.Audio;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    public TMPro.TMP_Text text;
    public AudioMixer audioMixer;
    CreateAndJoinRooms createAndJoinRooms;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate =  PlayerPrefs.GetInt("FrameRate", 60);
        audioMixer.SetFloat("MusicVolume", PlayerPrefs.GetFloat("MusicVolume", 0));
        audioMixer.SetFloat("GameplayVolume", PlayerPrefs.GetFloat("GameplayVolume", 0));

        createAndJoinRooms = GetComponent<CreateAndJoinRooms>();
        PhotonNetwork.PhotonServerSettings.StartInOfflineMode = false;
        TryToConnect();
    }

    private void TryToConnect() {
        text.text = "CONNECTING...";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause) {
        Debug.Log(cause);
        text.text = "Couldn't connect! Trying again in 5 seconds";
        Invoke(nameof(TryToConnect), 5);
        
    }

    public override void OnJoinedLobby()
    {
        text.gameObject.SetActive(false);
        createAndJoinRooms.Enable();
    }

}
