using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private byte maxPlayers = 4;

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master.");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Successfully joined room.");
        LoadGameScene();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join room. Creating new room.");
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = maxPlayers;
        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    public void OnJoinClicked()
    {
        Debug.Log("Attempted to join random room.");
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnNameChanged(string name)
    {
        Debug.Log("Changed name to: " + name);
        PhotonNetwork.NickName = name;
    }

    private void LoadGameScene()
    {
        PhotonNetwork.LoadLevel("Main");
    }
}
