using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SelectJob : MonoBehaviourPunCallbacks
{
    public int selectedJob;
    public List<Sprite> jobImageList;
    public List<Button> jobButtonList;
    public List<GameObject> jobPrefabList;
    public Image jobImage;
    public Text jobName;
    public Text hpText;
    public Text atkText;
    public Text descriptionText;
    public GameObject currentJob;
    public Button submitJob;
    private GameObject currentGameObject = null;

    private List<List<String>> jobDescriptionList = new List<List<String>>
    {
        new List<String>
        {
            "탱커",
            "100",
            "10",
            "이 전사의 방패는 매우 단단합니다. 하지만 너무 무거운 탓에 자신도 버티기 어려워합니다.\nQ키를 누를 시 방패를 소환해 자신을 보호합니다."
        },
        new List<String>
        {
            "암살자",
            "40",
            "5",
            "Q키를 누를 시 자신의 모습을 연막으로 숨깁니다. 하지만 연막을 숨기는 법은 알지 못했나 봅니다."
        },
        new List<String>
        {
            "겜블러",
            "Random",
            "Random",
            "인생은 한방!\n도박에 중독되어 자신의 체력과 데미지마저 도박으로 결정합니다.\nHP:20~50,ATK:-5~15"
        },
        new List<String>
        {
            "거너",
            "20",
            "2",
            "사거리가 매우 긴 총을 발사합니다. 하지만 총알은 두고 왔나보네요.\n총알이 50발로 한정되어 있습니다."
        },
        new List<String>
        {
            "매지션",
            "30",
            "1",
            "라이터 수준의 불의 마법을 다루는 겉멋 마법사입니다. 겉멋에 너무 치중한 나머지 데미지를 높이는 법은 알지 못했나 봅니다."
        },
    };

    void Start()
    {
        for (int i = 0; i < jobButtonList.Count; i++)
        {
            int j = i;
            jobButtonList[i].onClick.AddListener(() =>
            {
                if (currentGameObject != null)
                {
                    Destroy(currentGameObject);
                }
                currentJob.SetActive(true);
                submitJob.gameObject.SetActive(true);
                Debug.Log(j);
                jobImage.sprite = jobImageList[j];
                jobName.text = jobDescriptionList[j][0];
                hpText.text = "HP: " + jobDescriptionList[j][1];
                atkText.text = "ATK: " + jobDescriptionList[j][2];
                descriptionText.text = jobDescriptionList[j][3];
                currentGameObject = Instantiate(
                    jobPrefabList[j],
                    new Vector3(15, 0.1f, -40),
                    Quaternion.identity
                );
                selectedJob = j;
            });
        }
    }

    public void SubmitJob()
    {
        var hash = PhotonNetwork.LocalPlayer.CustomProperties;
        hash["Ready"] = true;
        hash["SelectedJob"] = selectedJob;
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        submitJob.gameObject.SetActive(false);

        if (!PhotonNetwork.IsMasterClient)
            return;

        CheckAllPlayersReady();
    }

    public override void OnPlayerPropertiesUpdate(
        Photon.Realtime.Player targetPlayer,
        ExitGames.Client.Photon.Hashtable changedProps
    )
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (!changedProps.ContainsKey("Ready"))
            return;

        CheckAllPlayersReady();
    }

    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        if (newMasterClient != PhotonNetwork.LocalPlayer)
            return;

        CheckAllPlayersReady();
    }

    private void CheckAllPlayersReady()
    {
        var players = PhotonNetwork.PlayerList;

        // This is just using a shorthand via Linq instead of having a loop with a counter
        // for checking whether all players in the list have the key "Ready" in their custom properties
        if (
            players.All(
                p => p.CustomProperties.ContainsKey("Ready") && (bool)p.CustomProperties["Ready"]
            )
        )
        {
            Debug.Log("All players are ready!");
            PhotonNetwork.LoadLevel("Battle_Scenes");
        }
    }
}
