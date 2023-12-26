using Photon.Pun;
using UnityEngine;

public class Shield : Job
{
    public override void Initialize()
    {
        base.hp = 20;
        base.attack = 1;
    }

    public GameObject SkillPrefab;
    public bool isSkillActive = false;
    private float skillActivationTime = 30f;
    private float skillTimer = 0f;

    private bool isSkillOnCooldown = false;
    private float skillCooldownTime = 60f;
    private PhotonView pv;

    void Start()
    {
        pv = GetComponentInParent<PhotonView>();
        if (SkillPrefab != null)
        {
            SkillPrefab.SetActive(false);
        }
        else
        {
            Debug.LogError("SkillPrefab is not set in the inspector!");
        }
    }

    void Update()
    {
        if (pv != null && !pv.IsMine)
            return;
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!isSkillActive && !isSkillOnCooldown)
            {
                pv.RPC("ActivateSkill", RpcTarget.All);
            }
        }

        if (isSkillActive)
        {
            skillTimer += Time.deltaTime;
            if (skillTimer >= skillActivationTime)
            {
                pv.RPC("DeactivateSkill", RpcTarget.All);
                StartCooldown();
            }
        }

        if (isSkillOnCooldown)
        {
            UpdateCooldown();
        }
    }

    [PunRPC]
    public void ActivateSkill()
    {
        SkillPrefab.SetActive(true);
        isSkillActive = true;
        skillTimer = 0f;
    }

    [PunRPC]
    public void DeactivateSkill()
    {
        SkillPrefab.SetActive(false);
        isSkillActive = false;
    }

    void StartCooldown()
    {
        isSkillOnCooldown = true;
        skillTimer = 0f;
    }

    void UpdateCooldown()
    {
        skillTimer += Time.deltaTime;
        if (skillTimer >= skillCooldownTime)
        {
            isSkillOnCooldown = false;
        }
    }
}
