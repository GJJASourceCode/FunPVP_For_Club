using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    private StartSceneUIManager startSceneUIManager;

    void Awake()
    {
        Screen.SetResolution(1920, 1080, true);
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;
        startSceneUIManager = FindObjectOfType<StartSceneUIManager>();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected To Master");
        startSceneUIManager.joinRoomButton.interactable = true;
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Left Room");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Join Room Failed");
    }

    public void JoinOrCreateRoom(string roomName, string nickName)
    {
        PhotonNetwork.NickName = nickName;
        RoomOptions roomopt = new RoomOptions();
        roomopt.MaxPlayers = 2;
        roomopt.BroadcastPropsChangeToAll = true;
        PhotonNetwork.JoinOrCreateRoom(roomName, roomopt, null);
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        startSceneUIManager.OnPlayerJoinedRoom();
        Debug.Log("Player " + newPlayer.NickName + " Entered");
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player leftPlayer)
    {
        Debug.Log("Player " + leftPlayer.NickName + " Left");
    }
}
