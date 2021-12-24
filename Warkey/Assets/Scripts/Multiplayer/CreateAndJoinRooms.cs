using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public GameObject panel;
    public TMP_InputField createInput;
    public TMP_InputField joinInput;
    public TMP_InputField nicknameInput;

    public void Enable() {
        panel.SetActive(true);
    }

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(createInput.text, new Photon.Realtime.RoomOptions { MaxPlayers = 4 });
        
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinInput.text);
    }

    public override void OnJoinedRoom()
    {
        
        PhotonNetwork.LoadLevel("Lobby");
        PhotonNetwork.LocalPlayer.NickName = nicknameInput.text;
    }
}
