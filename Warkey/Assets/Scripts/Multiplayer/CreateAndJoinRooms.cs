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
    public TMP_Text errorText;

    private string nickname;

    public void Enable() {
        panel.SetActive(true);
    }

    public void CreateRoom()
    {
        if (!IsNicknameValid()) return;

        errorText.gameObject.SetActive(false);
        PhotonNetwork.CreateRoom(createInput.text, new Photon.Realtime.RoomOptions { MaxPlayers = 4 });        
    }

    public void JoinRoom()
    {
        if (!IsNicknameValid()) return;

        errorText.gameObject.SetActive(false);
        PhotonNetwork.JoinRoom(joinInput.text);
    }

    private bool IsNicknameValid() {
        errorText.gameObject.SetActive(false);

        nickname = nicknameInput.text.Trim();
        if(nickname == "") {
            errorText.gameObject.SetActive(true);
            errorText.text = "Nickname can't be empty!";
            return false;
        }
        if(nickname.Length < 4) {
            errorText.gameObject.SetActive(true);
            errorText.text = "Nickname is too short!";
            return false;
        }
        return true;
    }

    public override void OnCreatedRoom() {
        GameTracker.Instance.AddPlayer(PhotonNetwork.LocalPlayer);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Lobby");
        PhotonNetwork.LocalPlayer.NickName = nicknameInput.text;
    }

    public override void OnCreateRoomFailed(short returnCode, string message) {
        errorText.gameObject.SetActive(true);
        errorText.text = message;
    }

    public override void OnJoinRoomFailed(short returnCode, string message) {
        errorText.gameObject.SetActive(true);
        errorText.text = message;
    }
}
