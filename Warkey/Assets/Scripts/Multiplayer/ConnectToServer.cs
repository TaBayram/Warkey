using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    public GameObject panel;
    CreateAndJoinRooms createAndJoinRooms;

    // Start is called before the first frame update
    void Start()
    {
        createAndJoinRooms = GetComponent<CreateAndJoinRooms>();
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        panel.SetActive(false);
        createAndJoinRooms.Enable();
    }
}
