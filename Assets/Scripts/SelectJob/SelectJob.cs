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
        new List<String> { "탱커", "80", "10", "Q키를 누를 시 30초간 쉴드가 플레이어 주위를 돌며 보호합니다. (쿨타임 60초)" },
        new List<String>
        {
            "암살자",
            "55",
            "20",
            "Q키를 누를 시 몸을 숨깁니다. 그와 동시에 검은색 연기가 따라옵니다. 이동 속도가 가장 빠릅니다."
        },
        new List<String> { "겜블러", "30", "Random", "데미지가 -30~30까지인 주사위를 던집니다." },
        new List<String>
        {
            "거너",
            "45",
            "5",
            "에너지가 담긴 탄을 발사합니다. 데미지가 5인 총알 100발이 있습니다. 재장전은 불가능 합니다."
        },
        new List<String> { "매지션", "40", "1", "중력의 영향을 받는 불 마법을 구사합니다. 무한으로 마법이 가능하나 데미지가 1입니다." },
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

        if (!PhotonNetwork.IsMasterClient) return;

        CheckAllPlayersReady();
    
   }


    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (!changedProps.ContainsKey("Ready")) return;

        CheckAllPlayersReady();
    }

    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        if (newMasterClient != PhotonNetwork.LocalPlayer) return;

        CheckAllPlayersReady();
    }


    private void CheckAllPlayersReady()
    {
        var players = PhotonNetwork.PlayerList;

        // This is just using a shorthand via Linq instead of having a loop with a counter
        // for checking whether all players in the list have the key "Ready" in their custom properties
        if (players.All(p => p.CustomProperties.ContainsKey("Ready") && (bool)p.CustomProperties["Ready"]))
        {
            Debug.Log("All players are ready!");
            PhotonNetwork.LoadLevel("Battle_Scenes");
        }
    }
}
