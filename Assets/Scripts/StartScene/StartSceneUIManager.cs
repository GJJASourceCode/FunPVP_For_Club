using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.UI;

public class StartSceneUIManager : MonoBehaviour
{
    public GameObject titlePanel;
    public Button joinRoomButton;
    public TMP_InputField nicknameTextField;
    public TMP_InputField roomNameTextField;
    public GameObject lobbyPanel;
    public TMP_Text roomNameText;
    public TMP_Text player1NicknameText;
    public TMP_Text player2NicknameText;
    public Button startButton;
    private NetworkManager networkManager;

    public void Start()
    {
        networkManager = FindObjectOfType<NetworkManager>();
    }

    public void JoinRoom()
    {
        networkManager.JoinOrCreateRoom(roomNameTextField.text, nicknameTextField.text);
        titlePanel.SetActive(false);
        lobbyPanel.SetActive(true);
        StartCoroutine(RefreshLobbywithDelay());
    }

    public void OnPlayerJoinedRoom()
    {
        StartCoroutine(RefreshLobbywithDelay());
    }

    public void OnGameStartButtonClicked()
    {
        PhotonNetwork.LoadLevel("Select_Scene");
    }

    IEnumerator RefreshLobbywithDelay()
    {
        yield return new WaitForSeconds(1f);
        if (PhotonNetwork.IsMasterClient)
        {
            startButton.gameObject.SetActive(true);
        }
        else
        {
            startButton.gameObject.SetActive(false);
        }
        roomNameText.SetText(PhotonNetwork.CurrentRoom.Name);
        if (PhotonNetwork.PlayerList.Length > 0)
            player1NicknameText.SetText(PhotonNetwork.PlayerList[0].NickName);
        if (PhotonNetwork.PlayerList.Length > 1)
        {
            player2NicknameText.SetText(PhotonNetwork.PlayerList[1].NickName);
            startButton.interactable = true;
        }
    }

}
