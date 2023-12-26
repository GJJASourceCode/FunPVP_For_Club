using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using System;

public class GameManager : MonoBehaviourPunCallbacks
{

    public List<GameObject> playerPrefabList;

    public List<Transform> spawnPos;

    public void SpawnPlayer()
    {
        var hash = PhotonNetwork.LocalPlayer.CustomProperties;

        int i = Array.IndexOf(PhotonNetwork.PlayerList, PhotonNetwork.LocalPlayer);

        var player = PhotonNetwork.Instantiate(playerPrefabList[(int)hash["SelectedJob"]].name, spawnPos[i].position, spawnPos[i].rotation);
        FindObjectOfType<CameraController>().target = player.transform.Find("CameraHolder");
    }

    void Start()
    {
        SpawnPlayer();
    }

}

